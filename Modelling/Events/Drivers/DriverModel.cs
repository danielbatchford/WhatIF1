using Newtonsoft.Json.Linq;
using System;

namespace WhatIfF1.Modelling.Events.Drivers
{
    public class DriverModel
    {
        public EventModel ParentModel { get; }

        private readonly double _trackLength;

        public DriverModel(EventModel parentModel, double trackLength, JArray lapTimesJson)
        {
            ParentModel = parentModel;
            _trackLength = trackLength;

            // TODO - parse lap times
        }

        public Position GetPositionAtTime(int totalMs)
        {
            throw new NotImplementedException();
        }
    }
}
