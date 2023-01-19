using System;

namespace WhatIfF1.Scenarios.Exceptions
{
    public class ScenarioException : Exception
    {
        public ScenarioException(string message) : base(message)
        {
        }
    }
}
