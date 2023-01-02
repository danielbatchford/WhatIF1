using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using WhatIfF1.Adapters;
using WhatIfF1.Util;
using WhatIfF1.Util.Extensions;

namespace WhatIfF1.Modelling.Events.Drivers
{
    public class Driver : PropertyChangedWrapper, IEquatable<Driver>
    {

        public static IEnumerable<Driver> GetDriverListFromJSON(JArray json)
        {
            ICollection<Driver> drivers = new HashSet<Driver>(json.Count);

            foreach (JObject driverJson in json)
            {
                string driverID = driverJson["Driver"]["driverId"].ToObject<string>();
                string driverLetters = driverJson["Driver"]["code"].ToObject<string>();

                string firstName = driverJson["Driver"]["givenName"].ToObject<string>();
                string lastName = driverJson["Driver"]["familyName"].ToObject<string>();
                string driverWikiLink = driverJson["Driver"]["url"].ToObject<string>();

                int driverNumber = driverJson["number"].ToObject<int>();

                JObject constructorJson = driverJson["Constructor"].ToObject<JObject>();

                drivers.Add(new Driver(driverID, driverLetters, firstName, lastName, driverWikiLink, constructorJson, driverNumber));
            }

            return drivers;
        }

        public string DriverID { get; }
        public string DriverLetters { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Name { get; }
        public string ImagePath { get; }
        public string WikiLink { get; }

        public Constructor Constructor { get; }

        public int DriverNumber { get; }

        private ICommand _aboutDriverCommand;
        public ICommand AboutDriverCommand
        {
            get
            {
                if (_aboutDriverCommand is null)
                {
                    bool canExecute = !string.IsNullOrEmpty(WikiLink);
                    _aboutDriverCommand = new CommandHandler(() => WikiLink.OpenInBrowser(), () => canExecute);
                }

                return _aboutDriverCommand;
            }
            set
            {
                _aboutDriverCommand = value;
                OnPropertyChanged();
            }
        }




        public Driver(string driverID, string driverLetters, string firstName, string lastName, string wikiLink, JObject constructorJson, int driverNumber)
        {
            DriverID = driverID;
            DriverLetters = driverLetters;
            FirstName = firstName;
            LastName = lastName;

            string driverFolder = FileAdapter.Instance.DriverPicsRoot;

            ImagePath = Path.Combine(driverFolder, $"{driverLetters}.png");

            // If this image does not exist, use the default one
            if (!File.Exists(ImagePath))
            {
                ImagePath = Path.Combine(driverFolder, "default.png");
            }

            Name = $"{firstName} {lastName}";
            WikiLink = wikiLink;

            Constructor = new Constructor(constructorJson);

            DriverNumber = driverNumber;
        }

        public bool Equals(Driver other)
        {
            return DriverID.Equals(other.DriverID);
        }

        public override string ToString()
        {
            return DriverLetters;
        }
    }
}
