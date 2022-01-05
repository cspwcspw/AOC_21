using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

/* So  some hard-work insights.
 * Different people get different ALU programs.  I don't know if their answers are also personalized.
 * There are PUSH and POP instructions and each of the 14 rounds is z=q(z,w,t,u,v) where t,u,v "parameterize" the round.
 * z is the running "stack", so to speak. w is the new input digit. 
 * t deterimines whether to perform a PUSH (t==1) or a POP_and_validate(t==26).
 * The stack is stored as a big base 26 number.  Each round starts by saving TOP by modding off the lowest base 26 digit.
 * For a PUSH, the stack is multipled by 26, and the new incoming digit from the licence key is added (together with an 
 * extra offset - parameter v).   u is unused during a PUSH.  
 * For a POP operation, the stack is divided by 26 to remove the element.  
 * During a POP, the element being considered is adjusted by u before comparison to the input digit from the key.
 * So the trick to help the elves is to pair each pop to its corresponding push, and then use u and v to work out what
 * possible push digits will be matched by corresponding pop values for that pair.
 * * */

namespace Y2021
{
    internal class Validator
    {
 
        // Helps me to see z as a stack of elements
        string toStr(BigInteger z)
        {
            Stack<int> nums =  new Stack<int>();
            do
            {
                int d = (int)(z % 26);
                z = z / 26;
                nums.Push(d);

            }
            while (z != 0);

            string sep = "";
            StringBuilder sb = new StringBuilder();
            do
            { int num = nums.Pop();
                sb.Append(sep);
                sep = ":";
                sb.Append(num.ToString("D2"));
            } while (nums.Count > 0);

            return sb.ToString() ;
        }

        public BigInteger Validate(string key)
        {
            BigInteger z = 0;

            z = q(z, key[0] - '0', 1, 15, 15);
            z = q(z, key[1] - '0', 1, 12, 5);
            z = q(z, key[2] - '0', 1, 13, 6);
            z = q(z, key[3] - '0', 26, -14, 7);
            z = q(z, key[4] - '0', 1, 15, 9);
            z = q(z, key[5] - '0', 26, -7, 6);

            z = q(z, key[6] - '0', 1, 14, 14);
            z = q(z, key[7] - '0', 1, 15, 3);

            z = q(z, key[8] - '0', 1, 15, 1);
            z = q(z, key[9] - '0', 26, -7, 3);

            z = q(z, key[10] - '0', 26, -8, 4);
            z = q(z, key[11] - '0', 26, -7, 6);
            z = q(z, key[12] - '0', 26, -5, 7);
            z = q(z, key[13] - '0', 26, -10, 1); // Round 14
            return z;
        }


        BigInteger q(BigInteger z, int w, int t, int u, int v)
        {
            int x = (int) ( z % 26);  // Save current top of stack in x

            z = z / t;                // Either pop the stack (if t==26) or leave it as is (if t==1)

            string op = t == 26 ? "POP " : "PUSH";

            x += u;                   // add an extra (negative always in my case) offset to x

            x = (x == w) ? 1 : 0;     // Check if it matches the incoming licence key digit
            x = (x == 0) ? 1 : 0;     // Then boolean negate the answer. This match is only relevant on a POP

            string hadMatch = x == 1 ? (t==1 ? "dontCare" : "NO MATCH") : "Matched ";

            int y = 25 * x + 1;       // Now set Y to either be 26 (for a push) or 1 to leave z alone.

            z = z * y;               // push the element to make new space on z (or not if y==1)

            y = (w + v) * x;         // x will be 1 when pushing, so give the inbound digit an extra offset
            z += y;                  // and add it to the space we made in the stack

            Console.WriteLine($"inp={w} {op} {hadMatch} round result z = {toStr(z)}");
            return z;
        }
    }
}
