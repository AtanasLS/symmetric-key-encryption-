# Symmetric-Key Encryption CLI Application

This command line interface (CLI) application allows you to perform symmetric-key encryption and decryption using the Advanced Encryption Standard (AES) in Galois/Counter Mode (AES-GCM). It provides the option to encrypt a message and save it to a file or read a previously encrypted message from a file, decrypt it, and display the original message.

## Prerequisites

Before using this application, ensure you have the following prerequisites installed:

- .NET Core Runtime: [Download .NET Core](https://dotnet.microsoft.com/download)

## Getting Started

1. Clone or download this repository to your local machine.

2. Open your command prompt or terminal and navigate to the application's directory.

3. Build the application:

   ```bash
   dotnet build
4. Run the application:

## Usage

**Encrypt Message** 

1. Select the option to "Encrypt a Message" in the CLI menu.

2. Enter a passphrase. This passphrase will be used for encryption and decryption. Make sure it is a strong, secure passphrase.
3. Enter the message you want to encrypt.

4. Choose a file name to save the encrypted message.

5. The application will encrypt the message and save it to the specified file.

**Decrypt Message**

1. Select the option to "Decrypt a Message" in the CLI menu.

2. Enter the passphrase that was used to encrypt the message.

3. Specify the file containing the encrypted message.

4. The application will decrypt the message and display the original text.

## Additional Resources

 -For more information on authenticated encryption in .NET with AES-GCM, refer to Authenticated Encryption in .NET with AES-GCM.

- To learn how to write text to a file in .NET, check out How to: Write text to a file.

- To learn how to read text from a file in .NET, refer to How to: Read text from a file.

> Security Note: Always keep your passphrase secure and do not share it with others. A strong passphrase is essential for the security of your encrypted messages.

## Licencse

This project is licensed under the MIT License - see the LICENSE file for details.



