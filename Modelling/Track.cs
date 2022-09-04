using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatIfF1.Util;

namespace WhatIfF1.Modelling
{
    public class Track : PropertyChangedWrapper
    {
        private string _trackName;

        public string TrackName
        {
            get => _trackName;
            set
            {
                _trackName = value;
                OnPropertyChanged();
            }
        }

        private double _trackLength;

        public double TrackLength
        {
            get => _trackLength;
            set 
            {
                _trackLength = value;
                OnPropertyChanged();
            }
        }

        public static bool TryGetTrackFromEvent(string eventName, out Track track)
        {
            track = new Track("Test", double.NaN);

            return true;
        }

        private Track(string trackName, double trackLength)
        {
            TrackName = trackName;
            TrackLength = trackLength;
        }
    }
}
