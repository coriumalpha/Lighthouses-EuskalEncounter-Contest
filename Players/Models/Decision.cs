using Entities;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Players
{
    public class Decision : IDecision
    {
        public PlayerActions Action { get; set; }
        public int? Energy { get; set; }
        public Vector2 Target { get; set; }
    }
}
