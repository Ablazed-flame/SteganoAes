using System;

public static class SteganographyFlow
{
    public static void HideMessage()
    {
        Console.Write("Enter cover image path (PNG/BMP): ");
        string inputImagePath = Console.ReadLine();

        Console.Write("Enter message to hide: ");
        string message = Console.ReadLine();

        Console.Write("Enter password for encryption: ");
        string password = Console.ReadLine();

        var payload = EncryptionHelper.Encrypt(message, password);

        Console.Write("Enter steganopassword: ");
        string steganoKey = Console.ReadLine();

        Console.Write("Enter output image path: ");
        string outputImagePath = Console.ReadLine();

        SteganographyHelper.EmbedText(inputImagePath, outputImagePath, payload, steganoKey);

        Console.WriteLine("Message hidden successfully.");
    }

    public static void ExtractMessage()
    {
        Console.Write("Enter stego image path: ");
        string stegoImagePath = Console.ReadLine();

        Console.Write("Enter steganopassword: ");
        string steganoKey = Console.ReadLine();

        Console.Write("Enter password to decrypt: ");
        string password = Console.ReadLine();

        string payload = SteganographyHelper.ExtractText(stegoImagePath, steganoKey);
       
        string message = EncryptionHelper.Decrypt(payload, password);
        Console.WriteLine($"\nDecrypted message: {message}");
    }
}
