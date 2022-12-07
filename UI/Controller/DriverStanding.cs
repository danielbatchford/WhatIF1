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
                if(_gapToLead == value)
                {
                    return;
                }
                _gapToLead = value;
                OnPropertyChanged();
            }
        }

        private int _gapToNextCar;

        public int GapToNextCar
        {
            get => _gapToNextCar;
            set
            {
                if(_gapToNextCar == value)
                {
                    return;
                }
                _gapToNextCar = value;
                OnPropertyChanged();
            }
        }

        private TireCompound _tireCompound;

        public TireCompound TireCompound
        {
            get => _tireCompound;
            set 
            {
                if(_tireCompound == value)
                {
                    return;
                }
                _tireCompound = value;
                OnPropertyChanged();
            }
        }

        public DriverStanding(Driver driver, int racePosition, int gapToLead, int gapToNextCar, TireCompound tireCompound)
        {
            Driver = driver;
            RacePosition = racePosition;
            GapToLead = gapToLead;
            GapToNextCar = gapToNextCar;
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
