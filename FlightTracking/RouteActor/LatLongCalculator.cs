using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteActor
{
    public static class LatLongCalculator
    {
        const double PIx = 3.141592653589793;
        const double RADIUS = 6378.16;

        public static double GetDistance(double originLatitude, double originLongitude, double destinationLatitude, double destinationLongitude)
        {
            double dlon = Radians(destinationLongitude - originLongitude);
            double dlat = Radians(destinationLatitude - originLatitude);

            double a = (Math.Sin(dlat / 2) * Math.Sin(dlat / 2)) + Math.Cos(Radians(originLatitude)) * Math.Cos(Radians(destinationLatitude)) * (Math.Sin(dlon / 2) * Math.Sin(dlon / 2));
            double angle = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            // return (angle * RADIUS);  // (Kilometers)
            return ((angle * RADIUS) / 1.609344);

        }

        public static double Radians(double x)
        {
            return x * PIx / 180;
        }
    }
}
