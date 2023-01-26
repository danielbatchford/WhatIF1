using System;
using System.Collections.Generic;
using WhatIfF1.Modelling.Events.Drivers.Interfaces;
using WhatIfF1.UI.Controller.Graphing.SeriesData;
using WhatIfF1.UI.Controller.Graphing.SeriesData.Interfaces;
using WhatIfF1.UI.Controller.Interfaces;

namespace WhatIfF1.UI.Controller.Graphing
{
    public class LapGraph : XYGraph
    {
        protected LapGraph(IEventController parentController, string yTitle) : base(parentController, "Time (Lap)", yTitle)
        {
            MinY = 0;

            // Realistic upper bound on maximum speed achieved by formula 1 cars (kph)
            MaxY = 350;
        }

        public override async void UpdateGraph()
        {
            if (TargetDriver is null)
            {
                return;
            }

            int driverLap = await _parentController.DataProvider.GetCurrentLapForDriver(_parentController.CurrentTime, TargetDriver);

            int leaderLap = _parentController.CurrentLap;

            string xTitle;
            if (driverLap < leaderLap)
            {
                string pluralString = leaderLap - driverLap == 1 ? string.Empty : "s";
                xTitle = $"{TargetDriver} | Lap {driverLap} | {leaderLap - driverLap} Lap{pluralString} Behind Leader)";
            }
            else
            {
                xTitle = $"{TargetDriver} | Lap {driverLap}";
            }

            XTitle = xTitle;

            IVelocityDistanceTimeContainer vdtContainer = await _parentController.DataProvider.GetVDTContainer(TargetDriver, driverLap);

            Data.Clear();
            Data.AddRange(ToXYPoints(vdtContainer));
        }

        private IList<IXYDataPoint<double>> ToXYPoints(IVelocityDistanceTimeContainer vdtContainer)
        {
            var points = new List<IXYDataPoint<double>>(vdtContainer.NumSamples);

            for (int i = 0; i < vdtContainer.NumSamples; i++)
            {
                double sec = Math.Round((double)vdtContainer.Ms[i] / 1000, 2);
                points.Add(new XYDataPoint<double>(sec, vdtContainer.Velocity[i]));
            }

            return points;
        }
    }
}
