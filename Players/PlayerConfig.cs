using Entities;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Players
{
    public class PlayerConfig : IPlayerConfig
    {
        public int Id { get; set; }
        public int PlayerCount { get; set; }
        public Vector2 Position { get; set; }
        public IMap Map { get; set; }
        public IEnumerable<Vector2> Lighthouses { get; set; }
    }
}
