using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ETS_LIBRARY
{
    class Donation
    {
        string donationID;
        string donorID;
        string donationDate;
        double donationAmount;
        string prizeID;
        int quantityAwarded;

        public Donation (string donationID, string donorID , string donationDate, double donationAmount, string prizeID, int quantityAwarded)
        {
            this.donationID = donationID;
            this.donorID = donorID;
            this.donationDate = donationDate;
            this.donationAmount = donationAmount;
            this.prizeID = prizeID;
            this.quantityAwarded = quantityAwarded;
        }

        public string DonationID { get => donationID; set => donationID = value; }
        public string DonorID { get => donorID; set => donorID = value; }
        public string DonationDate { get => donationDate; set => donationDate = value; }
        public double DonationAmount { get => donationAmount; set => donationAmount = value; }
        public string PrizeID { get => prizeID; set => prizeID = value; }
        public int QuantityAwarded { get => quantityAwarded; set => quantityAwarded = value; }

        public string toString()
        {
            return DonationID + "," + DonorID + "," + DonationDate + "," + DonationAmount + "," + PrizeID + "," + QuantityAwarded;
        }

        public string getID()
        {
            return DonationID;
        }

        public string getPrizeInfo()
        {
            return PrizeID + " " + quantityAwarded;
        }

    }
}
