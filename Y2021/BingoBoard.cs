using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Y2021
{
    public class BingoBoard
    {
        const int N = 5;    // Size of board, must be square
        List<int> TheBoard;
        List<bool> isHit;

        public int SumOfUnmarked { get; internal set; }

        public BingoBoard(List<int> chunk)
        {
            TheBoard = chunk;
            Reset();
        }

        public void Reset()
        {
            isHit = new List<bool>();
            SumOfUnmarked = 0;
            for (int i = 0; i < TheBoard.Count; i++)
            {
                SumOfUnmarked += TheBoard[i];
                isHit.Add(false);
            }
        }

        internal bool PlayOneNum(int d)
        {
            int indx = TheBoard.IndexOf(d);
            if (indx < 0) return false;
            Debug.Assert(!isHit[indx]);
            SumOfUnmarked -= d;
            isHit[indx] = true;
            return hasWinningRowOrCol();
        }

        private bool hasWinningRowOrCol()
        {
            // Check rows
            for (int i=0; i < N*N; i+=N)
            {
                if (isHit[i] && isHit[i + 1] && isHit[i + 2] && isHit[i + 3] && isHit[i + 4]) return true;
            }

            // Check Cols
            for (int i = 0; i < N ; i++)
            {
                if (isHit[i] && isHit[i + N] && isHit[i + 2*N] && isHit[i + 3*N] && isHit[i + 4*N]) return true;
            }
            return false;
        }
    }
}
