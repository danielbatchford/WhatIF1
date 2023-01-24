using System.Collections.Generic;
using System.Threading.Tasks;

namespace WhatIfF1.UI.Controller.DataBuffering.Interfaces
{
    public interface IBufferableDataProvider<TPacket> where TPacket : IBufferedDataPacket
    {
        int MinFrame { get; }
        int MaxFrame { get; }

        int NoOfBufferedFrames { get; }

        Task<TPacket> GetDataAtTime(int frame);

        IDictionary<int, TPacket> Buffer { get; }

        void Invalidate();
    }
}
