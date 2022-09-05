using System;

namespace WhatIfF1.Scenarios.Exceptions
{
    public class ScenarioLoadException : Exception
    {
        public ScenarioLoadException(string message) : base(message)
        {
        }
    }
}
