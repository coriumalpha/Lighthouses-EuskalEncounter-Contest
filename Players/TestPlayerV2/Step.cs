using Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Players.TestPlayerV2
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
