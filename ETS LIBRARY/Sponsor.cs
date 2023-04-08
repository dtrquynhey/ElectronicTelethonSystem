using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETS_LIBRARY
{
    class Sponsor : Person
    {
        string sponsorID;
        double value;

        public Sponsor(string sponsorID, string firstName, string lastName, double value) : base(firstName, lastName)
        {
            this.sponsorID = sponsorID;
            this.value = value;
        }

        public string SponsorID { get => sponsorID; set => sponsorID = value; }
        public double Value { get => value; set => this.value = value; }


        public override string toString()
        {
            return SponsorID + "," + base.toString() +  "," + Value;
        }

        public string getID()
        {
            return SponsorID;
        }

        public void addValue(double value)
        {
            Value += value;
        }

        public void deductValue(double value)
        {
            Value -= value;
        }
        
    }
}
