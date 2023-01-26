using System.Collections.Generic;
using WhatIfF1.Modelling.Events.Drivers.Interfaces;
using WhatIfF1.UI.Controller.Graphing.Interfaces;
using WhatIfF1.UI.Controller.Graphing.SeriesData.Interfaces;
using WhatIfF1.UI.Controller.Interfaces;
using WhatIfF1.Util;
using WhatIfF1.Util.Enumerables;

namespace WhatIfF1.UI.Controller.Graphing
{
    public abstract class XYGraph : NotifyPropertyChangedWrapper, IXYGraph
    {
        public ObservableRangeCollection<IXYDataPoint<double>> Data { get; }

        protected IEventController _parentController;

        private IDriver _targetDriver;

        public IDriver TargetDriver
        {
            get => _targetDriver;
            set
            {
                if (_targetDriver == value)
                {
                    return;
                }

                _targetDriver = value;
                UpdateGraph();
                OnPropertyChanged();
            }
        }

        private string _xTitle;

        public string XTitle
        {
            get => _xTitle;
            set
            {
                if (_xTitle == value)
                {
                    return;
                }
                _xTitle = value;
                OnPropertyChanged();
            }
        }

        private string _yTitle;

        public string YTitle
        {
            get => _yTitle;
            set
            {
                if (_yTitle == value)
                {
                    return;
                }
                _yTitle = value;
                OnPropertyChanged();
            }
        }

        private double _maxY;

        public double MaxY
        {
            get => _maxY;
            set 
            {
                if(_maxY == value)
                {
                    return;
                }
                _maxY = value;
                OnPropertyChanged();
            }
        }

        private double _minY;

        public double MinY
        {
            get => _minY;
            set
            {
                if (_minY == value)
                {
                    return;
                }
                _minY = value;
                OnPropertyChanged();
            }
        }

        protected XYGraph(IEventController parentController, string xTitle, string yTitle)
        {
            _parentController = parentController;
            XTitle = xTitle;
            YTitle = yTitle;

            Data = new ObservableRangeCollection<IXYDataPoint<double>>();
        }

        public void Clear()
        {
            Data.Clear();
            XTitle = string.Empty;
            YTitle = string.Empty;
        }

        public abstract void UpdateGraph();
    }
}
