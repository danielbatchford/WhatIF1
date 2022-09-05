using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatIfF1.Modelling.Events;

namespace WhatIfF1.Modelling.Events.Drivers
{
    public class DriverModel
    {
        public EventModel ParentModel { get; }

        public DriverModel(EventModel parentModel)
        {
            ParentModel = parentModel;
        }
    }
}
