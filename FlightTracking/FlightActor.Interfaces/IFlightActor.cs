using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using RouteActor.Interfaces;

namespace FlightActor.Interfaces
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IFlightActor : IActor
    {
        Task UpdateFlightLocationAsync(double latitude, double longitude, double feet);

        Task SetFlightRoute(string routeId, List<FlightRoute> currentRoute);
    }
}
