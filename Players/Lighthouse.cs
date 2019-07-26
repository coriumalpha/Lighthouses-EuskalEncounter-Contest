using Entities;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Players
{
    public class Lighthouse : ILighthouse
    {
        public Vector2 Position { get; set; }
        public int? IdOwner { get; set; }
        public int Energy { get; set; }
        public ICollection<Lighthouse> Connections { get; set; }
    }
}
