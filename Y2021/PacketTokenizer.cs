using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Y2021
{
    public class PacketTokenizer
    {
        List<byte> binary;
        public int nextPos { get; private set; }
        const string HexDigits = "0123456789ABCDEF";
        public PacketTokenizer(string inputString)
        {
            binary = new List<byte>();

            foreach (char c in inputString)
            {
                int n = HexDigits.IndexOf(c);
                int d0 = n % 2;
                int d1 = (n >> 1) % 2;
                int d2 = (n >> 2) % 2;
                int d3 = (n >> 3) % 2;
                binary.Add((byte)d3);
                binary.Add((byte)d2);
                binary.Add((byte)d1);
                binary.Add((byte)d0);
            }
            nextPos = 0;
        }

        public int FindChunks()
        {
            return 42;
        }

        public byte getVersion()
        {
          byte b =  getByte(3);
            return b;
        }

        public byte getId()
        {
            byte b = getByte(3);
            return b;
        }

        public byte getByte(int numBits)
        {
            if (numBits == 0) return 0;
            byte result = binary[nextPos++];
            for(int i=1; i < numBits; i++)
            {
                result = (byte) ((result << 1) + binary[nextPos++]);
            }
            return result;
        }

        internal int getInt(int numBits)
        {
            if (numBits == 0) return 0;
            int result = binary[nextPos++];
            for (int i = 1; i < numBits; i++)
            {
                result = (result << 1) + binary[nextPos++];
            }
            return result;
        }

        public long getLong()
        {   // assume at least one 5-bit chunk
            long result = 0;
            bool moreChunksToCome = true;
            do
            {
                moreChunksToCome = getByte(1) == 1;
                byte thisChunk = getByte(4);
                result = (result << 4) + thisChunk;

            }
            while (moreChunksToCome);
            return result;
        }

        public Packet getPacket()
        {

            byte ver = getVersion();
            byte id = getId();
            long num;
            Packet result = null;
            switch (id)
            {
                case 4:
                    num = getLong();
                    result = new Packet(ver, id, num);
                    break;
                default:
                    bool lengthSubpackets = getByte(1) == 1;
                    if (lengthSubpackets)
                    {
                        int numPackets = getInt(11);
                        result = new Packet(ver, id, 0);
                        for (int i=0; i <numPackets; i++)
                        {
                            Packet sub = getPacket();
                            result.Children.Add(sub);
                        }
                    }
                    else
                    {
                        int numBits = getInt(15);
                        int endPos = nextPos + numBits;
                        result = new Packet(ver, id, 0);
                        while (nextPos < endPos)
                        {
                            Packet sub = getPacket();
                        
                            result.Children.Add(sub);
                        }
                    }
                    break;
            }

            return result;
        }



    }




    public class Packet
    {
        public byte Version { get; set; }
        public byte ID { get; set; }
        public long Val { get; set; }  // valid if ID==4
        public List<Packet> Children { get; set; }

        public Packet(byte v, byte id, long val)
        {
            Children = new List<Packet>();
            Version = v;
            ID = id;
            Val = val;
        }

        public int sumAllVersions()
        {
            List<int> childSums = new List<int>(Children.Select(p => p.sumAllVersions()));
            int result = childSums.Sum();
            result += Version;
            return result;
        }

        public long eval()
        {
            if (ID == 4) return Val;

            List<long> subVals = new List<long>(Children.Select(p => p.eval()));

            switch (ID)
            {
                case 0: // Sum
                    return subVals.Sum();
  
                case 1: { // Product
                        long prod = subVals[0];
                        for (int i = 1; i < subVals.Count; i++)
                        {
                            prod *= subVals[i];
                        }

                        return prod;
                    }

                case 2: // min
                    return subVals.Min();
 
                case 3: // Max
                    return subVals.Max();
             
                case 5: // GT of 2
                    if (subVals[0] > subVals[1]) return 1; else return 0;
             
                case 6: // LT of 2
                    if (subVals[0] < subVals[1]) return 1; else return 0;
             
                case 7: // EQ of 2
                    if (subVals[0] == subVals[1]) return 1; else return 0;
            }


            return -9999;
        }
    }
}
