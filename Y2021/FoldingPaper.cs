using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Y2021
{
    class FoldingPaper
    {
   
        public HashSet<Point> points;
        List<int> folds;   // Y-folds encoded as negative numbers

        public FoldingPaper(string[] lines)
        {
            points = new HashSet<Point>();
            folds = new List<int>();
            bool section1 = true; 
            foreach (string line in lines)
            {
                if (line.Trim().Length == 0)
                {
                    section1 = false;
                    continue;
                }
                if (section1)
                {
                   string[] parts = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    Point p = new Point(int.Parse(parts[0]),  int.Parse(parts[1]) );
                    points.Add(p);
                }
                else
                {
                    int foldLine = int.Parse(line.Substring(13));
                    if (line[11] == 'y')
                    {
                        foldLine = -foldLine;
                    }
                    folds.Add(foldLine);
                }
            }
        }

        public void DoFirstFold()
        {
            int n = folds[0];
            if (n < 0)
            {
                points = FoldY(-n);
            }
            else
            {
                points = FoldX(n);
            }
        }

        public void DoAllFolds()
        {
            foreach (int n in folds)
            {
                if (n < 0)
                {
                    points = FoldY(-n);
                }
                else
                {
                    points = FoldX(n);
                }
            }
            Render();
        }

        private void Render()
        {
         
            const int rows = 10;
            const int cols = 40;
            List<List<char>> pixels = new List<List<char>>();
            for (int r = 0; r < rows; r++)
            {
                List<char> row = new List<char>();
                pixels.Add(row);
                for (int c = 0; c < cols; c++)
                {
                    row.Add(' ');
                }
            }
            foreach (Point p in points)
            {
                pixels[p.Y][p.X] = '*';
            }

           
            foreach (List<char> row in pixels)
            {
                int n = 0;
                foreach (char c in row)
                {
                    if (n++ % 5 == 0) Console.Write("  ");
                    Console.Write(c);
                }

                Console.WriteLine();
            }

        }

        internal HashSet<Point> FoldY(int v)
        {
            HashSet<Point> result = new HashSet<Point>();  
            foreach (Point p in points)
            {
                if (p.Y > v)
                {
                    result.Add(new Point(p.X, 2*v - p.Y));
                }
                else
                {
                    result.Add(p);
                }
            }
            return result;
        }

        internal HashSet<Point> FoldX(int v)
        {
            HashSet<Point> result = new HashSet<Point>();
            foreach (Point p in points)
            {
                if (p.X > v)
                {
                    result.Add(new Point(2 * v - p.X, p.Y));
                }
                else
                {
                    result.Add(p);
                }
            }
            return result;
        }

    }
 
}
