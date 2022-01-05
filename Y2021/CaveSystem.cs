using System;
using System.Collections.Generic;


namespace Y2021
{
    public class CaveSystem
    {
        Dictionary<string, List<string>> edges;

        List<CavePath> paths;

        public CaveSystem(string[] lines)
        {
            edges = new Dictionary<string, List<string>>();

            foreach(string line in lines)
            {
                string[] parts = line.Split('-', StringSplitOptions.RemoveEmptyEntries);
                addEdges(parts[0].Trim(), parts[1].Trim());
            }
            Show();
        }

        private void addEdges(string a, string b)
        {
            addDirectedEdge(a, b);
            addDirectedEdge(b, a);
        }

        private void addDirectedEdge(string v1, string v2)
        {
            if (v1 == "end") return;
            if (v2 == "start") return;

            if (edges.ContainsKey(v1))
            {
                edges[v1].Add(v2);
            }
            else
            {
                edges.Add(v1, new List<string>() { v2 });
            }
        }

        public long NumPaths()
        {
            paths = new List<CavePath>();
            List<CavePath> pendingPaths = new List<CavePath>();

            CavePath cp = new CavePath();
            cp.Add("start");
            pendingPaths.Add(cp) ;

            int count = 0;
            while (pendingPaths.Count > 0)
            {
                count++;
                CavePath currPath = pendingPaths[pendingPaths.Count - 1];
                pendingPaths.RemoveAt(pendingPaths.Count - 1);
                string lastNode = currPath[currPath.Count - 1];
                if (lastNode == "end")
                {
                    paths.Add(currPath);
                //    currPath.Show();
                }
                else // generate children, put them in pending.
                {
                    List<string> children = edges[lastNode];
                    foreach (string child in children)
                    {
                        int situation = currPath.IsEligibleChild(child);
                        if (situation >= 0)  // allow the path to extend
                        {
                            // Clone a new path with an extension for this child
                            CavePath newP = currPath.Clone();
                            newP.Add(child);
                            if (situation == 0) // this is first small node re-visit
                            {
                                newP.smallNodeRevisited = true;
                            }
                            pendingPaths.Add(newP);
                        }
                    }
                }
            }

            return paths.Count;
        }

 
        public void Show()
        {
            foreach (string s in edges.Keys)
            {
                string rhs = String.Join(',', edges[s]);
                Console.WriteLine($"{s} -> {rhs}");
            }
        }
    }

    class CavePath : List<string>
    {
        public bool smallNodeRevisited;

        public CavePath(): base()
        {
            smallNodeRevisited = false;
        }
        internal CavePath Clone()
        {
            CavePath result = new CavePath();
            result.AddRange(this);
            result.smallNodeRevisited = this.smallNodeRevisited;
            return result;
        }

        public void Show()
        {
            string pth = string.Join(',', this);
            Console.WriteLine(pth);
        }

        internal int IsEligibleChild(string child)
        {
            if (char.IsLower(child[0]) && this.Contains(child))
            {
                if (smallNodeRevisited) return -1;  // no revisit allowd
                return 0;  // yes, but it uses up revisit on smallnode
            }  
            else
            {
                return 1; // yes
            }

        }
    }

}
