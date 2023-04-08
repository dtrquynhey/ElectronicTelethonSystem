using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETS_LIBRARY
{
    class Donor : Person
    {
        string donorID;
        string address;
        string phone;
        char cardType;
        string cardNumber;
        string cardExpiry;

        public Donor(string donorID, string fn, string ln, string address, string phone, char cardType, string cardNumber, string cardExpiry) : base(fn, ln)
        {
            this.donorID = donorID;
            this.address = address;
            this.phone = phone;
            this.cardType = cardType;
            this.cardNumber = cardNumber;
            this.cardExpiry = cardExpiry;
        }

        public string DonorID { get => donorID; set => donorID = value; }
        public string Address { get => address; set => address = value; }
        public string Phone { get => phone; set => phone = value; }
        public char CardType { get => cardType; set => cardType = value; }
        public string CardNumber { get => cardNumber; set => cardNumber = value; }
        public string CardExpiry { get => cardExpiry; set => cardExpiry = value; }

        public override string toString()
        {
            return DonorID + "," + base.toString() + "," + Address + "," + Phone + "," + CardType + "," + CardNumber + "," + CardExpiry;
        }

        public string getID()
        {
            return DonorID;

        }
    }
}
