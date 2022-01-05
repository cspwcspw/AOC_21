using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using Helpers;

namespace Y2021
{
    class Program
    {
        //static BigInteger Factorial(int n)
        //{
        //    if (n == 0) return 1;
        //    return n * Factorial(n - 1);
        //}

        static void Main(string[] args)
        {
            //Console.WriteLine(Factorial(10));
            //Console.WriteLine(Factorial(1000));
            //d01();
            //d02();
            //d03();
            //d04();
            //d05();
            //d06();
            //d07();
            //d08();
            //d09();
            //d10();
            //d11();
            //d12();
            //d13();
            //d14();
            //  d15();
            //  d16();
            // d17();
            // d18();
            // d19();
            // d20();
            // d21_deterministic();
            //   d21_Dirac_part2();
            //    d22_Boot_Reactor();

           Amphipod_Burrow2.RunBothParts();
  
            // d24_LicenceKey();
            //  d25();
            // 
        }

        #region older

        static void d24_LicenceKey()
        {
            string[] tests =
            {
              "11911316821816",
              "49917929934999"
            };

            Validator v = new Validator();
            foreach (string test in tests)
            {
                BigInteger z = v.Validate(test);
                string outcome = z == 0 ? "PASS" : "FAIL";
                Console.WriteLine($"{test} {outcome}   {z}");
            }
        }


        static void d25()
        {
            const string livedata = "D:\\softworks\\AdventOfCode\\Data2021\\d25_live.txt";
            const string testdata = "D:\\softworks\\AdventOfCode\\Data2021\\d25_test.txt";

            CucumberSystem cs;
            DateTime t0, t1;
            t0 = DateTime.Now;
            string outcome;
            long iterations;
            long expected;
            t0 = DateTime.Now;
            cs = new CucumberSystem(File.ReadAllLines(testdata));
            iterations = cs.Run();
            t1 = DateTime.Now;
            expected = 58;
            outcome = (iterations == expected) ? "PASS" : "FAIL"!;
            Console.WriteLine($"{outcome} Test  Steps={iterations} (error={iterations - expected})");
            Console.WriteLine($"{ (t1 - t0).TotalSeconds} secs");


            t0 = DateTime.Now;
            cs = new CucumberSystem(File.ReadAllLines(livedata));
            iterations = cs.Run();
            t1 = DateTime.Now;
            expected = 367;
            outcome = (iterations == expected) ? "PASS" : "FAIL"!;
            Console.WriteLine($"{outcome} Live  Steps={iterations} (error={iterations - expected})");
            Console.WriteLine($"{ (t1 - t0).TotalSeconds} secs");
        }


  



