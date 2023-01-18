using System;
using System.Collections.Generic;

namespace WhatIfF1.Modelling.Events.Drivers.Telemetry
{
    public class VelocityDistanceTimeContainer
    {

        public VelocityDistanceTimeContainer(IEnumerable<TelemetryTimeStamp> timeStamp)
        {
        }

        public void GetVDTData(int lapMs, out int forecastLapTime, out double lapFraction, out int lapDistance)
        {
            throw new NotImplementedException();
        }
    }
}
