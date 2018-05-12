using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HSBot.Helpers;

namespace HSBot.Persistent
{
    internal class DataStorage
    {
        private static readonly string resourcesFolder = "Resources";

        static DataStorage()
        {
            if (!Directory.Exists(resourcesFolder))
            {
                Directory.CreateDirectory(resourcesFolder);
            }
        }

        internal static void SaveEnumeratedObject<T>(IEnumerable<T> Objects, string file, Formatting formatting)
        {
            string json = JsonConvert.SerializeObject(Objects, formatting);
            File.WriteAllText(file, json);
        }

        internal static void SaveEnumeratedObject<T>(IEnumerable<T> Objects, string file, bool useIndentations)
        {
            var formatting = (useIndentations) ? Formatting.Indented : Formatting.None;
            SaveEnumeratedObject<T>(Objects, file, formatting);
        }

        internal static void StoreObject(object obj, string file, Formatting formatting)
        {
            string json = JsonConvert.SerializeObject(obj, formatting);
            string filePath = String.Concat(resourcesFolder, "/", file);
            File.WriteAllText(filePath, json);
        }

        internal static void StoreObject(object obj, string file, bool useIndentations)
        {
            var formatting = (useIndentations) ? Formatting.Indented : Formatting.None;
            StoreObject(obj, file, formatting);
        }

        public static IEnumerable<T> LoadEnumeratedObject<T>(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Utilities.Log(MethodBase.GetCurrentMethod(), "Failure to find datafile for " + filePath, Discord.LogSeverity.Critical);
                return null;
            }
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<T>>(json);
        }

        internal static T RestoreObject<T>(string file)
        {
            string json = GetOrCreateFileContents(file);
            return JsonConvert.DeserializeObject<T>(json);
        }

        internal static bool LocalFileExists(string file, bool BuildFileIfNonExistant = false)
        {
            string filePath = String.Concat(resourcesFolder, "/", file);
            if (BuildFileIfNonExistant & !(File.Exists(filePath)))
            {
                File.WriteAllText(filePath, "");
                return false;
            }
            return File.Exists(filePath);
        }

        internal static string[] GetFilesInFolder(string folder)
        {
            string folderPath = String.Concat(resourcesFolder, "/", folder);
            string[] Files = Directory.GetFiles(folderPath);
            int i;
            for (i = 0; i < Files.Length; i++)
            {
                Files[i] = Path.GetFileName(Files[i]);
            }
            return Files;
        }

        internal static bool LocalFolderExists(string folder, bool BuildFolderIfNonExistant = false)
        {
            string Path = String.Concat(resourcesFolder, "/", folder);
            if (BuildFolderIfNonExistant & !(Directory.Exists(Path)))
            {
                Directory.CreateDirectory(Path);
                return false;
            }
            return Directory.Exists(Path);
        }

        internal static string GetText(string file)
        {
            return File.ReadAllText(resourcesFolder + "/" + file);
        }

        internal static void DeleteFile(string file)
        {
            File.Delete(resourcesFolder + "/" + file);
        }

        private static string GetOrCreateFileContents(string file)
        {
            string filePath = String.Concat(resourcesFolder, "/", file);
            if (!File.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
                File.Create(filePath);
                return "";
            }
            return File.ReadAllText(filePath);
        }
    }
}
