using System;
using System.Text;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;

public static class SteganographyHelper
{
    private const string MagicHeader = "STEGANO:";

    // XOR-encrypts the message using the steganoKey and returns binary string
    private static string ToBinaryWithKey(string text, string steganoKey)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < text.Length; i++)
        {
            char keyChar = steganoKey[i % steganoKey.Length];
            char encryptedChar = (char)(text[i] ^ keyChar);
            sb.Append(Convert.ToString(encryptedChar, 2).PadLeft(8, '0'));
        }

        return sb.ToString();
    }

    // Converts binary to text, and applies XOR with steganoKey to decrypt
    private static string FromBinaryWithKey(string binary, string steganoKey, int expectedLength)
    {
        StringBuilder result = new StringBuilder();

        for (int i = 0; i + 8 <= binary.Length && result.Length < expectedLength; i += 8)
        {
            string byteStr = binary.Substring(i, 8);
            char encryptedChar = (char)Convert.ToByte(byteStr, 2);
            char keyChar = steganoKey[(i / 8) % steganoKey.Length];
            char originalChar = (char)(encryptedChar ^ keyChar);
            result.Append(originalChar);
        }

        return result.ToString();
    }

    public static void EmbedText(string inputImagePath, string outputImagePath, string message, string steganoKey)
    {
        inputImagePath = inputImagePath.Trim('"');
        outputImagePath = outputImagePath.Trim('"');

        try
        {
            using var image = Image.Load<Rgba32>(inputImagePath);

            string lengthEncoded = message.Length.ToString().PadLeft(6, '0');
            string payload = MagicHeader + lengthEncoded + message;
            string binaryMessage = ToBinaryWithKey(payload, steganoKey);
            int index = 0;

            for (int y = 0; y < image.Height && index < binaryMessage.Length; y++)
            {
                for (int x = 0; x < image.Width && index < binaryMessage.Length; x++)
                {
                    Rgba32 pixel = image[x, y];

                    byte r = (byte)((pixel.R & ~1) | (binaryMessage[index++] - '0'));
                    byte g = (byte)((pixel.G & ~1) | (index < binaryMessage.Length ? binaryMessage[index++] - '0' : 0));
                    byte b = (byte)((pixel.B & ~1) | (index < binaryMessage.Length ? binaryMessage[index++] - '0' : 0));

                    image[x, y] = new Rgba32(r, g, b, pixel.A);
                }
            }

            image.Save(outputImagePath, new PngEncoder());
        }
        catch (FileNotFoundException)
        {
            throw new InvalidOperationException("Input image file not found. Please check the path.");
        }
        catch (DirectoryNotFoundException)
        {
            throw new InvalidOperationException("Directory does not exist for the given input or output path.");
        }
        catch (UnauthorizedAccessException)
        {
            throw new InvalidOperationException("You don't have permission to access the input or output file.");
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException("Error reading or writing image file: " + ex.Message);
        }
        catch (SixLabors.ImageSharp.ImageFormatException)
        {
            throw new InvalidOperationException("The input file is not a valid image or is corrupted.");
        }
    }

    public static string ExtractText(string imagePath, string steganoKey)
    {
        imagePath = imagePath.Trim('"');

        try
        {
            using var image = Image.Load<Rgba32>(imagePath);
            StringBuilder binary = new StringBuilder();

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Rgba32 pixel = image[x, y];
                    binary.Append(pixel.R & 1);
                    binary.Append(pixel.G & 1);
                    binary.Append(pixel.B & 1);
                }
            }

            string headerAndLength = FromBinaryWithKey(binary.ToString(), steganoKey, MagicHeader.Length + 6);

            if (!headerAndLength.StartsWith(MagicHeader))
                throw new InvalidOperationException("Steganography extraction failed. The steganoKey may be incorrect.");

            string lenStr = headerAndLength.Substring(MagicHeader.Length, 6);
            if (!int.TryParse(lenStr, out int actualMessageLength))
                throw new InvalidOperationException("Failed to parse hidden message length.");

            string total = FromBinaryWithKey(binary.ToString(), steganoKey, MagicHeader.Length + 6 + actualMessageLength);
            return total.Substring(MagicHeader.Length + 6);
        }
        catch (FileNotFoundException)
        {
            throw new InvalidOperationException("Image file not found. Please check the path.");
        }
        catch (DirectoryNotFoundException)
        {
            throw new InvalidOperationException("Directory does not exist for the given image path.");
        }
        catch (UnauthorizedAccessException)
        {
            throw new InvalidOperationException("You don't have permission to access the image file.");
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException("Error reading image file: " + ex.Message);
        }
        catch (SixLabors.ImageSharp.ImageFormatException)
        {
            throw new InvalidOperationException("The file is not a valid image or is corrupted.");
        }
    }

}
