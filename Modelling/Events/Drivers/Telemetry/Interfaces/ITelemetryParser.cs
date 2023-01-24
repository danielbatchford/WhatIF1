using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using WhatIfF1.Modelling.Events.Drivers.Interfaces;

namespace WhatIfF1.Modelling.Events.Drivers.Telemetry.Interfaces
{
    public interface ITelemetryParser<TJToken> where TJToken : JToken
    {
        IDictionary<IDriver, IList<IVelocityDistanceTimeContainer>> ParseTelemetryJson(IEnumerable<IDriver> drivers, IDictionary<IDriver, ICollection<int>> lapTimes, TJToken telemetryJson);
    }
}
