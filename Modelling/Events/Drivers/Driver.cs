using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace WhatIfF1.Modelling.Events.Drivers
{
    public class Driver : IEquatable<Driver>
    {
        public static IEnumerable<Driver> GetDriverListFromJSON(JArray json)
        {
            ICollection<Driver> drivers = new HashSet<Driver>(json.Count);

            foreach(JObject driverJson in json)
            {
                string driverID = driverJson["Driver"]["driverId"].ToObject<string>();
                string driverLetters = driverJson["Driver"]["code"].ToObject<string>();

                string firstName = driverJson["Driver"]["givenName"].ToObject<string>();
                string lastName= driverJson["Driver"]["familyName"].ToObject<string>();
                string driverWikiLink = driverJson["Driver"]["url"].ToObject<string>();

                string constrName = driverJson["Constructor"]["name"].ToObject<string>();
                string constrWikiLink = driverJson["Constructor"]["url"].ToObject<string>();

                int driverNumber = driverJson["number"].ToObject<int>();

                drivers.Add(new Driver(driverID, driverLetters, firstName, lastName, driverWikiLink, constrName, constrWikiLink, driverNumber));
            }

            return drivers;
        }

        public string DriverID { get; }
        public string DriverLetters { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string DriverName { get; }
        public string DriverWikiLink { get; }
        public string ConstructorName { get; }
        public string ConstructorWikiLink { get; }
        public int DriverNumber { get; }

        public Driver(string driverID, string driverLetters, string firstName, string lastName, string driverLink, string constrName, string constrLink, int driverNumber)
        {
            DriverID = driverID;
            DriverLetters = driverLetters;
            FirstName = firstName;
            LastName = lastName;

            DriverName = $"{firstName} {lastName}";
            DriverWikiLink = driverLink;

            ConstructorName = constrName;
            ConstructorWikiLink = constrLink;

            DriverNumber = driverNumber;
        }

        public bool Equals(Driver other)
        {
            return DriverID.Equals(other.DriverID);
        }

        public override string ToString()
        {
            return $"{DriverLetters} - {DriverNumber}";
        }
    }
}
