using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Arena
{
    class Game
    {
        private const string MAP_PATH = @"C:\Users\Corium\Desktop\LSMaterial\Map1.txt";
        private const int LIGHTHOUSES_NUM = 3;

        private Map map;
        private List<Lighthouse> lighthouses;
        private IEnumerable<IPlayer> players;
        private Random rand;

        public Game(IEnumerable<IPlayer> players)
        {
            Setup(players);
        }

        #region Public methods
        public void Start()
        {
            Renderer.Render(this.map);
            Console.ReadLine();
        }
        #endregion

        #region Private methods

        private void Setup(IEnumerable<IPlayer> players)
        {
            this.rand = new Random();
            this.map = Parser.ConvertToMap(MAP_PATH);
            SetupLighthouses();
            SetupPlayers(players);
        }

        private void SetupPlayers(IEnumerable<IPlayer> players)
        {
            this.players = players;

            int counter = 0;
            foreach (IPlayer player in players)
            {
                player.Setup(CreatePlayerConfig(counter));
                counter++;
            }
        }

        private PlayerConfig CreatePlayerConfig(int id)
        {
            PlayerConfig config = new PlayerConfig()
            {
                Id = id,
                Lighthouses = this.lighthouses,
                Map = this.map,
                PlayerCount = players.Count(),
                Position = GetRandomPlayablePosition()
            };

            return config;
        }

        private void SetupLighthouses()
        {
            this.lighthouses = new List<Lighthouse>();

            for (int i = 0; i < LIGHTHOUSES_NUM; i++)
            {
                Lighthouse lighthouse = new Lighthouse()
                {
                    Id = i,
                    Position = GetRandomPlayablePosition(),
                };

                lighthouses.Add(lighthouse);
            }
        }

        private Vector2 GetRandomPlayablePosition()
        {
            IEnumerable<Cell> playableCells = this.map.Grid
                .Where(x => x.Playable == true);

            return playableCells.ElementAt(rand.Next(playableCells.Count())).Position;
        }
        #endregion
    }
}
