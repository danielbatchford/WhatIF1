using WhatIfF1.Modelling.Events.Drivers.Telemetry.Interfaces;

namespace WhatIfF1.Modelling.Events.Drivers.Telemetry
{
    public struct TelemetryTimeStamp : IMsTimeStamp
    {
        public int Ms { get; set; }

        public double Velocity { get; set; }

        public double Value { get => Velocity; set => Velocity = value; }

        public override string ToString()
        {
            return $"{Ms}ms: {Value}kph";
        }
    }
}
