namespace WhatIfF1.UI.Controller.DataBuffering.Interfaces
{
    public interface IBufferedDataPacket
    {
        bool WasCacheHit { get; set; }
    }
}