using System.Collections.Generic;

namespace Y2021
{
    public class Dijkstra
    {

        int width; int height;

        public int HighTide { get; private set; }

        List<List<Cell>> theMap;
        PriorityQueue<Point, int > toDo;  // only in NET 6


        public Dijkstra(string[] lines, bool withExtension)
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
            int PruningThreshold = 9 * (width * height);

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
            toDo = new PriorityQueue<Point, int>();
            HighTide = 0;
            toDo.Enqueue(new Point(0, 0), 0);
            theMap[0][0].pathCost = 0;

            while (toDo.Count > 0)
            {
                if (toDo.Count > HighTide)
                {
                    HighTide = toDo.Count;
                }
                Point curr = toDo.Dequeue();
                Cell cCell = theMap[curr.Y][curr.X];
                cCell.isRed = true;
                int currCost = cCell.pathCost;

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
            return theMap[height - 1][width - 1].pathCost;
        }

        private   void addTo(int x, int y, int currPathCost)
        {
            Cell c = theMap[y][x];
            if (c.isRed) return;
            int newPathCost = currPathCost + c.cellCost;
            if (newPathCost < c.pathCost)
            {
                c.pathCost = newPathCost;
                Point pt = new Point(x, y);
                toDo.Enqueue(pt, newPathCost);
            }
        }

        public class Cell
        {
            public int cellCost;
            public int pathCost;
            public bool isRed = false;

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