        static void d21_Dirac_part2()
        {
            //    Dirac game = new Dirac(4, 8); // test case
            DateTime t0, t1;
            t0 = DateTime.Now;
            string outcome;
            //Dirac game = new Dirac(4, 8); // test case
            Dirac game = new Dirac(4, 8); // live case
            game.Run();
            var (P1Wins, P2Wins) = game.CountWinners();
            t1 = DateTime.Now;
            BigInteger expectedP1 = BigInteger.Parse("444356092776315");
            BigInteger expectedP2 = BigInteger.Parse("341960390180808");
            outcome = (P1Wins == expectedP1 && P2Wins == expectedP2) ? "PASS" : "FAIL"!;
            Console.WriteLine($"{outcome} Test (4,8)  P1 wins={P1Wins} (error={P1Wins - expectedP1})  P2Wins={P2Wins} (error={P2Wins - expectedP2})");
            Console.WriteLine($"{ (t1 - t0).TotalSeconds} secs");
            Console.WriteLine($"{expectedP1 + expectedP2} expected.");
            Console.WriteLine($"{P1Wins + P2Wins} wins in total.");
            Console.WriteLine();

            //492043106122795
            game = new Dirac(9, 6); // live case
            game.Run();
            (P1Wins, P2Wins) = game.CountWinners();
            expectedP1 = BigInteger.Parse("492043106122795");
            expectedP2 = BigInteger.Parse("267086464416104");
            outcome = (P1Wins == expectedP1 && P2Wins == expectedP2) ? "PASS" : "FAIL"!;
            Console.WriteLine($"{outcome} Test (4,8)  P1 wins={P1Wins} (error={P1Wins - expectedP1})  P2Wins={P2Wins} (error={P2Wins - expectedP2})");
            Console.WriteLine($"{ (t1 - t0).TotalSeconds} secs");
            Console.WriteLine($"{expectedP1 + expectedP2} expected.");
            Console.WriteLine($"{P1Wins + P2Wins} wins in total.");
            Console.WriteLine();

        }

    
        static void d22_Boot_Reactor()
        {
            const string livedata = "D:\\softworks\\AdventOfCode\\Data2021\\d22_live2.txt";
            const string testdata = "D:\\softworks\\AdventOfCode\\Data2021\\d22_test.txt";
            const string tinydata = "D:\\softworks\\AdventOfCode\\Data2021\\d22_tiny3.txt";
            const string testdata2 = "D:\\softworks\\AdventOfCode\\Data2021\\d22_test2.txt";

            Reactor rx;
            BigInteger answer;
            BigInteger expected = BigInteger.Zero;
            bool clip = false;
            string description = "";

            string filename = "";
            foreach (int k in new int[]{ 0, 1, 2, 3, 4 })
            {
                switch (k)
                {
                    case 0:
                        description = "Clipped test data, part 1";
                        filename = testdata;
                        expected = BigInteger.Parse("590784");
                        clip = true;
                        break;
                    case 1:
                        description = "Clipped live data, part 1";
                        filename = livedata;
                        expected = BigInteger.Parse("598616");
                        clip = true;
                        break;
                    case 2:
                        description = "Clipped test2 data, part 2";
                        filename = testdata2;
                        expected = BigInteger.Parse("474140");
                        clip = true;
                        break;
                    case 3:
                        description = "Unclipped test2 data, part 2";
                        filename = testdata2;
                        expected = BigInteger.Parse("2758514936282235");
                        clip = false;
                        break;

                    case 4:
                        description = "Unclipped live data, part 2";
                        filename = livedata;
                        expected = BigInteger.Parse("1193043154475246");
                        clip = false;
                        break;
                }

                rx = new Reactor(File.ReadAllLines(filename), clip);
                answer = rx.DoTheWork();
                string outcome = answer == expected ? "PASS" : "FAIL"!;
                Console.WriteLine($"{outcome}   {description}  Answer = {answer}  (error {answer-expected}) \n");
            }
        }



        static void d21_deterministic()
        {
            //    Dirac game = new DiracD(4, 8); // test case
            DiracD game = new DiracD(9, 6); // live case

            int winner = game.Run();
            int loser = (winner + 1) % 2;
            long loserscore = game.Score[loser];
            long score = loserscore * game.DieRolls;
            Console.WriteLine($"Winner={winner} loserscore={loserscore} dieRolls={game.DieRolls}  answer={score} (expect 739785 test, 1004670 live)");
        }


            static void d19()
        {
            const string livedata = "D:\\softworks\\AdventOfCode\\Data2021\\d19_live.txt";
            const string testdata = "D:\\softworks\\AdventOfCode\\Data2021\\d19_test.txt";

     

            BeaconMap bm;

            DateTime t0 = DateTime.Now;
            bm = new BeaconMap(File.ReadAllLines(livedata));
      
            DateTime t1 = DateTime.Now;
            Console.WriteLine($"Edges = {bm.Edges.Count}, Unique={bm.UniqueBeacons.Count} (expect test79 / live=320)   et={(t1 - t0).TotalSeconds} secs");

            long md = bm.GetMaxManhattanDistance();
            Console.WriteLine($"Manhattan max dist is {md}  (expect test=3621 live=???)");

            //s0 = bm.Scanners[0];
            //s1 = bm.Scanners[1];

            // self = s0.FindMatches(s1, 2);
            //List<BeaconPt> pts = s1.TransformAll(s1.MyTransforms[0]);
            //BeaconPt.ShowPoints("S1 beacons in S0 system", pts);

            //s0 = bm.Scanners[1];
            //s1 = bm.Scanners[4];
            //for (int tf = 0; tf < 24; tf++)
            //{
            //    self = s0.FindMatches(s1, tf);
            //    if (self.Count >= 12) break;
            //}




            //for (int tNum = 0; tNum < 1; tNum++)
            //{
            //    List<Match> other = s0.FindMatches(s1, tNum);
            //    Console.WriteLine($"tnum {tNum} yields {other.Count} matches.");
            //}

            //      Console.WriteLine(sn);
            //         Utils.validate("case 1", new long[] { 79, sn.FindBeacons() });
        }




