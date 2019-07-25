using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Entities
{
    public class MapArena : IMap
    {
        public MapArena(Vector2 size, List<ICell> cells)
        {
            Grid = cells;
            Size = size;
        }

        public Vector2 Size { get; set; }
        public List<ICell> Grid { get; set; }
        public List<RendererCell> RenderGrid { get; set; }
    }
}
