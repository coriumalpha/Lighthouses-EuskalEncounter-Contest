using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Entities
{
    public interface IPlayer
    {
        void Setup(IPlayerConfig playerConfig);
        IDecision Play(ITurnState state);
    }

    
}