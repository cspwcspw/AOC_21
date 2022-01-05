using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Y2021
{

    enum ComposerOutcome  { Subsumed, Covered, Disjoint, Fragmented, Error };
    internal class Reactor
    {

        const int sz = 101;

        BigInteger sanityCheck = BigInteger.Zero;

        public Queue<Box> instructions { get; set; }

        public Reactor(string[] lines, bool clip)
        {
            Box ClipBoundary = new Box(true, new Span(-50,50), new Span(-50,50), new Span(-50,50));
            instructions = new Queue<Box>();
            foreach (var line in lines)
            {
                string rest = "";
                bool onOff = false;
                if (line.StartsWith("on"))
                {
                    onOff = true;
                    rest = line.Substring(3);
                }
                else
                {
                    rest = line.Substring(4);
                }
                string[] XYZ = rest.Split(',');
                Box bi = new Box(onOff,parse(XYZ[0]), parse(XYZ[1]), parse(XYZ[2]));
                if (!clip)
                {
                    instructions.Enqueue(bi);
                }
                else
                {
                    if (ClipBoundary.ContainsBox(bi))
                    {
                        instructions.Enqueue(bi);
                    }
                    else if (ClipBoundary.Disjoint(bi))
                    {
                        // ignore box
                  //      Console.WriteLine($"Discard box outside clip boundary: {bi}");
                    }
                    else
                    {
                        Box trimmed = new Box(bi.TurnOn, bi.Xspan.Clip(-50, 50), bi.Yspan.Clip(-50, 50), bi.Zspan.Clip(-50, 50));
                   //     Console.WriteLine($"Clipped {bi} to {trimmed}");
                        instructions.Enqueue(trimmed);
                    }
                }
            }
        }

        public Span parse(string v)
        {
            string[] parts = v.Substring(2).Split("..");
            int v0 = int.Parse(parts[0]);
            int v1 = int.Parse(parts[1]);
            Debug.Assert(v0 <= v1);
            return new Span(v0, v1);
        }

        public BigInteger DoTheWork()
        {
            var newList = FoldToDisjointBoxes(instructions);
            var result = sumHits(newList);
            return result;
        }

        private List<Box> FoldToDisjointBoxes(Queue<Box> instructions)
        {   int initialInstructions = instructions.Count;
   
            List<Box> disjointBoxes = new List<Box>();
            Box init = instructions.Peek();
            Debug.Assert(init.TurnOn);
            while (instructions.Count > 0)
            {
                Box box = instructions.Dequeue();
                //  Console.WriteLine($"Instructions to go:{boxes.Count+1}:  Folding {box} into {disjointBoxes.Count} disjoint ones.");
                Console.Write($"{instructions.Count + 1} ");
                disjointBoxes = composeOneBox(disjointBoxes, box);
            }
            Console.WriteLine();
            Console.WriteLine($"The original {initialInstructions} instructions have become {disjointBoxes.Count} disjoint ON instructions");

            return disjointBoxes;
        }

        private List<Box> composeOneBox(List<Box> disjointInstructions, Box nextInstruction)
        {
            List<Box> results = new List<Box>();
            for (int i = 0; i < disjointInstructions.Count; i++)
            {
                List<Box> pending = new List<Box>();
                pending.Add(disjointInstructions[i]);
                while (pending.Count > 0)
                {
                    Box existingInstruction = pending[0];
                    pending.RemoveAt(0);


                    if (existingInstruction.ContainsBox(nextInstruction) && (nextInstruction.TurnOn))
                    {
                        results.Add(existingInstruction);
                        for (int k = i + 1; k < disjointInstructions.Count; k++)
                        {
                            results.Add(disjointInstructions[k]);
                        }
                        return results;
                    }

                    if (nextInstruction.ContainsBox(existingInstruction))
                    {
                        // throw away the existing box rather than retain it in results
                        continue;
                    }

                    var fragments = explodeBox(existingInstruction, nextInstruction);

                    //     Console.WriteLine($"Fragmented {existingBox} into {fragments.Count} pieces. Newbox {newbox}");
                    for (int f = 0; f < fragments.Count; f++)
                    {
                        Box theFrag = fragments[f];
                    //    Console.WriteLine($"  Frag {f} is {fragments[f]}.  Covered? = {newbox.ContainsBox(theFrag)}  Disjoint? {theFrag.Disjoint(newbox)} ");
                    }

                    for (int f = 0; f < fragments.Count; f++)
                    {
                        Box theFrag = fragments[f];
                        if (theFrag.Disjoint(nextInstruction))
                        {
                            results.Add(theFrag);
                        }
                        else if (nextInstruction.ContainsBox(theFrag))  // we can discard the fragment                                                              // ement
                        {

                        }
                        else
                        {
                            if (theFrag.TotalPoints == sanityCheck)
                            {
                                Console.WriteLine($"The box {theFrag} did not split correctly against {nextInstruction}");
                            }
                            sanityCheck = theFrag.TotalPoints;
              //              Console.WriteLine($"frag[{f}] is being queued for another split");

                            pending.Add(theFrag); // for another pass
                        }
                    }
                }
            }

            // Assert: The new instruction is now disjoint from all other boxes.
            // If it turn off cells, it can be discarded. 
            // Otherwise we keep it.
            if (nextInstruction.TurnOn)
            {
                results.Add(nextInstruction);
            }
            return results;
        }

        private void showPieces(string description, List<Box> ps)
        {
            return;

            Console.Write(description);
            Console.WriteLine($" we get {ps.Count} pieces.");
            foreach (Box piece in ps)
            {
                Console.WriteLine(piece);
            }
            Console.WriteLine();
        }

        private List<Box> explodeBox(Box existingBox, Box newBox)
        {
            List<Box> xPieces;

           // Console.WriteLine($"\n{newBox} will exploding {existingBox}, already disjoint? {existingBox.Disjoint(newBox)}");
            if (existingBox.Disjoint(newBox))
            {
                xPieces = new List<Box>();
                xPieces.Add(existingBox);
                return xPieces;
            }


            int loX = existingBox.Xspan.From;
            int hiX = existingBox.Xspan.To;

            if (loX < newBox.Xspan.From && loX <= newBox.Xspan.To)
            {
                xPieces = existingBox.ChopAtX(newBox.Xspan.From);
                showPieces($"After chopping X at {newBox.Yspan.From}", xPieces);
            }
            else  if (newBox.Xspan.To+1 <= hiX)
            {
                xPieces = existingBox.ChopAtX(newBox.Xspan.To+1);
                showPieces($"After chopping X at {newBox.Xspan.To+1}", xPieces);
            }
            else
            {
                xPieces = new List<Box>();
                xPieces.Add(existingBox);
              //  Console.WriteLine("No XCuts make sense.");
            }

            int u = xPieces.Count-1;
            while (xPieces[u].Disjoint(newBox)) {
                u--;
            }

            Box b = xPieces[u];
            xPieces.RemoveAt(u);


            int loY = b.Yspan.From;
            int hiY = b.Yspan.To;
            List<Box> yPieces;
            if (loY < newBox.Yspan.From && loY <= newBox.Yspan.To)
            {
                yPieces = b.ChopAtY(newBox.Yspan.From);
                showPieces($"After chopping Y at {newBox.Yspan.From}", yPieces);
            }
            else if (newBox.Yspan.To + 1 <= hiY)
            {
                yPieces = b.ChopAtY(newBox.Yspan.To + 1);
                showPieces($"After chopping Y at {newBox.Yspan.To+1}", yPieces);
            }
            else
            {
               yPieces = new List<Box>();
               yPieces.Add(b);
           //    Console.WriteLine("No YCuts make sense.");
            }

            u = yPieces.Count - 1;
            while (yPieces[u].Disjoint(newBox)) {
                u--;
            }


            Box c = yPieces[u];
            yPieces.RemoveAt(u);

            List<Box> zPieces;
            int loZ= c.Zspan.From;
            int hiZ = c.Zspan.To;
            if (loZ < newBox.Zspan.From && loZ <= newBox.Zspan.To)  
            {
                zPieces = c.ChopAtZ(newBox.Zspan.From);
                showPieces($"After chopping Z at {newBox.Zspan.From}", zPieces);
            }
            else if (newBox.Zspan.To + 1<= hiZ)
            {
                zPieces = c.ChopAtZ(newBox.Zspan.To+1);
                showPieces($"After chopping Z at {newBox.Zspan.To+1}", zPieces);
            }
            else
            {
                zPieces = new List<Box>();
                zPieces.Add(c);
             //   Console.WriteLine("No ZCuts make sense.");
            }


            xPieces.AddRange(yPieces);
            xPieces.AddRange(zPieces);
            return xPieces;
        }

        public BigInteger sumHits(List<Box> disjoints)
        {
            BigInteger count = new BigInteger(0);
            foreach (Box b in disjoints)
            {
                count += b.TotalPoints;
            }

            return count;
        }
    }

    public struct Span
    {        // Span semantics: both endpoints included.  From <= To
        public int From { get; set; }
        public int To { get; set; }

        public int Length
        {
            get
            {
                return To - From + 1;
            }
        }

        public Span(int a, int b)
        {
            Debug.Assert(a <= b);
            From = a;
            To = b;
        }

        public bool Contains(int v)
        {
            return v >= From && v <= To;
        }

        public bool Contains(Span v)
        {
            return Contains(v.From) && Contains(v.To);
        }

        public bool Disjoint(Span v)
        {
            return (v.To < From) || (v.From > To);
        }

        public override string ToString()
        {
            return $"{From}..{To}";
        }

        internal Span Clip(int v1, int v2)
        {
           return new Span(Math.Max(v1, From), Math.Min(v2, To));
        }
    }

    public struct Box
    {
        public bool TurnOn { get; set; } = false;
        public Span Xspan { get; set; }
        public Span Yspan { get; set; }
        public Span Zspan { get; set; }

        public BigInteger TotalPoints
        {
            get
            {
                return new BigInteger(Xspan.Length) * new BigInteger(Yspan.Length) * new BigInteger(Zspan.Length);
            }
        }

        public bool ContainsBox(Box c)
        {
            return Xspan.Contains(c.Xspan) && Yspan.Contains(c.Yspan) && Zspan.Contains(c.Zspan);
        }

        public Box(bool onOff, Span x, Span y, Span z)
        {
            TurnOn = onOff;
            Xspan = x;
            Yspan = y;
            Zspan = z;
        }

        public override string ToString()
        { string instruct = TurnOn ? "on" : "off";
          return $"[{instruct} X={Xspan}, Y={Yspan}, Z={Zspan} ({TotalPoints})]";
        }

        public bool Disjoint(Box other)
        {
            return Xspan.Disjoint(other.Xspan) || Yspan.Disjoint(other.Yspan) || Zspan.Disjoint(other.Zspan);
        }

        internal List<Box> ChopAtY(int yCut)
        {
            List<Box> pieces = new List<Box>();
            //      if (Yspan.ProperlyContains(yCut))
            {
                if (Yspan.From <= yCut - 1)
                {
                    pieces.Add(new Box(TurnOn, Xspan, new Span(Yspan.From, yCut - 1), Zspan));
                }
                if (yCut <= Yspan.To)
                {
                    pieces.Add(new Box(TurnOn, Xspan, new Span(yCut, Yspan.To), Zspan));
                }
                // Console.WriteLine($"Cut y={yCut} gives {pieces[0]},{pieces[1]}");
            }
            //else
            //{
            //    pieces.Add(this);  // Dont cut so as to make an empty fragment
            //}
            
            return pieces;
        }

        internal List<Box> ChopAtZ(int zCut)
        {
            List<Box> pieces = new List<Box>();
    //        if (Zspan.ProperlyContains(zCut))
            {
                if (Zspan.From <= zCut - 1)
                {
                    pieces.Add(new Box(TurnOn, Xspan, Yspan, new Span(Zspan.From, zCut - 1)));
                }
                if (zCut <= Zspan.To)
                {
                    pieces.Add(new Box(TurnOn, Xspan, Yspan, new Span(zCut, Zspan.To)));
                }
              //  Console.WriteLine($"Cut z={zCut} gives {pieces[0]},{pieces[1]}");
            }
            //else
            //{
            //    pieces.Add(this);  // Dont cut so as to make an empty fragment
            //}

            return pieces;
        }

        internal List<Box> ChopAtX(int xCut)
        {
            List<Box> pieces = new List<Box>();
      //      if (Xspan.ProperlyContains(xCut))
            {
                if (Xspan.From <= xCut - 1)
                {
                    pieces.Add(new Box(TurnOn, new Span(Xspan.From, xCut - 1), Yspan, Zspan));
                }
                if (xCut <= Xspan.To)
                {
                    pieces.Add(new Box(TurnOn, new Span(xCut, Xspan.To), Yspan, Zspan));
                }
                    
                  
         //       Console.WriteLine($"Cut x={xCut} gives {pieces[0]},{pieces[1]}");
            }
            //else
            //{
            //    pieces.Add(this);  // Dont cut so as to make an empty fragment
            //}

            return pieces;
        }
    }
}