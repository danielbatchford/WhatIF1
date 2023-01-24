using System;

namespace WhatIfF1.UI.Controller.Graphing.SeriesData.Interfaces
{
    public interface IXYDataPoint<T> : IEquatable<IXYDataPoint<T>>
    {
        T XValue { get; }
        T YValue { get; }
    }
}
