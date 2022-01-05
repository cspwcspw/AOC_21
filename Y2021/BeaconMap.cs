using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Y2021
{
    public class BeaconMap
    {
        public List<Scanner> Scanners;
        public List<Edge> Edges { get; set; }


        public HashSet<Point3> UniqueBeacons;

        public BeaconMap(string[] rawData)
        {
            Scanners = new List<Scanner>();
            int nextLineIndx = 0;

            while (true)
            {
                if (nextLineIndx >= rawData.Length) break;
                string line = rawData[nextLineIndx].Trim();
                if (line.Length == 0)
                {
                    nextLineIndx++;
                }
                else
                {
                    Debug.Assert(line.StartsWith("--- scanner"));
                    string snum = line.Substring(12, 2);
                    int myNum = int.Parse(snum);
                    
                    nextLineIndx++;
                    Scanner scn = new Scanner(myNum, rawData, ref nextLineIndx);
                    Scanners.Add(scn);
                }
            }

            Edges = ConnectAllScanners();
      //      List<List<int>> pathsBack =  new List<List<int>>();
            UniqueBeacons = new HashSet<Point3>();
            for (int scanNum = 0; scanNum < Scanners.Count; scanNum++)
            {
                Scanner theScanner = Scanners[scanNum];
                List<int> path = FindPathToZero(scanNum, new List<int>());
                theScanner.PathHome = path;

                List<Point3> theseBeacons = getScanner0ViewFrom(path);
                theScanner.PosnInS0CoordinateSystem = theseBeacons[0];
                theseBeacons.RemoveAt(0);
                Console.Write($"path from {path[0]} (at {theScanner.PosnInS0CoordinateSystem} relative to S0): ");
                
                for (int i = 1; i < path.Count; i++)
                {
                    Console.Write($" -> {path[i]}");
                }
                Console.WriteLine();

                foreach (Point3 pt in theseBeacons)
                {
                    UniqueBeacons.Add(pt);
                }         
            }
           

        }

        public long GetMaxManhattanDistance()
        {
            long maxDist = 0;

            for (int i=0; i < Scanners.Count; i++)
            {
                for (int j=0; j < Scanners.Count; j++)
                {
                    long thisDist = ManhattanDistance(Scanners[i].PosnInS0CoordinateSystem, Scanners[j].PosnInS0CoordinateSystem);
                    if (thisDist > maxDist)
                    {
                        maxDist = thisDist;
                    }
                }
            }

            return maxDist;
        }

        private long ManhattanDistance(Point3 pt1, Point3 pt2)
        {
            long result = Math.Abs(pt1.X - pt2.X) + Math.Abs(pt1.Y - pt2.Y) + Math.Abs(pt1.Z - pt2.Z);
            return result;
        }

        private List<Point3> getScanner0ViewFrom(List<int> path)
        {
            int currNodeNum = path[0];
            Scanner currScanner = Scanners[currNodeNum];
            List<Point3> currPts = Scanner.TransformAll(0, currScanner.beacons); // makes copy of points
            // Cheat.  Insert the scanner origin too, so it also gets transformed;
            currPts.Insert(0, new Point3(0, 0, 0));
            for (int i = 1; i < path.Count; i++)
            {
                int next = path[i];
                MyTransform tf = currScanner.findTransformToGetTo(next);
                currNodeNum = next;
                currScanner = Scanners[next];

                currPts = Scanner.TransformAll(tf, currPts);
           }
           return currPts;              
        }

        private List<int> FindPathToZero(int nodeNum, List<int> visited)
        {
            
            if (nodeNum == 0) return new List<int> () { nodeNum};
            foreach (Edge e in Edges)
            {
                if (e.U == nodeNum && !visited.Contains(e.V))
                {
                    visited.Add(e.V);
                    List<int> rest = FindPathToZero(e.V, visited);

                    if (rest != null)
                    {
                        rest.Insert(0, nodeNum);
                        return rest;
                    }
                    visited.Remove(e.V);
                }
            }
            return null;
        }

        internal List<Edge> ConnectAllScanners()
        {
            // Find overlapping cubes as a set of directed edges

            List<Edge> result = new List<Edge>();

            for (int i = 0; i < Scanners.Count; i++)
            {
                for (int j = 0; j < Scanners.Count; j++)
                {
                    if (i != j) 
                    {
                        if (findOverlap(i, j))
                        {
                            Console.WriteLine($"Found overlap {i}->{j}");
                            result.Add(new Edge(i,j));
                        }
                    }
                }
            }
            return result;
        }

        public bool findOverlap(int i, int j)
        {
            Scanner s0 = Scanners[i];
            Scanner s1 = Scanners[j];
            for (int tf = 0; tf < 24; tf++)
            {
                List<Match> ms = s0.FindMatches(s1, tf);
                if (ms.Count >= 12)
                {        
                    return true;
                }
            }
            return false;
        }
    }

    public class Scanner
    {
        public int MyNum { get; set; }
        public List<int> PathHome { get; internal set; } = null;

        public List<Point3> beacons;

        public List<MyTransform> MyTransforms;
        public Point3 PosnInS0CoordinateSystem { get; internal set; } = new Point3(0,0,0);

        public Scanner(int myNum, string[] rawData, ref int nextLineIndx)
        {
            MyNum = myNum;
            beacons = new List<Point3>();
            while (true)
            {
                if (nextLineIndx >= rawData.Length) break;
                string line = rawData[nextLineIndx++].Trim();
                if (line.Length == 0) break;
                beacons.Add(Point3.FromString(line));
            }
            MyTransforms = new List<MyTransform>();
        }


        public List<Match> FindMatches(Scanner other, int tNum)
        {
            List<Match> result = new List<Match>();

            List<Point3> theirPoints = TransformAll(tNum, other.beacons);

            for (int i = 0; i < beacons.Count; i++)
            {
                for (int j = 0; j < theirPoints.Count; j++)
                {
                    // Assume these two points map pairwise, work out offsets
                    Point3 mine = beacons[i];
                    Point3 theirs = theirPoints[j];
                    int dx = mine.X - theirs.X;
                    int dy = mine.Y - theirs.Y;
                    int dz = mine.Z - theirs.Z;
                    result = new List<Match>();
                //    Console.WriteLine($"align their {j} ==> my {i}:  ");

                    bool gotOverlap = false;
                    for (int k = 0; k < theirPoints.Count; k++)
                    {
                        Point3 bp = theirPoints[k];
                        int seekX = bp.X + dx;
                        int seekY = bp.Y + dy;
                        int seekZ = bp.Z + dz;

                        for (int m = 0; m < beacons.Count; m++)
                        {
                            Point3 pt = beacons[m];
                            if (pt.X == seekX && pt.Y == seekY && pt.Z == seekZ)
                            {
                                result.Add(new Match(m, k, tNum, dx, dy, dz));
                   //             Console.Write($"{k}->{m} ");
                                if (result.Count >= 12)
                                {
                                    gotOverlap = true;
                                }
                            }
                        }
                    }
               //     Console.WriteLine();
                    if (gotOverlap)
                    {
                      
                        //Console.WriteLine("First scanner");
                        //foreach (Match mt in result)
                        //{
                        //    Console.WriteLine(beacons[mt.indexA]);
                        //}
                        //Console.WriteLine($"Second scanner at {dx},{dy},{dz} relative to first, and its points needed transform {tNum} ");
                        //foreach (Match mt in result)
                        //{
                        //    Console.WriteLine(other.beacons[mt.indexB]);
                        //}

                        other.MyTransforms.Add(new MyTransform(MyNum, tNum, dx, dy, dz, theirPoints));
                        return result;
                    }

                }
            }
            return result;
        }

       static public List<Point3> TransformAll(MyTransform mt, List<Point3> beacons)
        {
            List<Point3> result = TransformAll(mt.Tnum, beacons);
            for (int i=0; i < result.Count; i++)
            { Point3 pt = result[i];
                result[i] = new Point3(pt.X + mt.X, pt.Y + mt.Y,  pt.Z + mt.Z);
            }
            return result;
        }

       // static public 

      static public List<Point3> TransformAll(int tNum, List<Point3> beacons)
        {
            List<Point3> result = new List<Point3>();
   
            switch (tNum)
            {
                case 0: foreach (var p in beacons) 
                    { result.Add(new Point3(p.X, p.Y, p.Z)); 
                    } 
                    break;
                case 1:
                    foreach (var p in beacons)
                    {
                        result.Add(new Point3(-p.Z, p.Y, p.X));
                    }
                    break;
                case 2:
                    foreach (var p in beacons)
                    {
                        result.Add(new Point3(-p.X, p.Y, -p.Z));
                    }
                    break;
                case 3:
                    foreach (var p in beacons)
                    {
                        result.Add(new Point3(p.Z, p.Y, -p.X));
                    }
                    break;
                case 4:
                    foreach (var p in beacons)
                    {
                        result.Add(new Point3(p.X, -p.Z, p.Y));
                    }
                    break;
                case 5:
                    foreach (var p in beacons)
                    {
                        result.Add(new Point3(-p.Y, -p.Z, p.X));
                    }
                    break;
                case 6:
                    foreach (var p in beacons)
                    {
                        result.Add(new Point3(-p.X, -p.Z, -p.Y));
                    }
                    break;
                case 7:
                    foreach (var p in beacons)
                    {
                        result.Add(new Point3(p.Y, -p.Z, -p.X));
                    }
                    break;
                case 8:
                    foreach (var p in beacons)
                    {
                        result.Add(new Point3(p.X, -p.Y, -p.Z));
                    }
                    break;
                case 9:
                    foreach (var p in beacons)
                    {
                        result.Add(new Point3(p.Z, -p.Y, p.X));
                    }
                    break;
                case 10:
                    foreach (var p in beacons)
                    {
                        result.Add(new Point3(-p.X, -p.Y, p.Z));
                    }
                    break;
                case 11:
                    foreach (var p in beacons)
                    {
                        result.Add(new Point3(-p.Z, -p.Y, -p.X));
                    }
                    break;
                case 12:
                    foreach (var p in beacons)
                    {
                        result.Add(new Point3(p.X, p.Z, -p.Y));
                    }
                    break;
                case 13:
                    foreach (var p in beacons)
                    {
                        result.Add(new Point3(p.Y, p.Z, p.X));
                    }
                    break;
                case 14:
                    foreach (var p in beacons)
                    {
                        result.Add(new Point3(-p.X, p.Z, p.Y));
                    }
                    break;
                case 15:
                    foreach (var p in beacons)
                    {
                        result.Add(new Point3(-p.Y, p.Z, -p.X));
                    }
                    break;
                case 16:
                    foreach (var p in beacons)
                    {
                        result.Add(new Point3(-p.Y, p.X, p.Z));
                    }
                    break;
                case 17:
                    foreach (var p in beacons)
                    {
                        result.Add(new Point3(-p.Z, p.X, -p.Y));
                    }
                    break;
                case 18:
                    foreach (var p in beacons)
                    {
                        result.Add(new Point3(p.Y, p.X, -p.Z));
                    }
                    break;
                case 19:
                    foreach (var p in beacons)
                    {
                        result.Add(new Point3(p.Z, p.X, p.Y));
                    }
                    break;
                case 20:
                    foreach (var p in beacons)
                    {
                        result.Add(new Point3(p.Y, -p.X, p.Z));
                    }
                    break;
                case 21:
                    foreach (var p in beacons)
                    {
                        result.Add(new Point3(-p.Z, -p.X, p.Y));
                    }
                    break;
                case 22:
                    foreach (var p in beacons)
                    {
                        result.Add(new Point3(-p.Y, -p.X, -p.Z));
                    }
                    break;
                case 23:
                    foreach (var p in beacons)
                    {
                        result.Add(new Point3(p.Z, -p.X, -p.Y));
                    }
                    break;
            }
            return result;
        }

        internal MyTransform findTransformToGetTo(int next)
        {
            foreach (MyTransform t in MyTransforms)
            {
                if (t.TargetScanner == next) return t;
            }
            throw new ApplicationException("Can't find my way home!");
        }
    }

    public struct MyTransform
    {
        public int TargetScanner { get; set; }
        public int Tnum { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public List<Point3> TargetPoints { get; set; }

        public MyTransform(int target, int tnum, int x, int y, int z, List<Point3> theirPoints)
        {
            TargetScanner = target;
            Tnum = tnum;
            X = x;
            Y = y;
            Z = z;
            TargetPoints = theirPoints;
        }
    }

    public struct Point3
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public Point3()
        {
            X = 0; Y = 0; Z = 0;
        }

        public Point3(int x, int y, int z)
        {
            X = x; Y = y; Z = z;
        }


        public static Point3 FromString(string s)
        {
            string[] parts = s.Split(',');
            return new Point3() { X = int.Parse(parts[0]), Y = int.Parse(parts[1]), Z = int.Parse(parts[2]) };
        }

        public override string ToString()
        {
            return $"({X},{Y},{Z})";
        }

        public static void ShowPoints(string description, List<Point3> pts)
        {
            Console.WriteLine(description);
            foreach(Point3 p in pts)
            {
                Console.WriteLine(p.ToString());
            }
        }
    }

    public struct Edge
    {
        public int U { get; set; }
        public int V { get; set; }
        public Edge(int u, int v)
        {
            U = u; V = v;
        }

    }

    public struct Match
    {
        public int indexA { get; set; }
        public int indexB { get; set; }

        public int tNum { get; set; }
        public int dx { get; set; }
        public int dy{ get; set; }
        public int dz { get; set; }

        public Match(int a, int b, int t, int x, int y, int z)
        {
            indexA = a; indexB = b; tNum = t; dx = x; dy = y; dz = z; 
        }

    }
}

