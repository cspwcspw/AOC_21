using System;
using System.Collections.Generic;
using System.IO;

namespace Helpers
{
    static public class Utils
    {

        public static void validate(string description, long e1, long a1, long e2 = -1, long a2 = -2, long e3 = -1, long a3 = -2, long e4 = -1, long a4 = -2)
        {
            Console.WriteLine($"{description} {e1 == a1} {e2 == a2} {e3 == a3} {e4 == a4}  answers = {a2} and {a4}");
        }

        public static void validate(string description, long[] vals)
        {
            Console.Write($"{description} ");
            for (int i = 1; i < vals.Length; i += 2)
            {
                Console.Write($"{vals[i - 1] == vals[i]} ");
            }
            Console.Write("   answers = ");
            for (int i = 1; i < vals.Length; i += 2)
            {
                Console.Write($"{vals[i]} ");
            }
            Console.WriteLine();

        }
        public static List<int> ParseInts(string input)
        {
            string[] parts  = input.Split(new char[] { }, StringSplitOptions.RemoveEmptyEntries);
            List<int> result = new List<int>(parts.Length);
            foreach (var s in parts)
            {
                result.Add(int.Parse(s));
            }
            return result;
        }

        public static int countDeeper(List<int> inp)
        {

            int result = 0;
            for (int i = 1; i < inp.Count; i++)
            {
                if (inp[i] > inp[i - 1]) result++;
            }
            return result;
        }

        public static List<int> slidingSums(List<int> data)
        {
            List<int> result = new List<int>();
            for (int i = 2; i < data.Count; i++)
            {
                result.Add(data[i - 2] + data[i - 1] + data[i]);
            }
            return result;
        }

        public static List<List<int>> GeneratePerms(List<int> seed)
        {
            List<List<int>> result = new List<List<int>>();
            switch(seed.Count)
            {
                case 0:  return result;
                case 1: result.Add(new List<int>(seed));
                    return result;
                default:
                    for (int i=0; i < seed.Count; i++)
                    {
                        List<int> sublist = new List<int>(seed);
                        sublist.RemoveAt(i);
                        List<List<int>> subresult = GeneratePerms(sublist);
                        foreach (var oneResult in subresult)
                        {
                            oneResult.Insert(0, seed[i]);
                            result.Add(oneResult);
                        }
                    }
                    break;
            }


            return result;
        }


    }
}
