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
        private const string MAP_PATH = @"C:\Users\Corium\Desktop\Mapas Lighthouses\test.txt";

        private const int LOOP_WAIT_TIME = 40;

        private const int MAX_CELL_ENERGY = 100;
        private const int LIGHTHOUSE_ENERGY_LOST_PER_TURN = 10;

        private MapArena map;
        private List<Lighthouse> _lighthouses;
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
                Lighthouses = this._lighthouses,
                Position = player.Position
            };

            IDecision decision = player.Play(state);

            switch (decision.Action)
            {
                case PlayerActions.Move:
                    HandleMovement(player, decision.Target);
                    break;
                case PlayerActions.Attack:
                    HandleAttack(player, decision.Energy.Value);
                    break;
                //case PlayerActions.Connect:
                //    HandleConnect(player, decision.Target);
                //    break;
                //case PlayerActions.Pass:
                //    HandlePass(player);
                //    break;
            }

            Renderer.Render(this.map, this.players, this._lighthouses);
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

            Lighthouse lighthouse = this._lighthouses.Where(x => x.Position == target).Single();

            if (energy > lighthouse.Energy)
            {
                if (players.Where(x => x.Lighthouses.Contains(lighthouse)).Any())
                {
                    ArenaPlayer oldOwner = players.Where(x => x.Lighthouses.Contains(lighthouse)).Single();
                    oldOwner.Lighthouses.Remove(lighthouse);
                }
                player.Lighthouses.Add(lighthouse);
                lighthouse.Owner = player;
            }

            player.Energy = player.Energy - energy;
            lighthouse.Energy = Math.Abs(lighthouse.Energy - energy);
        }

        private void HandleMovement(ArenaPlayer player, Vector2 target)
        {
            Vector2 destination = player.Position + target;
            if (!(GameLogic.IsValidMovement(destination, map.Grid)))
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
            UpdateLighthousesEnergy();
            RemoveConsumedEnergyFromCells();

            foreach (ArenaPlayer player in this.players)
            {
                Turn(player);
                Thread.Sleep(LOOP_WAIT_TIME);
            }
            TurnDispatcher();
        }

        #region Turn methods
        private void UpdateLighthousesEnergy()
        {
            IEnumerable<Lighthouse> energyzedLighthouses = _lighthouses.Where(x => x.Energy > 0);
            foreach (Lighthouse lighthouse in energyzedLighthouses)
            {
                if (lighthouse.Energy > LIGHTHOUSE_ENERGY_LOST_PER_TURN)
                {
                    lighthouse.Energy = lighthouse.Energy - LIGHTHOUSE_ENERGY_LOST_PER_TURN;
                    continue;
                }

                lighthouse.Energy = 0;
            }
        }

        private void RemoveConsumedEnergyFromCells()
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

            IEnumerable<Lighthouse> lighthousesInRange = this._lighthouses.Where(x => Vector2.Distance(cell.Position, x.Position) <= maxEnergyDistance);
            foreach (Lighthouse lighthouse in lighthousesInRange)
            {
                 cell.Energy += (int)Math.Floor(maxEnergyDistance - Vector2.Distance(cell.Position, lighthouse.Position));
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
                ArenaPlayer arenaPlayer = new ArenaPlayer(player.Name);

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
                Lighthouses = this._lighthouses.Select(x => x.Position),
                Map = this.map,
                PlayerCount = playerCount,
                Position = GetRandomPlayablePosition(),
                PlayerDCI = playerPCI
            };

            return config;
        }

        private void SetupLighthouses(List<Vector2> lighthousePositions)
        {
            this._lighthouses = new List<Lighthouse>();

            foreach (Vector2 lighthousePosition in lighthousePositions)
            {
                Lighthouse lighthouse = new Lighthouse()
                {
                    Position = lighthousePosition,
                };

                _lighthouses.Add(lighthouse);
            }
        }
        #endregion

        #region Helper methods

        private bool IsLighthouse(Vector2 target)
        {
            return this._lighthouses.Where(x => x.Position == target).Any();
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
