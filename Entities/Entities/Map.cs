using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Entities
{
    public class Map : IMap
    {
        public Map()
        {        
        }

        public Map(Vector2 size, List<ICell> grid)
        {
            Size = size;
            Grid = grid;
        }

        public Vector2 Size { get; set; }
        public List<ICell> Grid { get; set; }
    }
}
