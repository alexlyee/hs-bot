using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Discord;
using HSBot.Helpers;
using System.Globalization;
using System.Linq;

namespace HSBot.Persistent
{
    /// <summary>
    /// Hard Disk Persistence.
    /// </summary>
    public static class DataStorage
    {
        public const string ResourcesFolder = "Resources";
        public static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal },
                new StringEnumConverter { AllowIntegerValues = true }
            },
        };

        static DataStorage()
        {
            if (!Directory.Exists(ResourcesFolder))
            {
                Directory.CreateDirectory(ResourcesFolder);
            }
            Utilities.Log("DataStorage()", $"Resources folder found in {Directory.GetCurrentDirectory()}\\{ResourcesFolder}");
        }

        public static FileStream StoreStringArray(string[] set, string file, Formatting formatting = Formatting.None)
        {
            try
            {
                string filePath = String.Concat(Directory.GetCurrentDirectory(), "/", ResourcesFolder, "/", file);
                SerializerSettings.Formatting = formatting;
                JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
                using (StreamWriter sw = new StreamWriter(filePath))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, set);
                }
                var fileStream = File.OpenRead(filePath);
                Utilities.Log("DataStorage.StoreStringArray", $"{file} stored.", LogSeverity.Debug);
                return fileStream;
            }
            catch (Exception ex)
            {
                Utilities.Log("DataStorage.StoreStringArray", "Failed to store string array", LogSeverity.Error, ex);
                return null;
            }
        }

        public static FileStream StoreStringArray(string[] set, string file, bool useIndentations)
        {
            var formatting = (useIndentations) ? Formatting.Indented : Formatting.None;
            return StoreStringArray(set, file, formatting);
        }

        public static string[] LoadStringArray(string file, bool noerror = false)
        {
            string filePath = String.Concat(Directory.GetCurrentDirectory(), "/", ResourcesFolder, "/", file);
            if (!LocalFileExists(file))
            {
                if (!noerror) Utilities.Log(MethodBase.GetCurrentMethod(), "Failure to find datafile for " + file, Discord.LogSeverity.Critical);
                return null;
            }
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<string[]>(json);
        }

        public static FileStream StoreEnumeratedObject<T>(IEnumerable<T> objects, string file, Formatting formatting = Formatting.None)
        {
            try
            {
                string filePath = String.Concat(Directory.GetCurrentDirectory(), "/", ResourcesFolder, "/", file);
                SerializerSettings.Formatting = formatting;
                JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
                using (StreamWriter sw = new StreamWriter(filePath))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, objects);
                }
                var fileStream = File.OpenRead(filePath);
                Utilities.Log("DataStorage.SaveEnumeratedObject", $"{file} stored.", LogSeverity.Debug);
                return fileStream;
            }
            catch (Exception ex)
            {
                Utilities.Log("DataStorage.SaveEnumeratedObject", "Failed to store objects", LogSeverity.Error, ex);
                return null;
            }
        }

        public static FileStream StoreEnumeratedObject<T>(IEnumerable<T> objects, string file, bool useIndentations)
        {
            var formatting = (useIndentations) ? Formatting.Indented : Formatting.None;
            return StoreEnumeratedObject<T>(objects, file, formatting);
        }

        public static FileStream StoreObject(object obj, string file, Formatting formatting = Formatting.None)
        {
            try
            {
                string filePath = String.Concat(Directory.GetCurrentDirectory(), "/", ResourcesFolder, "/", file);
                SerializerSettings.Formatting = formatting;
                JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
                using (StreamWriter sw = new StreamWriter(filePath))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, obj);
                }
                var fileStream = File.OpenRead(filePath);
                Utilities.Log("DataStorage.StoreObject", $"{file} stored.", LogSeverity.Debug);
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

        public static IEnumerable<T> LoadEnumeratedObject<T>(string file)
        {
            if (!LocalFileExists(file))
            {
                Utilities.Log(MethodBase.GetCurrentMethod(), "Failure to find datafile for " + file, Discord.LogSeverity.Critical);
                return null;
            }
            string json = File.ReadAllText(file);
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
            for (int i = 0; i < files.Length; i++) files[i] = Path.GetFileName(files[i]);
            return files;
        }

        public static string[] GetFoldersInFolder(string folder)
        {
            string folderPath = String.Concat(ResourcesFolder, "/", folder);
            string[] dirs = Directory.GetDirectories(folderPath);
            for (int i = 0; i < dirs.Length; i++)
                dirs[i] = dirs[i].Remove(0, dirs[i].LastIndexOf('\\') + 1);
            return dirs;
        }

        /// <summary>
        /// Returns false only if directory had to be created, or if it doesn't exist.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="buildFolderIfNonExistant"></param>
        /// <returns></returns>
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

        public static string GetText(string file) => File.ReadAllText(ResourcesFolder + "/" + file);

        public static void DeleteFile(string file) => File.Delete(ResourcesFolder + "/" + file);

        public static void DeleteFolder(string folder) => Directory.Delete(ResourcesFolder + "/" + folder);

        public static FileStream GetFileStream(string file) => File.OpenRead(ResourcesFolder + "/" + file);

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
