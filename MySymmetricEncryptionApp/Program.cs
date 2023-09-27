using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MySymmetricEncryptionApp
{
    class Program
    {
        static void Main()
        {
            while (true)
            {
                Console.WriteLine("1: Safely store message");
                Console.WriteLine("2: Read message");
                Console.WriteLine("0: Exit");

                int choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        SafelyStoreMessage();
                        break;

                    case 2:
                        ReadMessage();
                        break;

                    case 0:
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        static void SafelyStoreMessage()
        {
            Console.WriteLine("Passphrase:");
            string passphrase = Console.ReadLine();

            Console.WriteLine("Type a message to encrypt:");
            string messageToEncrypt = Console.ReadLine();

            byte[] aesKey = GenerateAesKey(passphrase);
            string encryptedMessage = EncryptMessage(messageToEncrypt, aesKey);

            File.WriteAllText("encrypted.txt", encryptedMessage);
            Console.WriteLine("Message encrypted and saved to 'encrypted.txt'");
        }

        static void ReadMessage()
        {
            if (File.Exists("encrypted.txt"))
            {
                Console.WriteLine("Passphrase:");
                string passphrase = Console.ReadLine();

                string encryptedText = File.ReadAllText("encrypted.txt");
                byte[] aesKeyForDecryption = GenerateAesKey(passphrase);
                string decryptedMessage = DecryptMessage(encryptedText, aesKeyForDecryption);
                Console.WriteLine(decryptedMessage);
            }
            else
            {
                Console.WriteLine("No encrypted message found.");
            }
        }

        static byte[] GenerateAesKey(string passphrase)
        {
            using Rfc2898DeriveBytes kdf = new Rfc2898DeriveBytes(passphrase, new byte[16], 10000);
            return kdf.GetBytes(16); // 16 bytes = 128 bits
        }

        static string EncryptMessage(string message, byte[] aesKey)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = aesKey;
            aesAlg.Mode = CipherMode.CBC;

            aesAlg.GenerateIV();

            using ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            byte[] encryptedBytes;
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(message);
                    }
                }
                encryptedBytes = msEncrypt.ToArray();
            }

            byte[] result = new byte[aesAlg.IV.Length + encryptedBytes.Length];
            aesAlg.IV.CopyTo(result, 0);
            encryptedBytes.CopyTo(result, aesAlg.IV.Length);

            return Convert.ToBase64String(result);
        }

        static string DecryptMessage(string encryptedText, byte[] aesKey)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = aesKey;
            aesAlg.Mode = CipherMode.CBC;

            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            byte[] iv = new byte[aesAlg.IV.Length];
            byte[] encryptedData = new byte[encryptedBytes.Length - aesAlg.IV.Length];
            Array.Copy(encryptedBytes, iv, aesAlg.IV.Length);
            Array.Copy(encryptedBytes, aesAlg.IV.Length, encryptedData, 0, encryptedData.Length);

            using ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, iv);

            using MemoryStream msDecrypt = new MemoryStream(encryptedData);
            using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new StreamReader(csDecrypt);
            string decryptedMessage = srDecrypt.ReadToEnd();

            return decryptedMessage;
        }
    }
}