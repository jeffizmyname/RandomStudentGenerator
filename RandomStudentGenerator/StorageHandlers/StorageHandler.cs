using CsvHelper;
using CsvHelper.Configuration;
using RandomStudentGenerator.CsvMaps;
using RandomStudentGenerator.CustomControls;
using RandomStudentGenerator.Models;
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
                var records = csv.GetRecords<Student>().ToList();
                Debug.WriteLine(records);
                currentClassSize = records.Count;
                currentClass = className;
                return new Class(className, records);
                
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
