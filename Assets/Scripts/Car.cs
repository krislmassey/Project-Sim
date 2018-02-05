using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CAVS.LexusShowcase
{

    /// <summary>
    /// Basic bitch car representation
    /// </summary>
    public class Car
    {

        // Main car descriptors
        private int id;
        private string make;
        private string model;
        private int year;
        private string trim;
        private string description;
        private string carClass;
        private string url;

        // ANYTHING ELSE
        Dictionary<string, string> extraData;

        public Car(int id, string make, string model, int year, string trim, string description, string carClass, string url, Dictionary<string, string> extraData)
        {
            // Stuff you use to identify the car..
            this.id = id;
            this.make = make;
            this.model = model;
            this.year = year;

            // Other random stuff
            this.trim = trim;
            this.description = description;
            this.carClass = carClass;
            this.url = url;
            this.extraData = extraData;
        }

        public int getId()
        {
            return this.id;
        }

        public string getMake()
        {
            return this.make;
        }

        public string getModel()
        {
            return this.model;
        }

        public int getYear()
        {
            return this.year;
        }

        public string getTrim()
        {
            return this.trim;
        }

        public string getDescription()
        {
            return this.description;
        }

        public string getCarClass()
        {
            return this.carClass;
        }

        public string getPictureUrl()
        {
            return this.url;
        }

        /// <summary>
        /// Attempts to grab a random piece of data associated with the car..
        /// </summary>
        /// <param name="nameOfData">Identifier of the random data</param>
        /// <returns>Random Data</returns>
        public string getData(string nameOfData)
        {
            if (extraData.ContainsKey(nameOfData))
            {
                return extraData[nameOfData];
            }
            return null;
        }

        public Dictionary<string, string> getAllExtraData()
        {
            return this.extraData;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} ({2})", this.make, this.model, this.year);
        }

    }

}
