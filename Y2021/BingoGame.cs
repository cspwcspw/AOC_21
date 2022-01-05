using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Y2021
{
    public class BingoGame
    {
        List<int> Draw;
        List<BingoBoard> Boards;
        public int WinningBoardIndex = -1;

        public BingoGame(string filename)
        {
            string[] lines = File.ReadAllLines(filename);
            string[] drawNums = lines[0].Split(',');
            Draw = new List<int>(drawNums.Select(int.Parse));

            string content = File.ReadAllText(filename);
            // Strip off first line from content
            content = content.Substring(lines[0].Length);
            string [] snums  = content.Split(new char[] { }, StringSplitOptions.RemoveEmptyEntries);
            List<int> bNums = new List<int>(snums.Select(int.Parse));

            Debug.Assert(bNums.Count % 25 == 0);
            Boards = new List<BingoBoard>();
            while (bNums.Count > 0)
            {
                List<int> chunk = bNums.GetRange(0, 25);
                Boards.Add(new BingoBoard(chunk));
                bNums.RemoveRange(0, 25);
            }
        }

 
        public int PlayToFirstWinner()
        {
            WinningBoardIndex = -1;
            foreach (int d in Draw)
            {
                for (int b=0; b< Boards.Count; b++)
                {
                    BingoBoard bb = Boards[b];
                    bool wins = bb.PlayOneNum(d);
                    if (wins)
                    {
                        WinningBoardIndex = b;
                        return bb.SumOfUnmarked * d;
                    }
                }
            }
            throw new ApplicationException("No board every won the Bingo.");
        }

        public int PlayToLastWinner()
        {
            while (true)
            { 
                foreach (BingoBoard b in Boards)
                {
                    b.Reset();
                }
                int ans = PlayToFirstWinner();
                if (Boards.Count == 1)
                {
                    return ans;
                }
                Boards.RemoveAt(WinningBoardIndex);
            }

            throw new ApplicationException("The last board did not win the Bingo.");
        }
    }
}
