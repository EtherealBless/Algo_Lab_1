using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_1.Utils
{
    class Point
    {
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        //public Point(int x, double y)
        //{
        //    X = x;
        //    Y = y;
        //}

        public double X { get; set; }
        public double Y { get; set; }
    }

    static class PointExtensions
    {
        public static Pair<List<double>, List<double>> ListsFromPoints(List<Point> points)
        {
            List<double> xs = new List<double>();
            List<double> ys = new List<double>();

            foreach (Point p in points)
            {
                xs.Add(p.X);
                ys.Add(p.Y);
            }

            return new Pair<List<double>, List<double>>(xs, ys);
        }

        public static List<Point> PointsFromCoordinates(this List<Coordinates> coordinates)
        {
            List<Point> points = new List<Point>();

            foreach (Coordinates c in coordinates)
            {
                points.Add(new Point(c.X, c.Y));
            }

            return points;
        }
        public static Pair<List<double>, List<double>> ToLists(this List<Point> points) => ListsFromPoints(points);

        public static Pair<List<double>, List<double>> ToLists(this List<Coordinates> coordinates) => ListsFromPoints(coordinates.Select(c => new Point(c.X, c.Y)).ToList());

    }
}
