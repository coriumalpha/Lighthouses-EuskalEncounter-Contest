using Entities;
using Entities.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Arena
{
    public static class Parser
    {
        private static MapArena map;

        public static MapDTO LoadToMap(MapNames mapName)
        {
            IEnumerable<string> lines = Helpers.Maps.GetFileLines(mapName);
            List<ICell> cells = new List<ICell>();
            List<Vector2> lighthouses = new List<Vector2>();

            int counter = 0;
            foreach (string line in lines)
            {
                //TODO: Convendría refactorizar en una única función para evitar ciclos redundantes
                //No se prioriza al no formar parte del reto
                lighthouses.AddRange(LineToLighthouses(line, counter));
                cells.AddRange(LineToCells(line, counter));
                counter++;
            }

            int sizeX = lines.First().Length;
            int sizeY = lines.Count();
            Parser.map = new MapArena(new Vector2(sizeX, sizeY), cells);

            MapDTO mapData = new MapDTO()
            {
                Map = map,
                Lighthouses = lighthouses
            };

            return mapData;
        }

        private static List<Vector2> LineToLighthouses(string line, int positionY)
        {
            List<Vector2> lighthouses = new List<Vector2>();

            int counter = 0;
            foreach (char cellchar in line)
            {
                if (cellchar != '!')
                {
                    counter++;
                    continue;
                }

                lighthouses.Add(new Vector2(counter, positionY));
                counter++;
            }

            return lighthouses;
        }
        private static List<ICell> LineToCells(string line, int positionY)
        {
            List<ICell> cells = new List<ICell>();

            int counter = 0;
            foreach (char cellchar in line)
            {
                ICell cell = new Cell()
                {
                    Position = new Vector2(counter, positionY),
                    IsPlayable = cellchar != '#'
                };
                cells.Add(cell);
                counter++;
            }

            return cells;
        }
    }
}
