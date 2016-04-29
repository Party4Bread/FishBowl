using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManyMonkeys.Cryptography;
using System.Security.Cryptography;
using System.IO;

namespace FishBowl
{
    public class Decrypter
    {
        public static void decrypt(string filename, string key, string sname)
        {
            Twofish fish = new Twofish();
            fish.Mode = CipherMode.CBC;
            fish.KeySize = 128;
            while (key.Length < 16)
            {
                key += " ";
            }
            byte[] Key = new byte[16];
            Key = Encoding.ASCII.GetBytes(key);
            byte[] iv = new byte[16];
            iv = Encoding.ASCII.GetBytes(key);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            ICryptoTransform decrypt = fish.CreateDecryptor(Key, iv);
            CryptoStream cryptostream = new CryptoStream(ms, decrypt, CryptoStreamMode.Write);
            byte[] data = File.ReadAllBytes(filename);
            cryptostream.Write(data, 0, data.Length);
            cryptostream.Close();
            byte[] bytOut = ms.ToArray();
            File.WriteAllBytes(sname, bytOut);
        }
        //http://www.codeproject.com/Articles/2593/A-C-implementation-of-the-Twofish-cipher#_comments
    }
    //http://www.codeproject.com/Articles/2593/A-C-implementation-of-the-Twofish-cipher#_comments
}

