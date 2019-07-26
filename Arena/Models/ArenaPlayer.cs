using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Arena
{
    public class ArenaPlayer
    {
        public IPlayer PlayerDCI;
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
        public Cell[] View { get; set; }

        public ArenaPlayer()
        {
            this.Keys = new List<Vector2>();
        }

        public IDecision Play(ITurnState state)
        {
            return this.PlayerDCI.Play(state);
        }

        public void Setup(PlayerConfig playerConfig)
        {
            this.Id = playerConfig.Id;
            this.PlayerCount = playerConfig.PlayerCount;
            this.Position = playerConfig.Position;
            this.Map = playerConfig.Map;
            this.PlayerDCI = playerConfig.PlayerDCI;
            this.Lighthouses = playerConfig.Lighthouses.Select(x => new Lighthouse() { Position = x }).ToList();
        }
    }
}
