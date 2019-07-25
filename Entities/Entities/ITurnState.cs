using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Entities
{
    public interface ITurnState
    {
        Vector2 Position { get; set; }
        int Score { get; set; }
        int Energy { get; set; }
        Map View { get; set; } 
        List<Lighthouse> Lighthouses { get; set; }
    }
}
