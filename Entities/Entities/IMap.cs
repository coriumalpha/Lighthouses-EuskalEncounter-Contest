using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Entities
{
    public interface IMap
    {
        Vector2 Size { get; set; }
        List<ICell> Grid { get; set; }
    }
}
