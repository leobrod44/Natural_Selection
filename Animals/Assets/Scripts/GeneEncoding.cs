using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GeneEncoding
{
    private static readonly int largePrime = 1493;
    public static string IdToBinary(int num)
    {
        string fmt = "0000000.##";
        string bin = "";
        ValidateBinaryFormat(num);
        if (num > 0)
            bin = Convert.ToInt32(Convert.ToString(num, 2)).ToString(fmt);
        else
        {
            bin = Convert.ToString(num, 2);
            bin = bin.Substring(bin.Length - 7);
        }
        return bin;
    }

    public static int BinaryToId(string bin)
    {
        int value;
        if (bin[0] == '0')
            value = Convert.ToInt32(bin, 2);
        else
        {
            string newBin = bin.Replace('1', 'a').Replace('0', 'b').Replace('a', '0').Replace('b', '1');
            value = -Convert.ToInt32(newBin, 2) - 1;
        }
        return value;
    }


    public static string FloatToBinary(float num)
    {
        int value;

        value = (int)(num * largePrime);
        string fmt = "0000000000000000.##";
        string bin = "";
        if (num > 0)
            bin = Convert.ToInt64(Convert.ToString(value, 2)).ToString(fmt);
        else
        {
            bin = Convert.ToString(value, 2);
            if (bin == "0")
            {
                bin = "0000000000000001";
            }
            bin = bin.Substring(bin.Length - 16);
        }
        float val = BinaryToFloat(bin);
        return bin;
        

       
    }

    public static float BinaryToFloat(string bin)
    {
        int value;
        if (bin[0] == '0')
            value = Convert.ToInt32(bin, 2);
        else
        {
            string newBin = bin.Replace('1', 'a').Replace('0', 'b').Replace('a', '0').Replace('b', '1');
            value = -Convert.ToInt32(newBin, 2) - 1;
        }
        float dividedValue = value != 0 ? (float)value / (float)largePrime : 0;
        return dividedValue;
    }

    public static void ValidateBinaryFormat(int num)
    {
        if (Math.Abs(num) > 64)
            throw new InvalidBinaryFormatException(num);
    }

    public static string BinToHex(string bin)
    {
        return Convert.ToInt64(bin, 2).ToString("X");
    }
    public static string HexToBin(string hex)
    {
        string bin = String.Join(String.Empty,hex.Select(
            c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));
        return bin;
    }
    public static int GetDestinationID(string bin)
    {
        return BinaryToId(bin.Substring(9, 8));
    }
    public static byte GetOriginBit(string bin)
    {
        if (bin.Substring(0, 1) == "0")
            return 0;
        else if (bin.Substring(0, 1) == "1")
            return 1;
        else
            return 2;
    }
    public static byte GetDestinationBit(string bin)
    {
        if (bin.Substring(8, 1) == "0")
            return 0;
        else if (bin.Substring(8, 1) == "1")
            return 1;
        else
            return 2;
    }
    public static int GetOriginID(string bin)
    {
        return BinaryToId(bin.Substring(1, 7));
    }
    public static float GetWeight(string bin)
    {
        return (float)BinaryToFloat(bin.Substring(16, 16));
    }
    public class InvalidBinaryFormatException : Exception
    {
        public InvalidBinaryFormatException() { }

        public InvalidBinaryFormatException(int num)
            : base(String.Format("Invalid Binary Format, Exceeding maxium of 256 for 8 bit binary: {0}", num))
        {

        }
    }
    public static string GenerateLatinName()
    {
        System.Random r = new System.Random();
        int lenFirst = r.Next(2, 6);
        int lenLast = r.Next(2, 6);
        string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "t" };
        string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
        string[] ends = { "o", "is", "us", "ien", "a" };
        string firstName = "";
        firstName += consonants[r.Next(consonants.Length)].ToUpper();
        firstName += vowels[r.Next(vowels.Length)];
        int c = 2; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
        while (c < lenFirst)
        {
            firstName += consonants[r.Next(consonants.Length)];
            c++;
            firstName += vowels[r.Next(vowels.Length)];
            c++;
        }

        firstName += consonants[r.Next(consonants.Length)];
        c++;
        firstName += ends[r.Next(ends.Length)];
        c++;
        string secondName = "";
        secondName += consonants[r.Next(consonants.Length)].ToUpper();
        secondName += vowels[r.Next(vowels.Length)];
        c = 2; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
        while (c < lenLast)
        {
            secondName += consonants[r.Next(consonants.Length)];
            c++;
            secondName += vowels[r.Next(vowels.Length)];
            c++;
        }
        secondName += consonants[r.Next(consonants.Length)];
        c++;
        secondName += ends[r.Next(ends.Length)];
        c++;

        return firstName + " " + secondName;
    }
}

