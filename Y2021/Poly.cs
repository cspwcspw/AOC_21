using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Y2021
{
    public class Poly
    {
        string curr;
        Dictionary<PolyPair, char> rules;
        Memo theMemo;

        public Poly(string[] lines)
        {
            curr = lines[0].Trim();
            rules = new Dictionary<PolyPair, char>();
            for (int i = 2; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split("->", StringSplitOptions.RemoveEmptyEntries);
                string lhs = parts[0].Trim();
                string rhs = parts[1].Trim();
                rules.Add(new PolyPair(lhs[0], lhs[1]), rhs[0]);
            }
        }


        private FrequencyTable DepthFirstVisit(PolyPair pp, int stepsToGo)
        {
            if (stepsToGo == 0) return new FrequencyTable();
            FrequencyTable cached = theMemo.GetCachedEntry(pp, stepsToGo);
            if (cached != null)
            {
                return cached;
            }

            // Turn the polypair into two children, count the newly injected token, and
            // recursively traverse the children PolyPairs.
            char newInsertion = rules[pp];

            PolyPair child1 = new PolyPair(pp.U, newInsertion);
            FrequencyTable f1 = DepthFirstVisit(child1, stepsToGo - 1);

            PolyPair child2 = new PolyPair(newInsertion, pp.V);
            FrequencyTable f2 = DepthFirstVisit(child2, stepsToGo - 1);

            FrequencyTable result = f1.AddTable(f2);
            result.RecordHit(newInsertion);
            theMemo.Remember(pp, stepsToGo, result);
            return result;
        }

        public long DoSubs(int steps)
        {
            theMemo = new Memo();
            FrequencyTable ft = new FrequencyTable();
            for (int i = 0; i < curr.Length; i++)
            {
                ft.RecordHit(curr[i]);
            }
            for (int i = 1; i < curr.Length; i++)
            {
                PolyPair pp = new PolyPair(curr[i - 1], curr[i]);
                ft = ft.AddTable(DepthFirstVisit(pp, steps));
            }

            long result = ft.GetSpan();

            return result;
        }


        public struct PolyPair
        {
            public char U;
            public char V;

            public PolyPair(char u, char v)
            {
                U = u; V = v;
            }
        }

        public class FrequencyTable
        {
            long[] counts = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // 26 bins

            public FrequencyTable() { }

            public void RecordHit(char c)
            {
                counts[c - 'A']++;
            }

            public long GetSpan()
            {
                long mc = counts.Max();
                long lc = mc;
                foreach (long v in counts)
                {
                    if (v > 0 && v < lc) lc = v;
                }
                return mc - lc;
            }

            public FrequencyTable AddTable(FrequencyTable other)
            {
                FrequencyTable result = new FrequencyTable();
                for (int i = 0; i < counts.Length; i++)
                {
                    result.counts[i] = this.counts[i] + other.counts[i];
                }
                return result;
            }
        }


        // Keep a tble of inputs and results to DepthFirstVisit.  The recursion depth is 40,
        // and there are only about 100 symbol pair possibilities, so a memo of results in
        // about 4000 cases can save us exponentially exploding the number of recursive calls.
        public class Memo
        {
            Dictionary<Tuple<PolyPair, int>, FrequencyTable> theCache;
            public Memo()
            {
                theCache = new Dictionary<Tuple<PolyPair, int>, FrequencyTable>();
            }

            public void Remember(PolyPair pp, int stepsToGo, FrequencyTable result)
            {
                theCache.Add(new Tuple<PolyPair, int>(pp, stepsToGo), result);
            }

            internal FrequencyTable GetCachedEntry(PolyPair pp, int stepsToGo)
            {
                Tuple<PolyPair, int> searchKey = new Tuple<PolyPair, int>(pp, stepsToGo);
                if (theCache.ContainsKey(searchKey))
                {
                    return theCache[searchKey];
                }
                return null; // if we don't already have the answer
            }
        }
    }
}

