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
    public class APIEventCacheWorker : IApiCacheable<FetchResult>
    {
        public readonly static string _cacheRoot = FileAdapter.Instance.CacheRoot;

        private readonly string _debugTypeString;

        public Task<FetchResult> ApiFetchTask { get; }

        public string CachePath { get; }

        public bool CachedDataExists { get; }

        public APIEventCacheWorker(Task<FetchResult> apiFetchTask, string localCacheDirName, string debugTypeString, string cacheFileName)
        {
            ApiFetchTask = apiFetchTask;

            _debugTypeString = debugTypeString.ToLower();

            string cacheDir = Path.Combine(_cacheRoot, localCacheDirName);

            if (!Directory.Exists(cacheDir))
            {
                Directory.CreateDirectory(cacheDir);
                Logger.Instance.Info($"Created the cache directory at \"{cacheDir}\" as it did not exist");
            }

            CachePath = Path.Combine(cacheDir, cacheFileName);

            CachedDataExists = File.Exists(CachePath);
        }

        public Task<FetchResult> GetDataTask()
        {
            if (FileAdapter.Instance.UseCaching && CachedDataExists)
            {
                Logger.Instance.Info($"Loading cached {_debugTypeString} data from \"{CachePath}\"");
                return ReadFromCache();
            }
            else
            {
                Logger.Instance.Info($"Loading {_debugTypeString} data from API");
                ApiFetchTask.ContinueWith(async fetchResultTask =>
                {
                    if (fetchResultTask.IsFaulted)
                    {
                        return;
                    }

                    FetchResult result = fetchResultTask.Result;

                    if (!result.Success)
                    {
                        return;
                    }

                    await WriteToCache(result.Data);

                });

                return ApiFetchTask;
            }
        }

        private async Task<bool> WriteToCache(JToken token)
        {
            Logger.Instance.Info($"Writing data to cache at \"{CachePath}\"");

            try
            {
                using (FileStream stream = new FileStream(CachePath, FileMode.Create))
                {
                    using (StreamWriter streamWriter = new StreamWriter(stream))
                    {
                        await streamWriter.WriteAsync(token.ToString(Formatting.Indented));
                    }
                }

                Logger.Instance.Info($"Wrote data to cache at \"{CachePath}\"");

                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.Error($"Failed to write data to cache at \"{CachePath}\"");
                Logger.Instance.Exception(e);

                return false;
            }
        }

        private async Task<FetchResult> ReadFromCache()
        {
            using (StreamReader file = File.OpenText(CachePath))
            {
                using (JsonTextReader jsonReader = new JsonTextReader(file))
                {
                    JToken loaded = await JToken.ReadFromAsync(jsonReader);
                    return new FetchResult(loaded);
                }
            }
        }
    }
}
