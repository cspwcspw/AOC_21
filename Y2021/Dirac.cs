using System;
using System.Collections.Generic;
using System.Numerics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;

namespace Y2021
{
    internal class Dirac
    {   
        // Pending universes represent incomplete games. They are split into two 
        // sections, one in which it is p1's turn to play, one in which it is 
        // p2's turn to play.  (Turn to play is implicit - I don't keep it in
        // the game state.)
        // Furthermore, each group of pending Universes is organized as 21 sublists 
        // based on the score of whose turn it is to play.  This allows me to always
        // pick the lowest score games next - essentially this is my heuristic for 
        // ensuring that I have the best opportunity of "folding" multiple universes
        // into a single pending proxy. 

        List<List<GameState>> p1UniversesPending, p2UniversesPending;

        List<GameState> P1WonUniverses;
        List<GameState> P2WonUniverses;

        public Dirac(int p1, int p2)
        {
            P1WonUniverses = new List<GameState>();
            P2WonUniverses = new List<GameState>(); 

            p1UniversesPending = new List<List<GameState>>();
            p2UniversesPending = new List<List<GameState>>();

            for (int i = 0; i < 21; i++)
            {
                p1UniversesPending.Add(new List<GameState>());
                p2UniversesPending.Add(new List<GameState>());
            }
            GameState initial = new GameState(0, 0, p1, p2, 1);
            p1UniversesPending[0].Add(initial);
         }

        int lowerBound = 0;
        internal void Run()
        {
            lowerBound = 0;
            while (true)
            {
                if (p1UniversesPending[lowerBound].Count == 0 && p2UniversesPending[lowerBound].Count == 0)
                {
                    int p1 = countPendingUniverses(p1UniversesPending);
                    int p2 = countPendingUniverses(p2UniversesPending);
                    Console.WriteLine($"Player scores above {lowerBound}. Pending p1={p1} p2={p2}");

                    lowerBound++;
                    if (lowerBound == 21)
                    {

                        Debug.Assert(p1==0 && p2==0);
                        // We are done! :-)
                        return;
                    }
                }
                while (p1UniversesPending[lowerBound].Count > 0)
                {
                    int last = p1UniversesPending[lowerBound].Count - 1;
                    GameState theState = p1UniversesPending[lowerBound][last];
                    p1UniversesPending[lowerBound].RemoveAt(last);
                    PlayOneTurn(true, theState, P1WonUniverses, p2UniversesPending);
                }
            
                while (p2UniversesPending[lowerBound].Count > 0)
                {
                    int last = p2UniversesPending[lowerBound].Count - 1;
                    GameState theState = p2UniversesPending[lowerBound][last];
                    p2UniversesPending[lowerBound].RemoveAt(last);
                    PlayOneTurn(false, theState, P2WonUniverses, p1UniversesPending);
                }
            }
        }

        private int countPendingUniverses(List<List<GameState>> universesPending)
        {
            int count = 0;
            foreach (var u in universesPending)
            {
                count += u.Count;
            }
            return count;
        }

        int[] freqs = { 1, 3, 6, 7, 6, 3, 1 };
        private void PlayOneTurn(bool p1ToPlay, GameState theState, List<GameState> WonUniverses, List<List<GameState>> otherUniversesPending)
        {
         //   Console.WriteLine($"Children of {theState}");
            for (int move = 3; move < 10; move++) {
                GameState child = theState.SplitUniverse(p1ToPlay, move, freqs[move - 3]);
                //       Console.WriteLine(child);
                int theMinPos = Math.Min(child.p1Score, child.p2Score);
                if (p1ToPlay)
                {  
                    if (child.p1Score >= 21)
                    {
                        WonUniverses.Add(child);
                    }
                    else
                    {
                        MergeInto(child, otherUniversesPending[theMinPos]);
                    }
                }
                else
                {
                    if (child.p2Score >= 21)
                    {
                        WonUniverses.Add(child);
                    }
                    else
                    {
                        MergeInto(child, otherUniversesPending[theMinPos]);
                    }
                }
             }
        }

        private void MergeInto(GameState child, List<GameState> pending)
        {
            int indx = pending.IndexOf(child);
            if (indx >= 0)
            {
                var elem = pending[indx];
                pending.RemoveAt(indx);
                GameState replaceWith = new GameState(elem.p1Score, elem.p2Score, elem.p1Pos, elem.p2Pos, elem.proxies + child.proxies);
                pending.Add(replaceWith);
            }
            else
            {
                pending.Add(child);
            }             
        }

        public Tuple<BigInteger, BigInteger> CountWinners()
        {
            return new Tuple<BigInteger, BigInteger>(SumUp(P1WonUniverses), SumUp(P2WonUniverses));
        }

        private BigInteger SumUp(List<GameState> states)
        {
            BigInteger sum = 0;
            foreach (GameState state in states)
            {
                sum += state.proxies;
            }
            return sum;
        }


        void unitTest()
        {
            P1WonUniverses = new List<GameState>();
            GameState s1 = new GameState(3, 4, 5, 6, new BigInteger(42));
            P1WonUniverses.Add(s1);
            GameState s2 = new GameState(3, 4, 5, 6, new BigInteger(23424));
            if (s1.Equals(s2))
            {
                Console.WriteLine("Pass 1");
            }
            else
            {
                Console.WriteLine("Fail 1");
            }
            bool b1 = P1WonUniverses.Contains(s1);
            bool b2 = P1WonUniverses.Contains(s2);
            if (b1 && b2)
            {
                Console.WriteLine("Pass 2");
            }
            else
            {
                Console.WriteLine("Fail 2");
            }
        }
    }

    struct GameState
    {
        public byte p1Score {get;set;}
        public byte p2Score { get; set; }
        public byte p1Pos { get; set; }
        public byte p2Pos { get; set; }
        public BigInteger proxies { get; set; }

        public override string ToString()
        {
            return $"(Pos,Score) P1={p1Pos},{p1Score} P2={p2Pos},{p2Score} representing {proxies} universes.";
        }

        public override bool Equals([NotNullWhen(true)] object obj)
        {
            GameState gs = (GameState) obj;
            return p1Pos == gs.p1Pos && p2Pos == gs.p2Pos && p1Score == gs.p1Score && p2Score == gs.p2Score;
        }

        internal GameState SplitUniverse(bool p1ToPlay, int moves, int replicas)
        {
            if (p1ToPlay)
            {           
                int pos = ((p1Pos + moves - 1) % 10) + 1;
                return new GameState(p1Score + pos, p2Score, pos, p2Pos, proxies * replicas);
            }
            else
            {
                int pos = ((p2Pos + moves - 1) % 10) + 1;
                return new GameState(p1Score, p2Score+pos, p1Pos, pos, proxies * replicas);
            }
        }

        public GameState(int p1S, int p2S, int p1Posn, int p2Posn, BigInteger proxyCount)
        {
            p1Score = (byte)p1S;
            p2Score = (byte)p2S;
            p1Pos = (byte)p1Posn;
            p2Pos = (byte)p2Posn;
            proxies = proxyCount;
        }
    }
}
