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
        public IEnumerable<string> Keys { get; set; }
        public IEnumerable<Lighthouse> Lighthouses { get; set; }
        public Cell[] View { get; set; }
        #endregion

        private Random rand;

        public TestPlayer()
        {
            this.rand = new Random();
        }

        public void Setup(PlayerConfig playerConfig)
        {
            this.Id = playerConfig.Id;
            this.PlayerCount = playerConfig.PlayerCount;
            this.Position = playerConfig.Position;
            this.Map = playerConfig.Map;
            this.Lighthouses = playerConfig.Lighthouses;
        }

        public IDecision Play(ITurnState state)
        {
            IDecision decision = new Decision();
            decision.Action = PlayerActions.Move;
            decision.Target = RandomMovement();

            return decision;
        }

        private Vector2 RandomMovement()
        {
            Vector2 move = new Vector2(rand.Next(2), rand.Next(2));
            if (!IsValidMovement(move))
            {
                return RandomMovement();
            }
            return move;
        }

        private bool IsValidMovement(Vector2 destination)
        {   
            return this.Map.Grid.Where(x => x.Position == destination).Single().IsPlayable;
        }
    }
}
