using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Y2021
{
    public class FloorMap
    {
        List<Point> LowPoints;
        List<string> framedData;
        int framedWidth;
        List<List<Point>> Basins;


        public FloorMap(string[] rawdata)
        {
            framedWidth = rawdata[0].Length + 2;
            string frameLine = "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@".
                Substring(0, framedWidth);
            framedData = new List<string>();
            framedData.Add(frameLine);
            foreach (string oneLine in rawdata)
            {
                framedData.Add("@" + oneLine + "@");
            }
            framedData.Add(frameLine);
        }

        public long SumRisks()
        {
            findLowPoints();
            long result = 0;
            foreach (var lp in LowPoints)
            {   int risk = framedData[lp.X][lp.Y] - '0' + 1;
                result += risk;
            }
            return result;
        }

        private void findLowPoints()
        {
            LowPoints = new List<Point>();
           for (int r=1; r < framedData.Count-1; r++)
            {
                for (int c=1; c < framedWidth-1; c++)
                {
                    if (isLowPoint(r,c))
                    {
                        LowPoints.Add(new Point() { X = r, Y = c });
                      //  Console.WriteLine($"Lowpoint at {r-1} {c-1}");
                    }
                }
            }
        }

        public long FindAllBasins()
        {
            Basins = new List<List<Point>>();
            foreach (Point p in LowPoints)
            {
                List<Point> b = buildBasin(p);
                Basins.Add(b);
            }
            List<int> lengths = new List<int>(Basins.Select(u=>u.Count));
            lengths.Sort();
            List<int> bigThree =  new List<int> (lengths.TakeLast(3));
            long prod = 1;
            foreach (int n in bigThree)
            {
                prod *= n;
            }
            return prod;

        }

        private List<Point> buildBasin(Point p)
        {
            List<Point> Pending = new List<Point>() { p };
            List<Point> theBasin = new List<Point>();
            while (Pending.Count > 0)
            {
                Point curr = Pending[0];
                Pending.RemoveAt(0);
                if (theBasin.Contains(curr)) continue;
                theBasin.Add(curr);
 
                // Now add any higher neighbours of curr to pending.
                char h = framedData[curr.X][curr.Y];
                if (isUpstream(h, curr.X - 1, curr.Y)) Pending.Add(new Point() { X = curr.X - 1, Y = curr.Y });
                if (isUpstream(h, curr.X + 1, curr.Y)) Pending.Add(new Point() { X = curr.X + 1, Y = curr.Y });
                if (isUpstream(h, curr.X, curr.Y+1)) Pending.Add(new Point() { X = curr.X, Y = curr.Y+1 });
                if (isUpstream(h, curr.X, curr.Y-1)) Pending.Add(new Point() { X = curr.X, Y = curr.Y-1 });
            }
            return theBasin;
        }

        private bool isUpstream(char h, int v, int y)
        {
            char c = framedData[v][y];
            return c < '9' && h < c;
        }

        private bool isLowPoint(int r, int c)
        {
            char v = framedData[r][c];
            return v < framedData[r - 1][c] && v < framedData[r + 1][c] && v < framedData[r][c - 1] && v < framedData[r][c + 1];
        }
    }

    public struct Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }
}
