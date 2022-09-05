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
                string driverCode = driverJson["Driver"]["code"].ToObject<string>();

                string firstName = driverJson["Driver"]["givenName"].ToObject<string>();
                string lastName= driverJson["Driver"]["familyName"].ToObject<string>();
                string driverWikiLink = driverJson["Driver"]["url"].ToObject<string>();

                string constructorName = driverJson["Constructor"]["name"].ToObject<string>();
                string constructorWikiLink = driverJson["Constructor"]["url"].ToObject<string>();

                int driverNumber = driverJson["number"].ToObject<int>();

                drivers.Add(new Driver(driverCode, firstName, lastName, driverWikiLink, constructorName, constructorWikiLink, driverNumber));
            }

            return drivers;
        }

        public string DriverCode { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string DriverName { get; }
        public string DriverWikiLink { get; }
        public string ConstructorName { get; }
        public string ConstructorWikiLink { get; }
        public int DriverNumber { get; }

        public Driver(string driverCode, string firstName, string lastName, string driverWikiLink, string constructorName, string constructorWikiLink, int driverNumber)
        {
            DriverCode = driverCode;
            FirstName = firstName;
            LastName = lastName;

            DriverName = $"{firstName} {lastName}";
            DriverWikiLink = driverWikiLink;

            ConstructorName = constructorName;
            ConstructorWikiLink = constructorWikiLink;

            DriverNumber = driverNumber;
        }

        public bool Equals(Driver other)
        {
            return DriverCode.Equals(other.DriverCode);
        }

        public override string ToString()
        {
            return $"{DriverCode} - {DriverNumber}";
        }
    }
}
