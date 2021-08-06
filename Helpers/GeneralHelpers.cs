using System;
using System.Linq;

namespace Hotvenues.Helpers
{
    public class GeneralHelpers
    {
        private static readonly Random Random = new Random();

        public static string TokenCode(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwsyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public static string TokenLetterCode(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwsyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public static string TokenDigitCode(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public static DateTime ParseDate(string dateStr)
        {
            var parsed = DateTime.TryParse(dateStr, out var date);
            return parsed ? date : DateTime.Today;
        }
    }
}