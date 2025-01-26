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
                return new Class(className, records);
            }
        }

        public static void AddStudent(Student student, string className)
        {
            Init();
            var filePath = Path.Combine(rootPath, className + ".csv");
            if (!File.Exists(filePath)) return;

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                DetectDelimiter = true,
                HasHeaderRecord = true,
            };

            using (var stream = new StreamWriter(filePath, append: true))
            using (var csv = new CsvWriter(stream, config))
            {
                csv.Context.RegisterClassMap<StudentMap>();
                csv.WriteRecord(student);
            }

        }
    }
}
