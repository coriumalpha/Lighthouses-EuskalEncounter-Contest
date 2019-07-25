using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Entities
{
    public class Cell : ICell
    {
        public Vector2 Position { get; set; }
        public bool IsPlayable { get; set; }
        public int Energy { get; set; }
    }
}
