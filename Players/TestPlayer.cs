using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Players
{
    public class TestPlayer : IPlayer
    {
        #region Public properties
        public int Id { get; set; }
        public string Name { get; set; }
        public int PlayerCount { get; set; }
        public IMap Map { get; set; }
        public Vector2 Position { get; set; }
        public int Score { get; set; }
        public int Energy { get; set; }
        public int MaxEnergy { get; set; }
        public List<Vector2> Keys { get; set; }
        public List<Lighthouse> Lighthouses { get; set; }
        public List<Lighthouse> OwnLighthouses { get; set; }
        public Cell[] View { get; set; }
        #endregion

        private Random rand;

        public TestPlayer()
        {
            this.rand = new Random();
            this.Keys = new List<Vector2>();
            this.Lighthouses = new List<Lighthouse>();
        }

        public void Setup(PlayerConfig playerConfig)
        {
            this.Id = playerConfig.Id;
            this.PlayerCount = playerConfig.PlayerCount;
            this.Position = playerConfig.Position;
            this.Map = playerConfig.Map;
            this.Lighthouses = playerConfig.Lighthouses.ToList();
        }

        public IDecision Play(ITurnState state)
        {
            IDecision decision = new Decision();

            if (state.Lighthouses.Where(x => x.Position == state.Position).Any())
            {
                if (this.Keys.Where(x => x == state.Position).Any())
                {
                    if (state.Lighthouses.Where(x => x.Position == state.Position).FirstOrDefault().Owner?.Id != this.Id)
                    {
                        decision.Action = PlayerActions.Attack;
                        decision.Energy = (int)Math.Floor(this.Energy / 0.8);

                        return decision;
                    }
                }
            }
            
            decision.Action = PlayerActions.Move;
            decision.Target = RandomMovement();
                       
            return decision;
        }

        private Vector2 RandomMovement()
        {
            Vector2 move = new Vector2(rand.Next(3) - 1, rand.Next(3) - 1);
            if (move.X == 0 && move.Y == 0)
            {
                return RandomMovement();
            }

            Vector2 target = this.Position + move;
            if (!IsValidMovement(target))
            {
                return RandomMovement();
            }

            return move;
        }

        private bool IsValidMovement(Vector2 destination)
        {
            IEnumerable<ICell> cell = this.Map.Grid.Where(x => x.Position == destination);

            if (!cell.Any())
            {
                return false;
            }

            return cell.Single().IsPlayable;
        }
    }
}
