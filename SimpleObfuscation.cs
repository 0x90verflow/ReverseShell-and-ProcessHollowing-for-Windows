using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace caesar_encode
{
    internal class Program
    {
        static void Main(string[] args)
        {
            byte[] payload = new byte[3] { 0x90, 0x90, 0x90  };

            byte[] cipherpayload = new byte[payload.Length];

            for (int i = 0; i < payload.Length; i++)
            {
                cipherpayload[i] = (byte)(((uint)payload[i] + 20) & 0xFF);
            }

            byte[] xoredpaylaod = new byte[cipherpayload.Length];
            string key = "Key for xoring the paylaod";

            for (int i = 0; i < xoredpaylaod.Length; i++)
            {
                xoredpaylaod[i] = (byte)((uint)cipherpayload[i] ^ (key[i] % key.Length));
            }
            Console.WriteLine("The Obfuscated payload :");
            Console.WriteLine("buffer = new byte[] { " + string.Join(", ", xoredpaylaod.Select(b => $"0x{b:X2}")) + " };");

          
        }

    }
}
