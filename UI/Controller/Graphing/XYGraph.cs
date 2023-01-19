using System.Collections.Generic;
using WhatIfF1.Modelling.Events.Drivers;
using WhatIfF1.UI.Controller.Graphing.SeriesData;
using WhatIfF1.Util;

namespace WhatIfF1.UI.Controller.Graphing
{
    public abstract class XYGraph : NotifyPropertyChangedWrapper
    {
        public List<XYDataPoint<double>> Data { get; }

        protected EventController _parentController;

        private Driver _targetDriver;


        public Driver TargetDriver
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


        protected XYGraph(EventController parentController, string xTitle, string yTitle)
        {
            _parentController = parentController;
            XTitle = xTitle;
            YTitle = yTitle;

            Data = new List<XYDataPoint<double>>();
        }

        public void Clear()
        {
            Data.Clear();
            XTitle = string.Empty;
            YTitle = string.Empty;
        }

        public virtual void UpdateGraph()
        {
        }
    }
}
