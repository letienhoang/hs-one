using System.Text.RegularExpressions;
using System.Text;

namespace HSOne.Core.Helpers
{
    public static class TextHelper
    {
        public static string ToUnsignedString(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            input = input.Trim().ToLower();

            input = Regex.Replace(input, @"[^\w\s-]", "-");
            input = Regex.Replace(input, @"\s+", "-");

            var regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
            var normalized = input.Normalize(NormalizationForm.FormD);
            input = regex.Replace(normalized, string.Empty).Replace('đ', 'd');

            input = Regex.Replace(input, @"-+", "-").Trim('-');

            return input;
        }
    }
}