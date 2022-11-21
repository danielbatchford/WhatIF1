using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using WhatIfF1.Adapters;
using WhatIfF1.Modelling.Events;
using WhatIfF1.Modelling.Events.Drivers;
using WhatIfF1.Modelling.Tracks;
using WhatIfF1.Util.Enumerables;

namespace WhatIfF1.UI.Controller.TrackMaps
{
    public sealed class TrackMapProvider
    {
        private static readonly IDictionary<Track, PointCollection> _cachedTracks = new Dictionary<Track, PointCollection>();

        private static readonly Rect _boundingBox = new Rect(-1, -1, 2, 2);

        private static readonly Point _origin = new Point(_boundingBox.Left + _boundingBox.Width / 2, _boundingBox.Top + _boundingBox.Height / 2);

        public PointCollection TrackPoints { get; }

        public IDictionary<Driver, Point> DriverPoints { get; }

        public Point StartPoint { get; }

        public TrackMapProvider(Track track, IEnumerable<Driver> drivers)
        {
            // If the track has already been parsed, use the cached version.
            if (_cachedTracks.TryGetValue(track, out PointCollection trackPoints))
            {
                TrackPoints = trackPoints;
                return;
            }

            // Else, parse the track

            var lines = FileAdapter.ReadLines(track.TrackFilePath, true);

            int numPoints = lines.Count();

            // List to store unnormalised points from file
            var rawPoints = new List<Point>(numPoints);

            const char delimiter = ',';

            foreach (string line in lines)
            {
                // lines are in format "x,y"
                string[] split = line.Split(delimiter);

                if(split.Length != 2)
                {
                    throw new EventControllerException($"Failed to parse a track file as an invalid line was recieved ({line})");
                }

                double x = int.Parse(split[0]);
                double y = int.Parse(split[1]);

                rawPoints.Add(new Point(x, y));
            }

            // Find minX, minY and maxX, maxY, to allow for normalisation to the bounding box

            double minX = rawPoints.Min(point => point.X);
            double minY = rawPoints.Min(point => point.Y);
            double maxX = rawPoints.Max(point => point.X);
            double maxY = rawPoints.Max(point => point.Y);

            // precompute multiplicative scale factors between raw points and bounding box
            double scaleFactorX = _boundingBox.Width / (maxX - minX);
            double scaleFactorY = _boundingBox.Height / (maxY - minY);

            // Take scale factor as the smaller of the x and y factors, to preserve aspect ratio
            double scaleFactor = Math.Min(scaleFactorX, scaleFactorY);

            // create list of translated points
            var translatedPoints = new PointCollection(numPoints);

            foreach (Point rawPoint in rawPoints)
            {
                // Translate points from the source file to the bounding box positions
                double normX = (scaleFactor * (rawPoint.X - minX)) + _boundingBox.Left;
                double normY = (scaleFactor * (rawPoint.Y - minY)) + _boundingBox.Top;

                translatedPoints.Add(new Point(normX, normY));
            }

            const double rotationIncrement = 2 * Math.PI / 20;
            const int sampleStep = 10;
            var rotatedPoints = GetApproximateMaximalWidthCollection(translatedPoints, rotationIncrement, sampleStep);

            // Rotate track points to maximise width
            TrackPoints = new PointCollection(rotatedPoints);

            // Cache the parsed points for reuse
            _cachedTracks.Add(track, TrackPoints);

            StartPoint = TrackPoints[0];

            // Build driver points dictionary.
            // Initialise all drivers at the start line
            DriverPoints = new Dictionary<Driver, Point>(drivers.Count());

            foreach(Driver driver in drivers) 
            {
                //DriverPoints.Add(driver, StartPoint);
            }
        }

        public void UpdateDriverMapPosition(Driver driver, Position driverPos)
        {
            double propAlongLap = driverPos.LapDistance / driverPos.TrackLength;

            // Find the closest index in the track points list based on the distance around the lap
            int trackIndex = (int)Math.Round(propAlongLap * TrackPoints.Count, 0);

            DriverPoints[driver] = TrackPoints[trackIndex];
        }

        /// <summary>
        /// Rotate all points through 2pi radians using the provided increment. 
        /// Return the collection which yields the maximal largest horizontal disstance for the set of points.
        /// Returned collection is approximate and is not the exact optimal rotation.
        /// Only a subset of points are sampled, for optimisation.
        /// </summary>
        private IEnumerable<Point> GetApproximateMaximalWidthCollection(IList<Point> points, double radIncrement, int sampleStep)
        {
            double optimalAngle = 0;
            double bestHorizDist = 0;

            double dx;
            double dy;

            for(double testAngle = 0; testAngle < 2 * Math.PI; testAngle += radIncrement)
            {
                double maxHorizDist = 0;

                for (int i = 0; i < points.Count; i+=sampleStep)
                {
                    dx = points[i].X - _origin.X;
                    dy = points[i].Y - _origin.Y;

                    double angleToHoriz = Math.Atan2(dy, dx);
                    double distToOrigin = Math.Sqrt(dx * dx + dy * dy);

                    double horizDist = distToOrigin * Math.Abs(Math.Cos(testAngle + angleToHoriz));

                    if (horizDist > maxHorizDist)
                    {
                        maxHorizDist = horizDist;
                    }
                }

                if(maxHorizDist > bestHorizDist)
                {
                    bestHorizDist = maxHorizDist;
                    optimalAngle = testAngle;
                }
            }

            // No need to rotate points if the optimal angle is zero. Simply return original collection
            if(optimalAngle == 0)
            {
                return points;
            }

            // Rotate original points by the calculated optimal angle
            ICollection<Point> rotatedPoints = new List<Point>(points.Count);

            double sinTheta = Math.Sin(optimalAngle);
            double cosTheta = Math.Cos(optimalAngle);

            foreach (Point point in points)
            {
                dx = point.X - _origin.X;
                dy = point.Y - _origin.Y;

                double x = cosTheta * dx - sinTheta * dy + _origin.X;
                double y = sinTheta * dx + cosTheta * dy + _origin.Y;
                rotatedPoints.Add(new Point(x, y));
            }

            return rotatedPoints;
        }
    }
}
