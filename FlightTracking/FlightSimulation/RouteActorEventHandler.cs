using RouteActor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulation
{
    public class RouteActorEventHandler : IRouteActorEvents
    {
        public void StatusReported(string flightId, string status)
        {
            Console.WriteLine("Flight {0} is Near {1}", flightId, status);
        }
    }
}
