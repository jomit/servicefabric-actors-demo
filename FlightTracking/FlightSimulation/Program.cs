using FlightActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using RouteActor.Interfaces;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulation
{
    class Program
    {
        const string flightId = "ASA319-20161223-KSJC-KSEA";
        const string routeId = "KSJC-LOUPE4-BMRNG-HRNER-BTG-HAWKZ5-KSEA";
        static void Main(string[] args)
        {
            Task.Run(() => SimulateSingleFlight());

            SubscribeToRouteEventsAsync().GetAwaiter().GetResult();

            Console.ReadLine();
        }

        static async Task SimulateSingleFlight()
        {
            var simulatedLogs = GetSimulatedTrackingLog(flightId);
            var simulatedRoute = GetSimulatedRoute(routeId);

            var currentFlight = ActorProxy.Create<IFlightActor>(new ActorId(flightId), "FlightTracking", "FlightActorService");
            await currentFlight.SetFlightRoute(routeId, simulatedRoute);

            foreach (var log in simulatedLogs)
            {
                await currentFlight.UpdateFlightLocationAsync(log.Latitude, log.Longitude, log.Feet);

                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }

        static List<FlightLog> GetSimulatedTrackingLog(string flight)
        {
            // The .txt file for this flight has been generated from http://flightaware.com/live/flight/ASA319/history/20161223/0255Z/KSJC/KSEA/tracklog
            string[] lines = File.ReadAllLines(@"flightlogs/" + flight + ".txt");
            var flightLog = new List<FlightLog>();
            for (int i = 0; i < lines.Length;)
            {
                flightLog.Add(new FlightLog()
                {
                    Latitude = Convert.ToDouble(lines[i + 1]),
                    Longitude = Convert.ToDouble(lines[i + 2]),
                    Feet = Convert.ToDouble(lines[i + 7])
                });
                i += 10;
            }
            return flightLog;
        }

        static List<FlightRoute> GetSimulatedRoute(string route)
        {
            // The .txt file for this flight has been generated from http://flightaware.com/live/flight/ASA319/history/20161224/0255Z/KSJC/KSEA/route
            string[] lines = File.ReadAllLines(@"flightroutes/" + route + ".txt");
            var flightRoute = new List<FlightRoute>();
            for (int i = 0; i < lines.Length;)
            {
                flightRoute.Add(new FlightRoute()
                {
                    Name = lines[i],
                    Latitude = Convert.ToDouble(lines[i + 1]),
                    Longitude = Convert.ToDouble(lines[i + 2]),
                    DistanceFromOrigin = Convert.ToDouble(lines[i + 3]),
                    DistanceFromDestination = Convert.ToDouble(lines[i + 4]),
                    Type = lines[i + 5]
                });
                i += 6;
            }
            return flightRoute;
        }

        static async Task SubscribeToRouteEventsAsync()
        {
            var routeEventHandler = new RouteActorEventHandler();
            var route = ActorProxy.Create<IRouteActor>(new ActorId(routeId), "FlightTracking", "RouteActorService");

            await route.SubscribeAsync<IRouteActorEvents>(routeEventHandler);
        }
    }
}
