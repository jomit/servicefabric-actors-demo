using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteActor.Interfaces
{
    public class FlightRoute
    {
        public string Name { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double DistanceFromOrigin { get; set; }

        public double DistanceFromDestination { get; set; }

        public string Type { get; set; }
    }
}
