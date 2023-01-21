using System;
using System.Windows.Media;

namespace WhatIfF1.Logging.Interfaces
{
    public interface IUILogger
    {
        string CurrentMessage { get; set; }
        Color CurrentColor { get; set; }

        void Exception(Exception exception);
        void Info(object obj);
        void Warn(object obj);
        void Error(object obj);
    }
}
