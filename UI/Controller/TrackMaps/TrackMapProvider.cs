using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using WhatIfF1.Adapters;
using WhatIfF1.Modelling.Events.Drivers;
using WhatIfF1.Modelling.Tracks;
using WhatIfF1.Util;
using WhatIfF1.Util.Enumerables;

namespace WhatIfF1.UI.Controller.TrackMaps
{
    public sealed class TrackMapProvider : NotifyPropertyChangedWrapper
    {
        private static readonly IDictionary<Track, PointCollection> _cachedTracks = new Dictionary<Track, PointCollection>();

        private static readonly Rect _boundingBox = new Rect(0, 0, 1050, 600);

        private static readonly int _pointsReductionFactor = 15;

        public PointCollection TrackPoints { get; }

        public ObservableCollection<DriverMapPoint> DriverPoints { get; }

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

            // List to store unnormalised points from file
            var rawPoints = new List<Point>();

            const char delimiter = ',';

            int increment = lines.Count() / _pointsReductionFactor;
            for (int i = 0; i < lines.Count(); i += increment)
            {

            }

            foreach (string line in lines)
            {
                // lines are in format "x,y"
                string[] split = line.Split(delimiter);

                if (split.Length != 2)
                {
                    throw new EventControllerException($"Failed to parse a track file as an invalid line was recieved ({line})");
                }

                double x = int.Parse(split[0]);
                double y = int.Parse(split[1]);

                rawPoints.Add(new Point(x, y));
            }

            const double rotationIncrement = 2 * Math.PI / 20;
            const int sampleStep = 10;
            var rotatedPoints = GetApproximateMaximalWidthCollection(rawPoints, rotationIncrement, sampleStep);

            // Find minX, minY and maxX, maxY, to allow for normalisation to the bounding box

            double minX = rotatedPoints.Min(point => point.X);
            double minY = rotatedPoints.Min(point => point.Y);
            double maxX = rotatedPoints.Max(point => point.X);
            double maxY = rotatedPoints.Max(point => point.Y);

            // precompute multiplicative scale factors between raw points and bounding box
            double scaleFactorX = _boundingBox.Width / (maxX - minX);
            double scaleFactorY = _boundingBox.Height / (maxY - minY);

            // Take scale factor as the smaller of the x and y factors, to preserve aspect ratio
            double scaleFactor = Math.Min(scaleFactorX, scaleFactorY);

            // create list of translated points
            var translatedPoints = new List<Point>(rotatedPoints.Count());

            foreach (Point rotatedPoint in rotatedPoints)
            {
                double normX = (scaleFactor * (rotatedPoint.X - minX)) + _boundingBox.Left;
                double normY = (scaleFactor * (rotatedPoint.Y - minY)) + _boundingBox.Top;

                translatedPoints.Add(new Point(normX, normY));
            }

            translatedPoints = TranslatePointsToBoundingBoxCenter(translatedPoints).ToList();

            TrackPoints = new PointCollection(translatedPoints);

            // Cache the parsed points for reuse
            _cachedTracks.Add(track, TrackPoints);

            StartPoint = TrackPoints[0];

            // Build driver points collection
            // Initialise all drivers at the start line
            var driverPoints = new List<DriverMapPoint>(drivers.Count());

            foreach (Driver driver in drivers)
            {
                driverPoints.Add(new DriverMapPoint(driver, StartPoint));
            }

            minX = translatedPoints.Min(point => point.X);
            minY = translatedPoints.Min(point => point.Y);
            maxX = translatedPoints.Max(point => point.X);
            maxY = translatedPoints.Max(point => point.Y);

            DriverPoints = new ObservableCollection<DriverMapPoint>(driverPoints);
        }

        public void UpdateDriverMapPosition(Driver driver, double proportionOfLap)
        {
            // Find the closest index in the track points list based on the distance around the lap
            int trackIndex = (int)Math.Round(proportionOfLap * (TrackPoints.Count - 1), 0);

            // Find the index of the requested driver in the driverpoints list
            int driverIndex = DriverPoints.Select(dp => dp.Driver).ToList().IndexOf(driver);

            DriverPoints[driverIndex].Point = TrackPoints[trackIndex];
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

            double minX = points.Min(p => p.X);
            double minY = points.Min(p => p.Y);

            double maxX = points.Max(p => p.X);
            double maxY = points.Max(p => p.Y);

            Point origin = new Point(minX + (maxX - minX) / 2, minY + (maxY - minY) / 2);

            for (double testAngle = 0; testAngle < 2 * Math.PI; testAngle += radIncrement)
            {
                double maxHorizDist = 0;

                for (int i = 0; i < points.Count; i += sampleStep)
                {
                    dx = points[i].X - origin.X;
                    dy = points[i].Y - origin.Y;

                    double angleToHoriz = Math.Atan2(dy, dx);
                    double distToOrigin = Math.Sqrt(dx * dx + dy * dy);

                    double horizDist = distToOrigin * Math.Abs(Math.Cos(testAngle + angleToHoriz));

                    if (horizDist > maxHorizDist)
                    {
                        maxHorizDist = horizDist;
                    }
                }

                if (maxHorizDist > bestHorizDist)
                {
                    bestHorizDist = maxHorizDist;
                    optimalAngle = testAngle;
                }
            }

            // No need to rotate points if the optimal angle is zero. Simply return original collection
            if (optimalAngle == 0)
            {
                return points;
            }

            // Rotate original points by the calculated optimal angle
            ICollection<Point> rotatedPoints = new List<Point>(points.Count);

            double sinTheta = Math.Sin(optimalAngle);
            double cosTheta = Math.Cos(optimalAngle);

            foreach (Point point in points)
            {
                dx = point.X - origin.X;
                dy = point.Y - origin.Y;

                double x = cosTheta * dx - sinTheta * dy + origin.X;
                double y = sinTheta * dx + cosTheta * dy + origin.Y;
                rotatedPoints.Add(new Point(x, y));
            }

            return rotatedPoints;
        }

        private IEnumerable<Point> TranslatePointsToBoundingBoxCenter(IEnumerable<Point> points)
        {
            double minX = points.Min(p => p.X);
            double minY = points.Min(p => p.Y);

            double maxX = points.Max(p => p.X);
            double maxY = points.Max(p => p.Y);

            // Find which dimension is not in center of bounding box
            if (maxX - minX == _boundingBox.Width)
            {
                double dy = (_boundingBox.Height - (maxY - minY)) / 2;
                return points.Select(point => new Point(point.X, point.Y + dy));
            }
            else
            {
                double dx = (_boundingBox.Width - (maxX - minX)) / 2;

                return points.Select(point => new Point(point.X + dx, point.Y));
            }
        }
    }
}
