using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Entities
{
    public interface IPlayer
    {
        int Id { get; set; }
        string Name { get; set; }
        int PlayerCount { get; set; }
        Map Map { get; set; }
        Vector2 Position { get; set; }
        int Score { get; set; }
        int Energy { get; set; }
        int MaxEnergy { get; set; }
        IEnumerable<string> Keys { get; set; }
        IEnumerable<Lighthouse> Lighthouses { get; set; }
        Cell[] View { get; set; }
        void Setup(PlayerConfig playerConfig);
    }

    
}