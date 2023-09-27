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
            (byte[] ciphertext, byte[] nonce, byte[] tag) = EncryptWithNet(messageToEncrypt, aesKey);

            // Save the ciphertext, nonce, and tag to separate files
            File.WriteAllBytes("ciphertext.bin", ciphertext);
            File.WriteAllBytes("nonce.bin", nonce);
            File.WriteAllBytes("tag.bin", tag);

            Console.WriteLine("Message encrypted and saved.");
        }

        static void ReadMessage()
        {
            if (File.Exists("ciphertext.bin") && File.Exists("nonce.bin") && File.Exists("tag.bin"))
            {
                Console.WriteLine("Passphrase:");
                string passphrase = Console.ReadLine();

                byte[] ciphertext = File.ReadAllBytes("ciphertext.bin");
                byte[] nonce = File.ReadAllBytes("nonce.bin");
                byte[] tag = File.ReadAllBytes("tag.bin");

                byte[] aesKeyForDecryption = GenerateAesKey(passphrase);
                string decryptedMessage = DecryptWithNet(ciphertext, nonce, tag, aesKeyForDecryption);
                Console.WriteLine("Decrypted Message:");
                Console.WriteLine(decryptedMessage);
            }
            else
            {
                Console.WriteLine("No encrypted message found.");
            }
        }

         static byte[] GenerateAesKey(string passphrase)
        {
            // Use a key derivation function (KDF) like PBKDF2 to derive a valid AES key
            using Rfc2898DeriveBytes kdf = new Rfc2898DeriveBytes(passphrase, new byte[16], 10000);
            return kdf.GetBytes(16); // 16 bytes = 128 bits
        }

       
        private static (byte[] ciphertext, byte[] nonce, byte[] tag) EncryptWithNet(string plaintext, byte[] key)
        {
            using (var aes = new AesGcm(key))
            {
                var nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
                RandomNumberGenerator.Fill(nonce);

                var tag = new byte[AesGcm.TagByteSizes.MaxSize];

                var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
                var ciphertext = new byte[plaintextBytes.Length];

                aes.Encrypt(nonce, plaintextBytes, ciphertext, tag);

                return (ciphertext, nonce, tag);
            }
        }

        private static string DecryptWithNet(byte[] ciphertext, byte[] nonce, byte[] tag, byte[] key)
        {
            using (var aes = new AesGcm(key))
            {
                var plaintextBytes = new byte[ciphertext.Length];

                aes.Decrypt(nonce, ciphertext, tag, plaintextBytes);

                return Encoding.UTF8.GetString(plaintextBytes);
            }
        }
    }
}