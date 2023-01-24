using System;

namespace WhatIfF1.Scenarios.Exceptions
{
    public class ScenarioException : Exception
    {
        public ScenarioException()
        {
        }

        public ScenarioException(string message) : base(message)
        {
        }

        public ScenarioException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
