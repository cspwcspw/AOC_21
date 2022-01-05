using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Y2021
{
 
    class DumboGrid
    {
        List<List<int>> grid;

        public DumboGrid(string[] rawData)
        {
            grid = new List<List<int>>();
            foreach(string s in rawData)
            {
                grid.Add(new List<int>(s.Select(c => c - '0')));
            }
            fixBorders();
           // Show("initial");
        }



        public void Show(string caption)
        {
            Console.WriteLine(caption);
            for (int i = 1; i <= 10; i++)
            {
                for (int k = 1; k <= 10;  k++)
                {
                    Console.Write(grid[i][k]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }



        internal long SumOfFlashes()
        {
            long result = 0;
            int[] hits = { }; // 1, 10, 100 }; //,  2, 3, 10 }; // , 20, 30, 40, 50, 100 };
            for (int i=1; i <= 100; i++)
            {
                long newFlashes = DoOneStep();
                result += newFlashes;
                if (newFlashes == 100) return i;
                if (hits.Contains(i))
                {
                    Show($"After step {i}");
                }
            }

          return result;
        }

        internal long SimultaneousFlashes()
        {
            long result = 0;
            int[] hits = { }; // 1, 10, 100 }; //,  2, 3, 10 }; // , 20, 30, 40, 50, 100 };
            int i = 1;
            while(true)
            {
                long newFlashes = DoOneStep();
                if (newFlashes == 100) return i;
                i++;
            }
        }

        private long DoOneStep()
        {
            List<int> hits = new List<int>();
            long flashes = 0;

            for (int r = 1; r <= 10; r++)
            {
                for (int c = 1; c <= 10; c++)
                {
                    grid[r][c]++;
                }
            }

            bool hadFlash = true;
            while (hadFlash)
            {
                hadFlash = false;
                for (int r = 1; r <= 10; r++)
                {
                    for (int c = 1; c <= 10; c++)
                    {
                        if (grid[r][c] > 9)
                        {
                            int v = r * 100 + c;
                            if (hits.Contains(v))
                            {
                                Console.WriteLine("oops");
                            }
                            else
                            {
                                hits.Add(v);
                                hadFlash = true;
                                grid[r][c] = -888;
                                hitNeighbours(r, c);
                            }
                        }
                    }
                }
            }
            // Turn off flashes, etc
            for (int r = 1; r <= 10; r++)
            {
                for (int c = 1; c <= 10; c++)
                {
                    if (grid[r][c] < 0)
                    {
                        flashes++;
                        grid[r][c] = 0;
                    }
                }
            }

            fixBorders();

            return flashes;
        }

        void fixBorders()
        {
          
            // Fix borders 
            for (int i = 0; i < 12; i++)
            {
                grid[0][i] = -9999;
                grid[11][i] = -9999;
                grid[i][0] = -9999;
                grid[i][11] = -9999;
            }
        }

    private void hitNeighbours(int r, int c)
        {
        //    Console.WriteLine($"<{ r - 1},{ c - 1}>");
            grid[r - 1][c - 1]++;
            grid[r][c - 1]++;
            grid[r + 1][c - 1]++;

            grid[r - 1][c + 1]++;
            grid[r][c + 1]++;
            grid[r + 1][c + 1]++;

            grid[r - 1][c]++;
            grid[r + 1][c]++;
        }
    }
}
