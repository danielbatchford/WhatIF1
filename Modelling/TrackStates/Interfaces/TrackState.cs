using System.Globalization;
using System.Windows.Media;
using WhatIfF1.Util;

namespace WhatIfF1.Modelling.TrackStates.Interfaces
{
    public class TrackState : ITrackState
    {
        public FlagType Flag { get; }

        public SafetyCarState SafetyCarState { get; }

        public Color Color { get; }

        public int StartMs { get; }

        public int EndMs { get; }

        public int StartLap { get; }

        public int EndLap { get; }

        public string SupportingTextA { get; }

        public string SupportingTextB { get; }

        public TrackState(int startMs, int endMs, int startLap, int endLap, FlagType flag, SafetyCarState safetyCarState)
        {
            StartMs = startMs;
            EndMs = endMs;
            StartLap = startLap;
            EndLap = endLap;
            Flag = flag;
            SafetyCarState = safetyCarState;

            string flagString = new CultureInfo("en-US").TextInfo.ToTitleCase(flag.ToString());

            if (safetyCarState == SafetyCarState.NONE)
            {
                SupportingTextA = $"{flagString} flag";
                Color = SpecialColorStore.Instance.GreenColor;
            }
            else
            {
                if (safetyCarState == SafetyCarState.SC)
                {
                    SupportingTextA = $"{flagString} Flag - Virtual Safety Car";
                    Color = SpecialColorStore.Instance.YellowColor;
                }
                else
                {
                    SupportingTextA = $"{flagString} Flag - Virtual Safety Car";
                    Color = SpecialColorStore.Instance.YellowColor;
                }
            }

            if (flag == FlagType.RED)
            {
                Color = SpecialColorStore.Instance.RedColor;
            }

            SupportingTextB = $"Laps {StartLap} to {EndLap}";
        }

        public override string ToString()
        {
            return $"{SupportingTextA}, {SupportingTextB}";
        }

        public bool Equals(ITrackState other)
        {
            return other != null &&
                   Flag == other.Flag &&
                   SafetyCarState == other.SafetyCarState &&
                   Color.Equals(other.Color) &&
                   StartMs == other.StartMs &&
                   EndMs == other.EndMs &&
                   SupportingTextA == other.SupportingTextA &&
                   SupportingTextB == other.SupportingTextB;
        }
    }
}
