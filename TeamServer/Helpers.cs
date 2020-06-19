using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamServer
{
    public class Helpers
    {
        public static byte[] GeneratePseudoRandomBytes(int lengh)
        {
            return Encoding.UTF8.GetBytes(GeneratePseudoRandomString(lengh));
        }

        public static string GeneratePseudoRandomString(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
