using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Entities
{
    public interface IDecision
    {
        PlayerActions Action { get; set; }
        int? Energy { get; set; }
        Vector2 Target { get; set; }
    }
}
