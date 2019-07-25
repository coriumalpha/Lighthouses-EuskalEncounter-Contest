using Entities;
using Players;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Arena
{
    class Program
    {
        const int PLAYER_COUNT = 2;

        static void Main(string[] args)
        {
            List<IPlayer> players = new List<IPlayer>();

            for (int i = 0; i < PLAYER_COUNT; i++)
            {
                IPlayer player = new TestPlayer();
                players.Add(player);
            }

            Game game = new Game(players);
            game.Start();
        }
    }
}
