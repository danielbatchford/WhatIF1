namespace WhatIfF1.UI.Controller.Interfaces
{
    public interface IPlayable
    {
        bool Playing { get; set; }
        int CurrentTime { get; set; }
        int TotalTime { get; set; }
    }
}
