using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Entities
{
    public interface IPlayerConfig
    {
        int Id { get; set; }
        int PlayerCount { get; set; }
        Vector2 Position { get; set; }
        IMap Map { get; set; }
        IEnumerable<Vector2> Lighthouses { get; set; }
    }
}
