using System;
using System.Globalization;
using System.Windows.Data;
using WhatIfF1.Scenarios;
using WhatIfF1.Scenarios.Exceptions;

namespace WhatIfF1.UI.Converters
{
    public class ScenarioTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ScenarioType scenarioType))
            {
                return null;
            }

            switch (scenarioType)
            {
                case ScenarioType.RACE:
                    return "Race";
                case ScenarioType.MODEL:
                    return "Model";
                default:
                    throw new ScenarioException($"Recieved an invalid scenario type while using an {nameof(IValueConverter)}");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
