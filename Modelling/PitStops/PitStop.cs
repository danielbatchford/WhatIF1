using WhatIfF1.Modelling.PitStops.Interfaces;

namespace WhatIfF1.Modelling.PitStops
{
    public class PitStop : IPitStop
    {
        public int StopNumber { get; }
        public int PitTime { get; }
        public int InLap { get; }
        public int OutLap { get; }
        public ITireCompound OldCompound { get; }
        public ITireCompound NewCompound { get; }

        public PitStop(int stopNumber, int pitTime, int inLap, int outLap, ITireCompound oldCompound, ITireCompound newCompound)
        {
            StopNumber = stopNumber;
            PitTime = pitTime;
            InLap = inLap;
            OutLap = outLap;
            OldCompound = oldCompound;
            NewCompound = newCompound;
        }

        public override string ToString()
        {
            return $"Stop {StopNumber}, {OldCompound} to {NewCompound} on laps {InLap} to {OutLap}. Time: {PitTime}ms";
        }

        public bool Equals(IPitStop other)
        {
            return
                StopNumber == other.StopNumber &&
                PitTime == other.PitTime &&
                InLap == other.InLap &&
                OutLap == other.OutLap &&
                OldCompound == other.OldCompound &&
                NewCompound == other.NewCompound;
        }
    }
}
