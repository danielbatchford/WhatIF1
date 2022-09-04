using System;
using System.Windows.Media;
using WhatIfF1.Modelling;
using WhatIfF1.Scenarios.Exceptions;
using WhatIfF1.Util;
using WhatIfF1.Util.Extensions;

namespace WhatIfF1.Scenarios
{
    public class Scenario : PropertyChangedWrapper
    {
        public string EventCode { get; }
        public Track Track { get; }
        public DateTime EventDate { get; }
        public DateTime RetrieveDate { get; }

        private Color _color;

        public Color Color
        {
            get => _color;
            set 
            {
                _color = value;
                OnPropertyChanged();
            }
        }

        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set 
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventName">The name of the event (e.g Bahrain)</param>
        /// <param name="yearCode">The year code (e.g 2022)</param>
        public Scenario(string eventName, int yearCode)
        {
            if(yearCode > DateTime.Now.Year)
            {
                throw new ScenarioException($"Provided year code exceeded the current year (Got year code {yearCode}");
            };

            if(Track.TryGetTrackFromEvent(eventName, out Track track))
            {
                Track = track;
            }
            else
            {
                throw new ScenarioException($"Failed to retrieve a track from the provided event (Got event name {eventName}");
            }

            EventCode = $"{eventName} {yearCode}";

            // TODO - event date
            EventDate = DateTime.Now;

            RetrieveDate = DateTime.Now;

            // Initialise scenario color as a random color
            Color = ColorExtensions.GetRandomColor();
        }
    }
}
