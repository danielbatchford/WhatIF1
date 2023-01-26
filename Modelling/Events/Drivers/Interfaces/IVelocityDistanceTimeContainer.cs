namespace WhatIfF1.Modelling.Events.Drivers.Interfaces
{
    public interface IVelocityDistanceTimeContainer
    {
        int NumSamples { get; }
        int[] Ms { get; }
        double[] Velocity { get; }

        void GetLapDistanceAndVelocity(int lapMs, out double lapDistance, out double velocity);
    }
}
