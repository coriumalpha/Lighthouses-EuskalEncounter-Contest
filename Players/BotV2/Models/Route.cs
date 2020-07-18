using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Players.BotV2.Models
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
        public List<ICell> RemainingWay
        {
            get
            {
                return Way.Except(TraveledWay).ToList();
            }
        }
    }
}
