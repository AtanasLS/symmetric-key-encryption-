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
                Console.WriteLine("Passphrase:");
                string passphrase = Console.ReadLine();

                Console.WriteLine("1: Safely store message");
                Console.WriteLine("2: Read message");
                Console.WriteLine("0: Exit");

                int choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        Console.WriteLine("Type a message to encrypt:");
                        string messageToEncrypt = Console.ReadLine();

                        // Generate a valid AES key from the passphrase
                        byte[] aesKey = GenerateAesKey(passphrase);

                        // Encrypt the message and save it to a file
                        string encryptedMessage = EncryptMessage(messageToEncrypt, aesKey);
                        File.WriteAllText("encrypted.txt", encryptedMessage);
                        Console.WriteLine("Message encrypted and saved to 'encrypted.txt'");
                        break;

                    case 2:
                        if (File.Exists("encrypted.txt"))
                        {
                            // Read the encrypted message from a file, decrypt, and display it
                            string encryptedText = File.ReadAllText("encrypted.txt");
                            byte[] aesKeyForDecryption = GenerateAesKey(passphrase);
                            string decryptedMessage = DecryptMessage(encryptedText, aesKeyForDecryption);
                            Console.WriteLine(decryptedMessage);
                        }
                        else
                        {
                            Console.WriteLine("No encrypted message found.");
                        }
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

        static byte[] GenerateAesKey(string passphrase)
        {
            // Use a key derivation function (KDF) like PBKDF2 to derive a valid AES key
            using Rfc2898DeriveBytes kdf = new Rfc2898DeriveBytes(passphrase, new byte[16], 10000);
            return kdf.GetBytes(16); // 16 bytes = 128 bits
        }

        static string EncryptMessage(string message, byte[] aesKey)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = aesKey;
            aesAlg.Mode = CipherMode.CBC;

            // Generate a random IV (Initialization Vector)
            aesAlg.GenerateIV();

            // Create an encryptor with the AES key and IV
            using ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Encrypt the message
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

            // Combine IV and encrypted data into a single byte array
            byte[] result = new byte[aesAlg.IV.Length + encryptedBytes.Length];
            aesAlg.IV.CopyTo(result, 0);
            encryptedBytes.CopyTo(result, aesAlg.IV.Length);

            // Convert the combined byte array to Base64 for easy storage and transport
            return Convert.ToBase64String(result);
        }

        static string DecryptMessage(string encryptedText, byte[] aesKey)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = aesKey;
            aesAlg.Mode = CipherMode.CBC;

            // Extract the IV and encrypted data from the Base64 string
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            byte[] iv = new byte[aesAlg.IV.Length];
            byte[] encryptedData = new byte[encryptedBytes.Length - aesAlg.IV.Length];
            Array.Copy(encryptedBytes, iv, aesAlg.IV.Length);
            Array.Copy(encryptedBytes, aesAlg.IV.Length, encryptedData, 0, encryptedData.Length);

            // Create a decryptor with the AES key, IV, and additional authenticated data
            using ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, iv);

            // Decrypt the message
            using MemoryStream msDecrypt = new MemoryStream(encryptedData);
            using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new StreamReader(csDecrypt);
            string decryptedMessage = srDecrypt.ReadToEnd();

            return decryptedMessage;
        }
    }
}