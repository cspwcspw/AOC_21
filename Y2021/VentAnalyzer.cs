using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Y2021
{
    public class VentAnalyzer
    {
        List<VLine> vents;

        Dictionary<VPoint, int> Coverage;

        public VentAnalyzer(string inputFilename)
        {
            string[] rawdata = File.ReadAllLines(inputFilename);
            vents = new List<VLine>();
            foreach (string v in rawdata)
            {
                vents.Add(parseRawToVent(v));
            }
        }

        public void buildCoverageMap(bool allowDiagonals)
        {
            Coverage = new Dictionary<VPoint, int>();
            foreach (VLine v in vents)
            {
                AddPointsCovered(v, allowDiagonals);      
            }
        }

        private void AddPointsCovered(VLine vent, bool allowDiagonals)
        {
            if (vent.IsVertical)
            {   
                int y0 = vent.U.Y;
                int y1 = vent.V.Y;
                int x = vent.U.X;
                if (y1 < y0) // swap them
                {
                    int tmp = y0;
                    y0 = y1;
                    y1 = tmp;
                }
                Debug.Assert(y0 <= y1);
                for(int y= y0; y <= y1; y++)
                {
                    CoverPoint(x, y);
                }
            }
            else if (vent.IsHorizontal)
            {
                int x0 = vent.U.X;
                int x1 = vent.V.X;
                int y = vent.U.Y;
                if (x1 < x0) // swap them
                {
                    int tmp = x0;
                    x0 = x1;
                    x1 = tmp;
                }
                Debug.Assert(x0 <= x1);
                for (int x = x0; x <= x1; x++)
                {
                    CoverPoint(x, y);
                }
            }
           else if (allowDiagonals)
            {
                int x0 = vent.U.X;
                int y0 = vent.U.Y;
                int y1 = vent.V.Y;
                int x1 = vent.V.X;

                int dx = x1 - x0;
                int dy = y1 - y0;

                Debug.Assert(Math.Abs(dx) == Math.Abs(dy));  // Are they really easy diagonals?
                int n = Math.Abs(dx)+1;  // number of pints to colour
                int xStep = dx < 0 ? -1 : 1;
                int yStep = dy < 0 ? -1 : 1;
                int x = x0;
                int y = y0;
                for (int i=0; i < n; i++)
                {
                    CoverPoint(x, y);
                    x += xStep;
                    y += yStep;
                }
            }
        }

        private void CoverPoint(int x, int y)
        {
         
            VPoint pt = new VPoint() { X = x, Y = y };
            if (Coverage.ContainsKey(pt))
            {
                Coverage[pt]++;
     //           Console.WriteLine($"Re-covered pt {x}, {y} to get count {Coverage[pt]}");
            }
            else
            {
                Coverage.Add(pt, 1);
    //            Console.WriteLine($"New pt {x}, {y} to get count {Coverage[pt]}");
            }
        }

        public int CountMultiCoveredPoints()
        {
            int count = 0;
            foreach(int n in Coverage.Values)
            {
                if (n > 1) count++;
            }
            return count;
        }

        private VLine parseRawToVent(string v)
        {
            string[] parts = v.Split("->");
            Debug.Assert(parts.Length == 2);
            VPoint p0 = parseRawToPoint(parts[0]);
            VPoint p1 = parseRawToPoint(parts[1]);
            return new VLine() { U = p0, V = p1 };
        }

        private VPoint parseRawToPoint(string v)
        {
            string[] parts = v.Split(',');

            Debug.Assert(parts.Length == 2);
            return new VPoint() { X = int.Parse(parts[0]), Y = int.Parse(parts[1]) };
        }
    }

    public struct VPoint
    {
        public int X;
        public int Y;
    }

    public struct VLine
    {
        public VPoint U;
        public VPoint V;

        public bool IsHorizontal
        { get { return U.Y == V.Y;  } }


        public bool IsVertical
        { get { return U.X == V.X; } }
    }
}
