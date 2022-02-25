namespace Utils
{
    using System;
    using System.Linq;

    public static class StringExtensions
    {
        private static Random random = new Random();
        public static string FillWithRandomString(this string str, int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789qwertyuioplkjhgfdsazxcvbnmx";
            str = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return str;
        }
    }
}