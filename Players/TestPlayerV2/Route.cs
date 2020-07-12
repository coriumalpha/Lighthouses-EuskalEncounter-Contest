using Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Players.TestPlayerV2
{
    class Route
    {
        public Route(ICell origin, ICell destination)
        {
            this.Origin = origin;
            this.Destination = destination;
            this.Way = new List<ICell>();
            this.TraveledWay = new List<ICell>();
        }

        public ICell Origin { get; }
        public ICell Destination { get; }
        public List<ICell> Way { get; set; }
        public List<ICell> TraveledWay { get; set; }
    }
}
