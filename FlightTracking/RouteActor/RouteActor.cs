using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using RouteActor.Interfaces;
using System.IO;
using System.Fabric;

namespace RouteActor
{
    [StatePersistence(StatePersistence.Persisted)]
    internal class RouteActor : Actor, IRouteActor, IRemindable
    {
        const string CHECK_ROUTE_REMINDER = "CheckRouteReminder";
        private const string FLIGHT_ROUTE_STATE_NAME = "flightroute";
        public RouteActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            return RegisterReminderAsync(CHECK_ROUTE_REMINDER, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
        }

        public Task ReportRouteStatusAsync(string flightId, double latitude, double longitude)
        {
            return StateManager.SetStateAsync(flightId, GetRouteName(flightId, latitude, longitude));
        }

        public Task SetRouteAsync(List<FlightRoute> currentRoute)
        {
            return StateManager.SetStateAsync(FLIGHT_ROUTE_STATE_NAME, currentRoute);
        }

        private string GetRouteName(string flightId, double latitude, double longitude)
        {
            var currentRoute = StateManager.GetStateAsync<List<FlightRoute>>(FLIGHT_ROUTE_STATE_NAME).Result;
            var destinationCoordinates = currentRoute.Last();

            //TODO need to find the route name based on 50 mile radius later. For now just calculated and report distance remaining.
            var distance = LatLongCalculator.GetDistance(latitude, longitude, destinationCoordinates.Latitude, destinationCoordinates.Longitude);
          
            return string.Format("Distance Remaining {0} miles. ({1}, {2})", distance, latitude, longitude);
        }

      


        public async Task ReceiveReminderAsync(string reminderName, byte[] context, TimeSpan dueTime, TimeSpan period)
        {
            if (reminderName == CHECK_ROUTE_REMINDER)
            {
                var allflights = await StateManager.GetStateNamesAsync();
                foreach (var flightId in allflights)
                {
                    var routeName = await StateManager.GetStateAsync<string>(flightId);
                    //ActorEventSource.Current.ActorMessage(this, "Flight {0} is on Route {1}.", flightId, routeName);

                    var routeEvents = GetEvent<IRouteActorEvents>();
                    routeEvents.StatusReported(flightId, routeName);
                }
            }
        }

        //private List<FlightRoute> GetFlightRoute(string routeId)
        //{
        //    // The .txt file for this flight has been generated from http://flightaware.com/live/flight/ASA319/history/20161224/0255Z/KSJC/KSEA/route
        //    var mycontext = this.ActorService.Context.CodePackageActivationContext;
        //    var configpackage = mycontext.GetConfigurationPackageObject("Config");
        //    string filePath = configpackage.Path + @"\flightroutes\" + routeId  +".txt";
        //    string[] lines = File.ReadAllLines(filePath);
        //    var flightRoute = new List<FlightRoute>();
        //    for (int i = 0; i < lines.Length;)
        //    {
        //        flightRoute.Add(new FlightRoute()
        //        {
        //            Name = lines[i],
        //            Latitude = Convert.ToDecimal(lines[i + 1]),
        //            Longitude = Convert.ToDecimal(lines[i + 2]),
        //            DistanceFromOrigin = Convert.ToDecimal(lines[i + 3]),
        //            DistanceFromDestination = Convert.ToDecimal(lines[i + 4]),
        //            Type = lines[i + 5]
        //        });
        //        i += 6;
        //    }
        //    return flightRoute;
        //}
    }
}
