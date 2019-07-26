using Entities;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Entities
{
    public interface ILighthouse
    {
        Vector2 Position { get; set; }
        int Energy { get; set; }
        int? IdOwner { get; }
    }
}
