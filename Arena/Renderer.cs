using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arena
{
    public static class Renderer
    {
        private const string CELLFORMAT = " {0} ";
        private const int MAX_CELL_ENERGY = 100; //TODO: Add to config
        public static void Render(Map map)
        {
            Console.Write(RenderMap(map));
        }

        private static string RenderCell(Cell cell)
        {
            if (!cell.IsPlayable)
            {
                return String.Format(CELLFORMAT, "##");
            }

            if (cell.Energy == MAX_CELL_ENERGY)
            {
                return String.Format(CELLFORMAT, "++");
            }

            if (cell.IsLighthouse)
            {
                return String.Format(CELLFORMAT, "||");
            }

            return String.Format(CELLFORMAT, cell.Energy.ToString("00"));
        }

        private static string RenderRow(Cell[] row)
        {
            StringBuilder strRow = new StringBuilder();
            foreach (Cell cell in row)
            {
                strRow.Append(RenderCell(cell));
            }
            return strRow.ToString();
        }

        private static string RenderMap(Map map)
        {
            StringBuilder strMap = new StringBuilder();

            for (int i = 0; i < map.Size.Y; i++)
            {
                Cell[] row = map.Grid.Where(c => c.Position.Y == i).ToArray();
                strMap.AppendLine(RenderRow(row));
            }
            return strMap.ToString();
        }
    }
}
