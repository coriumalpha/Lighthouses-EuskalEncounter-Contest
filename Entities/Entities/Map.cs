﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Entities
{
    public class Map
    {
        public Vector2 Size { get; set; }
        public List<ICell> Grid { get; set; }
    }
}
