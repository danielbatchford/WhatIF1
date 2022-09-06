using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using WhatIfF1.Adapters;
using WhatIfF1.Modelling.Tracks;

namespace WhatIfF1.UI.Controller.TrackMaps
{
    public class TrackMapProvider
    {
        private static readonly IDictionary<Track, PointCollection> _cachedTracks = new Dictionary<Track, PointCollection>();

        private static readonly Rect _boundingBox = new Rect(0, 0, 100, 100);

        public PointCollection TrackPoints { get; }

        public TrackMapProvider(Track track)
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

            var rotatedPoints = new PointCollection(numPoints);


            // Rotate track points to maximise width
            TrackPoints = new PointCollection(translatedPoints);

            // Cache the parsed points for reuse
            _cachedTracks.Add(track, TrackPoints);

        }
    }
}
