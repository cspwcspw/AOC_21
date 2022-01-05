using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Y2021
{
    public class Submarine
    {
        public int Y { get; set; }
        public int Z { get; set; }

        public int Aim { get; set; }

        public Submarine()
        {
            Reset();
        }

        public void Reset()
        {
            Y = 0;
            Z = 0;
            Aim = 0;
        }

        public void Move(int dy, int dz)
        {
            Y += dy;
            Z += dz;
        }

        public void FollowDirections_V1(string script)
        {
            string[] parts = script.Split(new char[] { }, StringSplitOptions.RemoveEmptyEntries);
            for (int i=1; i < parts.Length; i+=2)
            {
                int arg = int.Parse(parts[i]);
                string cmd = parts[i - 1].Trim().ToLower();
                switch (cmd)
                {
                    case "forward":  Move(arg, 0);  break;
                    case "up": Move(0, -arg);  break;
                    case "down": Move(0, arg);  break;
                    default: throw new ApplicationException($"Unrecognized submarine command {cmd}");
                }
            }
        }

        public void FollowDirections_V2(string script)
        {
            string[] parts = script.Split(new char[] { }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < parts.Length; i += 2)
            {
                int arg = int.Parse(parts[i]);
                string cmd = parts[i - 1].Trim().ToLower();
                switch (cmd)
                {
                    case "forward": Y += arg;
                                    Z += Aim * arg; break;
                    case "up": Aim -= arg; break;
                    case "down": Aim += arg; break;
                    default: throw new ApplicationException($"Unrecognized submarine command {cmd}");
                }
            }
        }

        internal Tuple<int, int> ParseDiagnostics(string data)
        {
           string [] parts = data.Split(new char[] { }, StringSplitOptions.RemoveEmptyEntries);
           string g = "";
            string e = "";
           for (int i=0; i < parts[0].Length; i++)
            {
                char r = mostCommonBit(parts, i);
                g += r;           
                e += invert(r);
            }

            int gamma = Convert.ToInt32(g, 2);
            int epsilon = Convert.ToInt32(e, 2);
            return new Tuple<int, int>(gamma, epsilon);
        }
        char invert(char x)
        {
            return x =='1'? '0' : '1';
        }

        char mostCommonBit(string[] parts, int posn)
        {
            int oneBits = 0;
            foreach (string part in parts)
            {
                if (part[posn] == '1') oneBits++;
            }
            if (oneBits * 2 == parts.Length) return '?';
            return oneBits * 2 > parts.Length ? '1' : '0';
        }


        public int OGR(string data)
        {
            List<string> parts = new List<string>(data.Split(new char[] { }, StringSplitOptions.RemoveEmptyEntries));
            string g = "";
            int col = 0;
            while (parts.Count > 1)
            {
                switch (mostCommonBit(parts.ToArray(), col))
                {
                    case '?':
                        KeepValsWith(parts, '1', col);
                        break;

                    case '0':
                        KeepValsWith(parts, '0', col);
                        break;   

                    case '1':
                        KeepValsWith(parts, '1', col);
                        break;   
                }            
                col++;
            }
            int ogr = Convert.ToInt32(parts[0], 2);
            return ogr;
        }

        public int CO2SR(string data)
        {
            List<string> parts = new List<string>(data.Split(new char[] { }, StringSplitOptions.RemoveEmptyEntries));
            string g = "";
            int col = 0;
            while (parts.Count > 1)
            {
                switch (mostCommonBit(parts.ToArray(), col))
                {
                    case '?':
                        KeepValsWith(parts, '0', col);
                        break;

                    case '0':
                        KeepValsWith(parts, '1', col);
                        break;

                    case '1':
                        KeepValsWith(parts, '0', col);
                        break;
                }
                col++;
            }
            int co2 = Convert.ToInt32(parts[0], 2);
            return co2;
        }

        private void KeepValsWith(List<string> parts, char v, int col)
        {
            for (int i = parts.Count - 1; i >= 0; i--)
            { 
                if (parts[i][col] != v)
                {
                    parts.RemoveAt(i);
                }          
            }
        }
    }
}
