using Entities;
using Entities.Enums;
using Players;
using Players.Bot;
using Players.BotV2;
using System.Collections.Generic;

namespace Arena
{
    class Program
    {
        const int PLAYER_COUNT = 0;

        static void Main(string[] args)
        {
            //Player config
            List<IPlayer> players = new List<IPlayer>();

            players.Add(new BotV2("Key/Energy farm equilibrium"));
            players.Add(new BotV2("Key/Energy farm equilibrium 2"));
            players.Add(new BotV2("Key farm priority", false));
            players.Add(new BotV2("Key farm priority 2", false));

            for (int i = 0; i < (PLAYER_COUNT - 1); i++)
            {
                IPlayer player = new Bot();
                players.Add(player);
            }

            //Map config
            MapNames mapName = MapNames.Old;

            //Engine setup and start
            Engine game = new Engine(players, mapName);
            game.Start();
        }
    }
}
