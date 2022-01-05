using System;
using System.Collections.Generic;
using System.Linq;

namespace Y2021
{
    public class LanternShoal
    {
        // We don't track individual fist. We keep bins of how many fish there are 
        // with TTS counts (time-to-spawn) of 0 days, 1 day, etc.
        public List<long> TTS_bins = new List<long>() { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public LanternShoal(string startingConfig)
        {
            string[] parts = startingConfig.Split(',');
            var ages = parts.Select(int.Parse);
            foreach(int age in ages)
            {
                TTS_bins[age]++;   // Count number of fish of each age till next spawn
            }
  //          showState();
        }

        public void AgeByOneDay()
        {
            long spawners = TTS_bins[0];
            TTS_bins.RemoveAt(0);     // age everyone else by one less day to spawn
            TTS_bins[6] += spawners;  // Recycle the spawners
            TTS_bins.Add(spawners);   // And add their offspring
 //         showState();
        }

        public void showState()
        {
            for (int i=0; i < TTS_bins.Count; i++)
            {
                Console.Write($"{TTS_bins[i]},"); 
            }
            Console.WriteLine($"  Fish={TTS_bins.Sum()}");
        }

        public long NumberOfFish()
        {
            return TTS_bins.Sum();
        }
    }
}
