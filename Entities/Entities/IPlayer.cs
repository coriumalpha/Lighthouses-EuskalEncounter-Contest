using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public interface IPlayer
    {
        int Id { get; set; }
        string Name { get; set; }
        Cell Position { get; set; }
        int Score { get; set; }
        int Energy { get; set; }
        int MaxEnergy { get; set; }
        HashSet<Lighthouse> Keys { get; set; }
        Cell[] View { get; set; }
    }

    public enum PlayerActions
    {
        Pass,
        Move,
        Attack,
        Charge,
        Connect
    }
}