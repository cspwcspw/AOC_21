using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Y2021
{
    public enum ProbeOutcome {  Hit, TooLow, TooFar, Error };
    public class Probe
    {
        int X0, X1, Y0, Y1;

        public long Hits { get; set; }
    
        public Probe(int x0, int x1, int y0, int y1)
        {
            X0 = x0;
            X1 = x1;
            Y0 = y0;
            Y1 = y1;
        }

        internal long GetMaxY()
        {
            Hits = 0;
            int count = 0;
            int maxY = Y1 + 1;
            Result thisM;
            Debug.Assert(X0 > 0);
            int minX = 1;
            while (minX * (minX + 1) / 2 < X0) minX++;
            Console.WriteLine($"MinX = {minX}");
            for (int vx = minX; vx <= X1; vx++)
            {
                int bestHeight = tryAllCasesAtFixed(vx);
                if (bestHeight > maxY) {
                    maxY = bestHeight;
                }
            }
            return maxY;
        }

        private int tryAllCasesAtFixed(int vx)
        {
            int maxY = 0;  // Assume launch Y coordinate counts, only work with positive intitally positive vy
   
            // The parabola always hits y=0 on the way down.  If it is falling too fast it will miss the area.
            // So the y launch velocity is bounded b
        //   for (int vy=1; vy <= (-Y1); vy++)
            for (int vy = Y1; vy <= (-Y1); vy++)
                {
                Result outcome = launch(vx, vy);

                if (outcome.reason == ProbeOutcome.Hit)
                {
                    maxY = Math.Max(maxY, outcome.maxY);
                    Hits++;             
                }
                else if (outcome.reason == ProbeOutcome.TooLow)
                {
                    
                }

                else if(outcome.reason == ProbeOutcome.TooFar)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("OOPS!");
                    break;
                }
            }
            return maxY;
        }

        private Result launch(int vx, int vy)
        {
            int x = 0;
            int y = 0;
            int dvx = vx;
            int dvy = vy;
            int maxY = 0;


            int n = 0;
            while (true)
            {
              
                x += dvx;
                y += dvy;
                //    Console.Write($"({dvx},{dvy}) xy={x},{y} ");
      //          Console.Write($"({x},{y}) ");
                if (y > maxY)
                {
                    maxY = y;
                }

                if (dvx > 0)
                {
                    dvx--;
                }
                else if (dvx < 0)
                {
                    dvx++;
                }
                dvy--;

                if (isHit(x, y))
                {
         //       Console.WriteLine($"  Hit with vx,vy = ({vx},{vy})");
                    return new Result(ProbeOutcome.Hit, maxY);
                }


                // n++;
                //if (n > 100)
                //{
                //    Console.WriteLine($" too many steps");
                //    return new Result(ProbeOutcome.Error, 100);
                //}
                if (y < Y1)
                {
               //     Console.WriteLine($" undershotX");
                    return new Result(ProbeOutcome.TooLow, 0);
                }
                if (x > X1)
                {
             //       Console.WriteLine($" overshot x");
                    return new Result(ProbeOutcome.TooFar, 0);
                }
            }

            return new Result(ProbeOutcome.Error, -1);
        }

        bool isHit(int x, int y)
        {
            return X0 <= x && x <= X1 && y <= Y0 && y >= Y1;
        }
    }

    public struct Result
    {
        public ProbeOutcome reason;
        public int maxY;

        public Result(ProbeOutcome why, int mY)
        {
            reason = why;
            maxY = mY;
        }
             
    }
}
