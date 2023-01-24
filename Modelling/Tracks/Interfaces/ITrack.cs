using System;

namespace WhatIfF1.Modelling.Tracks.Interfaces
{
    public interface ITrack : IEquatable<ITrack>
    {
        string TrackName { get; }
        double TrackLength { get; }
        string CountryName { get; }
        string LocationName { get; }
        string WikipediaURL { get; }
        double Latitude { get; }
        double Longitude { get; }
        string FlagFilePath { get; }
        string TrackFilePath { get; }
    }
}
