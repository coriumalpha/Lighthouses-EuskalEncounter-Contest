using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Entities
{
    public class PlayerConfig
    {
        public int Id { get; set; }
        public int PlayerCount { get; set; }
        public Vector2 Position { get; set; }
        public Map Map { get; set; }
        public IEnumerable<Lighthouse> Lighthouses { get; set; }
    }
}
