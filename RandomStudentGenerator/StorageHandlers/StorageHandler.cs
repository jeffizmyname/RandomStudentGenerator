using CsvHelper;
using CsvHelper.Configuration;
using RandomStudentGenerator.CsvMaps;
using RandomStudentGenerator.CustomControls;
using RandomStudentGenerator.Models;
using RandomStudentGenerator.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomStudentGenerator.StorageHandlers
{
    public static class StorageHandler
    {
        private static readonly string rootPath = Path.Combine(FileSystem.AppDataDirectory, "studentsGenerator");

        public static string currentClass { get; set; } = "";
        public static int currentClassSize { get; set; } = 0;
        public static int happyNumber { get; set; } = 0;

        public static ClassViewModel currentClassModel { get; set; }

        private static void Init()
        {
            if(!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);
        }

        public static void AddClass(string filePath)
        {
            Init();
            var fileName = Path.GetFileName(filePath);
            var destPath = Path.Combine(rootPath, fileName);
            File.Copy(filePath, destPath, true);
            currentClass = fileName;
            currentClassSize = getClassSize(fileName);
        }

        public static void CreateNewClass(string className)
        {
            Init();
            var filePath = Path.Combine(rootPath, className + ".csv");
            if (File.Exists(filePath)) return;
            using (var stream = new StreamWriter(filePath))
            using (var csv = new CsvWriter(stream, CultureInfo.CurrentCulture))
            {
                csv.Context.RegisterClassMap<StudentMap>();
                csv.WriteRecords(new List<Student>());
            }
            currentClass = className;
            currentClassSize = 0;
        }

        public static List<string> ReadClassNames()
        {
            Init();
            var files = Directory.GetFiles(rootPath);
            return files
                 .Select(f => Path.GetFileNameWithoutExtension(f) ?? string.Empty)
                 .ToList();
        }

        public static Class ReadClass(string className)
        {
            Init();
            var filePath = Path.Combine(rootPath, className + ".csv");
            if (!File.Exists(filePath)) return new Class(":<", new List<Student>());

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                DetectDelimiter = true,
                HasHeaderRecord = true,
                PrepareHeaderForMatch = args => args.Header.ToLower(),
                HeaderValidated = null,
                MissingFieldFound = null
            };

            using (var stream = new StreamReader(filePath))
            using (var csv = new CsvReader(stream, config))
            {
                var rows = csv.GetRecords<dynamic>().ToList();
                if (!rows.Any()) return new Class(className, new List<Student>());

                var headers = csv.Context.Reader.HeaderRecord;
                var presenceDates = headers.Skip(3).ToList();

                List<Student> students = new List<Student>();

                foreach (var row in rows)
                {
                    var dictRow = row as IDictionary<string, object>; 

                    int id = int.Parse(dictRow["id"].ToString());
                    string name = dictRow["name"].ToString();
                    string surname = dictRow["surname"].ToString();

                    var student = new Student(id, name, surname, true) { Presences = new List<Presence>() };

                    foreach (var date in presenceDates)
                    {
                        if (DateTime.TryParse(date, out DateTime parsedDate) && dictRow.ContainsKey(date))
                        {
                            bool isPresent = bool.TryParse(dictRow[date]?.ToString(), out bool result) && result;
                            student.Presences.Add(new Presence(parsedDate, isPresent));
                        }
                    }

                    student.Presences = student.Presences.OrderBy(p => p.date).ToList();

                    if (student.Presences.Any())
                    {
                        student.CurrentPresence = student.Presences.Find(p => p.date.Date == currentSelectedDate.Date)
                                                ?? new Presence(DateTime.Now, false);
                    }
                    else
                    {
                        student.CurrentPresence = new Presence(DateTime.Now, false);
                    }

                    students.Add(student);
                }

                currentClassSize = students.Count;
                currentClass = className;
                return new Class(className, students);
            }
        }


        public static void AddStudent(List<Student> students, string className)
        {
            Init();
            var filePath = Path.Combine(rootPath, className + ".csv");
            if (!File.Exists(filePath)) return;

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                DetectDelimiter = true,
                HasHeaderRecord = true,
                NewLine = Environment.NewLine
            };

            using (var stream = new StreamWriter(filePath))
            using (var csv = new CsvWriter(stream, config))
            {
                csv.Context.RegisterClassMap<StudentMap>();
                csv.WriteRecords(students);
            }
            currentClassSize = students.Count;
        }

        public static void savePresence(string className, List<Student> students)
        {
            Init();
            var filePath = Path.Combine(rootPath, className + ".csv");

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                NewLine = Environment.NewLine
            };

            List<string[]> rows = new List<string[]>();
            List<string> headers = new List<string> { "Id", "Name", "Surname" };

            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                if (lines.Length > 0)
                {
                    headers = lines[0].Split(',').ToList();
                    rows = lines.Skip(1).Select(line => line.Split(',')).ToList();
                }
            }

            var presenceDates = students.SelectMany(s => s.Presences)
                                        .Select(p => p.date.ToString("yyyy-MM-dd"))
                                        .Distinct()
                                        .OrderBy(d => d)
                                        .ToList();

            foreach (var date in presenceDates)
            {
                if (!headers.Contains(date))
                    headers.Add(date);
            }

            var updatedRows = new List<string[]>();
            foreach (var student in students)
            {
                var row = new List<string> { student.Id.ToString(), student.Name, student.Surname };

                foreach (var date in presenceDates)
                {
                    var presence = student.Presences.FirstOrDefault(p => p.date.ToString("yyyy-MM-dd") == date);
                    row.Add(presence != null && presence.isPresent ? "true" : "false");
                }

                updatedRows.Add(row.ToArray());
            }

            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteField(headers);
                csv.NextRecord();

                foreach (var row in updatedRows)
                {
                    foreach (var field in row)
                        csv.WriteField(field);
                    csv.NextRecord();
                }
            }
        }

        public static int getClassSize(string className)
        {
            Init();
            var filePath = Path.Combine(rootPath, className + ".csv");
            if (!File.Exists(filePath)) return -1;
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                DetectDelimiter = true,
                HasHeaderRecord = true,
                PrepareHeaderForMatch = args => args.Header.ToLower(),
                HeaderValidated = null,
                MissingFieldFound = null
            };
            using (var stream = new StreamReader(filePath))
            using (var csv = new CsvReader(stream, config))
            {
                var records = csv.GetRecords<Student>().ToList();
                return records.Count;
            }
        }
    }
}
