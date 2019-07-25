using Entities;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Entities
{
    public class TurnState : ITurnState
    {
        public Vector2 Position { get; set; }
        public int Score { get; set; }
        public int Energy { get; set; }
        public Map View { get; set; }
        public List<Lighthouse> Lighthouses { get; set; }
    }
}
