using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Discord;
using HSBot.Helpers;

namespace HSBot.Persistent
{
    /// <summary>
    /// Hard Disk Persistence.
    /// </summary>
    public static class DataStorage
    {
        private const string ResourcesFolder = "Resources";

        static DataStorage()
        {
            if (!Directory.Exists(ResourcesFolder))
            {
                Directory.CreateDirectory(ResourcesFolder);
            }
        }

        public static void SaveEnumeratedObject<T>(IEnumerable<T> objects, string file, Formatting formatting)
        {
            string json = JsonConvert.SerializeObject(objects, formatting);
            File.WriteAllText(file, json);
        }

        public static void SaveEnumeratedObject<T>(IEnumerable<T> objects, string file, bool useIndentations)
        {
            var formatting = (useIndentations) ? Formatting.Indented : Formatting.None;
            SaveEnumeratedObject<T>(objects, file, formatting);
        }

        public static FileStream StoreObject(object obj, string file, Formatting formatting = Formatting.None)
        {
            try
            {
                string json = JsonConvert.SerializeObject(obj, formatting);
                string filePath = String.Concat(ResourcesFolder, "/", file);
                File.WriteAllText(filePath, json);
                var fileStream = File.OpenRead(filePath);
                return fileStream;
            }
            catch (Exception ex)
            {
                Utilities.Log("DataStorage.StoreObject", "Failed to store object", LogSeverity.Error, ex);
                return null;
            }
        }

        public static FileStream StoreObject(object obj, string file, bool useIndentations)
        {
            var formatting = (useIndentations) ? Formatting.Indented : Formatting.None;
            return StoreObject(obj, file, formatting);
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

        public static T RestoreObject<T>(string file)
        {
            string json = GetOrCreateFileContents(file);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static bool LocalFileExists(string file, bool buildFileIfNonExistant = false)
        {
            string filePath = String.Concat(ResourcesFolder, "/", file);
            if (buildFileIfNonExistant & !(File.Exists(filePath)))
            {
                File.WriteAllText(filePath, "");
                return false;
            }
            return File.Exists(filePath);
        }

        public static string[] GetFilesInFolder(string folder)
        {
            string folderPath = String.Concat(ResourcesFolder, "/", folder);
            string[] files = Directory.GetFiles(folderPath);
            int i;
            for (i = 0; i < files.Length; i++)
            {
                files[i] = Path.GetFileName(files[i]);
            }
            return files;
        }

        public static bool LocalFolderExists(string folder, bool buildFolderIfNonExistant = false)
        {
            string path = String.Concat(ResourcesFolder, "/", folder);
            if (buildFolderIfNonExistant & !(Directory.Exists(path)))
            {
                Directory.CreateDirectory(path);
                return false;
            }
            return Directory.Exists(path);
        }

        public static string GetText(string file)
        {
            return File.ReadAllText(ResourcesFolder + "/" + file);
        }

        public static void DeleteFile(string file)
        {
            File.Delete(ResourcesFolder + "/" + file);
        }

        public static FileStream GetFileStream(string file)
        {
            return File.OpenRead(ResourcesFolder + "/" + file);
        }

        private static string GetOrCreateFileContents(string file)
        {
            string filePath = String.Concat(ResourcesFolder, "/", file);
            if (!File.Exists(filePath))
            {
                Utilities.Log(MethodBase.GetCurrentMethod(), "File not found, creating " + filePath,
                    LogSeverity.Verbose);
                Directory.CreateDirectory(filePath);
                File.Create(filePath);
                return "";
            }
            return File.ReadAllText(filePath);
        }

        
    }
}
