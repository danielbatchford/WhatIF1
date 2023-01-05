using System;

namespace WhatIfF1.UI.Controller.Graphing.SeriesData
{
    public class XYDataPoint<T> : IEquatable<XYDataPoint<T>>
    {
        public T XValue { get; }
        public T YValue { get; }

        public XYDataPoint(T xValue, T yValue)
        {
            XValue = xValue;
            YValue = yValue;
        }

        public override string ToString()
        {
            return $"{XValue}, {YValue}";
        }

        public bool Equals(XYDataPoint<T> other)
        {
            return other.XValue.Equals(XValue) && other.YValue.Equals(YValue);
        }
    }
}
