using WhatIfF1.Modelling.Events.Drivers;
using WhatIfF1.Modelling.Events.Drivers.Interfaces;
using WhatIfF1.Modelling.Tires.Interfaces;
using WhatIfF1.UI.Controller.Interfaces;
using WhatIfF1.Util;
using WhatIfF1.Util.Extensions;

namespace WhatIfF1.UI.Controller
{
    public class DriverStanding : NotifyPropertyChangedWrapper, IDriverStanding
    {
        public static IDriverStanding GetNonRunningStanding(Driver driver, RunningState nonRunningState)
        {
            return new DriverStanding(driver, nonRunningState);
        }

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
                UpdateTimingScreenTextAndOpacity();
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

        private RunningState _state;

        public RunningState State
        {
            get { return _state; }
            set
            {
                if (_state == value)
                {
                    return;
                }
                _state = value;
                OnPropertyChanged();
            }
        }

        private string _timingScreenText;

        public string TimingScreenText
        {
            get => _timingScreenText;
            set
            {
                if (_timingScreenText?.Equals(value) == true)
                {
                    return;
                }
                _timingScreenText = value;
                OnPropertyChanged();
            }
        }

        private double _timingScreenTextOpacity;

        public double TimingScreenTextOpacity
        {
            get => _timingScreenTextOpacity;
            set
            {
                if (_timingScreenTextOpacity == value)
                {
                    return;
                }
                _timingScreenTextOpacity = value;
                OnPropertyChanged();
            }
        }

        private DriverStanding(Driver driver, RunningState state)
        {
            Driver = driver;
            State = state;
        }

        public DriverStanding(IDriver driver, int racePosition, int gapToLead, int gapToNextCar, double proportionOfLap, double velocity, ITireCompound tireCompound, RunningState state)
        {
            _driver = driver;
            _racePosition = racePosition;
            _gapToLead = gapToLead;
            _gapToNextCar = gapToNextCar;
            _proportionOfLap = proportionOfLap;
            _velocity = velocity;
            _tireCompound = tireCompound;
            _state = state;

            // Update timing screen text initially
            UpdateTimingScreenTextAndOpacity();
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
                && ProportionOfLap.Equals(other.ProportionOfLap)
                && State.Equals(other.State);
        }

        private void UpdateTimingScreenTextAndOpacity()
        {
            switch (State)
            {
                case RunningState.RETIRED:
                    TimingScreenText = "OUT";
                    TimingScreenTextOpacity = 0.7;
                    break;

                case RunningState.FINISHED:
                    TimingScreenText = "Finished";
                    TimingScreenTextOpacity = 1;
                    break;

                case RunningState.RUNNING:
                    TimingScreenTextOpacity = 1;

                    if (RacePosition == 1)
                    {
                        TimingScreenText = "Interval";
                    }
                    else
                    {
                        TimingScreenText = StringExtensions.ToF1TimingScreenFormat(GapToLead);
                    }
                    break;
            }
        }

        public override string ToString()
        {
            return $"{Driver} - P{RacePosition} - {TireCompound} - Gap: {GapToNextCar} - GapToLead: {GapToLead}";
        }
    }
}
