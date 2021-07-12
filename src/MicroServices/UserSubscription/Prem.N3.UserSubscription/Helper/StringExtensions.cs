using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Prem.N3.UserSubscription.Helper
{

    public static class StringExtensions
    {
        private static string _DefaultFormat = "MM-dd-yyyy";
        private static string[] _Formats = { 
                // Basic formats
                "yyyyMMddTHHmmsszzz",
                "yyyyMMddTHHmmsszz",
                "yyyyMMddTHHmmssZ",
                "yyyyMMddTHHmmss",
                "yyyyMMdd",
                // Extended formats
                "yyyy-MM-ddTHH:mm:sszzz",
                "yyyy-MM-ddTHH:mm:sszz",
                "yyyy-MM-ddTHH:mm:ssZ",
                "yyyy-MM-ddTHH:mm:ss",
                "yyyy-MM-dd",
                // All of the above with reduced accuracy
                "yyyyMMddTHHmmzzz",
                "yyyyMMddTHHmmzz",
                "yyyyMMddTHHmmZ",
                "yyyyMMddTHHmm",
                "yyyy-MM-ddTHH:mmzzz",
                "yyyy-MM-ddTHH:mmzz",
                "yyyy-MM-ddTHH:mmZ",
                "yyyy-MM-ddTHH:mm",
                // Accuracy reduced to hours
                "yyyyMMddTHHzzz",
                "yyyyMMddTHHzz",
                "yyyyMMddTHHZ",
                "yyyyMMddTHH",
                "yyyy-MM-ddTHHzzz",
                "yyyy-MM-ddTHHzz",
                "yyyy-MM-ddTHHZ",
                "yyyy-MM-ddTHH",
                "dd/MM/yyyy",
                "MM/dd/yyyy",
                "dd-MM-yyyy",
                "MM-dd-yyyy",
                "dd MMM, YYYY",
                "MM/dd/yyyy HH:mm:ss",
                "yyyyMMdd HH:mm:ss"
                };

        public static string ReadAsString(this Stream stream)
        {
            return new StreamReader(stream).ReadToEnd();
        }
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value?.Trim());
        }
        public static bool IsNotNullOrEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value?.Trim());
        }

        public static string MaskPAN(this string value)
        {
            string returnVal = null;

            if (value.Length >= 6)
            {
                returnVal = string.Concat("".PadLeft(11, '*'), value.Substring(value.Length - 6));
            }
            else
            {
                returnVal = value.PadLeft(17, '*');
            }

            return returnVal;
        }

        public static string MaskEmail(this string email)
        {
            if (email.Length == 0 || string.IsNullOrEmpty(email))
            {
                return string.Empty;
            }
            var maskedEmail = string.Format("{0}{1}****{2}", email[0], email[1], email.Substring(email.IndexOf('@') - 1));

            return maskedEmail;
        }

        public static string TryParseDate(this string strDate)
        {


            if (string.IsNullOrWhiteSpace(strDate))
            {
                return string.Empty;
            }
            else
            {
                try
                {
                    return DateTime.ParseExact(strDate, _Formats, CultureInfo.InvariantCulture, DateTimeStyles.None)
                        .ToString(_DefaultFormat);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public static string TryParseDate(this string strDate, string preferredFormat)
        {
            string defaultFormat = !string.IsNullOrEmpty(preferredFormat) ? preferredFormat : _DefaultFormat;
            return DateTime.ParseExact(strDate, _Formats, CultureInfo.InvariantCulture, DateTimeStyles.None)
                        .ToString(defaultFormat);
        }
        public static string TryParseDate(this DateTime date, string preferredFormat)
        {
            string defaultFormat = !string.IsNullOrEmpty(preferredFormat) ? preferredFormat : _DefaultFormat;
            return date.ToString(defaultFormat);
        }
    }
}
