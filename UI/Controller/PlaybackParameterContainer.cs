using WhatIfF1.UI.Controller.Interfaces;

namespace WhatIfF1.UI.Controller
{
    public struct PlaybackParameterContainer : IPlaybackParameterContainer
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

        public bool Equals(IPlaybackParameterContainer other)
        {
            return other is PlaybackParameterContainer pt &&
                pt.FrameRate == FrameRate &&
                pt.PlaybackSpeed == PlaybackSpeed &&
                pt.MsIncrement == MsIncrement &&
                pt.TimerUpdateMsIncrement == TimerUpdateMsIncrement;
        }
    }
}
