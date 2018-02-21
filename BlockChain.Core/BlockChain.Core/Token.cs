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
    }
}
