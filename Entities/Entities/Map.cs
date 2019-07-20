using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Entities
{
    public class Map
    {
        public Map(Vector2 size, List<Cell> cells)
        {
            Size = size;
            Grid = cells;
        }
        public Vector2 Size { get; private set; }
        public List<Cell> Grid { get; private set; }
    }
}
