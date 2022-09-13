using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

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

        public static IEnumerable<string> ReadLines(string path, bool ignoreEmptyLines = false)
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
    }
}
