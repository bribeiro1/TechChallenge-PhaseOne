using System.Text.RegularExpressions;

namespace Contacts.Application.Utils
{
    public static class StringUtils
    {
        public static bool ValidatePhoneDDD(string phoneDDD)
        {
            var validDDDs = new List<string>()
        {
            "11", "12", "13", "14", "15", "16", "17", "18", "19",
            "21", "22", "24", "27", "28",
            "31", "32", "33", "34", "35", "37", "38",
            "41", "42", "43", "44", "45", "46", "47", "48", "49",
            "51", "53", "54", "55",
            "61", "62", "64", "63",
            "65", "66", "67",
            "68", "69",
            "71", "73", "74", "75", "77", "79",
            "81", "82", "83", "84", "85", "86", "87", "88", "89",
            "91", "92", "93", "94", "95", "96", "97", "98", "99"
        };

            return validDDDs.Contains(phoneDDD);
        }

        public static bool ValidatePhoneNumber(string phoneNumber)
        {
            string mobilePhonePattern = @"^9\d{8}$";
            string fixedPhonePattern = @"^[2-5]\d{7}$";

            var isMobileValid = Regex.IsMatch(phoneNumber, mobilePhonePattern, RegexOptions.None, TimeSpan.FromMilliseconds(500));
            var isFixedValid = Regex.IsMatch(phoneNumber, fixedPhonePattern, RegexOptions.None, TimeSpan.FromMilliseconds(500));

            return isMobileValid || isFixedValid;
        }

        public static bool ValidateEmailAddress(string emailAddress)
        {
            string emailPattern = @"^[\w\.-]+@[a-zA-Z\d\.-]+\.[a-zA-Z]{2,}$";

            var isEmailValid = Regex.IsMatch(emailAddress, emailPattern, RegexOptions.None, TimeSpan.FromMilliseconds(500));
            return isEmailValid;
        }
    }
}