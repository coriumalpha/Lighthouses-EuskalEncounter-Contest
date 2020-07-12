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

        public static void Render(MapArena map, IEnumerable<ArenaPlayer> players, IEnumerable<Lighthouse> lighthouses)
        {
            SetRenderGrid(ref map);
            SetPlayers(ref map, players);
            SetLighthouses(ref map, lighthouses);

            StringBuilder renderResult = RenderMap(map);

            renderResult.AppendLine();

            foreach (ArenaPlayer player in players)
            {
                renderResult.AppendLine(String.Format("Player {0} {1}", player.Id, player.Name));
                renderResult.AppendLine(String.Format("    Position: [{0},{1}]", player.Position.X, player.Position.Y));
                renderResult.AppendLine(String.Format("    Energy: {0}", player.Energy));
                renderResult.AppendLine(String.Format("    Keys: {0}", player.Keys.Count()));
                renderResult.AppendLine(String.Format("    Lighthouses: {0}", lighthouses.Where(x => x.Owner?.Id == player.Id).Count()));
            }

            Console.SetCursorPosition(0, 0);
            Console.Write(renderResult);
        }

        private static void SetRenderGrid(ref MapArena map)
        {
            IEnumerable<RendererCell> cells = map.Grid.Select(x =>
                new RendererCell { Position = x.Position, IsPlayable = x.IsPlayable, Energy = x.Energy}
                );

            map.RenderGrid = cells.ToList();
        }

        private static void SetPlayers(ref MapArena map, IEnumerable<ArenaPlayer> players)
        {
            foreach (ArenaPlayer player in players)
            {
                RendererCell cell = map.RenderGrid.Where(x => x.Position == player.Position).Single();

                if (cell.Players == null)
                {
                    cell.Players = new List<ArenaPlayer>();
                }
                cell.Players.Add(player);
            }
        }

        private static void SetLighthouses(ref MapArena map, IEnumerable<Lighthouse> lighthouses)
        {
            foreach (Lighthouse lighthouse in lighthouses)
            {
                map.RenderGrid.Where(x => x.Position == lighthouse.Position).Single().Lighthouse = lighthouse;
            }
        }

        private static string RenderCell(RendererCell cell)
        {
            if (!cell.IsPlayable)
            {
                return String.Format(CELLFORMAT, "  ");
            }

            if (cell.Players != null && cell.Players.Any())
            {
                return String.Format("<{0}>", "P" + cell.Players.First().Id);
            }

            if (cell.IsLighthouse)
            {
                return "[[]]";
            }

            if (cell.Energy == MAX_CELL_ENERGY)
            {
                return String.Format(CELLFORMAT, "++");
            }

            return String.Format(CELLFORMAT, cell.Energy.ToString("00"));
        }

        private static string RenderRow(RendererCell[] row, int rowNumber)
        {
            StringBuilder strRow = new StringBuilder();

            strRow.Append(String.Format(CELLFORMAT, rowNumber.ToString("00")));

            foreach (RendererCell cell in row)
            {
                strRow.Append(RenderCell(cell));
            }
            return strRow.ToString();
        }

        private static StringBuilder RenderMap(MapArena map)
        {
            StringBuilder strMap = new StringBuilder();
            string scaleX = String.Format(CELLFORMAT, "XX");

            for (int i = 0; i < map.Size.X; i++)
            {
                scaleX += String.Format(CELLFORMAT, i.ToString("00"));
            }

            strMap.AppendLine(scaleX);

            for (int i = 0; i < map.Size.Y; i++)
            {
                RendererCell[] row = map.RenderGrid.Where(c => c.Position.Y == i).ToArray();
                strMap.AppendLine(RenderRow(row, i));
            }
            return strMap;
        }
    }
}
