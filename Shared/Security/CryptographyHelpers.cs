using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Highway.Shared.Security
{
    public class CryptographyHelpers
    {
        /// <summary>
        /// Encodes the password using the <see cref="SHA256Managed"/> <see cref="HashAlgorithm"/>.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="saltValue">The salt value. Null if no salt value.</param>
        /// <returns>The Base64 encoding <see cref="String"/> of the hashed password.</returns>
        public static string HashPassword(string password, string saltValue = null)
        {
            var encoding = new UnicodeEncoding();
            var builder = new StringBuilder();
            var salt = saltValue == null ? GenerateSalt() : GetSaltFromString(saltValue);
            var dataBuffer = new byte[encoding.GetByteCount(password) + SaltValueSize];
            var stringToHashBytes = encoding.GetBytes(password);

            salt.CopyTo(dataBuffer, 0);
            stringToHashBytes.CopyTo(dataBuffer, SaltValueSize);

            HashAlgorithm hashAlgorithm = new SHA256Managed();

            var hashedBytes = hashAlgorithm.ComputeHash(dataBuffer);

            foreach (var outputByte in salt) builder.Append(outputByte.ToString("x2").ToUpper());
            foreach (var outputByte in hashedBytes) builder.Append(outputByte.ToString("x2").ToUpper());

            return builder.ToString();
        }   

        /// <summary>
        /// Validates the provided password passes the required complexity tests.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="minRequiredPasswordLength">The min required length of the password.</param>
        /// <param name="minRequiredNonAlphanumericCharacters">The min required non alphanumeric characters in the password.</param>
        /// <param name="passwordStrengthRegularExpression">The password strength regular expression.</param>
        /// <returns>
        /// 	<c>true</c> if the password is strong enough; <c>false</c> otherwise.
        /// </returns>
        public static bool ValidatePasswordStrength(string password, int minRequiredPasswordLength,
                                             int minRequiredNonAlphanumericCharacters,
                                             string passwordStrengthRegularExpression)
        {
            if (string.IsNullOrEmpty(password)) return false;

            if (password.Length < minRequiredPasswordLength) return false;

            if (password.ToCharArray().Where(c => !char.IsLetterOrDigit(c)).Count() <
                minRequiredNonAlphanumericCharacters)
                return false;

            if (passwordStrengthRegularExpression != null)
            {
                if (!Regex.IsMatch(password, passwordStrengthRegularExpression)) return false;
            }

            return true;
        }

        public static bool ValidateEmail(string email)
        {
            if (email != null)
            {
                email = email.Trim();
            }

            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            const string simpleEmailRegex = @"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$";
            var emailRegexValidator = new Regex(simpleEmailRegex, RegexOptions.IgnoreCase);
            return emailRegexValidator.IsMatch(email);
        }

        const int SaltValueSize = 4;

        private static byte[] GetSaltFromString(string saltValue)
        {
            var saltBytes = new byte[SaltValueSize];

            saltBytes[0] = byte.Parse(saltValue.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat);
            saltBytes[1] = byte.Parse(saltValue.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat);
            saltBytes[2] = byte.Parse(saltValue.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat);
            saltBytes[3] = byte.Parse(saltValue.Substring(6, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat);

            return saltBytes;
        }

        private static byte[] GenerateSalt()
        {
            var saltBytes = new byte[SaltValueSize];
            var rng = new RNGCryptoServiceProvider();
            rng.GetNonZeroBytes(saltBytes);
     
            return saltBytes;
        }
    }
}