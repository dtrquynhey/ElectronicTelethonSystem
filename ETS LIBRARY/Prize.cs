using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETS_LIBRARY
{
    class Prize
    {
        string prizeID;
        string sponsorID;
        string desc;
        double value;
        double donationLimit;
        int originalQuantity;
        int currentQuantity;

        public Prize(string prizeID, string sponsorID, string desc, double value, double donationLimit, int originalQuantity, int currentQuantity)
        {
            this.prizeID = prizeID;
            this.sponsorID = sponsorID;
            this.desc = desc;
            this.value = value;
            this.donationLimit = donationLimit;
            this.originalQuantity = originalQuantity;
            this.currentQuantity = currentQuantity;
        }

        public string PrizeID { get => prizeID; set => prizeID = value; }
        public string SponsorID { get => sponsorID; set => sponsorID = value; }
        public string Desc { get => desc; set => desc = value; }
        public double Value { get => value; set => this.value = value; }
        public double DonationLimit { get => donationLimit; set => donationLimit = value; }
        public int OriginalQuantity { get => originalQuantity; set => originalQuantity = value; }
        public int CurrentQuantity { get => currentQuantity; set => currentQuantity = value; }

        public string toString()
        {
            return PrizeID + "," + SponsorID + "," + Desc + "," + Value + "," + DonationLimit + "," +
                OriginalQuantity + "," + CurrentQuantity;
        }

        public string getQualifiedPrize()
        {
            return PrizeID + "," + Desc + "," + CurrentQuantity;
        }

        public string getID()
        {
            return PrizeID;
        }

        public void decreasePrize(int awardedQuantity)
        {
            CurrentQuantity -= awardedQuantity;
        }

        public void returnPrize(int returnedQuantity)
        {
            CurrentQuantity += returnedQuantity;
        }

        public double calculateTotalValue()
        {
            return (double)(Value * OriginalQuantity);
        }
    }
}
