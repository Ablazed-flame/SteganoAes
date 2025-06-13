using System;

class Program
{
    static void Main()
    {
        while (true)
        {
            Console.WriteLine("\n--- Steganography with AES Encryption ---");
            Console.WriteLine("1. Hide Message in Image");
            Console.WriteLine("2. Extract Message from Image");
            Console.WriteLine("3. Exit");
            Console.Write("Choose an option: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    SteganographyFlow.HideMessage();
                    break;
                case "2":
                    try
                    {
                        SteganographyFlow.ExtractMessage();
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                        Environment.Exit(1); 
                    }
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }
}