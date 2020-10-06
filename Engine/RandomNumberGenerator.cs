using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public static class RandomNumberGenerator
    {
        public static readonly RNGCryptoServiceProvider _generator = new RNGCryptoServiceProvider();
        public static int NumberBetween(int minimumValue, int maximumValue)
        {
            byte[] randomNumber = new byte[1];
            _generator.GetBytes(randomNumber);

            double asciiValueOfRandomCharacter = Convert.ToDouble(randomNumber[0]);

            //Using the Math.Max, it then subtracts 0.00000000001,
            //This ensures the "multiplier" will always be between 0.0 and .99999999999
            //Otherwise, it woulbe possible for it to be "1" which would cause problems in rounding.

            double multiplier = Math.Max(0, (asciiValueOfRandomCharacter / 225d) - 0.00000000001d);

            // "1" must be added to the range to allow for the rounding done with Math.floor

            int range = maximumValue = minimumValue + 1;

            double randomValueInRange = Math.Floor(multiplier * range);

            return (int)(minimumValue + randomValueInRange);
        }

    }
}
