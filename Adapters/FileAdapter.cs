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

        private FileAdapter()
        {
            // TODO - THIS!!!!
            _resourcesRoot = Path.Combine(@"C:\Users\Daniel Batchford\Desktop\WhatIF1Root\WhatIfF1", "Resources");

            if (!Directory.Exists(_resourcesRoot))
            {
                throw new DirectoryNotFoundException($"Could not find the resources directory at {_resourcesRoot}");
            }

            CountryFlagsRoot = Path.Combine(_resourcesRoot, "Flags");

            if (!Directory.Exists(CountryFlagsRoot))
            {
                throw new DirectoryNotFoundException($"Could not find the flags directory at {CountryFlagsRoot}");
            }

            TrackLayoutsRoot = Path.Combine(_resourcesRoot, "Tracks");

            if (!Directory.Exists(TrackLayoutsRoot))
            {
                throw new DirectoryNotFoundException($"Could not find the tracks directory at {TrackLayoutsRoot}");
            }
        }

    }
}
