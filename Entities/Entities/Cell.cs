using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Entities
{
    public class Cell
    {
        public Vector2 Position { get; set; }
        public bool IsPlayable { get; set; }
        public bool IsLighthouse { get; set; } = false;
        public int Energy { get; set; }
    }
}
