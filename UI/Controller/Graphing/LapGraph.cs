using WhatIfF1.UI.Controller.Interfaces;

namespace WhatIfF1.UI.Controller.Graphing
{
    public class LapGraph : XYGraph
    {
        protected LapGraph(IEventController parentController, string yTitle) : base(parentController, "Time (Lap)", yTitle)
        {
        }

        public override void UpdateGraph()
        {
            int leaderLap = _parentController.CurrentLap;
            // this driver may not be on the lead lap
            int driverLap = _parentController.Model.GetCurrentLap(_parentController.CurrentTime, TargetDriver);

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
