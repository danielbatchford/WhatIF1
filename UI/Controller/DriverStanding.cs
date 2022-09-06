using System;
using WhatIfF1.Modelling.Events.Drivers;
using WhatIfF1.Modelling.Tires;
using WhatIfF1.Util;

namespace WhatIfF1.UI.Controller
{
    public class DriverStanding : PropertyChangedWrapper, IEquatable<DriverStanding>
    {
        private Driver _driver;

        public Driver Driver
        {
            get => _driver;
            set 
            {
                _driver = value;
                OnPropertyChanged();
            }
        }

        private int _racePosition;

        public int RacePosition
        {
            get => _racePosition;
            set
            {
                if (value <= 0)
                {
                    throw new EventControllerException($"Attempted to set race position to <= 0. (Got {value}");
                }

                _racePosition = value;
                OnPropertyChanged();
                _racePosition = value;
                OnPropertyChanged();
            }
        }

        private int _gapToLead;

        public int GapToLead
        {
            get => _gapToLead;
            set 
            {
                _gapToLead = value;
                OnPropertyChanged();
            }
        }

        private TireCompound _tireCompound;

        public TireCompound TireCompound
        {
            get => _tireCompound;
            set 
            {
                _tireCompound = value;
                OnPropertyChanged();
            }
        }

        public DriverStanding(Driver driver, int racePosition, int gapToLead, TireCompound tireCompound)
        {
            Driver = driver;
            RacePosition = racePosition;
            GapToLead = gapToLead;
            TireCompound = tireCompound;
        }

        public bool Equals(DriverStanding other)
        {
            return Driver.Equals(other.Driver)
                && RacePosition.Equals(other.RacePosition)
                && GapToLead.Equals(other.GapToLead)
                && TireCompound.Equals(other.TireCompound);
        }
    }
}
