using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Y2021
{
    internal class SnailNum
    {
        public SnailNum Parent { get; private set;  }
        public bool IsLeaf { get; private set; }
        public long Val { get; private set; }
        public SnailNum Left { get; private set; }
        public SnailNum Right { get; private set; }


        public SnailNum() { }

        public static SnailNum FromString(string s)
        {
            string ss = s.Replace(" ", "");  // Squeeze out spaces
            List<char> input = new List<char>(ss);
            SnailNum result = SnailNum.Parse(input);
            return result;
        }


        public static SnailNum MkLeaf(SnailNum parent, long val)
        {
            SnailNum result = new SnailNum() { IsLeaf=true, Val = val, Left=null, Right=null, Parent = parent };
            return result;
        }


        public static SnailNum Parse(List<char> input)
        {
           if (char.IsDigit(input[0]))
            {   int v = input[0] - '0';
                SnailNum n = new SnailNum() { IsLeaf = true, Val = v, Left = null, Right = null, Parent = null };
                input.RemoveAt(0);
                return n;
            }
           else
            {
                gobble(input, '[');
                SnailNum left = Parse(input);
                gobble(input, ',');
                SnailNum right = Parse(input);
                gobble(input,  ']');

                SnailNum n = new SnailNum() { IsLeaf = false, Val = 0, Left = left, Right = right };
                n.Left.Parent = n;
                n.Right.Parent = n;
                return n;
            }
        }

        private static void gobble(List<char> input, char v)
        {
            Debug.Assert(input[0] == v);
            input.RemoveAt(0);
        }

        public long Magnitude()
        {
            if (IsLeaf) return Val;
            long n1 = Left.Magnitude();
            long n2 = Right.Magnitude();
            return 3 * n1 + 2 * n2;
        }

        public bool TryExplode()
        {
            SnailNum culprit = findFirstNestingViolation(4);
            if (culprit == null)
            {
                return false;
            }
            culprit.Explode();
            return true;
        }

        private void Explode()
        { 
            Debug.Assert(Left.IsLeaf);
            Debug.Assert(Right.IsLeaf);
            SnailNum leftPredecessor = findLeftPredecessor();
            SnailNum rightSuccessor = findRightSuccessor();
            if (leftPredecessor != null)
            {
                Debug.Assert(leftPredecessor.IsLeaf);
                leftPredecessor.Val += Left.Val;
            }
            if (rightSuccessor != null)
            {
                Debug.Assert(rightSuccessor.IsLeaf);
                rightSuccessor.Val += Right.Val;
            }
            Debug.Assert(Parent != null && (Parent.Left.Equals(this) || Parent.Right.Equals(this)));
            SnailNum zero = new SnailNum() { IsLeaf = true, Val = 0, Left = null, Right = null, Parent = this.Parent };
            if (Parent.Right.Equals(this))
            {
                Parent.Right = zero;
            }
            else if (Parent.Left.Equals(this))
            {
                Parent.Left = zero;
            }
            else
            {
                throw new ApplicationException("OOps");
            }
        }

        private SnailNum findRightSuccessor()
        {
            SnailNum p = this;
            SnailNum q = p.Parent;
            while (q.Right.Equals(p))
            {
                p = q;
                q = p.Parent;
                if (q == null) return null;
            }
            q = q.Right;
            while (!q.IsLeaf)
            {
                q = q.Left;
            }
            return q;
        }

        private SnailNum findLeftPredecessor()
        {
            SnailNum p = this;
            SnailNum q = p.Parent;
            while (q.Left.Equals(p))
            {
                p = q;
                q = p.Parent;
                if (q == null) return null;
            }
            q = q.Left;
            while (!q.IsLeaf)
            {
                q = q.Right;
            }
            return q;
        }

        public SnailNum findFirstNestingViolation(int allowedDepth)
        {
            if (IsLeaf) return null; // no violation
            if (allowedDepth > 0)
            {
                SnailNum leftCulprit = Left.findFirstNestingViolation(allowedDepth - 1);
                if (leftCulprit != null)
                {
                    return leftCulprit;
                }
                SnailNum rightCulprit = Right.findFirstNestingViolation(allowedDepth - 1);
                return rightCulprit;
            }
            return this;
        }

        public bool TrySplit( )
        {
            SnailNum culprit = findFirstOversizeNumber();
            if (culprit == null)
            {
                return false;
            }
            culprit.Split();
            return true;
        }

        public SnailNum findFirstOversizeNumber()
        {
            if (IsLeaf)
            {
                if (Val < 10)
                {
                    return null;
                }
                return this;
            }
            SnailNum left = Left.findFirstOversizeNumber();
            if (left != null) return left;
            return Right.findFirstOversizeNumber();
        }

        public void Split()
        {
            Debug.Assert(IsLeaf);
            long n1 = Val / 2;
            long n2 = (Val + 1) / 2;
            Left = MkLeaf(this, n1);
            Right = MkLeaf(this, n2);
            IsLeaf = false;
            Val = 0;
        }


        public void Reduce()
        {
            bool hadChange = true;
            do
            {
                do
                {
                    hadChange = TryExplode(); 
                    //if (hadChange)
                    //{
                    //    Console.WriteLine($"after explode:   {this}");
                    //}
                }
                while (hadChange);
                hadChange = TrySplit();
                //if (hadChange)
                //{
                //    Console.WriteLine($"after split:     {this}");
                //}
            }
            while (hadChange);
        }

        public SnailNum Add(SnailNum other)
        {
            SnailNum result1 = new SnailNum() { IsLeaf = false, Parent = null, Left = this, Right = other };
            result1.Left.Parent = result1;
            result1.Right.Parent = result1;
            return result1;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            BuildString(sb);
            return sb.ToString(); 
        }

        public void BuildString(StringBuilder sb)
        {
            if (IsLeaf) { sb.Append(Val); }
            else
            {
                sb.Append('[');
                Left.BuildString(sb);
                sb.Append(',');
                Right.BuildString(sb);
                sb.Append(']');
            }
        }
    }
}
