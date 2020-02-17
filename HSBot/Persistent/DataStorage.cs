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

        /// <summary>
        /// Automatically converts list to array. Make sure to close filestream.
        /// </summary>
        /// <param name="set"></param>
        /// <param name="file"></param>
        /// <param name="formatting"></param>
        /// <returns></returns>
        public static FileStream StoreStringArray(List<string> set, string file, Formatting formatting = Formatting.None, bool returnFileStream = false)
        {
            try
            {
                string filePath = String.Concat(Directory.GetCurrentDirectory(), "/", ResourcesFolder, "/", file);
                SerializerSettings.Formatting = formatting;
                JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
                using (StreamWriter sw = new StreamWriter(filePath))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, set.ToArray<string>());
                }
                Utilities.Log("DataStorage.StoreStringArray", $"{file} stored.", LogSeverity.Debug);
                if (returnFileStream) return File.OpenRead(filePath); else return null;
            }
            catch (Exception ex)
            {
                Utilities.Log("DataStorage.StoreStringArray", "Failed to store string array", LogSeverity.Error, ex);
                return null;
            }
        }

        /// <summary>
        /// Automatically converts list to array. Make sure to close filestream.
        /// </summary>
        /// <param name="set"></param>
        /// <param name="file"></param>
        /// <param name="useIndentations"></param>
        /// <returns></returns>
        public static FileStream StoreStringArray(List<string> set, string file, bool useIndentations, bool returnFileStream = false)
        {
            var formatting = (useIndentations) ? Formatting.Indented : Formatting.None;
            return StoreStringArray(set, file, formatting, returnFileStream);
        }

        /// <summary>
        /// Automatically converts list to array.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="noerror"></param>
        /// <returns></returns>
        public static List<string> LoadStringArray(string file, bool noerror = false)
        {
            string filePath = String.Concat(Directory.GetCurrentDirectory(), "/", ResourcesFolder, "/", file);
            if (!LocalFileExists(file))
            {
                if (!noerror) Utilities.Log(MethodBase.GetCurrentMethod(), "Failure to find datafile for " + file, Discord.LogSeverity.Critical);
                return null;
            }
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<string[]>(json).ToList<string>();
        }

        public static FileStream StoreEnumeratedObject<T>(IEnumerable<T> objects, string file, Formatting formatting = Formatting.None, bool returnFileStream = false)
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
                Utilities.Log("DataStorage.SaveEnumeratedObject", $"{file} stored.", LogSeverity.Debug);
                if (returnFileStream) return File.OpenRead(filePath); else return null;
            }
            catch (Exception ex)
            {
                Utilities.Log("DataStorage.SaveEnumeratedObject", "Failed to store objects", LogSeverity.Error, ex);
                return null;
            }
        }

        /// <summary>
        /// Make sure to close filestream.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objects"></param>
        /// <param name="file"></param>
        /// <param name="useIndentations"></param>
        /// <param name="returnFileStream"></param>
        /// <returns></returns>
        public static FileStream StoreEnumeratedObject<T>(IEnumerable<T> objects, string file, bool useIndentations, bool returnFileStream = false)
        {
            var formatting = (useIndentations) ? Formatting.Indented : Formatting.None;
            return StoreEnumeratedObject<T>(objects, file, formatting, returnFileStream);
        }

        /// <summary>
        /// Make sure to close filestream.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="file"></param>
        /// <param name="formatting"></param>
        /// <param name="returnFileStream"></param>
        /// <returns></returns>
        public static FileStream StoreObject(object obj, string file, Formatting formatting = Formatting.None, bool returnFileStream = false)
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
                if (returnFileStream) return File.OpenRead(filePath); else return null;
            }
            catch (Exception ex)
            {
                Utilities.Log("DataStorage.StoreObject", "Failed to store object", LogSeverity.Error, ex);
                return null;
            }
        }

        /// <summary>
        /// Make sure to close filestream.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="file"></param>
        /// <param name="useIndentations"></param>
        /// <param name="returnFileStream"></param>
        /// <returns></returns>
        public static FileStream StoreObject(object obj, string file, bool useIndentations, bool returnFileStream = false)
        {
            var formatting = (useIndentations) ? Formatting.Indented : Formatting.None;
            return StoreObject(obj, file, formatting, returnFileStream);
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

        /// <summary>
        /// Assumes all files in folder are .json deserializable objects of the same type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static List<T> GetObjectsInFolder<T>(string folder)
        {
            List<T> Objects = new List<T>();
            foreach (string file in GetFilesInFolder(folder, "*.json", false))
            {
                try
                {
                    Objects.Add(RestoreObject<T>(folder + "/" + file));
                }
                catch (Exception ex)
                {
                    Utilities.Log(MethodBase.GetCurrentMethod(), $"Unable to restore object from {folder}/{file}", ex,
                        LogSeverity.Error);
                }
            }
            return Objects;
        }

        //public static void StoreObjectsInFolder(string folder, List<dynamic> objects)

        public static string[] GetFilesInFolder(string folder, string criterion = "*.json", bool removeextension = true)
        {
            string folderPath = String.Concat(ResourcesFolder, "/", folder);
            DirectoryInfo d = new DirectoryInfo(folderPath);//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles(criterion); //Getting Text files
            string str = "";
            List<string> strs = new List<string>();
            foreach (FileInfo file in Files)
            {
                str = str + ", " + file.Name;
                if (removeextension) strs.Add(Path.GetFileNameWithoutExtension(file.Name));
                else strs.Add(file.Name);
            }
            return strs.ToArray();
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

        /// <summary>
        /// Make sure to close filestream!
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static FileStream GetFileStream(string file) => File.OpenRead(ResourcesFolder + "/" + file);

        private static string GetOrCreateFileContents(string file)
        {
            string filePath = String.Concat(ResourcesFolder, "/", file);
            if (!File.Exists(filePath))
            {
                Utilities.Log(MethodBase.GetCurrentMethod(), "File not found, creating " + filePath,
                    LogSeverity.Verbose);
                // Directory.CreateDirectory(filePath);
                File.Create(filePath);
                return "";
            }
            return File.ReadAllText(filePath);
        }

        
    }
}
