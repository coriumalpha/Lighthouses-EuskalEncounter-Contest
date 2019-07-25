using Entities;
using System;
using System.Collections.Generic;
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
        public Map Map { get; set; }
        public Vector2 Position { get; set; }
        public int Score { get; set; }
        public int Energy { get; set; }
        public int MaxEnergy { get; set; }
        public IEnumerable<string> Keys { get; set; }
        public IEnumerable<Lighthouse> Lighthouses { get; set; }
        public Cell[] View { get; set; }
        #endregion

        public void Setup(PlayerConfig playerConfig)
        {
            this.Id = playerConfig.Id;
            this.PlayerCount = playerConfig.PlayerCount;
            this.Position = playerConfig.Position;
            this.Map = playerConfig.Map;
            this.Lighthouses = playerConfig.Lighthouses;
        }
    }
}