#region onceOff

//static public void ShowTransforms()
//{
//    string ops = "I Y YY YYY X XY XYY XYYY XX XXY XXYY XXYYY XXX XXXY XXXYY XXXYYY Z ZY ZYY ZYYY ZZZ ZZZY ZZZYY ZZZYYY";
//    string[] tf = ops.Split(' ');

//    int n = 0;
//    foreach (string op in tf)
//    {
//        BeaconPt p = new BeaconPt(1, 2, 3);
//        foreach (char c in op)
//        {
//            switch (c)
//            {
//                case 'I': break;
//                case 'X':
//                    p = new BeaconPt(p.X, -p.Z, p.Y);
//                    break;
//                case 'Y':
//                    p = new BeaconPt(-p.Z, p.Y, p.X);
//                    break;
//                case 'Z':
//                    p = new BeaconPt(-p.Y, p.X, p.Z);
//                    break;
//                default: throw new ApplicationException("Oops");

//            }
//        }
//        BeaconPt xCheck = Scanner.worra(n);

//        Console.WriteLine($"{p} {xCheck}  {n}:  Under transform {op} ");
//        //  Reverse engineer the result for some code 
//        //string p1 = revEng(p.X);
//        //string p2 = revEng(p.Y); ;
//        //string p3 = revEng(p.Z); ;
//        //Console.WriteLine($"case {n}: q = new BeaconPt({p1},{p2},{p3});  break; ");
//        n++;
//    }
//}
//static string revEng(int v)
//{
//    string result = "";
//    if (v < 0)
//    {
//        result = "-";
//        v = -v;
//    }
//    switch (v)
//    {
//        case 1: result += "p.X"; break;
//        case 2: result += "p.Y"; break;
//        case 3: result += "p.Z"; break;

//    }
//    return result;

//}

#endregion