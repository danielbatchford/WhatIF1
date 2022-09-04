using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatIfF1.Scenarios.Exceptions
{
    public class ScenarioException : Exception
    {
        public ScenarioException(string message) : base(message)
        {
        }
    }
}
