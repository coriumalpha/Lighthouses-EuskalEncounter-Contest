using Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Arena
{
    public static class Parser
    {
        private static MapArena map;

        public static MapDTO LoadToMap(string filePath)
        {
            IEnumerable<string> lines = GetFileLines(filePath);
            List<ICell> cells = new List<ICell>();
            List<Vector2> lighthouses = new List<Vector2>();
            List<Vector2> playerSlots = new List<Vector2>();

            int counter = 0;
            foreach (string line in lines)
            {
                //TODO: Convendría refactorizar en una única función para evitar ciclos redundantes
                //No se prioriza al no formar parte del reto
                lighthouses.AddRange(LineToLighthouses(line, counter));
                cells.AddRange(LineToCells(line, counter));
                playerSlots.AddRange(LineToPlayerSlots(line, counter));
                counter++;
            }

            int sizeX = lines.First().Length;
            int sizeY = lines.Count();
            Parser.map = new MapArena(new Vector2(sizeX, sizeY), cells);

            MapDTO mapData = new MapDTO()
            {
                Map = map,
                Lighthouses = lighthouses,
                PlayerSlots = playerSlots
            };

            return mapData;
        }

        private static IEnumerable<string> GetFileLines(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new Exception("File doesn't exist");
            }

            return File.ReadLines(filePath);
        }

        private static List<Vector2> LineToPlayerSlots(string line, int positionY)
        {
            List<Vector2> playerSlots = new List<Vector2>();
            string slotchars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            int counter = 0;
            foreach (char cellchar in line)
            {
                if (!slotchars.Contains(cellchar))
                {
                    continue;
                }

                playerSlots.Add(new Vector2(counter, positionY));
                counter++;
            }

            return playerSlots;
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
