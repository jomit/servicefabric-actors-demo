using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FlightActor
{
    [DataContract]
    public class FlightActorState
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double Feet { get; set; }
    }
}
