using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Y2021
{
    public class CrabFleet
    {
        List<int> fleet;
        public int minX { get; }
        public int maxX { get; }
        public CrabFleet(string inp)
        {
            string[] drawNums = inp.Split(',');
            fleet = new List<int>(drawNums.Select(int.Parse));
            minX = fleet.Min();
            maxX = fleet.Max();
        }

        public int CostToAlignAt(int n)
        {
            //   List<int> costs = fleet.Select(u => Math.Abs(fleet[u] - n)) as List<int>;
            var costs = fleet.Select(u => Math.Abs(u - n));
            return costs.Sum();
        }

        internal int CountEasyCases()
        {
            int cost = CostToAlignAt(minX);
            for (int i = minX+1; i <= maxX; i++)
            {
                int c = CostToAlignAt(i);
                if (c < cost)
                {
                    cost = c;
                }
            }
            return cost;
        }

        public long CostToAlignAtV2(int n)
        {
            //   List<int> costs = fleet.Select(u => Math.Abs(fleet[u] - n)) as List<int>;
            var costs = fleet.Select(u => oneCost(u, n));
            return costs.Sum();
        }

        private long oneCost(int u, int v)
        {
            long dist = Math.Abs(u - v);
            long cost = (dist * (dist + 1)) / 2;
     //       Console.WriteLine($" {u} {v} {dist} {cost}");

            return cost;
        }

        internal long FindLeastCostAlignmentV2()
        {
            long cost = CostToAlignAtV2(minX);
            for (int i = minX + 1; i <= maxX; i++)
            {
                long c = CostToAlignAtV2(i);
                if (c < cost)
                {
                    cost = c;
                }
            }
            return cost;
        }
    }
}
