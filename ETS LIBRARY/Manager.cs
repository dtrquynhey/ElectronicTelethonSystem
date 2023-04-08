using ETS_LIBRARY;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Lifetime;
using System.Runtime.Remoting.Messaging;
using System.Security.AccessControl;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ETS_LIBRARY
{
    public class Manager
    {
        //struct to return message, valid or invalid, pointer to the property that is not valid
        public struct Message
        {
            public string message;
            public bool valid;
            public int errorPointer;
        }


        //auto title case the input (name, title..)
        TextInfo myFormat = CultureInfo.CurrentCulture.TextInfo;

        Sponsors mySponsors = new Sponsors();
        Donors myDonors = new Donors();
        Prizes myPrizes = new Prizes();
        Donations myDonations = new Donations();
        Message ms;

        readonly string fileSponsors = @"..\Debug\sponsors.txt";
        readonly string fileDonors = @"..\Debug\donors.txt";
        readonly string filePrizes = @"..\Debug\prizes.txt";
        readonly string fileDonations = @"..\Debug\donations.txt";

        /// <summary>
        /// VALIDATION 
        /// </summary>
        public bool isExistedDonorID(string ID)
        {
            bool existed = false;
            foreach (Donor donor in myDonors)
                if (ID == donor.getID())
                    existed = true;
            return existed;
        }

        public bool isExistedSponsorID(string ID)
        {
            bool existed = false;
            foreach (Sponsor sponsor in mySponsors)
                if (ID == sponsor.getID())
                    existed = true;
            return existed;
        }

        public bool isExistedDonationID(string ID)
        {
            bool existed = false;
            foreach (Donation donation in myDonations)
                if (ID == donation.getID())
                    existed = true;
            return existed;
        }

        public bool isExistedPrizeID(string ID)
        {
            bool existed = false;
            foreach (Prize prize in myPrizes)
                if (ID == prize.getID())
                    existed = true;
            return existed;
        }

        //used when adding donation
        public bool recordDonation(string prizeID, int quantityToAward)
        {
            bool added = false;
            foreach (Prize prize in myPrizes)
            {
                if (prizeID == prize.getID())
                {
                    if (prize.CurrentQuantity >= quantityToAward && quantityToAward > 0)
                    {
                        prize.decreasePrize(quantityToAward);
                        added = true;
                    }
                }
            }
            return added;
        }




        /// <summary>
        /// ADD
        /// </summary>
        public Message addSponsor(string sponsorID, string fn, string ln, double value)
        {
            sponsorID = myFormat.ToTitleCase(sponsorID);
            ms.valid = false;
            if (!Validator.isValidID(myFormat.ToTitleCase(sponsorID)))
            {
                ms.message = "ID must have 4-character (ex: A123)!";
                ms.errorPointer = 0;
            }
            else
            {
                if (isExistedSponsorID(myFormat.ToTitleCase(sponsorID)))
                    ms.message = $"Sponsor {sponsorID} already exists";
                else
                {
                    if (!Validator.isValidName(fn))
                    {
                        ms.message = "First name must be alphabetical and have maximum of 15 characters!";
                        ms.errorPointer = 1;
                    }
                    else
                    {
                        if (!Validator.isValidName(ln))
                        {
                            ms.message = "Last name must be alphabetical and have maximum of 15 characters!";
                            ms.errorPointer = 2;
                        }
                        else
                        {
                            ms.valid = true;
                            Sponsor sponsor = new Sponsor(sponsorID, myFormat.ToTitleCase(fn), myFormat.ToTitleCase(ln), value);
                            mySponsors.add(sponsor);
                            ms.message = "New sponsor has been successfully added to the list!";
                        }
                    }
                }
            }
            return ms;
        }

        public Message addDonor(string donorID, string fn, string ln, string addr, string phone, char cardType, string cardNum, string expiredDate)
        {
            donorID = myFormat.ToTitleCase(donorID);
            ms.valid = false;
            if (!Validator.isValidID(donorID))
            {
                ms.message = "ID must have 4-character (ex: A123)!";
                ms.errorPointer = 0;
            }
            else
            {
                if (isExistedDonorID(donorID))
                    ms.message = $"Donor {donorID} already exists";
                else
                {
                    if (!Validator.isValidName(fn))
                    {
                        ms.message = "First name must be alphabetical and have maximum of 15 characters!";
                        ms.errorPointer = 1;
                    }
                    else
                    {
                        if (!Validator.isValidName(ln))
                        {
                            ms.message = "Last name must be alphabetical and have maximum of 15 characters!";
                            ms.errorPointer = 2;
                        }
                        else
                        {
                            if (!Validator.isValidAddress(addr))
                            {
                                ms.message = "Invalid address!";
                                ms.errorPointer = 3;
                            }
                            else
                            {
                                if (!Validator.isValidPhoneNum(phone))
                                {
                                    ms.message = "Invalid phone number!";
                                    ms.errorPointer = 4;
                                }
                                else
                                {
                                    if (!Validator.isValidCardNum(cardNum))
                                    {
                                        ms.message = "Card number must be 16-digit number!";
                                        ms.errorPointer = 5;
                                    }
                                    else
                                    {
                                        if (!Validator.isValidExpiredDate(expiredDate))
                                        {
                                            ms.message = "Expired date must be in the range: 11/2022 - 12/2029!";
                                            ms.errorPointer = 6;
                                        }
                                        else
                                        {
                                            ms.valid = true;
                                            Donor donor = new Donor(donorID, myFormat.ToTitleCase(fn), myFormat.ToTitleCase(ln), myFormat.ToTitleCase(addr), phone,
                                                        cardType, cardNum, expiredDate);
                                            myDonors.add(donor);
                                            ms.message = "New donor has been successfully added to the list!";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return ms;
        }

        public Message addPrize(string prizeID, string sponsorID, string desc, double valuePerPrize, double donationLimit, int orginalQuantity, int currentQuantity)
        {
            prizeID = myFormat.ToTitleCase(prizeID);
            sponsorID = myFormat.ToTitleCase(sponsorID);
            desc = myFormat.ToTitleCase(desc);
            ms.valid = false;
            if (!Validator.isValidID(prizeID))
            {
                ms.message = "ID must have 4-character (ex: A123)!";
                ms.errorPointer = 0;
            }
            else
            {
                if (isExistedPrizeID(prizeID))
                    ms.message = $"Prize {prizeID} already exists!";
                else
                {
                    if (!isExistedSponsorID(sponsorID))
                    {
                        ms.message = $"Sponsor {sponsorID} does not exists!";
                        ms.errorPointer = 1;
                    }
                    else
                    {
                        if (!Validator.isValidName(desc))
                        {
                            ms.message = "Description must be alphabetical and have maximum of 15 characters!";
                            ms.errorPointer = 2;
                        }
                        else
                        {
                            if (!Validator.isValidPrizeValue(valuePerPrize))
                            {
                                ms.message = "Maximum value of a prize is $299,99!";
                                ms.errorPointer = 3;
                            } 
                            else
                            {
                                if (!Validator.isValidQuantity(orginalQuantity))
                                {
                                    ms.message = "Maximum quantity is 999 items!";
                                    ms.errorPointer = 4;
                                }
                                else
                                {
                                    ms.valid = true;
                                    Prize prize = new Prize(prizeID, sponsorID, desc, valuePerPrize, donationLimit, orginalQuantity, currentQuantity);
                                    myPrizes.add(prize);
                                    double totalValue = prize.calculateTotalValue();
                                    addValueToSponsor(sponsorID, totalValue);
                                    ms.message = "New prize has been successfully added to the list!";
                                }

                            }
                        }
                    }
                }
            }
            return ms;
        }

        public Message addDonation(string donationID, string donorID, string donationDate, double donationAmount, string prizeID, int quantityAwarded)
        {
            donationID = myFormat.ToTitleCase(donationID);
            donorID = myFormat.ToTitleCase(donorID);
            prizeID = myFormat.ToTitleCase(prizeID);
            ms.valid = false;
            if (!Validator.isValidID(donationID))
            {
                ms.message = "ID must have 4-character (ex: A123)!";
                ms.errorPointer = 0;
            }
            else
            {
                if (isExistedDonationID(donationID))
                    ms.message = $"Donation {donationID} already exists!";
                else
                {
                    if (!isExistedDonorID(donorID))
                    {
                        ms.message = $"Donor {donorID} does not exist!";
                        ms.errorPointer = 1;
                    }
                    else
                    {
                        if (!Validator.isValidDonationAmount(donationAmount))
                        {
                            ms.message = "Donation amount must be in the range: $5.0 - $999,999.99";
                            ms.errorPointer = 2;
                        }
                        else
                        {
                            if (!isExistedPrizeID(prizeID))
                            {
                                ms.message = $"Prize {prizeID} does not exist!";
                                ms.errorPointer = 3;
                            }
                            else
                            {
                                if (quantityAwarded < 1)
                                {
                                    ms.message = "Please award at least 1 prize!";
                                    ms.errorPointer = 4;
                                }
                                else
                                {
                                    if (!recordDonation(prizeID, quantityAwarded))
                                    {
                                        ms.message = "Not enough prizes in stock! Please enter lower quantity!";
                                        ms.errorPointer = 5;
                                    }
                                    else
                                    {
                                        ms.valid = true;
                                        Donation donation = new Donation(donationID, donorID, donationDate, donationAmount, prizeID, quantityAwarded);
                                        myDonations.add(donation);
                                        ms.message = "New donation has been successfully added to the list!";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return ms;
        }




        /// <summary>
        /// READ FROM FILE
        /// </summary>
        public void readSponsorRecords()
        {
            FileStream file = new FileStream(fileSponsors, FileMode.OpenOrCreate, FileAccess.Read);
            using (StreamReader sr = new StreamReader(file, true))
            {
                while (sr.Peek() >= 0)
                {
                    string line;
                    string[] field;
                    line = sr.ReadLine();
                    field = line.Split(',');
                    Sponsor sponsor = new Sponsor(field[0], field[1], field[2], double.Parse(field[3]));
                    mySponsors.add(sponsor);
                }
            }
        }

        public void readPrizeRecords()
        {
            FileStream file = new FileStream(filePrizes, FileMode.OpenOrCreate, FileAccess.Read);
            using (StreamReader sr = new StreamReader(file, true))
            {
                while (sr.Peek() >= 0)
                {
                    string line;
                    string[] field;
                    line = sr.ReadLine();
                    field = line.Split(',');
                    Prize prize = new Prize(field[0], field[1], field[2], double.Parse(field[3]),
                        double.Parse(field[4]), int.Parse(field[5]), int.Parse(field[6]));
                    myPrizes.add(prize);
                }
            }
        }

        public void readDonorRecords()
        {
            FileStream file = new FileStream(fileDonors, FileMode.OpenOrCreate, FileAccess.Read);
            using (StreamReader sr = new StreamReader(file, true))
            {
                while (sr.Peek() >= 0)
                {
                    string line;
                    string[] field;
                    line = sr.ReadLine();
                    field = line.Split(',');
                    Donor donor = new Donor(field[0], field[1], field[2], field[3], field[4], char.Parse(field[5]), field[6], field[7]);
                    myDonors.add(donor);
                }
            }
        }

        public void readDonationRecords()
        {
            FileStream file = new FileStream(fileDonations, FileMode.OpenOrCreate, FileAccess.Read);
            using (StreamReader sr = new StreamReader(file))
            {
                while (sr.Peek() >= 0)
                {
                    string line;
                    string[] field;
                    line = sr.ReadLine();
                    field = line.Split(',');
                    Donation donation = new Donation(field[0], field[1], field[2], double.Parse(field[3]), field[4], int.Parse(field[5]));
                    myDonations.add(donation);
                }
            }
        }

        public void readAllRecords()
        {
            readSponsorRecords();
            readDonorRecords();
            readPrizeRecords();
            readDonationRecords();
        }




        /// <summary>
        /// FETCH ALL DATA TO LIST
        /// </summary>
        /// <returns></returns>
        public string getAllSponsors()
        {
            string allSponsors = "";
            foreach (Sponsor sponsor in mySponsors)
                allSponsors += sponsor.toString() + '\n';
            allSponsors = allSponsors.Trim();
            return allSponsors;
        }

        public string getAllDonors()
        {
            string allDonors = "";
            foreach (Donor donor in myDonors)
                allDonors += donor.toString() + "\n";
            allDonors = allDonors.Trim();
            return allDonors;
        }

        public string getAllPrizes()
        {
            string allPrizes = "";
            foreach (Prize prize in myPrizes)
                allPrizes += prize.toString() + "\n";
            allPrizes = allPrizes.Trim();
            return allPrizes;
        }

        public string getAllDonations()
        {
            string allDonations = "";
            foreach (Donation donation in myDonations)
                allDonations += donation.toString() + "\n";
            allDonations = allDonations.Trim();
            return allDonations;
        }

        public List<string> getQualifiedPrize(double donationAmount)
        {
            List<string> allPrizesToAward = new List<string>();
            foreach (Prize prize in myPrizes)
            {
                if (donationAmount >= prize.DonationLimit && prize.CurrentQuantity > 0)
                {
                    allPrizesToAward.Add(prize.getQualifiedPrize());
                }
            }
            return allPrizesToAward;
        }

        public string getOneFromEntity(string ID, int num)
        {
            string one = string.Empty;
            switch (num)
            {
                case 1:
                    foreach (Sponsor sponsor in mySponsors)
                    {
                        if (ID == sponsor.getID())
                        {
                            one += sponsor.toString();
                        }
                    }
                    break;
                case 2:
                    foreach (Prize prize in myPrizes)
                    {
                        if (ID == prize.getID())
                        {
                            one += prize.toString();
                        }
                    }
                    break;
                case 3:
                    foreach (Donor donor in myDonors)
                    {
                        if (ID == donor.getID())
                        {
                            one += donor.toString();
                        }
                    }
                    break;
                case 4:
                    foreach (Donation donation in myDonations)
                    {
                        if (ID == donation.DonationID)
                        {
                            one += donation.getID();
                        }
                    }
                    break;
            }
            return one;
        }




        /// <summary>
        /// SAVE
        /// </summary>
        public Message saveSponsorRecords()
        {
            using (StreamWriter sw = new StreamWriter(fileSponsors, false))
            {
                foreach (Sponsor sponsor in mySponsors)
                {
                    sw.WriteLine(sponsor.SponsorID + "," + myFormat.ToTitleCase(sponsor.FirstName) + "," +
                        myFormat.ToTitleCase(sponsor.LastName) + "," + sponsor.Value);
                }
                ms.message = "All sponsors have been successfully saved!";
            }
            return ms;
        }

        public Message saveDonorRecords()
        {
            using (StreamWriter sw = new StreamWriter(fileDonors, false))
            {
                foreach (Donor donor in myDonors)
                {
                    sw.WriteLine(donor.DonorID + "," + myFormat.ToTitleCase(donor.FirstName) + "," + myFormat.ToTitleCase(donor.LastName) + "," +
                    myFormat.ToTitleCase(donor.Address) + "," + donor.Phone + "," + donor.CardType + "," + donor.CardNumber + "," + donor.CardExpiry);
                }
                ms.message = "All donors have been successfully saved!";
            }
            return ms;
        }

        public Message savePrizeRecords()
        {
            using (StreamWriter sw = new StreamWriter(filePrizes, false))
            {
                foreach (Prize prize in myPrizes)
                {
                    sw.WriteLine(prize.PrizeID + "," + prize.SponsorID + "," + prize.Desc + "," + prize.Value + "," +
                        prize.DonationLimit + "," + prize.OriginalQuantity + "," + prize.CurrentQuantity);
                }
                ms.message = "All prizes have been successfully saved!";
            }
            return ms;
        }

        public Message saveDonationRecords()
        {
            using (StreamWriter sw = new StreamWriter(fileDonations, false))
            {
                foreach (Donation donation in myDonations)
                {
                    sw.WriteLine(donation.DonationID + "," + donation.DonorID + "," + donation.DonationDate + "," +
                        donation.DonationAmount + "," + donation.PrizeID + "," + donation.QuantityAwarded);
                }
                ms.message = "All donations have been successfully saved!";
            }
            return ms;
        }




        /// <summary>
        /// DELETE
        /// </summary>
        //when deleting a donation, the awared prize of that donation will be returned
        public void returnAwardedPrize(string prizeID, int quantityToReturn)
        {
            foreach (Prize prize in myPrizes)
            {
                if (prizeID == prize.getID())
                {
                    prize.returnPrize(quantityToReturn);
                }
            }
        }

        //deleting donation -> return prize
        public Message deleteDonation(string donationID)
        {
            foreach (Donation donation in myDonations)
            {
                if (donation.getID() == donationID)
                {
                    myDonations.remove(donation);
                    returnAwardedPrize(donation.PrizeID, donation.QuantityAwarded);
                    ms.message = "Donation has been successfully deleted and the awarded prize has been returned!";
                    return ms;
                }
            }
            return ms;
            
        }

        //deleting donor -> delete his/her donations
        public Message deleteDonor(string donorID)
        {
            List<string> listDonationID = new List<string>();
            foreach (Donation donation in myDonations)
            {
                if (donation.DonorID == donorID)
                {
                    listDonationID.Add(donation.getID());
                }
            }

            foreach (Donor donor in myDonors)
            {
                if (donor.getID() == donorID)
                {
                    myDonors.remove(donor);
                    foreach (var item in listDonationID)
                    {
                        deleteDonation(item);
                    }
                    ms.message = "Donor and their donations have been successfully deleted from file! " +
                        "The awarded prize has also been returned!";
                    return ms;
                }
            }
            return ms;
        }

        //deleting prize -> delete all its' donations -> deduct totalvalue of sponsor
        public Message deletePrize(string prizeID)
        {
            List<string> listDonationID = new List<string>();
            foreach (Donation donation in myDonations)
            {
                if (donation.PrizeID == prizeID)
                {
                    listDonationID.Add(donation.getID());
                }
            }

            foreach (Prize prize in myPrizes)
            {
                if (prizeID == prize.getID())
                {
                    myPrizes.remove(prize);
                    foreach (Sponsor sponsor in mySponsors)
                    {
                        if (prize.SponsorID == sponsor.getID())
                        {
                            sponsor.deductValue(prize.calculateTotalValue());
                        }
                    }

                    foreach (var item in listDonationID)
                    {
                        deleteDonation(item);
                    }
                    ms.message = "Prize and its donations have been successfully deleted from file! Its sponsor's value has been also deducted!";
                    return ms;
                }
            }
            return ms;
        }

        //deleting sponsor -> delete all his/her prizes -> delete all prizes' donations
        public Message deleteSponsor(string sponsorID)
        {
            List<string> listPrizeID = new List<string>();
            foreach (Prize prize in myPrizes)
            {
                if (prize.SponsorID == sponsorID)
                {
                    listPrizeID.Add(prize.getID());
                }
            }

            List<string> listDonationID = new List<string>();
            foreach (Donation donation in myDonations)
            {
                foreach (var item in listPrizeID)
                {
                    if (donation.PrizeID == item)
                    {
                        listDonationID.Add(donation.getID());
                    }
                }
            }

            foreach (Sponsor sponsor in mySponsors)
            {
                if (sponsorID == sponsor.getID())
                {
                    mySponsors.remove(sponsor);
                    foreach (var item in listPrizeID)
                    {
                        deletePrize(item);
                    }
                    foreach (var item in listDonationID)
                    {
                        deleteDonation(item);
                    }
                    ms.message = "Sponsor and their prizes have been successfully deleted from file! " +
                        "All donations awarded with the prizes have been also deleted!";
                    return ms;
                }
            }
            return ms;
        }




        /// <summary>
        /// FETCH
        /// </summary>
        //fetch any entity's id 
        public List<string> fetchAnyID(string nameOfEntity)
        {
            List<string> listID = new List<string>();
            switch (nameOfEntity)
            {
                case "sponsor":
                    foreach (Sponsor sponsor in mySponsors)
                    {
                        listID.Add(sponsor.getID());
                    }
                    break;
                case "prize":
                    foreach (Prize prize in myPrizes)
                    {
                        listID.Add(prize.getID());
                    }
                    break;
                case "donor":
                    foreach (Donor donor in myDonors)
                    {
                        listID.Add(donor.getID());
                    }
                    break;
                case "donation":
                    foreach (Donation donation in myDonations)
                    {
                        listID.Add(donation.getID());
                    }
                    break;
            }
            return listID;
        }

        //fetch any entity's firstname
        public List<string> fetchFirstName(string nameOfEntity)
        {
            List<string> listFirstname = new List<string>();
            switch (nameOfEntity)
            {
                case "sponsor":
                    foreach (Sponsor sponsor in mySponsors)
                    {
                        listFirstname.Add(sponsor.FirstName);
                    }
                    break;
                case "donor":
                    foreach (Donor donor in myDonors)
                    {
                        listFirstname.Add(donor.FirstName);
                    }
                    break;
            }
            listFirstname.Sort();
            return listFirstname;
        }

        //fetch any entity's lastname
        public List<string> fetchLastName(string nameOfEntity)
        {
            List<string> listLastName = new List<string>();
            switch (nameOfEntity)
            {
                case "sponsor":
                    foreach (Sponsor sponsor in mySponsors)
                    {
                        listLastName.Add(sponsor.LastName);
                    }
                    break;
                case "donor":
                    foreach (Donor donor in myDonors)
                    {
                        listLastName.Add(donor.LastName);
                    }
                    break;
            }
            listLastName.Sort();
            return listLastName;
        }

        //fetch prize description
        public List<string> fetchPrizeDesc()
        {
            List<string> listDesc = new List<string>();
            foreach (Prize prize in myPrizes)
            {
                listDesc.Add(prize.Desc);
            }
            listDesc.Sort();
            return listDesc;
        }

        public List<string> fetchAllProps(int opt1, int opt2)
        {
            List<string> list = new List<string>();
            switch (opt1)
            {
                case 0:
                    switch (opt2)
                    {
                        case 0: list = fetchAnyID("sponsor"); break;
                        case 1: list = fetchFirstName("sponsor"); break;
                        case 2: list = fetchLastName("sponsor"); break;
                    }
                    break;
                case 1:
                    switch (opt2)
                    {
                        case 0: list = fetchAnyID("prize"); break;
                        case 1: list = fetchPrizeDesc(); break;
                        case 2: list = fetchAnyID("sponsor"); break;
                    }
                    break;
                case 2:
                    switch (opt2)
                    {
                        case 0: list = fetchAnyID("donor"); break;
                        case 1: list = fetchFirstName("donor"); break;
                        case 2: list = fetchLastName("donor"); break;
                        case 3: list = new List<string> { "Visa", "American Express", "Mastercard" }; break;
                    }
                    break;
                case 3:
                    switch (opt2)
                    {
                        case 0: list = fetchAnyID("donation"); break;
                        case 1: list = fetchAnyID("donor"); break;
                        case 2: list = fetchAnyID("prize"); break;
                    }
                    break;
            }
            list.Sort();
            return list;
        }
        


        /// <summary>
        /// SEARCH
        /// </summary>
        //search
        public List<string> search(int opt1, int opt2, string opt3)
        {
            List<string> list = new List<string>();
            switch (opt1)
            {
                case 0: //sponsor
                    foreach (Sponsor sponsor in mySponsors)
                    {
                        switch (opt2)
                        {
                            case 0:
                                if (opt3 == sponsor.getID())
                                    list.Add(sponsor.toString());
                                break;
                            case 1:
                                if (sponsor.FirstName.StartsWith(opt3, true, null))
                                    list.Add(sponsor.toString());
                                break;
                            case 2:
                                if (sponsor.LastName.StartsWith(opt3, true, null))
                                    list.Add(sponsor.toString());
                                break;
                        }
                    }
                    break;
                case 1:
                    foreach (Prize prize in myPrizes)
                    {
                        switch (opt2)
                        {
                            case 0:
                                if (opt3 == prize.getID())
                                    list.Add(prize.toString());
                                break;
                            case 1:
                                if (prize.Desc.StartsWith(opt3, true, null))
                                    list.Add(prize.toString());
                                break;
                            case 2:
                                if (opt3 == prize.SponsorID)
                                    list.Add(prize.toString());
                                break;
                        }
                    }
                    break;
                case 2:
                    foreach (Donor donor in myDonors)
                    {
                        switch (opt2)
                        {
                            case 0:
                                if (opt3 == donor.getID())
                                    list.Add(donor.toString());
                                break;
                            case 1:
                                if (donor.FirstName.StartsWith(opt3, true, null))
                                    list.Add(donor.toString());
                                break;
                            case 2:
                                if (donor.LastName.StartsWith(opt3, true, null))
                                    list.Add(donor.toString());
                                break;
                            case 3:
                                if (opt3.StartsWith(donor.CardType.ToString(), true, null))
                                    list.Add(donor.toString());
                                break;
                        }
                    }
                    break;
                case 3:
                    foreach (Donation donation in myDonations)
                    {
                        switch (opt2)
                        {
                            case 0:
                                if (opt3 == donation.getID())
                                    list.Add(donation.toString());
                                break;
                            case 1:
                                if (opt3 == donation.DonorID)
                                    list.Add(donation.toString());
                                break;
                            case 2:
                                if (opt3 == donation.PrizeID)
                                    list.Add(donation.toString());
                                break;
                        }
                    }
                    break;
            }
            list.Sort();
            return list;
        }



        /// <summary>
        /// MODIFY DATA
        /// </summary>
        //when adding a prize, increase totalvalue of sponsor
        public void addValueToSponsor(string sponsorID, double totalValue)
        {
            foreach (Sponsor sponsor in mySponsors)
            {
                if (sponsorID == sponsor.getID())
                {
                    sponsor.addValue(totalValue);
                }
            }
        }

        //when deleting a prize, totalvalue of sponsor will be deducted
        public void deductValueOfSponsor(string sponsorID, double totalValue)
        {
            foreach (Sponsor sponsor in mySponsors)
            {
                if (sponsorID == sponsor.getID())
                {
                    sponsor.deductValue(totalValue);
                }
            }
        }

    }
}
