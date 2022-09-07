using System;
using System.Collections.Generic;
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

                _currentTime = value;

                UpdateAtTime();
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
                TireCompound tireCompound = TireCompoundStore.SoftTyre;

                driverStandings.Add(new DriverStanding(drivers[i], racePos, gapToLead, gapToNextCar, tireCompound));
            }

            Standings = new ObservableRangeCollection<DriverStanding>(driverStandings);

            // TODO - n drivers
            MapProvider = new TrackMapProvider(track, driverStandings.ToList()[0].Driver);

            CurrentTime = 0;
        }

        private void UpdateAtTime()
        {
            var driverPositions = new List<(Driver, Position)>(Model.NumDrivers);

            foreach (DriverStanding standing in Standings)
            {
                Position driverPos = Model.GetPositionAtTime(standing.Driver, CurrentTime);
                driverPositions.Add((standing.Driver, driverPos));
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

                return Math.Sign(posA.TotalDistance - posB.TotalDistance);
            });

            var newStandings = new List<DriverStanding>(Model.NumDrivers);

            // TODO - lead time
            const int leadTime = 100;

            // TODO - tire compound changes
            // Build new driver standings

            for (int i = 0; i < Model.NumDrivers; i++)
            {
                int racePos = i + 1;

                Driver driver = driverPositions[i].Item1;
                Position driverPos = driverPositions[i].Item2;

                int gapToNextCar = 0;

                newStandings.Add(new DriverStanding(driver, racePos, leadTime - driverPos.TotalMs, gapToNextCar, TireCompoundStore.SoftTyre));

                MapProvider.UpdateDriverMapPosition(driver, driverPos);
            }

            // If driver standings has changed, replace the collection
            if (Standings.SequenceEqual(newStandings))
            {
                Standings.ReplaceRange(newStandings);
            }
        }
    }
}
