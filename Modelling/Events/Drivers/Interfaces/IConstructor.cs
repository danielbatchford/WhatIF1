using System.Windows.Media;

namespace WhatIfF1.Modelling.Events.Drivers.Interfaces
{
    public interface IConstructor
    {
        string Name { get; }
        string WikiLink { get; }
        string ImagePath { get; }
        Color Color { get; }
    }
}
