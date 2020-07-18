using Entities;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Arena
{
    public class RenderMap : IMap
    {
        public RenderMap(Vector2 size, List<ICell> grid)
        {
            Grid = grid;
            Size = size;
        }

        public Vector2 Size { get; set; }
        public List<ICell> Grid { get; set; }
        public List<RendererCell> RenderGrid { get; set; }
    }
}
