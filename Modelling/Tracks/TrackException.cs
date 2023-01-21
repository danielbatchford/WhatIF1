using System;

namespace WhatIfF1.Modelling.Tracks
{
    public class TrackException : Exception
    {
        public TrackException(string message) : base(message)
        {
        }

        public TrackException() : base()
        {
        }

        public TrackException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
