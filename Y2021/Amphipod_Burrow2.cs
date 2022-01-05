
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Y2021
{
    internal class Amphipod_Burrow2
    {
        public static void RunBothParts()
        {
            const string livedataA = "..-.-.-.-.. DC AA DB CB";
            const string testdataA = "..-.-.-.-.. BA CD BC DA";

            Console.WriteLine("\n--------- Test Case Part A --------");
            Amphipod_Burrow2 b1t = new Amphipod_Burrow2(testdataA, 12521);
            b1t.DoSearch(false);

            Console.WriteLine("\n--------- Live Case Part A --------");
            Amphipod_Burrow2 bw = new Amphipod_Burrow2(livedataA, 14546);
            bw.DoSearch(false);
    
            const string livedataB = "..-.-.-.-.. DDDC ACBA DBAB CACB";
            const string testdataB = "..-.-.-.-.. BDDA CCBD BBAC DACA";
            Console.WriteLine("\n--------- Test Case Part B --------");
            Amphipod_Burrow2 b2t = new Amphipod_Burrow2(testdataB, 44169);
            b2t.DoSearch(false);

            Console.WriteLine("\n--------- Live Case Part B --------");
            Amphipod_Burrow2 b2l = new Amphipod_Burrow2(livedataB, 42308);   
            b2l.DoSearch(false);

        }

        public List<BState> pathFromRoot { get; set; }

        public long LowestCost { get; set; } = 999999999;

        public BState LowestSolution = null;

        static public List<int> doorways = new List<int>() { 2, 4, 6, 8 };

        long Expected;

        long discardsOnCost = 0;
        long deadEnds = 0;
        long statesExplored = 0;
        int highTide = 0;

      

        string Initial;
        public Amphipod_Burrow2(string initial, long expected)
        {
            Initial = initial.Replace(" ", "");
            Expected = expected;
        }

        public static void TestMoves(string initial)
        {
            initial = initial.Replace(" ", "");

            BState initState = new BState(initial, 0);

            Console.WriteLine(initState.To2DString());

            List<Move> allMoves = initState.getAllPossibleMoves();
            Console.WriteLine($"{allMoves.Count} moves possible:");
            for (int i = 0; i < allMoves.Count; i++)
            {
                Move theMove = allMoves[i];
                Console.WriteLine($"{theMove}");
            }


            for (int i = 0; i < allMoves.Count; i++)
            {
                Move theMove = allMoves[i];
                BState child = initState.MakeMove(allMoves[i]);
                Console.WriteLine($"Test case in initializer {initState} {theMove} ==> \n{child.To2DString()}");
            }
        }

        static public void TestOracle()
        {
            List<Move> Oracle = new List<Move>();
        
            Oracle.Add(new Move(15, 3, 4));
            Oracle.Add(new Move(13,5,2));
            Oracle.Add(new Move(5, 15, 2));
            Oracle.Add(new Move(14,5,3));
            Oracle.Add(new Move(3,14,3));
            Oracle.Add(new Move(11,3,2));
            Oracle.Add(new Move(3,13,2));
            Oracle.Add(new Move(17,7,2));
            Oracle.Add(new Move(18,9,3));
            Oracle.Add(new Move(7,18,3));
            Oracle.Add(new Move(5,17,4));
            Oracle.Add(new Move(9,11,8));

            BState theState = new BState("..-.-.-.-..BACDBCDA", 0);

            Console.WriteLine(theState.To2DString());

            for (int i=0; i < Oracle.Count; i++)
            {
                List<Move> children = theState.getAllPossibleMoves();
                if (children.Contains(Oracle[i]))
                {
                    theState = theState.MakeMove(Oracle[i]);
                    Console.WriteLine(theState.To2DString());
                }
                else
                {
                    Console.WriteLine("Whoops, why not?");
                }
            }
        }


        public void DoSearch(bool withBackTrace)
        {
            DateTime t0 = DateTime.Now;
            Search();
            DateTime t1 = DateTime.Now;
            if (withBackTrace)
            {
                ShowBackTrace(LowestSolution);
            }
            Console.WriteLine($"LowestCost = {LowestCost} (error={LowestCost-Expected}) \ndiscards={discardsOnCost} \ndeadEnds={deadEnds} \nstateesExplored={statesExplored}\nhighTide={highTide}\net={(t1 - t0).TotalSeconds}");
        }

        public void ShowBackTrace(BState traceBack)
        {
            Console.WriteLine("TraceBack:");
            while (traceBack != null)
            {
                Console.WriteLine(traceBack.To2DString());
                traceBack = traceBack.parent;
            }
        }

        public void Search()
        {

            discardsOnCost = 0;
            deadEnds = 0;
            statesExplored = 0;
            highTide = 0;
            List<BState> pending = new List<BState>();
            pending.Add(new BState(Initial, 0));


             while (true)
            {
                int n = pending.Count;
                if (n == 0) break;  // Search is over
                if (n > highTide)
                {
                    highTide = n;
                }
                BState curr = pending[n - 1];
                pending.RemoveAt(n - 1);
                statesExplored++;
                if (curr.Cost < LowestCost)
                {
                    List<Move> allMoves = curr.getAllPossibleMoves();
                    for (int i = 0; i < allMoves.Count; i++)
                    {
                        Move theMove = allMoves[i];
                        BState child = curr.MakeMove(theMove);
                        if (child.isSolution())
                        {
                          //  Console.WriteLine($"Got solution from {curr} => {child} with Lowest={LowestCost} and pending={pending.Count}");
                            if (child.Cost < LowestCost)
                            {
                                LowestCost = child.Cost;
                                LowestSolution = child;
                                Console.WriteLine($"Lowest cost revised downwards to {LowestCost}");
                            }
                        }
                        else
                        {
                            //if (stateesExplored % 1000000 == 0)
                            //{
                            //    Console.WriteLine($"Iters {iterations}: {curr} {theMove} ==> {child} pending={pending.Count}");
                            //}

                            if (child.Cost + child.MinCostToFinalize < LowestCost)


                            {
                                int pos = pending.IndexOf(child);
                                if (pos < 0)
                                {
                                    pending.Add(child);
                                }
                                else
                                {
                                    if (child.Cost < pending[pos].Cost)
                                    {
                                        pending[pos] = child;
                                    }
                                }

                                if (pending.Count % 1000 == 0)
                                {
                                    Console.WriteLine($"Occasional Pending.Count={pending.Count}");
                                }
                            }
                            else
                            {
                                discardsOnCost++;
                            }
                        }
                    }
                    if (allMoves.Count == 0)
                    {
                        deadEnds++;
                    }
                }
                else
                {
                    discardsOnCost++;
                }
            }
        }
        private void showBetterCostPath(long cost, List<BState> path)
        {
            throw new NotImplementedException();
        }
    }

    public struct Move
    {
        public byte From;
        public byte To;
        public byte Steps;

        public Move(int f, int t, int s)
        {
            From = (byte)f;
            To = (byte)t;
            Steps = (byte)s;
        }

        public override string ToString()
        {
            return $"({From}->{To} in {Steps})";
        }
    }

    public class BState
    {
        static int RoomDepth;
        static int[] stepsToDoorway;
        static int[] doorwaysOut;
        public char[] board = new char[11 + 4 * RoomDepth];

        public BState parent = null;
        public int Depth { get; set; } = 0;

        static int[] backHomeOf;

        static int[] doorwaysIn = { 2, 4, 6, 8 };  // index by pieceNum

        static int[,] minCosts = new int[4,11] {
            {3, 2, 99, 2, 99, 4, 99, 6, 99, 8, 9 },
            {50, 40, 99, 20, 99, 20, 99, 40, 99, 60,70 },
            {700, 600, 99,400, 99, 200, 99, 200, 99, 400, 500 },
            {10000, 8000, 99, 6000, 99, 4000, 99, 2000, 99, 2000, 3000 }
        };

        public long Cost { get; set; }

        public long MinCostToFinalize
        {
            get
            {
                long sum = 0;
                for (int i = 0; i < 11; i++)
                {
                    if (board[i] >= 'A')
                    {
                        int pieceIndex = board[i] - 'A';
                        int lowerBound = minCosts[pieceIndex, i];
                        sum += lowerBound;
                    }
                }
                return sum;
            }
        }

        private BState()
        {
        }

        public BState(string init, long initCost)
        {
            init = init.Replace(" ", "");
            //     Debug.Assert(init.Length == board.Length);

            if (init.Length == 19)
            {
                RoomDepth = 2;
                stepsToDoorway = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 1, 2, 1, 2, 1, 2 };
                doorwaysOut = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 4, 4, 6, 6, 8, 8 }; // indexed by board position
            }
            else if (init.Length == 27)
            {
                RoomDepth = 4;
                stepsToDoorway = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4 };
                doorwaysOut = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 4, 4, 4, 4, 6, 6, 6, 6, 8, 8, 8, 8 }; // indexed by board position 
            }
            else
            {
                throw new ApplicationException("Initialization string length must be 19 or 27. We can only deal with cases where rooms are dept 2 or depth 4.");
            }

            backHomeOf = new int[] { 10 + RoomDepth, 10 + 2 * RoomDepth, 10 + 3 * RoomDepth, 10 + 4 * RoomDepth };
            board = new char[11 + 4 * RoomDepth];

            for (int i = 0; i < board.Length; i++)
            {
                board[i] = init[i];
            }
            board[2] = board[4] = board[6] = board[8] = '-';
            Cost = initCost;
        }

        public override bool Equals(object obj)
        {
            BState other = obj as BState;
            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] != other.board[i])
                {
                    return false;
                }
            }
            return true;
        }

        internal BState MakeMove(Move move)
        {
            BState result = new BState();
            result.parent = this;
            result.Cost = Cost;
            for (int i = 0; i < board.Length; i++)
            {
                result.board[i] = board[i];
            }
            char piece = result.board[move.From];
            result.board[move.From] = '.';
            result.board[move.To] = piece;
            result.Cost += getCost(piece) * move.Steps;
            result.Depth = Depth + 1;

            return result;
        }

        private long getCost(char piece)
        {
            long[] cost = { 1, 10, 100, 1000 };
            return cost[piece - 'A'];
        }

        public bool isBlockedToCorridor(int posn)
        {
            int topOfRoom = ((posn-11) / RoomDepth) * RoomDepth + 11;
            for (int i= posn-1; i>= topOfRoom; i--)
            {
                if (board[i] != '.')
                {
                    return true;
                }
            }
            return false;
        }

        private bool alreadyFinallyHome(int posn)
        {
            Debug.Assert(posn >= 11);
            char piece = board[posn];
            int owner = ((posn - 11) / RoomDepth);
            if (owner != piece - 'A')
            {
                return false;
            }
            //Debug.Assert(board[posn] == '.');  // is this in the home room?
            int bottomOfRoom = ((posn - 11) / RoomDepth) * RoomDepth + 10 + RoomDepth;
            for (int i = posn + 1; i <= bottomOfRoom; i++)
            {
                if (board[i] != piece)
                {
                    return false;
                }
            }
            return true;
        }


        private bool roomHasForeigners(char me, int ownRoomTop)
        {
            for (int i= ownRoomTop; i < ownRoomTop + RoomDepth; i++)
            {
                if (board[i] >= 'A' && board[i] != me) return true;
            }
            return false;
        }


        public List<Move> getAllPossibleMoves()
        {
            List<Move> allMoves = new List<Move>();

            // Pieces in the corridor can only move directly to their own room, if possible.
            // If we find one that can move, return the singleton immediately.
            for (int i = 0; i < 11; i++)
            {
                if (board[i] < 'A')
                {
                    continue;
                }

                int pieceNum = board[i] - 'A';
                if (AllClearBetween(i, doorwaysIn[pieceNum]))
                {
                    int dest = pieceNum * RoomDepth + 11;  // This is the first room slot closest to the door

                    if (roomHasForeigners(board[i], dest)) continue;
                    int stepsWithinRoom = 1;   // takes one step to get in from the doorway
                    int lastRoomSlot = dest + RoomDepth - 1;
                    while (dest < lastRoomSlot)
                    {
                        if (board[dest + 1] == '.')
                        {
                            dest++;
                            stepsWithinRoom++;
                        }
                        else break;
                    }
                    int steps = Math.Abs(i - doorwaysIn[pieceNum]) + stepsWithinRoom;
                    Debug.Assert(allMoves.Count == 0);
                    //    allMoves.Clear();     // Cull others
                    allMoves.Add(new Move(i, dest, steps));
                    return allMoves;
                }
            }

            // Now we handle pieces already in rooms. How can they move?
            for (int i = 11; i < board.Length; i++)
            {
                if (board[i] < 'A')
                {
                    continue;
                }

                if (alreadyFinallyHome(i))
                {
                    continue;
                }

                if (isBlockedToCorridor(i))
                {
                    continue;
                }

                int doorwayOut = doorwaysOut[i];

                int u = doorwayOut;
                int stepsToDway = stepsToDoorway[i];

                // If we can get to the destination diretly, so that to the exclusion of all other candidates.
                int pieceNum = board[i] - 'A';
                if (AllClearBetween(u, doorwaysIn[pieceNum]))
                {
                    int dest = pieceNum * RoomDepth + 11;  // This is the first room slot closest to the door

                    if (!roomHasForeigners(board[i], dest))
                    {
                        int stepsWithinRoom = 1;   // takes one step to get in from the doorway
                        int lastRoomSlot = dest + RoomDepth - 1;
                        while (dest < lastRoomSlot)
                        {
                            if (board[dest + 1] == '.')
                            {
                                dest++;
                                stepsWithinRoom++;
                            }
                            else break;
                        }
                        int steps = stepsToDway + Math.Abs(u - doorwaysIn[pieceNum]) + stepsWithinRoom;
                     //   Debug.Assert(allMoves.Count == 0);
                        allMoves.Clear();     // Cull others
                        allMoves.Add(new Move(i, dest, steps));
                        return allMoves;
                    }
                }




                while (true)  // try all moves to left of doorway
                {
                    u--;
                    if (u < 0) break;
                    stepsToDway++;
                    if (doorwaysIn.Contains(u)) continue;
                    if (board[u] == '.')
                    {
                        allMoves.Add(new Move(i, u, stepsToDway));
                    }
                    else
                    {
                        break; // I am blocked by another piece
                    }
                }

                u = doorwayOut;
                stepsToDway = stepsToDoorway[i];
                while (true)  // Rinse and repeat, moving to the right in the corrido this time
                {
                    u++;
                    if (u >= 11) break;
                    stepsToDway++;
                    if (doorwaysIn.Contains(u)) continue;
                    if (board[u] == '.')
                    {
                        allMoves.Add(new Move(i, u, stepsToDway));
                    }
                    else
                    {
                        break;
                    }
                }

            }
            return allMoves;
        }

        private bool AllClearBetween(int i, int v)
        {
            if (i < v)
            {
                for (int k = i + 1; k < v; k++)
                {
                    if (board[k] >= 'A') return false;
                }
                return true;
            }
            for (int k = i - 1; k > v; k--)
            {
                if (board[k] >= 'A') return false;
            }
            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (RoomDepth == 2)
            {
                for (int i = 0; i < 19; i++)
                {
                    if (i == 11 || i == 13 || i == 15 || i == 17)
                    {
                        sb.Append(' ');
                    }
                    sb.Append(board[i]);
                }
            }
            else
            {
                for (int i = 0; i < 27; i++)
                {
                    if (i == 11 || i == 15 || i == 19 || i == 23)
                    {
                        sb.Append(' ');
                    }
                    sb.Append(board[i]);
                }
            }
            sb.Append($" Depth={Depth} Cost={Cost}");
            return sb.ToString(); ;
        }

        public string To2DString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("#--2-4-6-8--#");
            sb.Append($" Depth={Depth} Cost={Cost}\n");
            sb.Append('#');
            for (int i = 0; i < 11; i++)
            {
                sb.Append(board[i]);
            }
            sb.Append('#');

            for (int i = 0; i < RoomDepth; i++)
            {
                sb.Append($"\n  #{board[11+i]}x{board[11+i + RoomDepth]}x{board[11+i + 2*RoomDepth]}x{board[11+i + 3*RoomDepth]}#  ");
            }
            sb.Append($"\n  #########");
            return sb.ToString(); ;
        }

        internal bool isSolution()
        {
            for (int h = 0; h < 4; h++)
            {
                char piece = "ABCD"[h];
                int lb = 11 + h * RoomDepth;
                for (int d = lb; d < lb+RoomDepth; d++)
                {
                    if (board[d] != piece) return false;
                }
            }
            return true;
        }
    }
}
