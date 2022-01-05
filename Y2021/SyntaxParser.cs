using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Y2021
{
    public class SyntaxParser
    {
        List<string> theLines;

        public SyntaxParser(string[] lines)
        {
            theLines = new List<string>(lines);

        }

        public long SumOfSyntaxErrors()
        {
            long result = 0;
            for(int i = theLines.Count-1; i >=0; i--)  
            {
                string line = theLines[i];
                long score = syntaxErrorScore(line);

                if (score > 0)
                {
                    result += score;
                    theLines.RemoveAt(i);
                }
            }
            return result;
        }

        private long syntaxErrorScore(string line)
        {
            Stack<char> stk = new Stack<char>();
            foreach (char c in line)
            {
                switch (c)
                {
                    case '(':
                    case '{':
                    case '<':
                    case '[':
                        stk.Push(c);
                        break;
                    case ')':
                        if (stk.Count <= 0 || stk.Peek() != '(') return 3;
                        stk.Pop();
                        break;
                    case '}':
                        if (stk.Count <= 0 || stk.Peek() != '{') return 1197;
                        stk.Pop();
                        break;
                    case '>':
                        if (stk.Count <= 0 || stk.Peek() != '<') return 25137;
                        stk.Pop();
                        break;
                    case ']':
                        if (stk.Count <= 0 || stk.Peek() != '[') return 57;
                        stk.Pop();
                        break;
                }
            }
            return 0;
        }

        public long AutoCompleteMedian()
        {
            List<long> acScores = new List<long>(theLines.Select(s => acScore(s)));
            acScores.Sort();
            Debug.Assert(acScores.Count % 2 == 1);
            return acScores[acScores.Count / 2];
        }

        private long acScore(string s)
        {
            Stack<char> stk = new Stack<char>();
            foreach (char c in s)
            {
                switch (c)
                {
                    case '(':
                    case '{':
                    case '<':
                    case '[':
                        stk.Push(c);
                        break;
                    case ')':
                        Debug.Assert (stk.Pop() == '(');
                           break;
                    case '}':
                        Debug.Assert(stk.Pop() == '{');
                        break;
                    case '>':
                        Debug.Assert(stk.Pop() == '<');
                          break;
                    case ']':
                        Debug.Assert(stk.Pop() == '[');
                   break;
                }
            }

            string codes = " ([{<";
            long acResult = 0;
            foreach (char c in stk)
            {
                acResult = acResult * 5 + codes.IndexOf(c);
            }
            return acResult;
        }
    }
}
