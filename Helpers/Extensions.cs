using Humanizer;
using System;

namespace Hotvenues.Helpers
{
    public static class Extensions
    {
        public static string ToWords(this decimal amount)
        {
            var result = "";
            if (amount == 0) return result;
            var amountParts = Math.Abs(amount).ToString("##,###.00").Split('.');
            var wholeNumber = long.Parse(amountParts[0]);
            if (wholeNumber > 0) result += $"{wholeNumber.ToWords()} Ghana Cedis ";

            if (amountParts.Length > 1)
            {
                var decimalUnits = long.Parse(amountParts[1]);
                if (decimalUnits > 0) result += $"{decimalUnits.ToWords()} Pesewas";
            }


            return $"{result.Trim()} Only".Titleize();
        }

        
    }
}
