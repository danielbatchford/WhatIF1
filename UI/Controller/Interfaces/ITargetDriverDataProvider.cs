using WhatIfF1.Modelling.Events.Drivers.Interfaces;

namespace WhatIfF1.UI.Controller.Interfaces
{
    public interface ITargetDriverDataProvider
    {
        IDriver TargetDriver { get; set; }

        void UpdateProvider();
    }
}
