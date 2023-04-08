using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ETS_LIBRARY
{
    public class Validator
    {
        public static bool isValidID(string ID) //valid: A999
        {
            bool valid = false;
            if (Regex.Match(ID, @"^[A-Za-z]\d{3}$").Success)
                valid = true;
            return valid;
        } 

        public static bool isValidName(string name) //valid: a-z or A-Z and 1-15 characters
        {
            bool valid = false;
            if (Regex.Match(name, @"^[a-zA-Z -&]{1,15}$").Success)
                valid = true;
            return valid;
        } 

        public static bool isValidAddress(string addr)
        {
            bool valid = false;
            if (Regex.Match(addr, @"^[#.0-9a-zA-Z\s-]{1,40}$").Success)
                valid = true;
            return valid;
        }

        public static bool isValidPhoneNum(string phone) //valid: (###) ###-####
        {
            bool valid = false;
            if (Regex.Match(phone, @"^\(([1-9][0-9]{2})\)\s([0-9]{3})\-([0-9]{4})$").Success)
                valid = true;
            return valid;
        }

        public static bool isValidCardNum(string cardNumber) //valid: 16 digits (not starting from 0)
        {
            bool valid = false;
            if (Regex.Match(cardNumber, @"^[1-9]\d{15}$").Success)
                valid = true;
            return valid;
        } 

        public static bool isValidExpiredDate(string expDate) // valid: 11/2022 - 12/2029
        {
            if (Regex.Match(expDate, @"^((0[1-9]|1[0-2])\/?(202[3-9]))|((1[1-2])\/?(202[2-9]))$").Success)
                return true;
            return false;
        }

        public static bool isValidPrizeValue(double price) //valid: (0.0 - 299.99)
        {
            bool valid = false;
            if (Regex.Match(price.ToString(), @"^([0-2]?[0-9]?[0-9])(?:\.\d{0,2})?$").Success)
                valid = true;
            return valid;
        } 

        public static bool isValidDonationAmount(double amount) //valid: (5.0 - 999999.99)
        {
            bool valid = false;
            if (Regex.Match(amount.ToString(), @"^(5|[1-9]\d{0,5})(?:\.\d{0,2})?$").Success)
                valid = true;
            return valid;   
        } 

        public static bool isValidQuantity(int quantity)
        {
            bool valid = false;
            if (quantity > 0 && quantity <= 999)
                valid = true;
            return valid;
        } //valid: 1-999
    }
}
