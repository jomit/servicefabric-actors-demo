using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace RouteActor.Interfaces
{
    public interface IRouteActor : IActor, IActorEventPublisher<IRouteActorEvents>
    {
        Task ReportRouteStatusAsync(string flightId, double latitude, double longitude);

        Task SetRouteAsync(List<FlightRoute> currentRoute);
    }
}
