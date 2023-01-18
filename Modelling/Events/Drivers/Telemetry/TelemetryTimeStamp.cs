namespace WhatIfF1.Modelling.Events.Drivers.Telemetry
{
    public struct TelemetryTimeStamp
    {
        public int Ms { get; set; }

        public double Velocity { get; set; }

        public override string ToString()
        {
            return $"{Ms}ms: {Velocity}kph";
        }
    }
}
