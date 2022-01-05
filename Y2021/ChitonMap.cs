using System;
using System.Collections.Generic;
using System.Linq;

namespace Y2021
{
    public class ChitonMap
    {
        int width; int height;

        public int HighTide { get; private set; }

        List<List<Cell>> theMap;
        Stack<Point> toDo;

        int PruningThreshold;

        int threshUpdates;

        public ChitonMap(string[] lines, bool withExtension)
        {
            height = lines.Length;
            width = lines[0].Length;
            theMap = new List<List<Cell>>();
            foreach (string line in lines)
            {
                List<Cell> theRow = new List<Cell>();
                foreach (char c in line)
                {
                    theRow.Add(new Cell(c - '0'));
                }
                theMap.Add(theRow);
            }

            if (withExtension)
            {
                for (int row = 0; row < height; row++)
                {
                    List<Cell> theRow = theMap[row];
                    for (int i = 0; i < 4 * width; i++)
                    {
                        theRow.Add(theRow[i].CloneInc());
                    }
                }
                width = 5 * width;

                for (int row = 0; row < 4 * height; row++)
                {
                    List<Cell> srcRow = theMap[row];
                    List<Cell> newRow = new List<Cell>();
                    for (int col = 0; col < width; col++)
                    {
                        newRow.Add(srcRow[col].CloneInc());
                    }
                    theMap.Add(newRow);
                }
                height = height * 5;
            }

            // Set some initial PruningThreshold
            PruningThreshold = 9 * (width * height);

            int sum;
            sum = 0;
            for (int i=1; i < height; i++)
            { sum += theMap[i][0].cellCost;
            }
            for (int i=1; i<width; i++)
            {
                sum += theMap[height-1][i].cellCost;
            }
            if (sum < PruningThreshold)
            {
                PruningThreshold = sum;
            }

            sum = 0;
            for (int i = 1; i < width; i++)
            {
                sum += theMap[0][i].cellCost;
            }
            for (int i = 1; i < height; i++)
            {
                sum += theMap[i][width-1].cellCost;
            }
            if (sum < PruningThreshold)
            {
                PruningThreshold = sum;
            }

            // initialize all known costs to the pruningThreshold
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    theMap[row][col].pathCost = PruningThreshold;
                }
            }
        }

        public int FindShortestPath()
        {
            // I think this is Dijkstra's shortest path algorithm, but I've not seen it for about 40 years.
            toDo = new Stack<Point>();
            HighTide = 0;
            toDo.Push(new Point(0,0));
            theMap[0][0].pathCost = 0;
            threshUpdates = 0;
            while (toDo.Count > 0)
            {
                if (toDo.Count > HighTide) {
                    HighTide = toDo.Count;
                }
                Point curr = toDo.Pop();
                Cell cCell = theMap[curr.Y][curr.X];
                int currCost = cCell.pathCost;
                if (currCost < PruningThreshold)
                {
                    if (curr.X > 0)
                    {
                        addTo(curr.X - 1, curr.Y, currCost);
                    }
                    if (curr.Y > 0)
                    {
                        addTo(curr.X, curr.Y - 1, currCost);
                    }
                    if (curr.X < width - 1)
                    {
                        addTo(curr.X + 1, curr.Y, currCost);
                    }
                    if (curr.Y < height - 1)
                    {
                        addTo(curr.X, curr.Y + 1, currCost);
                    }
                }
            }
            return theMap[height-1][width-1].pathCost;
        }

        private void addTo(int x, int y, int currPathCost)
        {
           // if (y == 0 && x == 0) return;
            Cell c = theMap[y][x];
            int newPathCost = currPathCost + c.cellCost;
          //   bool isSensible = newPathCost <= 9 * (x + y);
            if (newPathCost < c.pathCost) //  && isSensible)
            {
                c.pathCost = newPathCost;
                Point pt = new Point(x, y);
                toDo.Push(pt);

                if (x == width-1 && y == height-1)
                {
                    // new path to dest, can we tighten the pruning threshold
                    if (newPathCost < PruningThreshold)
                    {
                        PruningThreshold = newPathCost;
                        //Console.Write(PruningThreshold); Console.Write(' ');
                        //if (++threshUpdates % 20 == 0) Console.WriteLine();
                    }
                }
            }
        }

        public class Cell
        {
            public int cellCost;
            public int pathCost;

            public Cell(int cc)
            {
                cellCost = cc;
            }


            internal Cell CloneInc()
            {
                int newCost = this.cellCost + 1;
                if (newCost > 9) newCost = 1;
                Cell result = new Cell(newCost);
                return result;
            }
        }
    }
}
