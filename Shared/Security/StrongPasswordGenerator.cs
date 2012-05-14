using System;

namespace Highway.Shared.Security
{
    /// <summary>
    /// Generates a strong password. http://www.consultsarath.com/contents/articles/KB000011-snippet-generate-random-strong-password-string-using-cSharp.aspx
    /// </summary>
    public class StrongPasswordGenerator
    {
        // Create constant strings for each type of characters
        private const string AlphaCaps = "QWERTYUIOPASDFGHJKLZXCVBNM";
        private const string AlphaLow = "qwertyuiopasdfghjklzxcvbnm";
        private const string Numerics = "1234567890";
        private const string Special = "@#$!^~";

        // Create another string which is a concatenation of all above
        private const string AllChars = AlphaCaps + AlphaLow + Numerics + Special;
        private readonly Random r = new Random();

        public string GenerateStrongPassword(int length)
        {
            var generatedPassword = "";

            if (length < 4) throw new Exception("Number of characters should be greater than four otherwise it's not really strong, is it?");

            // Generate four repeating random numbers are postions of
            // lower, upper, numeric and special characters
            // By filling these positions with corresponding characters,
            // we can ensure the password has atleast one
            // character of those types
            int pLower, pUpper, pNumber, pSpecial;
            var posArray = "0123456789";
            if (length < posArray.Length)
            {
                posArray = posArray.Substring(0, length);
            }
            pLower = GetRandomPosition(ref posArray);
            pUpper = GetRandomPosition(ref posArray);
            pNumber = GetRandomPosition(ref posArray);
            pSpecial = GetRandomPosition(ref posArray);


            for (var i = 0; i < length; i++)
            {
                if (i == pLower)
                    generatedPassword += GetRandomChar(AlphaCaps);
                else if (i == pUpper)
                    generatedPassword += GetRandomChar(AlphaLow);
                else if (i == pNumber)
                    generatedPassword += GetRandomChar(Numerics);
                else if (i == pSpecial)
                    generatedPassword += GetRandomChar(Special);
                else
                    generatedPassword += GetRandomChar(AllChars);
            }
            return generatedPassword;
        }

        private string GetRandomChar(string fullString)
        {
            return fullString.ToCharArray()[(int)Math.Floor(r.NextDouble() * fullString.Length)].ToString();
        }

        private int GetRandomPosition(ref string posArray)
        {
            int pos;
            string randomChar = posArray.ToCharArray()[(int)Math.Floor(r.NextDouble() * posArray.Length)].ToString();
            pos = int.Parse(randomChar);
            posArray = posArray.Replace(randomChar, "");
            return pos;
        }
    }
}