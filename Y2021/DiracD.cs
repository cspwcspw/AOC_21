using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Y2021
{
    internal class DiracD
    {
        public int[] Posn { get; set; } = { 0, 0 };

        public int DieVal { get; set; } = 1;

        public long DieRolls { get; set; } = 0;

        public long[] Score { get; set; } = { 0, 0 };


        public DiracD(int posA, int posB)
        {
            Posn[0] = posA-1;
            Posn[1] = posB-1;
        }

        public int Run()
        {
            int whoseTurn = 0;  

            while (true)
            {
                int throws = take3Rolls();

                // my boardpos is 0..9
                Posn[whoseTurn] = (Posn[whoseTurn] + throws) % 10;

                Score[whoseTurn] += (1+Posn[whoseTurn]);
                if (Score[whoseTurn] >= 1000) return whoseTurn;
                whoseTurn = (whoseTurn + 1) % 2;
            }
        }

        int take3Rolls()
        {
            int sum = DieVal++;
            if (DieVal > 100) DieVal = 1;


            sum += DieVal++;
            if (DieVal > 100) DieVal = 1;
            sum += DieVal++;
            if (DieVal > 100) DieVal = 1;
            DieRolls += 3;
            return sum;
        }


    }
}
