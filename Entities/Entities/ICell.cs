using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Entities
{
    public interface ICell
    {
        Vector2 Position { get; set; }
        bool IsPlayable { get; set; }
        int Energy { get; set; }
    }
}
