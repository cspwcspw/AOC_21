using Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Y2021
{
    public class SegmentEvidence
    {
       public List<Evidence> theEvidence;
        List<string> expectedSegs;
        List<List<int>> rewirings;   // permutations of possible cross-connects

        public SegmentEvidence(string[] lines)
        {
            theEvidence = new List<Evidence>();
            foreach(string line in lines)
            {
                string[] parts = line.Split('|');
                List<string> lhand = new List<string>( parts[0].Split(' ', StringSplitOptions.RemoveEmptyEntries));
                Debug.Assert(lhand.Count == 10);
                List<string> rhand = new List<string>(parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries));
                Debug.Assert(rhand.Count == 4);
                Evidence ev = new Evidence() { lhs = lhand, rhs = rhand };
                theEvidence.Add(ev);

                expectedSegs= new List<string>() { "abcefg", "cf", "acdeg", "acdfg", "bcdf", "abdfg", "abdefg", "acf", "abcdefg", "abcdfg" };
                rewirings = Utils.GeneratePerms(new List<int>() { 0, 1, 2, 3, 4, 5, 6 });
            }
        }


      public long CountEasyCases()
        {
            long count = 0;
            foreach (Evidence ev in theEvidence)
            {
                foreach (string s in ev.rhs)
                {
                    switch (s.Length)
                    {
                        case 2:
                            count++;
                            break;
                        case 3:
                            count++;
                            break;
                        case 4:
                            count++;
                            break;
                        case 7:
                            count++;
                            break;

                        default: break;
                    }
                }
            }
            return count;
        }

        public long sumAllOutputs()
        {
            long result = 0;
            int n = 0;
            foreach (Evidence ev in theEvidence)
            {
                List<int> thePerm = FindViableRewiring(ev);
                long theNum = turnWiresToNum(thePerm, ev);
            //    Console.WriteLine($" {n}  = {theNum}");
                n++;
                result += theNum;

            }
            return result;
        }

        public List<int> FindViableRewiring(Evidence ev)
        {   
            foreach (List<int> theMap in rewirings)
            {
                if (isViable(theMap, ev)) return theMap;
            }
            return null;
        }

        private bool isViable(List<int> theMap, Evidence ev)
        {
            foreach (string s in ev.lhs) {
                string mapped = remap(theMap, s);
             //   Console.WriteLine($" {s} under {theMap[0]} {theMap[1]} {theMap[2]} {theMap[3]} {theMap[4]} {theMap[5]} {theMap[6]} remaps to {mapped}");
                if (!expectedSegs.Contains(mapped))
                {
                  //  Console.WriteLine("Which fails!");
                    return false;
                }
            }
            return true;
        }

        public long turnWiresToNum(List<int> theMap, Evidence ev)
        {
           long result = 0;
            foreach (string s in ev.rhs)
            {
                string mapped = remap(theMap, s);
                result = result * 10 + expectedSegs.IndexOf(mapped);
            }
            return result;
        }

        private string remap(List<int> theMap, string s)
        {
            List<char> remapped = new List<char>();
            foreach (char c in s)
            {
                int asc = ( c - 'a');
                remapped.Add((char)(theMap[asc]+'a'));
            }
            remapped.Sort();
            string result = "";
            foreach (char u in remapped)
            {
                result += u;
            }
            return result;
        }
    }

    public class Evidence
    {
        public List<string> lhs;
        public List<string> rhs;

        public Evidence()
        {
        }
    }
}
