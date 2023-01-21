using System.Threading.Tasks;

namespace WhatIfF1.Adapters
{
    public interface IApiCacheable<T>
    {
        string CachePath { get; }

        bool CachedDataExists { get; }

        Task<T> ApiFetchTask { get; }

        Task<T> GetDataTask();
    }
}
