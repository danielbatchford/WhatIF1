using System;
using System.IO;

namespace WhatIfF1.Adapters
{
    public class FileAdapter
    {

        #region LazyInitialization

        public static FileAdapter Instance => _lazy.Value;

        private readonly static Lazy<FileAdapter> _lazy = new Lazy<FileAdapter>(() => new FileAdapter());

        #endregion LazyInitialization

        private string _resourcesRoot;

        public string CountryFlagsRoot { get; }

        public string TrackLayoutsRoot { get; }

        public string DriverPicsRoot { get; }

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
