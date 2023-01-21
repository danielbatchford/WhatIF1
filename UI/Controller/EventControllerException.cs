using System;

namespace WhatIfF1.UI.Controller
{
    public class EventControllerException : Exception
    {
        public EventControllerException(string message) : base(message)
        {
        }

        public EventControllerException() : base()
        {
        }

        public EventControllerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
