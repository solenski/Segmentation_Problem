using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Segmentacja
{
    public static class Data
    {
        static Data()
        {
            SegmentationData = File.ReadAllLines("segmentacja.txt").Select(x => x.Replace("[", "").Replace("]", "").Split(',').Select(y => Convert.ToDouble(y)).ToList()).ToList();
        }

        public static List<List<double>> SegmentationData { get; }
    }
}
