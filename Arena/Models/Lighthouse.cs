using Entities;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Arena
{
    public class Lighthouse : ILighthouse
    {
        public Vector2 Position { get; set; }
        public ArenaPlayer Owner { get; set; }
        public int? IdOwner {
            get
            {
                return Owner?.Id;
            }
        }
        public int Energy { get; set; }
        public ICollection<Lighthouse> Connections { get; set; }
    }
}
