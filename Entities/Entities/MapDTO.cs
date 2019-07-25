using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Entities
{
    public class MapDTO
    {
        public MapArena Map { get; set; }
        public List<Vector2> Lighthouses { get; set; }
        public List<Vector2> PlayerSlots { get; set; }
    }
}
