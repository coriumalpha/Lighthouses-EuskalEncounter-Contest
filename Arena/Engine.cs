using Entities;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;

namespace Arena
{
    public class Engine
    {
        private const string MAP_PATH = @"C:\Users\Corium\Desktop\LSMaterial\Map1.txt";
        private const int LIGHTHOUSES_NUM = 3;
        private const int MAX_CELL_ENERGY = 100;

        private MapArena map;
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

        private void Turn(IPlayer player)
        {
            ITurnState state = new TurnState();

            IDecision decision = player.Play(state);

            switch (decision.Action)
            {
                //TODO: Implement other actions
                case PlayerActions.Move:
                    HandleMovement(player, decision.Target);
                    break;
            }

            Renderer.Render(this.map, this.players, this.lighthouses);
        }

        private void HandleMovement(IPlayer player, Vector2 target)
        {
            Vector2 destination = player.Position + target;
            if (!IsValidMovement(destination))
            {
                throw new Exception("Invalid movement");
            }

            player.Position = destination;
        }

        private void TurnDispatcher()
        {
            SetCellEnergy();
            SetPlayerEnergy();
            RemoveConsumedEnergy();

            foreach (IPlayer player in this.players)
            {
                Turn(player);
                //Console.ReadLine();
                Thread.Sleep(500);
            }
            TurnDispatcher();
        }

        #region Turn methods
        private void RemoveConsumedEnergy()
        {
            foreach (IPlayer player in this.players)
            {
                this.map.Grid.Where(x => x.Position == player.Position).Single().Energy = 0;
            }
        }

        private void SetPlayerEnergy()
        {
            foreach (IPlayer player in this.players)
            {
                int cellEnergy = this.map.Grid.Where(x => x.Position == player.Position).Single().Energy;

                if (this.players.Where(x => x.Position == player.Position).Count() > 1)
                {
                    player.Energy += (int)Math.Floor((double)cellEnergy / this.players.Where(x => x.Position == player.Position).Count());
                }

                player.Energy += cellEnergy;
            }
        }

        private void SetCellEnergy()
        {
            foreach (ICell cell in this.map.Grid)
            {
                UpdateCellEnergy(cell);
            }
        }

        private void UpdateCellEnergy(ICell cell)
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
        #endregion

        #region Setup methods
        private void Setup(IEnumerable<IPlayer> players)
        {
            this.rand = new Random();
            this.map = Parser.LoadToMap(MAP_PATH);
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
        #endregion

        #region Helper methods
        private bool IsValidMovement(Vector2 destination)
        {
            IEnumerable<ICell> cell = this.map.Grid.Where(x => x.Position == destination);

            if (!cell.Any())
            {
                return false;
            }

            return cell.Single().IsPlayable;
        }

        private ICell GetCellByPosition(Vector2 position)
        {
            return this.map.Grid.Where(x => x.Position == position).Single();
        }

        private Vector2 GetRandomPlayablePosition()
        {
            IEnumerable<ICell> playableCells = this.map.Grid.Where(x => x.IsPlayable);

            return playableCells.ElementAt(rand.Next(playableCells.Count())).Position;
        }
        #endregion
        #endregion
    }
}
