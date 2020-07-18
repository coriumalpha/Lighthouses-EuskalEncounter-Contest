using Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Players.BotV2.Models
{
    class Step
    {
        public Step(int stepNumber)
        {
            this.StepNumber = stepNumber;
            this.Discoveries = new List<Discovery>();
        }

        public int StepNumber { get; }
        public List<Discovery> Discoveries { get; set; }
    }
}
