using System;
using System.Collections.Generic;
using System.Linq;
using WhatIfF1.Modelling.Events.Drivers.Interfaces;
using WhatIfF1.Modelling.PitStops.Interfaces;
using WhatIfF1.UI.Controller.Interfaces;
using WhatIfF1.UI.Controller.Tires.Interfaces;
using WhatIfF1.Util;
using WhatIfF1.Util.Enumerables;

namespace WhatIfF1.UI.Controller.Tires
{
    public class PitStopDataProvider : NotifyPropertyChangedWrapper, IPitStopDataProvider, IDisposable
    {
        private readonly IEventController _parentController;

        public ObservableRangeCollection<ITireRun> TireRuns { get; }

        public ObservableRangeCollection<IPitStop> PitStops { get; }

        private int _noOfStops;
        public int NoOfStops
        {
            get => _noOfStops;
            set
            {
                if (_noOfStops == value)
                {
                    return;
                }

                _noOfStops = value;
                OnPropertyChanged();
            }
        }

        private int _fastestStopMs;
        public int FastestStopMs
        {
            get => _fastestStopMs;
            set
            {
                if (_fastestStopMs == value)
                {
                    return;
                }

                _fastestStopMs = value;
                OnPropertyChanged();
            }
        }

        private IDriver _targetDriver;

        public IDriver TargetDriver
        {
            get => _targetDriver;
            set
            {
                if (_targetDriver?.Equals(value) == true)
                {
                    return;
                }

                _targetDriver = value;

                if (value is null)
                {
                    NoOfStops = 0;
                    FastestStopMs = 0;
                }
                else
                {
                    UpdateCurrentDriverData(value);
                }

                OnPropertyChanged();
            }
        }

        public PitStopDataProvider(IEventController parentController)
        {
            _parentController = parentController;

            TireRuns = new ObservableRangeCollection<ITireRun>();
            PitStops = new ObservableRangeCollection<IPitStop>();

            _parentController.DataProvider.NoOfLapsChanged += DataProvider_NoOfLapsChanged;
        }

        private async void UpdateCurrentDriverData(IDriver driver)
        {
            (ITireCompound startCompound, IEnumerable<IPitStop> stopsEnumerable) = await _parentController.DataProvider.GetPitStopsForDriver(driver);

            PitStops.Clear();
            PitStops.AddRange(stopsEnumerable);

            var stops = stopsEnumerable.ToList();

            NoOfStops = stops.Count;
            FastestStopMs = stops.Min(stop => stop.PitTime);

            TireRuns.Clear();

            var newTireRuns = new List<ITireRun>
            {
                // Add run up to initial stop
                new TireRun(startCompound, 1, stops[0].InLap)
            };

            for (int i = 0; i < stops.Count - 1; i++)
            {
                IPitStop currStop = stops[i];
                IPitStop nextStop = stops[i + 1];

                int numLapsOnTire = nextStop.InLap - currStop.InLap;

                newTireRuns.Add(new TireRun(currStop.NewCompound, currStop.OutLap, numLapsOnTire));
            }

            int numDriverLaps = await _parentController.DataProvider.GetTotalLapsForDriver(driver);

            // Add run from last stop to end
            int numLapsOnLastTire = numDriverLaps - stops.Last().InLap;
            IPitStop lastStop = stops.Last();
            newTireRuns.Add(new TireRun(lastStop.NewCompound, lastStop.OutLap, numLapsOnLastTire));

            TireRuns.AddRange(newTireRuns);
        }

        public void UpdateProvider()
        {
        }

        public void Dispose()
        {
            _parentController.DataProvider.NoOfLapsChanged -= DataProvider_NoOfLapsChanged;
        }

        private void DataProvider_NoOfLapsChanged(object sender, Util.Events.ItemChangedEventArgs<int> e)
        {
            UpdateCurrentDriverData(TargetDriver);
        }
    }
}
