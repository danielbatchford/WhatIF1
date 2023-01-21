using System;

namespace WhatIfF1.UI.Controller.Interfaces
{
    public interface IPlaybackParameterContainer : IEquatable<IPlaybackParameterContainer>
    {
        int FrameRate { get; }

        double PlaybackSpeed { get; }

        int MsIncrement { get; }

        int TimerUpdateMsIncrement { get; }
    }
}
