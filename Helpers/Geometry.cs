using Entities;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Helpers
{
    public class Geometry
    {
        public double Distance(Vector2 a, Vector2 b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        public double Orient2D(Vector2 a, Vector2 b, Vector2 c)
        {
            return (b.X - a.X) * (c.Y - a.Y) - (c.X - a.X) * (b.Y - a.Y);
        }

        public bool Colinear(Vector2 a, Vector2 b, Vector2 c)
        {
            return Orient2D(a, b, c) == 0;
        }

        public bool Intersect(Line j, Line k)
        {
            return (Orient2D(k.a, k.b, j.a) * Orient2D(k.a, k.b, j.b) < 0)
            && (Orient2D(j.a, j.b, k.a) * Orient2D(j.a, j.b, k.b) < 0);
        }
    }
}
