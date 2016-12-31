using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteActor.Interfaces
{
    public interface IRouteActorEvents: IActorEvents
    {
        void StatusReported(string flightId, string status);

    }
}
