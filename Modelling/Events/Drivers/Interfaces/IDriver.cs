using System;

namespace WhatIfF1.Modelling.Events.Drivers.Interfaces
{
    public interface IDriver : IEquatable<IDriver>
    {
        string DriverID { get; }
        string DriverLetters { get; }
        string FirstName { get; }
        string LastName { get; }
        string Name { get; }
        string ImagePath { get; }
        string WikiLink { get; }
        IConstructor Constructor { get; }
        int DriverNumber { get; }
    }
}
