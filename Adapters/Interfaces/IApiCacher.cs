using System.Threading.Tasks;

namespace WhatIfF1.Adapters.Interfaces
{
    public interface IApiCacher<T>
    {
        string CachePath { get; }

        bool CachedDataExists { get; }

        Task<T> ApiFetchTask { get; }

        Task<T> GetDataTask();
    }
}
