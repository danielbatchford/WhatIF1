using System;

namespace WhatIfF1.UI.Windows
{
    public class WindowIntiializationException : Exception
    {
        public WindowIntiializationException(string message) : base(message)
        {
        }

        public WindowIntiializationException() : base()
        {
        }

        public WindowIntiializationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
