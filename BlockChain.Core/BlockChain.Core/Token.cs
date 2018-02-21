using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BlockChain.Core
{
    public static class Token
    {
        private const ulong _oneToken = 1 * 1000 * 1000 * 1000;

        private static readonly NumberFormatInfo _numberFormatInfo = new NumberFormatInfo() { NumberDecimalSeparator = DecimalDelimeterString };


        public static ulong OneToken => _oneToken; 

        public static char DecimalDelimeter => '.';

        public static string DecimalDelimeterString => DecimalDelimeter.ToString();

        public static string GetFormatted(ulong tokens)
        {
            decimal wholePart = tokens / (decimal) _oneToken;
            return wholePart.ToString(_numberFormatInfo);            
        }

        public static string GetFormattedTokens(this ulong tokens)
        {
            return GetFormatted(tokens);
        }

        public static ulong ConvertToTokens(string value)
        {
            string[] parts = value.Split(DecimalDelimeter);

            ulong wholeTokens = ulong.Parse(parts[0]) * OneToken;
            ulong fractionPart = ulong.Parse(parts[1].PadRight(9, '0'));

            return wholeTokens + fractionPart;
        }

        public static ulong ToTokens(this string value)
        {
            return ConvertToTokens(value);
        }
    }
}
