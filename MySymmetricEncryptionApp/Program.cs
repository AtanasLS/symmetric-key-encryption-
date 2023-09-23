using System;

namespace MySymmetricEncryptionApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to My CLI Application!");
            Console.Write("Please enter your name: ");
            string name = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine($"Hello, {name}! Nice to meet you.");
            }
            else
            {
                Console.WriteLine("Hello, anonymous! Nice to meet you.");
            }
        }
    }
}

