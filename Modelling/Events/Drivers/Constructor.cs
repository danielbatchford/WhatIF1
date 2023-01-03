using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using WhatIfF1.Adapters;
using WhatIfF1.Util;
using WhatIfF1.Util.Extensions;

namespace WhatIfF1.Modelling.Events.Drivers
{
    public class Constructor : NotifyPropertyChangedWrapper
    {
        private static readonly IDictionary<string, Color> _constructorColorMap = new Dictionary<string, Color>()
        {
            { "Mercedes", Color.FromRgb(0,210,90)},
            { "Red Bull", Color.FromRgb(6,0,239)},
            { "Ferrari", Color.FromRgb(220,0,0)},
            { "McLaren", Color.FromRgb(255,135,0)},
            { "Alpine F1 Team",Color.FromRgb(0,144,255)},
            { "AlphaTauri", Color.FromRgb(43,69,98)},
            { "Aston Martin", Color.FromRgb(0,111,98)},
            { "Alfa Romeo", Color.FromRgb(144,0,0)},
            { "Williams", Color.FromRgb(0,90,255)},
            { "Haas F1 Team", Color.FromRgb(255,255,255)}
        };

        /// <summary>
        /// Returns the associated constructor color for a given constructor
        /// </summary>
        private static Color GetConstructorColor(string key)
        {
            if (!_constructorColorMap.TryGetValue(key, out Color color))
            {
                throw new KeyNotFoundException($"Could not find an associated constructor color for the provided constructor key: {key}");
            }

            return color;
        }

        public string Name { get; }
        public string WikiLink { get; }
        public string ImagePath { get; }

        public Color Color { get; }

        private ICommand _aboutConstructorCommand;
        public ICommand AboutConstructorCommand
        {
            get
            {
                if (_aboutConstructorCommand is null)
                {
                    bool canExecute = !string.IsNullOrEmpty(WikiLink);
                    _aboutConstructorCommand = new CommandHandler(() => WikiLink.OpenInBrowser(), () => canExecute);
                }

                return _aboutConstructorCommand;
            }
            set
            {
                _aboutConstructorCommand = value;
                OnPropertyChanged();
            }
        }

        public Constructor(JObject constructorJson)
        {
            Name = constructorJson["name"].ToObject<string>();
            WikiLink = constructorJson["url"].ToObject<string>();

            Color = GetConstructorColor(Name);

            string constructorFolder = FileAdapter.Instance.ConstructorPicsRoot;

            ImagePath = Path.Combine(constructorFolder, $"{Name}.png");

            // If this image does not exist, use the default one
            if (!File.Exists(ImagePath))
            {
                ImagePath = Path.Combine(constructorFolder, "default.png");
            }
        }

    }
}
