using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Segmentacja
{
    public enum PointType
    {
        Core, // at least minpoints in its neighborhood
        Border, // at least one Core point in its neighborhood
        Noise // nothing else
    }

    public class DbScanPoint
    {
        public List<double> Coordinates { get; set; }
        public PointType? PointType { get; set; }
        public bool IsVisited { get; set; }
    }

    public class Cluster
    {
        public List<DbScanPoint> ClusterPoints;
    }

    public class DbScan
    {
        public DbScan(double radius, int minPoints)
        {
            Radius = radius;
            MinPoints = minPoints;
        }

        private Random rand = new Random();

        public double Radius { get; }
        public int MinPoints { get; }

        public Stats stats;

        public IEnumerable<Cluster> Cluster(List<List<double>> points)
        {
            this.stats = new Stats();

            //phase one visit all the points and categorize them to Border, Core, Noise
            var dbScanPoints = points.Select(x => new DbScanPoint { IsVisited = false, PointType = null, Coordinates = x }).ToList();
            foreach (var dbScanPoint in dbScanPoints)
            {
                Visit(dbScanPoints, dbScanPoint);
            }
            // phase two use DepthFirstSearch to Assign clusters
            var bordersAndCores = dbScanPoints.Where(x => x.PointType == PointType.Core || x.PointType == PointType.Border).ToList();
            bordersAndCores.ForEach(x => x.IsVisited = false);

            var clusters = new List<Cluster>();
            foreach (var dbScantPoint in bordersAndCores.Where(x=> x.PointType == PointType.Core))
            {
                this.curr = new Cluster
                {
                    ClusterPoints = new List<DbScanPoint>()
                };
                DepthFirst(dbScantPoint, bordersAndCores);

                if (this.curr.ClusterPoints.Count > 0)
                {
                    clusters.Add(new Cluster() { ClusterPoints = new List<DbScanPoint>(this.curr.ClusterPoints) });
                }
            }

            return clusters;
        }

        private Cluster curr;

        private void DepthFirst(DbScanPoint dbScantPoint, List<DbScanPoint> bordersAndCores)
        {
            if (!dbScantPoint.IsVisited)
            {

                dbScantPoint.IsVisited = true;
                var neighbors = Neigborhood(dbScantPoint, bordersAndCores);
                
            

                foreach (var neighbor in neighbors)
                {
                    DepthFirst(neighbor, bordersAndCores);

                    this.curr.ClusterPoints.Add(neighbor);
                }
            }
            else
            {
                return;
            }
        }

        private void Visit(List<DbScanPoint> dbScanPoints, DbScanPoint dbScanPoint)
        {
            dbScanPoint.IsVisited = true;
            var nValue = Neigborhood(dbScanPoint, dbScanPoints);
            if (nValue.Count() > MinPoints)
            {
                this.stats.CorePoints++;
                dbScanPoint.PointType = PointType.Core;
            }
            if (nValue.Count() == 2)
            {
                var theOtherPoint = nValue.First(x => x != dbScanPoint);
                if (theOtherPoint.IsVisited && theOtherPoint.PointType == PointType.Core)
                {
                    this.stats.BorderPoints++;

                    dbScanPoint.PointType = PointType.Border;
                }
                else if (!theOtherPoint.IsVisited)
                {
                    Visit(dbScanPoints, theOtherPoint);
                    if (theOtherPoint.PointType == PointType.Core)
                    {
                        this.stats.BorderPoints++;

                        dbScanPoint.PointType = PointType.Border;
                    }
                    else
                    {
                        this.stats.NoisePoints++;

                        dbScanPoint.PointType = PointType.Noise;
                    }
                }
            }
            if (nValue.Count() == 1)
            {
                dbScanPoint.PointType = PointType.Noise;
            }
        }

        private List<DbScanPoint> Neigborhood(DbScanPoint dbScanPoint, List<DbScanPoint> dbScanPoints)
        {
            var distances = dbScanPoints.Select(p => new { point = p, distance = Distance.Euclidean(dbScanPoint.Coordinates.ToArray(), p.Coordinates.ToArray()) });
            return distances.Where(x => x.distance < Radius).Select(x => x.point).ToList();
        }
    }
}

