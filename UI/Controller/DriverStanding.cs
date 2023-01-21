using WhatIfF1.Modelling.Events.Drivers.Interfaces;
using WhatIfF1.Modelling.Tires.Interfaces;
using WhatIfF1.UI.Controller.Interfaces;
using WhatIfF1.Util;

namespace WhatIfF1.UI.Controller
{
    public class DriverStanding : NotifyPropertyChangedWrapper, IDriverStanding
    {
        private IDriver _driver;

        public IDriver Driver
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
                if (_gapToLead == value)
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
                if (_gapToNextCar == value)
                {
                    return;
                }
                _gapToNextCar = value;
                OnPropertyChanged();
            }
        }

        private ITireCompound _tireCompound;

        public ITireCompound TireCompound
        {
            get => _tireCompound;
            set
            {
                if (_tireCompound == value)
                {
                    return;
                }
                _tireCompound = value;
                OnPropertyChanged();
            }
        }

        private double _proportionOfLap;

        public double ProportionOfLap
        {
            get => _proportionOfLap;
            set
            {
                if (_proportionOfLap == value)
                {
                    return;
                }

                _proportionOfLap = value;
                OnPropertyChanged();
            }
        }

        private double _velocity;

        public double Velocity
        {
            get => _velocity;
            set
            {
                if (_velocity == value)
                {
                    return;
                }
                _velocity = value;
                OnPropertyChanged();
            }
        }

        public DriverStanding(IDriver driver, int racePosition, int gapToLead, int gapToNextCar, double proportionOfLap, double velocity, ITireCompound tireCompound)
        {
            Driver = driver;
            RacePosition = racePosition;
            GapToLead = gapToLead;
            GapToNextCar = gapToNextCar;
            ProportionOfLap = proportionOfLap;
            Velocity = velocity;
            TireCompound = tireCompound;
        }

        public bool Equals(IDriverStanding other)
        {
            if (other is null)
            {
                return false;
            }

            return Driver.Equals(other.Driver)
                && RacePosition.Equals(other.RacePosition)
                && GapToLead.Equals(other.GapToLead)
                && TireCompound.Equals(other.TireCompound)
                && ProportionOfLap.Equals(other.ProportionOfLap);
        }

        public override string ToString()
        {
            return $"{Driver} - P{RacePosition} - {TireCompound} - Gap: {GapToNextCar} - GapToLead: {GapToLead}";
        }
    }
}
