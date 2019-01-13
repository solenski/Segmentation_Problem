using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Segmentacja
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbScan = new DbScan(1.5, 2);
            var clusters = dbScan.Cluster(Data.SegmentationData).Where(x => x.ClusterPoints != null || x.ClusterPoints.Count > 0).ToList();
            for (int i = 0; i < clusters.Count; i++)
            {
                Console.WriteLine($"Cluster {i}");
                Console.WriteLine("Points");
                foreach (var point in clusters[i].ClusterPoints)
                {
                    Console.WriteLine($"[  {string.Join(",", point.Coordinates.Select(x => x.ToString()).ToList())} ] ");
                }
            }

            Console.WriteLine($"Stats: Core {dbScan.stats.CorePoints}, Border {dbScan.stats.BorderPoints}, Noise {dbScan.stats.NoisePoints} ");

            Console.ReadLine();
        }
    }
}
