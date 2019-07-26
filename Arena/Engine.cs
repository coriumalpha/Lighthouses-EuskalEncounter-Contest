using Entities;
using Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;

namespace Arena
{
    public class Engine
    {
        //private const string MAP_PATH = @"C:\Users\Corium\Desktop\LSMaterial\grid.txt";
        //private const string MAP_PATH = @"C:\Users\Corium\Desktop\LSMaterial\island.txt";
        private const string MAP_PATH = @"C:\Users\Corium\Desktop\LSMaterial\square.txt";
        //private const string MAP_PATH = @"C:\Users\Corium\Desktop\LSMaterial\square_l.txt";
        //private const string MAP_PATH = @"C:\Users\Corium\Desktop\LSMaterial\square_xl.txt";

        private const int LOOP_WAIT_TIME = 75;

        private const int MAX_CELL_ENERGY = 100;

        private MapArena map;
        private List<Lighthouse> lighthouses;
        private IEnumerable<ArenaPlayer> players;
        private Random rand;

        public Engine(IEnumerable<IPlayer> players)
        {
            this.rand = new Random();
            Setup(players);
        }

        #region Public methods
        public void Start()
        {
            TurnDispatcher();
        }
        #endregion

        #region Private methods

        private void Turn(ArenaPlayer player)
        {
            ITurnState state = new TurnState()
            {
                Energy = player.Energy,
                Lighthouses = this.lighthouses,
                Position = player.Position
            };

            IDecision decision = player.Play(state);

            switch (decision.Action)
            {
                //TODO: Implement other actions
                case PlayerActions.Move:
                    HandleMovement(player, decision.Target);
                    break;
                case PlayerActions.Attack:
                    HandleAttack(player, decision.Energy.Value);
                    break;
            }

            Renderer.Render(this.map, this.players, this.lighthouses);
        }

        private void HandleAttack(ArenaPlayer player, int energy)
        {
            Vector2 target = player.Position;

            if (!IsLighthouse(target))
            {
                throw new Exception("Invalid target");
            }

            if (player.Energy < energy)
            {
                energy = player.Energy;
            }

            if (!player.Keys.Where(x => x == target).Any())
            {
                throw new Exception("Lighthouse not in keys");
            }

            Lighthouse lighthouse = this.lighthouses.Where(x => x.Position == target).Single();
            player.Lighthouses.Add(lighthouse);
            lighthouse.Owner = player;

            if (lighthouse.Energy > energy)
            {
                lighthouse.Energy -= energy;
                return;
            }

            lighthouse.Energy = energy - lighthouse.Energy;
        }

        private void HandleMovement(ArenaPlayer player, Vector2 target)
        {
            Vector2 destination = player.Position + target;
            if (!IsValidMovement(destination))
            {
                throw new Exception("Invalid movement");
            }

            if (IsLighthouse(destination))
            {
                player.Keys.Add(destination);
            }

            player.Position = destination;
        }

        private void TurnDispatcher()
        {
            SetCellEnergy();
            SetPlayerEnergy();
            RemoveConsumedEnergy();

            foreach (ArenaPlayer player in this.players)
            {
                Turn(player);
                //Console.ReadLine();
                Thread.Sleep(LOOP_WAIT_TIME);
            }
            TurnDispatcher();
        }

        #region Turn methods
        private void RemoveConsumedEnergy()
        {
            foreach (ArenaPlayer player in this.players)
            {
                this.map.Grid.Where(x => x.Position == player.Position).Single().Energy = 0;
            }
        }

        private void SetPlayerEnergy()
        {
            foreach (ArenaPlayer player in this.players)
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
            MapDTO mapData = Parser.LoadToMap(MAP_PATH);

            this.map = mapData.Map;
            SetupLighthouses(mapData.Lighthouses);
            SetupPlayers(players);
        }

        private void SetupPlayers(IEnumerable<IPlayer> players)
        {
            List<ArenaPlayer> playerList = new List<ArenaPlayer>();

            int counter = 0;
            foreach (IPlayer player in players)
            {
                ArenaPlayer arenaPlayer = new ArenaPlayer();

                PlayerConfig playerConfig = CreatePlayerConfig(counter, player, players.Count());

                arenaPlayer.Setup(playerConfig);
                player.Setup(playerConfig);

                playerList.Add(arenaPlayer);
                counter++;
            }

            this.players = playerList;
        }

        private PlayerConfig CreatePlayerConfig(int id, IPlayer playerPCI, int playerCount)
        {
            PlayerConfig config = new PlayerConfig()
            {
                Id = id,
                Lighthouses = this.lighthouses.Select(x => x.Position),
                Map = this.map,
                PlayerCount = playerCount,
                Position = GetRandomPlayablePosition(),
                PlayerDCI = playerPCI
            };

            return config;
        }

        private void SetupLighthouses(List<Vector2> lighthousePositions)
        {
            this.lighthouses = new List<Lighthouse>();

            foreach (Vector2 lighthousePosition in lighthousePositions)
            {
                Lighthouse lighthouse = new Lighthouse()
                {
                    Position = lighthousePosition,
                };

                lighthouses.Add(lighthouse);
            }
        }
        #endregion

        #region Helper methods

        private bool IsLighthouse(Vector2 target)
        {
            return this.lighthouses.Where(x => x.Position == target).Any();
        }
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
