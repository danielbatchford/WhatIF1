using System.Collections.Generic;
using System.Threading.Tasks;
using WhatIfF1.Logging;
using WhatIfF1.Modelling.Events.Interfaces;
using WhatIfF1.UI.Controller.DataBuffering.Interfaces;
using WhatIfF1.UI.Controller.Interfaces;

namespace WhatIfF1.UI.Controller.DataBuffering
{
    public class EventModelDataProvider : IEventModelDataProvider
    {
        private const int _lookaheadAmount = 20;
        private const int _maxCacheSize = 1000;
        private const int _adjacentSkipAmount = 2;

        public int MinFrame { get; }

        public int MaxFrame { get; }

        public IEventModel Model { get; }

        public IDictionary<int, IEventModelDataPacket> Buffer { get; }

        public int NoOfBufferedFrames => Buffer.Count;

        private readonly IPlaybackParameterContainer _playbackParameters;

        public EventModelDataProvider(IEventModel eventModel, IPlaybackParameterContainer playbackParameters)
        {
            Model = eventModel;

            MinFrame = 0;
            MaxFrame = eventModel.TotalTime;

            _playbackParameters = playbackParameters;

            Buffer = new Dictionary<int, IEventModelDataPacket>(_maxCacheSize);
        }

        public async Task<IEventModelDataPacket> GetDataAtTime(int requestedMs)
        {
            if (Buffer.TryGetValue(requestedMs, out IEventModelDataPacket packet))
            {
                packet.WasCacheHit = true;
                return packet;
            }
            else
            {
                packet = await LoadPacket(requestedMs);

                // Cache this packet
                lock (Buffer)
                {
                    Buffer.Add(requestedMs, packet);
                }

                TryRemoveOldestFramesFromCache();

                // Begin loading next packets
                _ = Task.Run(() => LoadNextNPackets(requestedMs + _playbackParameters.MsIncrement));

                return packet;
            }
        }

        public void Invalidate()
        {
            lock (Buffer)
            {
                Buffer.Clear();
            }
        }

        private bool TryRemoveOldestFramesFromCache()
        {
            // Currently removes all buffered frames

            if (NoOfBufferedFrames <= _maxCacheSize)
            {
                return false;
            }

            lock (Buffer)
            {
                Buffer.Clear();
            }

            Logger.Instance.Debug($"Removed {_maxCacheSize} frames from cache");

            return true;
        }

        private Task<IEventModelDataPacket> LoadPacket(int ms)
        {
            return Task.Run(() =>
            {
                var standings = Model.GetStandingsAtTime(ms, out int currentLap);
                return (IEventModelDataPacket)new EventModelDataPacket(standings, currentLap, false);
            });
        }

        private async Task<bool> LoadNextNPackets(int startMs)
        {
            int currMs = startMs + _playbackParameters.MsIncrement * _adjacentSkipAmount;

            for (int i = 0; i < _lookaheadAmount; i++)
            {
                if (currMs > MaxFrame)
                {
                    return true;
                }

                if (Buffer.ContainsKey(currMs))
                {
                    continue;
                }

                var packet = await LoadPacket(currMs);

                // Cache this packet
                lock (Buffer)
                {
                    Buffer.Add(currMs, packet);
                }

                currMs += _playbackParameters.MsIncrement * _adjacentSkipAmount;
            }

            TryRemoveOldestFramesFromCache();

            return true;
        }
    }
}
