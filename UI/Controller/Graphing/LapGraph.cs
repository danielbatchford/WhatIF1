using WhatIfF1.UI.Controller.Interfaces;

namespace WhatIfF1.UI.Controller.Graphing
{
    public class LapGraph : XYGraph
    {
        protected LapGraph(IEventController parentController, string yTitle) : base(parentController, "Time (Lap)", yTitle)
        {
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
                xTitle = $"{TargetDriver} (Lap {driverLap}. {leaderLap - driverLap} lap{pluralString} behind leader)";
            }
            else
            {
                xTitle = $"{TargetDriver} (Lap {driverLap})";
            }

            XTitle = xTitle;
        }
    }
}
