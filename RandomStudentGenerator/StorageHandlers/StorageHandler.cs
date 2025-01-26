using CsvHelper;
using CsvHelper.Configuration;
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

            var config = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                DetectDelimiter = true,
                HasHeaderRecord = false
            };

            using (var stream = new StreamReader(filePath))
            using (var csv = new CsvReader(stream, config))
            {
                var records = csv.GetRecords<Student>();
                Debug.WriteLine(records);
                List<Student> temp = new List<Student>();

                bool workaround = true;
                foreach (var record in records)
                {
                    //Debug.WriteLine("record: " + record.Name);
                    if (workaround)
                    {
                        workaround = false;
                        continue;
                    }
                    temp.Add(record);
                }

                return new Class(className, temp);
            }
        }
    }
}
