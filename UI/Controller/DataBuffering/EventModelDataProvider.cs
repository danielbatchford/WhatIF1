using System.Collections.Generic;
using System.Threading.Tasks;
using WhatIfF1.Logging;
using WhatIfF1.Modelling.Events.Drivers.Interfaces;
using WhatIfF1.Modelling.Events.Interfaces;
using WhatIfF1.Modelling.PitStops.Interfaces;
using WhatIfF1.Modelling.TrackStates.Interfaces;
using WhatIfF1.UI.Controller.DataBuffering.Interfaces;
using WhatIfF1.UI.Controller.Interfaces;
using WhatIfF1.Util.Events;

namespace WhatIfF1.UI.Controller.DataBuffering
{
    public class EventModelDataProvider : IEventModelDataProvider
    {
        private const int _lookaheadAmount = 20;
        private const int _maxCacheSize = 1000;
        private const int _adjacentSkipAmount = 2;

        public int MinFrame { get; }

        public int MaxFrame { get; }

        public IDictionary<int, IEventModelDataPacket> Buffer { get; }

        public int NoOfBufferedFrames => Buffer.Count;

        private readonly IEventModel _model;
        private readonly IPlaybackParameterContainer _playbackParameters;

        public EventModelDataProvider(IEventModel model, IPlaybackParameterContainer playbackParameters)
        {
            _model = model;
            _model.TotalTimeChanged += Model_TotalTimeChanged;
            _model.NoOfLapsChanged += Model_NoOfLapsChanged;

            MinFrame = 0;
            MaxFrame = model.TotalTime;

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

        public async Task<int> GetCurrentLapForDriver(int ms, IDriver targetDriver)
        {
            int driverLap = default;
            var result = await Task.Run(() => _model.TryGetCurrentLapForDriver(ms, targetDriver, out driverLap));
            return driverLap;
        }

        public Task<(ITireCompound startCompound, IEnumerable<IPitStop> stops)> GetPitStopsForDriver(IDriver driver)
        {
            var stops = _model.GetPitStopsForDriver(driver, out ITireCompound startCompound);
            return Task.Run(() => (startCompound, stops));
        }

        public Task<int> GetTotalLapsForDriver(IDriver driver)
        {
            return Task.Run(() => _model.GetTotalLapsForDriver(driver));
        }

        public IEnumerable<ITrackState> GetTrackStates()
        {
            return _model.GetTrackStates();
        }

        public Task<IVelocityDistanceTimeContainer> GetVDTContainer(IDriver driver, int lap)
        {
            return Task.Run(() => _model.GetVDTContainer(driver, lap));
        }

        public void Invalidate()
        {
            lock (Buffer)
            {
                Buffer.Clear();
            }
        }

        public void Dispose()
        {
            TotalTimeChanged -= Model_TotalTimeChanged;
            NoOfLapsChanged -= Model_NoOfLapsChanged;
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
                var standings = _model.GetStandingsAtTime(ms, out int currentLap, out ITrackState trackState);
                return (IEventModelDataPacket)new EventModelDataPacket(standings, currentLap, trackState, false);
            });
        }

        private async Task<bool> LoadNextNPackets(int startMs)
        {
            int currMs = startMs + (_playbackParameters.MsIncrement * _adjacentSkipAmount);

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

        private void Model_TotalTimeChanged(object sender, ItemChangedEventArgs<int> e)
        {
            TotalTimeChanged?.Invoke(sender, e);
        }

        private void Model_NoOfLapsChanged(object sender, ItemChangedEventArgs<int> e)
        {
            NoOfLapsChanged?.Invoke(sender, e);
        }

        public event ItemChangedEventHandler<int> TotalTimeChanged;

        public event ItemChangedEventHandler<int> NoOfLapsChanged;
    }
}
