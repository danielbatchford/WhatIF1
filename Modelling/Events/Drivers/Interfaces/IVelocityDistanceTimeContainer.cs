namespace WhatIfF1.Modelling.Events.Drivers.Interfaces
{
    public interface IVelocityDistanceTimeContainer
    {
        void GetLapDistanceAndVelocity(int lapMs, out double lapDistance, out double velocity);
    }
}
