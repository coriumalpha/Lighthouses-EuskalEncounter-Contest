using Entities;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Arena
{
    public class Engine
    {
        private const string MAP_PATH = @"C:\Users\Corium\Desktop\LSMaterial\Map1.txt";
        private const int LIGHTHOUSES_NUM = 1;
        private const int MAX_CELL_ENERGY = 100;

        private Map map;
        private List<Lighthouse> lighthouses;
        private IEnumerable<IPlayer> players;
        private Random rand;

        public Engine(IEnumerable<IPlayer> players)
        {
            Setup(players);
        }

        #region Public methods
        public void Start()
        {
            TurnDispatcher();
        }
        #endregion

        #region Private methods

        private void Turn()
        {
            Console.Clear();
            SetCellEnergy();
            Renderer.Render(this.map);
        }

        private void TurnDispatcher()
        {
            Turn();
            Console.ReadLine();
            TurnDispatcher();
        }

        private void SetCellEnergy()
        {
            foreach (Cell cell in this.map.Grid)
            {
                UpdateCellEnergy(cell);
            }
        }

        private void UpdateCellEnergy(Cell cell)
        {
            int maxEnergyDistance = 5;

            if (cell.Energy == MAX_CELL_ENERGY)
            {
                return;
            }

            IEnumerable<Lighthouse> lighthousesInRange = this.lighthouses.Where(x => Geometry.Distance(cell.Position, x.Position) <= maxEnergyDistance);
            foreach (Lighthouse lighthouse in lighthousesInRange)
            {
                 cell.Energy += (int)Math.Floor(maxEnergyDistance - Geometry.Distance(cell.Position, lighthouse.Position));
            }

            if (cell.Energy > MAX_CELL_ENERGY)
            {
                cell.Energy = 100;
            }
        }

        #region Setup methods
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
                this.map.Grid.Where(x => x.Position == lighthouse.Position).Single().IsLighthouse = true;
            }
        }
        #endregion

        private Vector2 GetRandomPlayablePosition()
        {
            IEnumerable<Cell> playableCells = this.map.Grid.Where(x => x.IsPlayable);

            return playableCells.ElementAt(rand.Next(playableCells.Count())).Position;
        }
        #endregion
    }
}
