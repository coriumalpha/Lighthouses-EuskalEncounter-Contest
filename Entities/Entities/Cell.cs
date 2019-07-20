using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Entities
{
    public struct Cell
    {
        const int MAX_ENERGY = 100;
        public Vector2 Position { get; set; }
        public bool Playable { get; set; }
        public int Energy { get; set; }
    }
}
