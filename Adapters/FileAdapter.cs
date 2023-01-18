using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatIfF1.Logging;

namespace WhatIfF1.Adapters
{
    public sealed class FileAdapter
    {

        #region LazyInitialization

        public static FileAdapter Instance => _lazy.Value;

        private readonly static Lazy<FileAdapter> _lazy = new Lazy<FileAdapter>(() => new FileAdapter());

        #endregion LazyInitialization

        private string _resourcesRoot;

        public string CountryFlagsRoot { get; }

        public string TrackLayoutsRoot { get; }

        public string DriverPicsRoot { get; }

        public string ConstructorPicsRoot { get; }

        public string TelemetryCacheRoot { get; }

        private FileAdapter()
        {
            // TODO - THIS!!!!
            _resourcesRoot = Path.Combine(@"C:\Users\Daniel Batchford\Desktop\WhatIF1Root\WhatIfF1", "Resources");

            if (!Directory.Exists(_resourcesRoot))
            {
                throw new DirectoryNotFoundException($"Could not find the resources directory at {_resourcesRoot}");
            }

            CountryFlagsRoot = GetAndDebugResourcePath("Flags", "flags");
            TrackLayoutsRoot = GetAndDebugResourcePath("Tracks", "tracks");
            DriverPicsRoot = GetAndDebugResourcePath("Drivers", "drivers");
            ConstructorPicsRoot = GetAndDebugResourcePath("Constructors", "constructors");

            string roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            TelemetryCacheRoot = Path.Combine(roamingPath, ((App)System.Windows.Application.Current).AppName, "TelemetryCache");

            if (!Directory.Exists(TelemetryCacheRoot))
            {
                Directory.CreateDirectory(TelemetryCacheRoot);
                Logger.Instance.Info($"Created cache root at \"{TelemetryCacheRoot}\" as it did not exist");
            }
        }

        public IEnumerable<string> ReadLines(string path, bool ignoreEmptyLines = false)
        {
            IEnumerable<string> lines = File.ReadAllLines(path, Encoding.UTF8);

            if (ignoreEmptyLines)
            {
                return lines.Where(line => !string.IsNullOrEmpty(line));
            }
            else
            {
                return lines;
            }
        }

        public bool TelemetryCacheFileExists(string eventName, DateTime eventDate)
        {
            return File.Exists(GetCacheFileName(eventName, eventDate));
        }

        public async Task<FetchResult> LoadTelemetryCacheFileAsync(string eventName, DateTime eventDate)
        {
            string cachePath = GetCacheFileName(eventName, eventDate);

            using (StreamReader file = File.OpenText(cachePath))
            {
                using (JsonTextReader jsonReader = new JsonTextReader(file))
                {
                    JToken loaded = await JToken.ReadFromAsync(jsonReader);
                    return new FetchResult(loaded);
                }
            }
        }

        public void WriteTelemetryCacheFile(string eventName, DateTime eventDate, JToken telemetryJson)
        {
            string cachePath = GetCacheFileName(eventName, eventDate);

            using (FileStream stream = new FileStream(cachePath, FileMode.Create))
            {
                using (StreamWriter streamWriter = new StreamWriter(stream))
                {
                    streamWriter.Write(telemetryJson.ToString(Formatting.Indented));
                }
            }
        }

        private string GetAndDebugResourcePath(string folderName, string debugName)
        {

            string fullPath = Path.Combine(_resourcesRoot, folderName);

            if (!Directory.Exists(fullPath))
            {
                throw new DirectoryNotFoundException($"Could not find the {debugName} directory at {fullPath}");
            }

            return fullPath;
        }

        private string GetCacheFileName(string eventName, DateTime eventDate)
        {
            return Path.Combine(TelemetryCacheRoot, $"{eventName} - {eventDate.Year}.json");
        }
    }
}
