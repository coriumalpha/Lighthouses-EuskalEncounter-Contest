using Entities;
using Players;
using Players.TestPlayerV2;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Arena
{
    class Program
    {
        const int PLAYER_COUNT = 4;

        static void Main(string[] args)
        {
            List<IPlayer> players = new List<IPlayer>();

            players.Add(new TestPlayerV2("Ren-1"));

            for (int i = 0; i < (PLAYER_COUNT - 1); i++)
            {
                IPlayer player = new TestPlayer();
                players.Add(player);
            }

            Engine game = new Engine(players);
            game.Start();
        }
    }
}
