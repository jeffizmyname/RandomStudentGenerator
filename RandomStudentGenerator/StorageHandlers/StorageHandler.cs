using System;
using System.Collections.Generic;
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

        public static void AddClass()
        {

        }
    }
}
