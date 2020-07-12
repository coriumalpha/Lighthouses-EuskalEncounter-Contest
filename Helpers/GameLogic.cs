using Entities;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Helpers
{
    public static class GameLogic
    {
        public static bool IsValidMovement(Vector2 destination, List<ICell> grid)
        {
            IEnumerable<ICell> cell = grid.Where(x => x.Position == destination);

            if (!cell.Any())
            {
                return false;
            }

            return cell.Single().IsPlayable;
        }

        public static bool IsValidStep(Vector2 origin, Vector2 step, List<ICell> grid)
        {
            Vector2 destination = Vector2.Add(origin, step);
            return IsValidMovement(destination, grid);
        }
    }
}
