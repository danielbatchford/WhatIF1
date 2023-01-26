using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WhatIfF1.Logging;

namespace WhatIfF1.Adapters
{
    public sealed class FileAdapter
    {
        #region LazyInitialization

        public static FileAdapter Instance => _lazy.Value;

        private readonly static Lazy<FileAdapter> _lazy = new Lazy<FileAdapter>(() => new FileAdapter());

        #endregion LazyInitialization

        private readonly string _resourcesRoot;

        public string CountryFlagsRoot { get; }

        public string TrackLayoutsRoot { get; }

        public string DriverPicsRoot { get; }

        public string ConstructorPicsRoot { get; }

        public string TrackMarkerIconsRoot { get; }

        public string TiresRoot { get; }

        public string CacheRoot { get; }

        public bool UseCaching { get; }

        private FileAdapter()
        {
            _resourcesRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");

            if (!Directory.Exists(_resourcesRoot))
            {
                throw new DirectoryNotFoundException($"Could not find the resources directory at {_resourcesRoot}");
            }

            CountryFlagsRoot = GetAndDebugResourcePath("Flags", "flags");
            TrackLayoutsRoot = GetAndDebugResourcePath("Tracks", "tracks");
            DriverPicsRoot = GetAndDebugResourcePath("Drivers", "drivers");
            ConstructorPicsRoot = GetAndDebugResourcePath("Constructors", "constructors");
            TrackMarkerIconsRoot = GetAndDebugResourcePath("TrackMarkers", "track markers");
            TiresRoot = GetAndDebugResourcePath("Tires", "tires");

            string roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            CacheRoot = Path.Combine(roamingPath, ((App)System.Windows.Application.Current).AppName);

            // Can be set accordingly
            UseCaching = true;

            if (UseCaching && !Directory.Exists(CacheRoot))
            {
                Directory.CreateDirectory(CacheRoot);
                Logger.Instance.Info($"Created cache root at \"{CacheRoot}\" as it did not exist");
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

        private string GetAndDebugResourcePath(string folderName, string debugName)
        {
            string fullPath = Path.Combine(_resourcesRoot, folderName);

            if (!Directory.Exists(fullPath))
            {
                throw new DirectoryNotFoundException($"Could not find the {debugName} directory at {fullPath}");
            }

            return fullPath;
        }
    }
}