        static void d20()
        {
            const string livedata = "D:\\softworks\\AdventOfCode\\Data2021\\d20_live.txt";
            const string testdata = "D:\\softworks\\AdventOfCode\\Data2021\\d20_test.txt";

            ImageEnhancer ie = new ImageEnhancer(File.ReadAllLines(testdata));

            ie.Enhance();
            ie.Enhance();
            long lit1 = ie.litPixels();

            for (int c = 2; c < 50; c++)
            {
                ie.Enhance();
            }
            long lit2 = ie.litPixels();
            Console.WriteLine($"Test lit pixels = {lit1}  (expect 35)      {lit2} (expect 3351)");

            ie = new ImageEnhancer(File.ReadAllLines(livedata));
            ie.Enhance();
            ie.Enhance();
            lit1 = ie.litPixels();

            for (int c = 2; c < 50; c++)
            {
                ie.Enhance();
            }
            lit2 = ie.litPixels();

            Console.WriteLine($"Live lit pixels = {lit1}  (expect 5400)    {lit2} (expect 18989)  ");
        }


        static void d18()
        {
            const string livedata = "D:\\softworks\\AdventOfCode\\Data2021\\d18_live.txt";
            const string testdata = "D:\\softworks\\AdventOfCode\\Data2021\\d18_test.txt";

            SnailNum sn;
            sn = SnailNum.FromString("[9,1]");
            Console.WriteLine(sn);
            Utils.validate("case 1", new long[] { 29, sn.Magnitude() });
            sn = SnailNum.FromString("[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]");
            Console.WriteLine(sn);
            Utils.validate("case 1", new long[] { 3488, sn.Magnitude() });

            //sn = SnailNum.FromString("[[[[[9,8],1],2],3],4]");
            //Console.WriteLine(sn);
            //bool exploded = sn.TryExplode();
            //Console.WriteLine($"exploded={exploded} and sn is now={sn}");


            //sn = SnailNum.FromString("[[3,[2,[1,[7,3]]]],[6,[5,[4,[3, 2]]]]]");
            //Console.WriteLine(sn);
            //do {
            //    exploded = sn.TryExplode();
            //}
            //while (exploded);
            //           Console.WriteLine($"exploded={exploded} and sn is now={sn}");

            sn = SnailNum.FromString("[[[[4,3],4],4],[7,[[8,4],9]]]");
            var sn2 = SnailNum.FromString("[1,1]");

            var sn3 = sn.Add(sn2);
            Console.WriteLine($"Added {sn} + {sn3} to get {sn3}");
            sn3.Reduce();
            Console.WriteLine($"To get result {sn3}   with magnitude {sn3.Magnitude()}");



            string[] lines = File.ReadAllLines(livedata);
            List<SnailNum> nums = new List<SnailNum>();
            foreach (string line in lines)
            {
                nums.Add(SnailNum.FromString(line));
            }
            SnailNum result = nums[0];
            for (int i = 1; i < nums.Count; i++)
            {
                result = result.Add(nums[i]);
                result.Reduce();
            }
            Console.WriteLine($"Test data result = {result}   with magnitude {result.Magnitude()}   (expect 3699)");


            lines = File.ReadAllLines(livedata);
            long bigPairwiseMag;
            bigPairwiseMag = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < lines.Length; j++)
                {
                    if (i != j)
                    {
                        var num1 = SnailNum.FromString(lines[i]);
                        var num2 = SnailNum.FromString(lines[j]);
                        var newNum = num1.Add(num2);
                        newNum.Reduce();
                        long thisMag = newNum.Magnitude();
                        if (thisMag > bigPairwiseMag)
                        {
                            bigPairwiseMag = thisMag;
                        }
                    }
                }
            }
            Console.WriteLine($"Biggest pairwise mag = {bigPairwiseMag} (epect 4735)");
        }

        static void d17()
        {
         
            const string livedata = "25,67, -200, -260";
            Probe p = new Probe(20, 30, -5, -10);
            long maxY = p.GetMaxY();
            Utils.validate("Test1", new long[] { 45, maxY, 112, p.Hits });

            p = new Probe(25, 67, -200, -260);
            maxY = p.GetMaxY();
            Utils.validate("Live1", new long[] { 33670, maxY, 4903, p.Hits });
        }

        static void d16()
        {
            const string livedata = "D:\\softworks\\AdventOfCode\\Data2021\\d16_live.txt";
            const string test1 = "8A004A801A8002F478"; // represents an operator packet(version 4) which contains an operator packet(version 1) which contains an operator packet(version 5) which contains a literal value(version 6); this packet has a version sum of 16.
            const string test2 = "620080001611562C8802118E34";// represents an operator packet(version 3) which contains two sub-packets; each sub-packet is an operator packet that contains two literal values. This packet has a version sum of 12.
            const string test3 = "C0015000016115A2E0802F182340"; // has the same structure as the previous example, but the outermost packet uses a different length type ID.This packet has a version sum of 23.
            const string test4 = "A0016C880162017C3686B18A3D4780"; // is an operator packet that contains an operator packet that contains an operator packet that contains five literal values; it has a version sum of 31.

            PacketTokenizer pt = new PacketTokenizer("D2FE28");
            Packet p = pt.getPacket();
            long[] expected = { 2021, p.Val };
            Utils.validate("Sample1", expected);

            pt = new PacketTokenizer("38006F45291200");
            p = pt.getPacket();
            expected = new long[] { 10, p.Children[0].Val, 20, p.Children[1].Val };
            Utils.validate("Sample2", expected);

            pt = new PacketTokenizer("EE00D40C823060");
            p = pt.getPacket();
            expected = new long[] { 1, p.Children[0].Val, 2, p.Children[1].Val,3,p.Children[2].Val };
            Utils.validate("Sample3", expected);

            runBinParser("test1", test1, 16);
            runBinParser("test2", test2, 12);
            runBinParser("test3", test3, 23);
            runBinParser("test4", test4, 31);
            runBinParser("Live part 1", File.ReadAllText(livedata), 986);

            runEval("sample1", "C200B40A82", 3);

            runEval("sample2", "04005AC33890", 54);
            runEval("sample3", "880086C3E88112", 7);
            runEval("sample4", "CE00C43D881120", 9);  // finds the maximum of 7, 8, and 9, resulting in the value 9.
            runEval("sample5", "D8005AC2A8F0", 1); // produces 1, because 5 is less than 15.
            runEval("sample6", "F600BC2D8F", 0); // produces 0, because 5 is not greater than 15.
            runEval("sample1", "9C005AC2F8F0", 0);//  produces 0, because 5 is not equal to 15.
            runEval("sample1", "9C0141080250320F1802104A08", 1);//  produces 1, because 1 + 3 = 2 * 2
            runEval("live part 2", File.ReadAllText(livedata), 18234816469452); 
        }

        static void runEval(string description, string data, long expected)
        {
            DateTime t0 = DateTime.Now;
            PacketTokenizer pp = new PacketTokenizer(data);
            Packet p = pp.getPacket();

            long part1 = p.eval();
            DateTime t1 = DateTime.Now;
            Utils.validate(description, new long[] { expected, part1 });
        }

        static void runBinParser(string description, string data, long expected)
        {
  
            DateTime t0 = DateTime.Now;
            PacketTokenizer pp = new PacketTokenizer(data);
            Packet p = pp.getPacket();

            long part1 = p.sumAllVersions();
            DateTime t1 = DateTime.Now;
            Utils.validate(description, new long[] { expected, part1 });
          //  Console.WriteLine($"\nDijkstra Elapsed = {(t1 - t0).TotalSeconds} secs.  HighTide = {pp.HighTide}");
        }

        static void d15()
        {
            const string livedata = "D:\\softworks\\AdventOfCode\\Data2021\\d15_live.txt";
            const string testdata = "D:\\softworks\\AdventOfCode\\Data2021\\d15_test.txt";

         //   RunChiton(testdata, false, 40);
            RunDijk(testdata, false, 40);
         //   RunChiton(livedata, false, 458);
            RunDijk(livedata, false, 458);
        //    RunChiton(testdata, true, 315);
            RunDijk(testdata, true, 315);
            RunDijk(livedata, true, 2800);
      //      RunChiton(livedata, true, 2800);
        }

        static void RunDijk(string datafile, bool withExtensions, long expected)
        {
            DateTime t0, t1;
            Dijkstra pp;
            t0 = DateTime.Now;
            pp = new Dijkstra(File.ReadAllLines(datafile), withExtensions);
            long part1 = pp.FindShortestPath();
            t1 = DateTime.Now;
            Utils.validate(datafile, new long[] { expected, part1 });
            Console.WriteLine($"Dijkstra Elapsed = {(t1 - t0).TotalSeconds} secs.  HighTide = {pp.HighTide}");
        }


        static void RunChiton(string datafile, bool withExtensions, long expected )
        {
            DateTime t0, t1;
            ChitonMap pp;
            t0 = DateTime.Now;
            pp = new ChitonMap(File.ReadAllLines(datafile), withExtensions);
            long part1 = pp.FindShortestPath();
            t1 = DateTime.Now;
            Utils.validate(datafile, new long[] { expected, part1 });
            Console.WriteLine($"Chiton Elapsed = {(t1 - t0).TotalSeconds} secs.  HighTide = {pp.HighTide}");
        }

        static void d14()
        {
            const string livedata = "D:\\softworks\\AdventOfCode\\Data2021\\d14_live.txt";
            const string testdata = "D:\\softworks\\AdventOfCode\\Data2021\\d14_test.txt";

            Poly pp = new Poly(File.ReadAllLines(testdata));
            long part1_test = pp.DoSubs(10);
          
            pp=new Poly(File.ReadAllLines(livedata));
            long part1 = pp.DoSubs(10);

            pp = new Poly(File.ReadAllLines(testdata));
            long part2_test = pp.DoSubs(40);

            pp = new Poly(File.ReadAllLines(livedata));
            long part2 = pp.DoSubs(40);

            long[] expected = new long[] { 1588, part1_test, 2003, part1, 2188189693529, part2_test, 2276644000111, part2 };

            Utils.validate("Day14", expected);
        }

        static void d13()
        {
            const string livedata = "D:\\softworks\\AdventOfCode\\Data2021\\d13_live.txt";
            const string testdata = "D:\\softworks\\AdventOfCode\\Data2021\\d13_test.txt";

            FoldingPaper fp = new FoldingPaper(File.ReadAllLines(testdata));
            fp.DoFirstFold();
            long part1_test = fp.points.Count;

            fp = new FoldingPaper(File.ReadAllLines(livedata));
            fp.DoAllFolds();
            long part1 = 0;//  fp.points.Count;


            //cs = new CaveSystem(File.ReadAllLines(livedata));
            //long part2 = cs.NumPaths();

            //  long[] expected = new long[] { 226, part1_test, 4549, part1 }; //, 195, part2_test, 268, part2 };
            long[] expected = new long[] { 17, part1_test, 4549, part1 }; //, 195, part2_test, 268, part2 };

            Utils.validate("Day13", expected);
        }

        static void d12()
        {
            const string livedata = "D:\\softworks\\AdventOfCode\\Data2021\\d12_live.txt";
            const string testdata = "D:\\softworks\\AdventOfCode\\Data2021\\d12_test1.txt";

            CaveSystem cs = new CaveSystem(File.ReadAllLines(testdata));
            long part2_test = cs.NumPaths();

            cs = new CaveSystem(File.ReadAllLines(livedata));
            long part2 = cs.NumPaths();

            //  long[] expected = new long[] { 226, part1_test, 4549, part1 }; //, 195, part2_test, 268, part2 };
            long[] expected = new long[] { 36, part2_test, 4549, part2 }; //, 195, part2_test, 268, part2 };

            Utils.validate("Day12", expected);
        }

        static void d11()
        {
            const string livedata = "D:\\softworks\\AdventOfCode\\Data2021\\d11_live.txt";
            const string testdata = "D:\\softworks\\AdventOfCode\\Data2021\\d11_test.txt";

            DumboGrid fMap = new DumboGrid(File.ReadAllLines(testdata));
            long part1_test = fMap.SumOfFlashes();

            fMap = new DumboGrid(File.ReadAllLines(livedata));
            long part1 = fMap.SumOfFlashes();

            fMap = new DumboGrid(File.ReadAllLines(testdata));
            long part2_test = fMap.SimultaneousFlashes();

            fMap = new DumboGrid(File.ReadAllLines(livedata));
            long part2 = fMap.SimultaneousFlashes();

            long[] expected = new long[] { 1656, part1_test, 1562, part1, 195, part2_test, 268, part2 };
            Utils.validate("Day11", expected);
        }

        static void d10()
        {
            const string livedata = "D:\\softworks\\AdventOfCode\\Data2021\\d10_live.txt";
            const string testdata = "D:\\softworks\\AdventOfCode\\Data2021\\d10_test.txt";

            SyntaxParser fMap = new SyntaxParser(File.ReadAllLines(testdata));
            long part1_test = fMap.SumOfSyntaxErrors();

            long part2_test = fMap.AutoCompleteMedian();

            fMap = new SyntaxParser(File.ReadAllLines(livedata));
            long part1 = fMap.SumOfSyntaxErrors();

           long part2 = fMap.AutoCompleteMedian();


            long[] expected = new long[] { 26397, part1_test, 296535, part1, 288957, part2_test, 4245130838, part2 };
            Utils.validate("Day10", expected);
        }

        static void d09()
        {
            const string livedata = "D:\\softworks\\AdventOfCode\\Data2021\\d09_live.txt";
            const string testdata = "D:\\softworks\\AdventOfCode\\Data2021\\d09_test.txt";

            FloorMap fMap = new FloorMap(File.ReadAllLines(testdata));
            long part1_test = fMap.SumRisks();
            long part2_test = fMap.FindAllBasins();

            fMap = new FloorMap(File.ReadAllLines(livedata));
            long part1 = fMap.SumRisks();
            long part2 = fMap.FindAllBasins();

            long[] expected = new long[] { 15, part1_test, 577, part1, 1134, part2_test, 1069200, part2 };
            Utils.validate("Day09", expected);
        }



        static void d08()
        {
           string []oneTest = {"acedgfb cdfbe gcdfa fbcad dab cefabd cdfgeb eafb cagedb ab |  cdfeb fcadb cdfeb cdbaf"} ;

            const string livedata = "D:\\softworks\\AdventOfCode\\Data2021\\d08_live.txt";
            const string testdata = "D:\\softworks\\AdventOfCode\\Data2021\\d08_test.txt";

            SegmentEvidence theEvidence = new SegmentEvidence(File.ReadAllLines(testdata));
            long part1_test = theEvidence.CountEasyCases();

            theEvidence = new SegmentEvidence(File.ReadAllLines(livedata));
            long part1 = theEvidence.CountEasyCases();

            // PART 2
            theEvidence = new SegmentEvidence(oneTest);

            long answer = theEvidence.sumAllOutputs();

            theEvidence = new SegmentEvidence(File.ReadAllLines(testdata));
            long part2_test = theEvidence.sumAllOutputs();

            theEvidence = new SegmentEvidence(File.ReadAllLines(livedata));
            long part2 = theEvidence.sumAllOutputs();

           // long[] expected = new long[] {26, part1_test, 493, part1,  5353, answer, 61229, part2_test, 1010460, part2 };
            long[] expected = new long[] { 26, part1_test, 493, part1, 61229, part2_test, 1010460, part2 };
            Utils.validate("Day08", expected);
        }

        static void d07()
        {
            const string liveFilename = "D:\\softworks\\AdventOfCode\\Data2021\\d07_live.txt";
            const string testdata = "16,1,2,0,4,2,7,1,2,14";

            CrabFleet fleet = new CrabFleet(testdata);
            int ans071_test = fleet.CountEasyCases();

            fleet = new CrabFleet(File.ReadAllText(liveFilename));
            int ans071 = fleet.CountEasyCases();

            fleet = new CrabFleet(testdata);
            long ans072_test = fleet.FindLeastCostAlignmentV2();

            fleet = new CrabFleet(File.ReadAllText(liveFilename));
            long ans072 = fleet.FindLeastCostAlignmentV2();

            long[] expected = new long[] { 37, ans071_test, 342730, ans071, 168, ans072_test, 92335207, ans072 };
            Utils.validate("Day07", expected);
        }

        static void d06()
        {
            const string livedata = "3,5,2,5,4,3,2,2,3,5,2,3,2,2,2,2,3,5,3,5,5,2,2,3,4,2,3,5,5,3,3,5,2,4,5,4,3,5,3,2,5,4,1,1,1,5,1,4,1,4,3,5,2,3,2,2,2,5,2,1,2,2,2,2,3,4,5,2,5,4,1,3,1,5,5,5,3,5,3,1,5,4,2,5,3,3,5,5,5,3,2,2,1,1,3,2,1,2,2,4,3,4,1,3,4,1,2,2,4,1,3,1,4,3,3,1,2,3,1,3,4,1,1,2,5,1,2,1,2,4,1,3,2,1,1,2,4,3,5,1,3,2,1,3,2,3,4,5,5,4,1,3,4,1,2,3,5,2,3,5,2,1,1,5,5,4,4,4,5,3,3,2,5,4,4,1,5,1,5,5,5,2,2,1,2,4,5,1,2,1,4,5,4,2,4,3,2,5,2,2,1,4,3,5,4,2,1,1,5,1,4,5,1,2,5,5,1,4,1,1,4,5,2,5,3,1,4,5,2,1,3,1,3,3,5,5,1,4,1,3,2,2,3,5,4,3,2,5,1,1,1,2,2,5,3,4,2,1,3,2,5,3,2,2,3,5,2,1,4,5,4,4,5,5,3,3,5,4,5,5,4,3,5,3,5,3,1,3,2,2,1,4,4,5,2,2,4,2,1,4";
            const string testdata = "3,4,3,1,2";

            LanternShoal theShoal = new LanternShoal(testdata);
            for (int i = 0; i < 80; i++)
            {
                theShoal.AgeByOneDay();
            }
            long ans061_test = theShoal.NumberOfFish();

            theShoal = new LanternShoal(livedata);
            for (int i = 0; i < 80; i++)
            {
                theShoal.AgeByOneDay();
            }
            long ans061 = theShoal.NumberOfFish();

            theShoal = new LanternShoal(testdata);
            for (int i = 0; i < 256; i++)
            {
                theShoal.AgeByOneDay();
            }
            long ans062_test = theShoal.NumberOfFish();

            theShoal = new LanternShoal(livedata);
            for (int i = 0; i < 256; i++)
            {
                theShoal.AgeByOneDay();
            }
            long ans062 = theShoal.NumberOfFish();

        //    Utils.validate("Day06", 5934, ans061_test, 343441, ans061, 26984457539, ans062_test, 1569108373832, ans062);

            long[] expected = new long[] { 5934, ans061_test, 343441, ans061, 26984457539, ans062_test, 1569108373832, ans062 };
            Utils.validate("Day06", expected);
        }

        static void d05()
        {
            const string liveFilename = "D:\\softworks\\AdventOfCode\\Data2021\\d05_live.txt";
            const string testFilename = "D:\\softworks\\AdventOfCode\\Data2021\\d05_test.txt";

            VentAnalyzer theAnalyzer = new VentAnalyzer(testFilename);
            theAnalyzer.buildCoverageMap(false);
            int ans051_test = theAnalyzer.CountMultiCoveredPoints();

            theAnalyzer = new VentAnalyzer(liveFilename);
            theAnalyzer.buildCoverageMap(false);
            int ans051 = theAnalyzer.CountMultiCoveredPoints();

            theAnalyzer = new VentAnalyzer(testFilename);
            theAnalyzer.buildCoverageMap(true);
            int ans052_test = theAnalyzer.CountMultiCoveredPoints();

            theAnalyzer = new VentAnalyzer(liveFilename);
            theAnalyzer.buildCoverageMap(true);
            int ans052 = theAnalyzer.CountMultiCoveredPoints();

            Utils.validate("Day05", 5, ans051_test, 6572, ans051, 12, ans052_test, 21466, ans052);
        }

        static void d04()
        {
            const string liveFilename = "D:\\softworks\\AdventOfCode\\Data2021\\d041.txt";
            const string testFilename = "D:\\softworks\\AdventOfCode\\Data2021\\d041_test.txt";

            BingoGame bg = new BingoGame(testFilename);
            int ans041_test = bg.PlayToFirstWinner();

            BingoGame bg2 = new BingoGame(liveFilename);
            int ans041 = bg2.PlayToFirstWinner();

            BingoGame bg3 = new BingoGame(testFilename);
            int ans042_test = bg3.PlayToLastWinner();

            BingoGame bg4 = new BingoGame(liveFilename);
            int ans042 = bg4.PlayToLastWinner();

            Utils.validate("Day04", 4512, ans041_test, 33348, ans041, 1924, ans042_test, 8112, ans042);
        }


        public static void d01()
        {
            const string inputFilename = "D:\\softworks\\AdventOfCode\\Data2021\\d011.txt";
            const string testCase = "199 200 208 210 200 207 240 269 260 263";
            int t1 = Utils.countDeeper(Utils.ParseInts(testCase));
            int ans011 = Utils.countDeeper(Utils.ParseInts(File.ReadAllText(inputFilename)));
            int t2 = Utils.countDeeper(Utils.slidingSums(Utils.ParseInts(testCase)));
            int ans012 = Utils.countDeeper(Utils.slidingSums(Utils.ParseInts(File.ReadAllText(inputFilename))));
            Utils.validate("Day01", 7, t1, 1529, ans011, 5, t2, 1567, ans012);
        }


        public static void d02()
        {
            const string inputFilename = "D:\\softworks\\AdventOfCode\\Data2021\\d021.txt";
            const string testCase = "forward 5 down 5 forward 8 up 3 down 8 forward 2";

            Submarine sub1 = new Submarine();
            sub1.FollowDirections_V1(testCase);
            int t1 = sub1.Y * sub1.Z;

            sub1.Reset();
            sub1.FollowDirections_V1(File.ReadAllText(inputFilename));
            int ans021 = sub1.Y * sub1.Z;

            sub1.Reset();
            sub1.FollowDirections_V2(testCase);
            int t2 = sub1.Y * sub1.Z;

            sub1.Reset();
            sub1.FollowDirections_V2(File.ReadAllText(inputFilename));
            int ans022 = sub1.Y * sub1.Z;
            Utils.validate("Day02", 150, t1, 1524750, ans021, 900, t2, 1592426537, ans022);
        }

        public static void d03()
        {
            const string inputFilename = "D:\\softworks\\AdventOfCode\\Data2021\\d031.txt";
            const string testCase = "00100 11110 10110 10111 10101 01111 00111 11100 10000 11001 00010 01010";

            Submarine sub1 = new Submarine();
            Tuple<int, int> diag = sub1.ParseDiagnostics(testCase);
            int t0 = diag.Item1 * diag.Item2;

            sub1.Reset();
            diag = sub1.ParseDiagnostics(File.ReadAllText(inputFilename));
            int ans031 = diag.Item1 * diag.Item2;

            int t1 = sub1.OGR(testCase) * sub1.CO2SR(testCase);
     
            sub1.Reset();

            string liveCase = File.ReadAllText(inputFilename);
            int ans = sub1.OGR(liveCase) * sub1.CO2SR(liveCase);

            Utils.validate("Day03", 198, t0, 775304, ans031, 230, t1, 1370737, ans);
        }
        #endregion
    }
}
