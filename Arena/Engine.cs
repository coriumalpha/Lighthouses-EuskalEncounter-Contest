using Entities;
using Entities.Enums;
using Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace Arena
{
    public class Engine
    {

        private const int LOOP_WAIT_TIME = 1;

        private const int MAX_CELL_ENERGY = 100;
        private const int LIGHTHOUSE_ENERGY_LOST_PER_TURN = 10;

        private MapNames selectedMap;
        private Map _map;
        private List<Lighthouse> _lighthouses;
        private IEnumerable<ArenaPlayer> _players;
        private Random _rand;

        public Engine(IEnumerable<IPlayer> players, MapNames mapName)
        {
            this._rand = new Random();
            this.selectedMap = mapName;
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

            Stopwatch decisionTime = new Stopwatch();

            decisionTime.Restart();
            IDecision decision = player.Play(state);
            decisionTime.Stop();

            player.OperationTime.Add(decisionTime.ElapsedMilliseconds);
            

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

            Renderer.Render(this._map, this._players, this._lighthouses);
        }

        #region Action methods

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
                if (_players.Where(x => x.Lighthouses.Contains(lighthouse)).Any())
                {
                    ArenaPlayer oldOwner = _players.Where(x => x.Lighthouses.Contains(lighthouse)).Single();
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
            if (!(GameLogic.IsValidMovement(destination, _map.Grid)))
            {
                throw new Exception("Invalid movement");
            }

            if (IsLighthouse(destination))
            {
                player.Keys.Add(destination);
            }

            player.Position = destination;
        }

        #endregion

        #region Turn methods

        private void TurnDispatcher()
        {
            SetCellEnergy();
            SetPlayerEnergy();
            UpdateLighthousesEnergy();
            RemoveConsumedEnergyFromCells();

            foreach (ArenaPlayer player in this._players)
            {
                Turn(player);
                Thread.Sleep(LOOP_WAIT_TIME);
            }
            TurnDispatcher();
        }

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
            foreach (ArenaPlayer player in this._players)
            {
                this._map.Grid.Where(x => x.Position == player.Position).Single().Energy = 0;
            }
        }

        private void SetPlayerEnergy()
        {
            foreach (ArenaPlayer player in this._players)
            {
                int cellEnergy = this._map.Grid.Where(x => x.Position == player.Position).Single().Energy;

                if (this._players.Where(x => x.Position == player.Position).Count() > 1)
                {
                    player.Energy += (int)Math.Floor((double)cellEnergy / this._players.Where(x => x.Position == player.Position).Count());
                }

                player.Energy += cellEnergy;
            }
        }

        private void SetCellEnergy()
        {
            foreach (ICell cell in this._map.Grid)
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
            MapDTO mapData = MapParser.LoadToMap(selectedMap);

            SetupMap(mapData.Map);
            SetupLighthouses(mapData.Lighthouses);
            SetupPlayers(players);
        }

        private void SetupMap(Map map)
        { 
            _map = map;
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
                arenaPlayer.PlayerDCI = playerConfig.PlayerDCI;

                player.Setup(playerConfig);

                playerList.Add(arenaPlayer);
                counter++;
            }

            this._players = playerList;
        }

        private PlayerConfig CreatePlayerConfig(int id, IPlayer playerPCI, int playerCount)
        {
            PlayerConfig config = new PlayerConfig()
            {
                Id = id,
                Lighthouses = this._lighthouses.Select(x => x.Position),
                Map = this._map,
                PlayerCount = playerCount,
                Position = GameLogic.GetRandomPlayablePosition(_map, _rand),
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
        #endregion
        #endregion
    }
}
