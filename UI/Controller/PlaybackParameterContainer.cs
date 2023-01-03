using WhatIfF1.Util;

namespace WhatIfF1.UI.Controller
{
    public class PlaybackParameterContainer
    {
        public int FrameRate { get; }

        public double PlaybackSpeed { get; }

        public int MsIncrement { get; }

        public int TimerUpdateMsIncrement { get; }

        public static PlaybackParameterContainer GetDefault()
        {
            int frameRate = (int)Properties.Settings.Default["playbackFramerate"];

            double playbackSpeed = (double)Properties.Settings.Default["playbackSpeed"];

            return new PlaybackParameterContainer(frameRate, playbackSpeed);
        }

        private PlaybackParameterContainer(int frameRate, double playbackSpeed)
        {
            FrameRate = frameRate;
            PlaybackSpeed = playbackSpeed;
            MsIncrement = (int)(1000 * playbackSpeed / frameRate);
            TimerUpdateMsIncrement = 1000 / frameRate;
        }
    }
}
