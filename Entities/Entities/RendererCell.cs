using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Entities
{
    public class RendererCell : ICell
    {
        public Vector2 Position { get; set; }
        public bool IsPlayable { get; set; }
        public bool IsLighthouse
        {
            get
            {
                return this.Lighthouse != null;
            }
        }
        public Lighthouse Lighthouse { get; set; }
        public IPlayer Player { get; set; }
        public int Energy { get; set; }
    }
}
