using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace SRTPluginProviderRE6
{
    /// <summary>
    /// SHA256 hashes for the RE6 game executables.
    /// </summary>
    public static class GameHashes
    {
        private static readonly byte[] RE6_1_0_6 = new byte[32] { 0x72, 0xB5, 0x2D, 0x06, 0xA1, 0xC8, 0x78, 0xA6, 0xD6, 0x67, 0x01, 0xB1, 0x92, 0xA4, 0x75, 0x57, 0xBD, 0x84, 0x4A, 0x04, 0x73, 0xD6, 0xF3, 0xCA, 0xFB, 0x0C, 0x73, 0x61, 0x44, 0x23, 0x5F, 0x74 };
        private static readonly byte[] RE6_1_1_0 = new byte[32] { 0xB8, 0xBC, 0xEA, 0xF0, 0x92, 0x72, 0xD5, 0xF3, 0x0B, 0x89, 0x94, 0x07, 0x37, 0x76, 0x3A, 0xFA, 0x6D, 0xFB, 0x78, 0x64, 0x7C, 0x81, 0x16, 0x5A, 0xF1, 0xAA, 0x8A, 0x10, 0xD0, 0xAF, 0xEB, 0x35 };

        public static GameVersion DetectVersion(string filePath)
        {
            byte[] checksum;
            using (SHA256 hashFunc = SHA256.Create())
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                checksum = hashFunc.ComputeHash(fs);

            if (checksum.SequenceEqual(RE6_1_0_6))
            {
                Console.WriteLine("Old Patch");
                return GameVersion.RE6_1_0_6;
            }

            else if (checksum.SequenceEqual(RE6_1_1_0))
            {
                Console.WriteLine("Latest Release");
                return GameVersion.RE6_1_1_0;
            }

            Console.WriteLine("Unknown Version");
            return GameVersion.Unknown;
        }
    }
}