using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Y2021
{
    internal class CucumberSystem
    {
        Herd GoEast;
        Herd GoDown;

        int Width;
        int Height;


        public CucumberSystem(string[] lines)
        {
            Height = lines.Length;
            Width = lines[0].Length;
            GoEast = new Herd(Width, Height, true);
            GoDown = new Herd(Height, Width, false);
            for (int r = 0; r < Height; r++)
            {
                for (int c = 0; c < Width; c++)
                {
                    char ch = lines[r][c];
                    if (ch == '>')
                    {
                        GoEast.Set(r, c, true);
                    }
                    else if (ch == 'v')
                    {
                        GoDown.Set(c, r, true);
                    }
                    else if (ch != '.')
                    {
                        throw new ApplicationException("Bad inout data");
                    }
                }
            }
            bool ok1 = GoEast.SanityCheck();
            bool ok2 = GoDown.SanityCheck();


        }

        string ShowBoard(string caption)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(caption);
            sb.Append('\n');
            for (int row=0; row< Height; row++)
            {
              
                for (int col=0; col < Width; col++)
                {
                    if (GoEast.Rows[row][col])
                    {
                        sb.Append('>');
                    }
                    else if (GoDown.Transpose[row][col])
                    {
                        sb.Append('v');
                    }
                    else
                    {
                        sb.Append('.');
                    }
                }
                sb.Append('\n');
            }
            sb.Append("---------------");
            return sb.ToString();
        }

        public long Run()
        {
            int count;
       //    Console.WriteLine(ShowBoard($"Initial state"));
            int i = 0;
            do
            {
                i++;
                count = 0;
                count += GoEast.MoveHerdOneStep(GoDown);
             //      Console.WriteLine(ShowBoard());
                count += GoDown.MoveHerdOneStep(GoEast);
           //    Console.WriteLine(ShowBoard($"On {i} double-step moves, {count} cucumbers moved.  State is now"));
               if (i%100 == 0)
                {
                   Console.WriteLine($"{i} iterations, {count} cucumbers moved.");
                }
            }
            while(count > 0);


            return i;
        }


    }
    public class Herd
    {
        // So some subtle data representation here. Each Herd is held as a matrix of bits.
        // But I simulaneosly also hold the transpose of the Matrix, and do double-work 
        // every time I need to move a member of the herd to a new location. 
        // The "working" array is always the Rows, and moving along the row means moving
        // East for an East-bound cucumber, but in a Southbound move it means moving
        // South.   The Transpose is only ever used to show the other herd which spaces
        // are occupied, in their frame of reference. 
        // My thinking is that for either Herd this is actually a simple 1-D puzzle 
        // replicated on many rows. 

        // The bitArrays let me do some detection of "who can move" efficiently.


        public int Width;
        public int Height;
        public bool Eastbound;
        public BitArray[] Rows { get; set; }
        public BitArray[] Transpose { get; set;}

        public void Set(int i, int j, bool val)
        {
            Rows[i].Set(j, val);
            Transpose[j].Set(i, val);
        }

        public int MoveHerdOneStep(Herd otherGuys)
        {
            int count = 0;
            for(int row=0; row<Height; row++)
            {   BitArray theRow = Rows[row];
            //    showRow("Original >", theRow);

                BitArray moveThese = (BitArray) theRow.Clone();
             //   showRow("Clone", theClonedRow);
              //  showRow("v's Transposed", otherGuys.Transpose[row]);

                moveThese.Or(otherGuys.Transpose[row]);
            //    showRow("OR - all busy cells ", theClonedRow);

                bool wrapBit = moveThese[0];
                BitArray shiftedRow = (BitArray) moveThese.Clone();
              
                // If I imagine the LSB on the left, but the binary representation puts the LSB on the right,
                // like the BitSet implementor does, then we have to shift the "wrong" way
                shiftedRow.RightShift(1);
                shiftedRow.Set(Width-1, wrapBit);
             //   showRow("shifted", shiftedRow);
                moveThese.Xor(shiftedRow);
             //   showRow("xor", theClonedRow);
                moveThese.And(theRow);
            //    showRow("moveable >", theClonedRow);

                //Console.Write($"Row {row.ToString("D3")}: ");
                //char rep = Eastbound ? '>' : 'v';
                //foreach (bool x in moveThese)
                //{
                //    count++;
                //    Console.Write($"{(x?rep:'.')}");
                // //   theRow.Set

                //}
                //Console.WriteLine();

                // Now move the buggers
                for(int col = 0; col < Width; col++)
                {
                    if (moveThese[col])
                    {
                        count++;
                        Set(row, col, false);
                        Set(row, (col + 1) % Width, true);
                    }
                }
            }
            return count;
        }

        private void showRow(string v, BitArray theRow)
        {
            Console.Write(v.PadLeft(30));
            Console.Write("  ");
            char rep = 'x';
            foreach (bool x in theRow)
            {
                Console.Write($"{(x ? rep : '.')}");
            }
            Console.WriteLine();
        }

        public Herd(int width, int height, bool movesEast)
        {
            Eastbound = movesEast;
            Width = width;
            Height = height;
            Rows = new BitArray[Height];
            Transpose = new BitArray[Width];
            for (int r=0; r<Height; r++)
            {
                Rows[r] = new BitArray(Width);
            }
            for (int c = 0; c < Width; c++)
            {
               Transpose[c] = new BitArray(Height);
            }
        }

        public bool SanityCheck()
        {
            for (int r = 0; r < Height; r++)
            {
                for (int c = 0; c < Width; c++)
                {
                   if (Rows[r][c] != Transpose[c][r])
                    {
                        return false;
                    };
                }
            }
            return true;
        }
    }
}
