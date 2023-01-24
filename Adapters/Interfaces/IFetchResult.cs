namespace WhatIfF1.Adapters.Interfaces
{
    public interface IFetchResult<T>
    {
        T Data { get; }

        bool Success { get; }
    }
}
