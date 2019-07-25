using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Entities
{
    public class Lighthouse
    {
        public int Id { get; set; }
        public Vector2 Position { get; set; }
        public IPlayer Owner { get; set; }
        public int Energy { get; set; }
        public string Key { get; set; }
        public ICollection<Lighthouse> Connections { get; set; }
    }
}
