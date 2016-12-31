using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using FlightActor.Interfaces;
using RouteActor.Interfaces;

namespace FlightActor
{
    [StatePersistence(StatePersistence.Persisted)]
    internal class FlightActor : Actor, IFlightActor
    {
        private const string FLIGHT_STATE_NAME = "flightlog";
        private IRouteActor route;
        public FlightActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Flight activated.");
            return this.StateManager.TryAddStateAsync(FLIGHT_STATE_NAME, new FlightActorState());
        }

        public async Task SetFlightRoute(string routeId, List<FlightRoute> currentRoute)
        {
            route = ActorProxy.Create<IRouteActor>(new ActorId(routeId), "FlightTracking", "RouteActorService");
            await route.SetRouteAsync(currentRoute);
        }

        public async Task UpdateFlightLocationAsync(double latitude, double longitude, double feet)
        {
            var flightlog = await StateManager.GetStateAsync<FlightActorState>(FLIGHT_STATE_NAME);

            flightlog.Feet = feet;
            flightlog.Latitude = latitude;
            flightlog.Longitude = longitude;
            await StateManager.SetStateAsync(FLIGHT_STATE_NAME, flightlog);

            ActorEventSource.Current.ActorMessage(this, "Updated location for {0} to {1}", Id.GetStringId(), flightlog);

            await route.ReportRouteStatusAsync(Id.GetStringId(), latitude, longitude);
        }
    }
}
