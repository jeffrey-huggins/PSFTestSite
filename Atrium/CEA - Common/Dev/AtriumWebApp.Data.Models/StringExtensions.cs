using System;

namespace AtriumWebApp.Models
{
    public static class StringExtensions
    {
        public static string Truncate(this string value, int maxLength, bool appendEllipsisOnTruncate = false)
        {
            if (!value.LengthExceeds(maxLength))
            {
                return value;
            }
            if (appendEllipsisOnTruncate)
            {
                if (maxLength <= 3)
                {
                    throw new ArgumentOutOfRangeException("maxLength", "Max truncated string length must be greater than three if an ellipsis is to be appended to the resulting string.");
                }
                return value.Substring(0, maxLength - 3) + "...";
            }
            return value.Substring(0, maxLength);
        }

        public static bool LengthExceeds(this string value, int length)
        {
            return value != null && value.Length > length;
        }
    }
}