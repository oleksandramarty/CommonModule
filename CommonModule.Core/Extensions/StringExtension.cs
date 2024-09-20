using System.Text.RegularExpressions;

namespace CommonModule.Core.Extensions;

public static class StringExtension
{
    public static byte[] StringToUtf8Bytes(this string str)
    {
        if (str == null)
            throw new ArgumentNullException(nameof(str));

        // Calculate the maximum possible size for UTF-8 encoding
        int maxSize = str.Length * 4;
        byte[] bytes = new byte[maxSize];
        int index = 0;

        foreach (char c in str)
        {
            if (c <= 0x7F)
            {
                // 1-byte sequence
                bytes[index++] = (byte)c;
            }
            else if (c <= 0x7FF)
            {
                // 2-byte sequence
                bytes[index++] = (byte)(0xC0 | (c >> 6));
                bytes[index++] = (byte)(0x80 | (c & 0x3F));
            }
            else if (c <= 0xFFFF)
            {
                // 3-byte sequence
                bytes[index++] = (byte)(0xE0 | (c >> 12));
                bytes[index++] = (byte)(0x80 | ((c >> 6) & 0x3F));
                bytes[index++] = (byte)(0x80 | (c & 0x3F));
            }
            else
            {
                // 4-byte sequence
                bytes[index++] = (byte)(0xF0 | (c >> 18));
                bytes[index++] = (byte)(0x80 | ((c >> 12) & 0x3F));
                bytes[index++] = (byte)(0x80 | ((c >> 6) & 0x3F));
                bytes[index++] = (byte)(0x80 | (c & 0x3F));
            }
        }

        // Return the exact size of the byte array
        Array.Resize(ref bytes, index);
        return bytes;
    }
    
    public static bool BeAValidUrl(this string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
    
    public static bool NotContainMaliciousContent(this string url)
    {
        // Check for common XSS patterns
        string pattern = @"<script|javascript:|data:|vbscript:|on\w+=";
        return !Regex.IsMatch(url, pattern, RegexOptions.IgnoreCase);
    }
}