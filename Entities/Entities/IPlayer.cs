﻿using System;
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
        IMap Map { get; set; }
        Vector2 Position { get; set; }
        int Score { get; set; }
        int Energy { get; set; }
        int MaxEnergy { get; set; }
        List<Vector2> Keys { get; set; }
        List<Lighthouse> Lighthouses { get; set; }
        Cell[] View { get; set; }
        void Setup(PlayerConfig playerConfig);
        IDecision Play(ITurnState state);
    }

    
}