using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Controls;
using WhatIfF1.Modelling.Events;
using WhatIfF1.Modelling.Events.Drivers;
using WhatIfF1.Modelling.Tires;
using WhatIfF1.Modelling.Tracks;
using WhatIfF1.UI.Controller.TrackMaps;
using WhatIfF1.Util;
using WhatIfF1.Util.Enumerables;

namespace WhatIfF1.UI.Controller
{
    public class EventController : PropertyChangedWrapper
    {
        private EventModel _model;

        public EventModel Model
        {
            get => _model;
            set
            {
                _model = value;
                OnPropertyChanged();
            }
        }

        private int _currentTime;

        public ObservableRangeCollection<DriverStanding> Standings { get; }

        public TrackMapProvider MapProvider { get; }

        public int CurrentTime
        {
            get => _currentTime;
            set
            {
                if (value < 0)
                {
                    throw new EventControllerException($"Requested time was less than 0. (Got {value})");
                }

                // No need to update if the value equals the current time
                if (value == _currentTime)
                {
                    return;
                }

                // If the current time exceeds the maximum time in the model, throw an exception
                if (value > Model.TotalTime)
                {
                    throw new EventControllerException($"Requested current time exceeds the maximum model time (Requested {value}, Max time is {Model.TotalTime}");
                }

                _currentTime = value;

                UpdateAtTime();
                OnPropertyChanged();
            }
        }

        private int _currentLap;

        public int CurrentLap
        {
            get => _currentLap;
            private set
            {
                if (value > Model.NoOfLaps)
                {
                    throw new EventControllerException($"Attempted to set max lap to {value} while only {Model.NoOfLaps} existed");
                }

                _currentLap = value;
                OnPropertyChanged();
            }
        }

        public EventController(Track track, EventModel model)
        {
            Model = model;

            // Retrieve drivers : TODO - order them based on grid position

            // TODO - tire compound

            var drivers = Model.GetDrivers().ToList();
            int numDrivers = drivers.Count();

            var driverStandings = new List<DriverStanding>(numDrivers);

            for (int i = 0; i < numDrivers; i++)
            {
                int racePos = i + 1;
                int gapToLead = 0;
                int gapToNextCar = 0;
                TireCompound tireCompound = TireCompoundStore.MediumTyre;

                driverStandings.Add(new DriverStanding(drivers[i], racePos, gapToLead, gapToNextCar, tireCompound));
            }

            Standings = new ObservableRangeCollection<DriverStanding>(driverStandings);

            MapProvider = new TrackMapProvider(track, drivers);

            CurrentTime = 0;
        }

        private void UpdateAtTime()
        {
            var driverPositions = new List<(Driver, Position)>();

            foreach (DriverStanding standing in Standings)
            {
                if(Model.TryGetPositionAtTime(standing.Driver, CurrentTime, out Position driverPos))
                {
                    driverPositions.Add((standing.Driver, driverPos));
                }
            }

            if(driverPositions.Count == 0)
            {
                return;
            }

            // Order drivers by position
            driverPositions.Sort((dPosA, dPosB) =>
            {
                (_, Position posA) = dPosA;
                (_, Position posB) = dPosB;

                if (dPosA == dPosB)
                {
                    return 0;
                }

                return -Math.Sign(posA.TotalDistance - posB.TotalDistance);
            });

            var newStandings = new List<DriverStanding>(driverPositions.Count);

            // Set the lead time to the standing of the first driver
            int leadTime = driverPositions[0].Item2.TotalMs;

            // TODO - tire compound changes

            // Build new driver standings

            for (int i = 0; i < driverPositions.Count; i++)
            {
                int racePos = i + 1;

                Driver driver = driverPositions[i].Item1;
                Position driverPos = driverPositions[i].Item2;

                int gapToNextCar = 0;
                int gapToLead = 0;

                var newStanding = new DriverStanding(driver, racePos, gapToLead, gapToNextCar, TireCompoundStore.SoftTyre);

                var oldStandingIndex = Standings.IndexOf(Standings.Single(s => s.Driver.Equals(driver)));

                // Update this driver's standing if it has changed
                if (!Standings[oldStandingIndex].Equals(newStanding)) 
                {
                    Standings[oldStandingIndex] = newStanding;
                }

                // Update this driver's position on the map
                MapProvider.UpdateDriverMapPosition(driver, driverPos);
            }

            // Update the current lap based off the leading driver
            CurrentLap = driverPositions[0].Item2.Lap;
        }
    }
}
