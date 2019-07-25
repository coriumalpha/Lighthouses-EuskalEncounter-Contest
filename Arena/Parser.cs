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
        private static IEnumerable<string> GetFileLines(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new Exception("File doesn't exist");
            }

            return File.ReadLines(filePath);
        }

        public static MapArena LoadToMap(string filePath)
        {
            IEnumerable<string> lines = GetFileLines(filePath).Reverse();
            List<ICell> cells = new List<ICell>();

            int counter = 0;
            foreach (string line in lines)
            {
                cells.AddRange(LineToCells(ref map, line, counter));
                counter++;
            }

            int sizeX = lines.First().Length;
            int sizeY = lines.Count();
            Parser.map = new MapArena(new Vector2(sizeX, sizeY), cells);

            return map;
        }

        private static List<Cell> LineToCells(ref MapArena map, string line, int positionY)
        {
            List<Cell> cells = new List<Cell>();

            int counter = 0;
            foreach (char cellchar in line)
            {
                Cell cell = new Cell();
                cell.Position = new Vector2(counter, positionY);

                switch (cellchar)
                {
                    case '#':
                        cell.IsPlayable = false;
                        break;
                    case '!':
                        cell.IsPlayable = true;
                        break;
                    default:
                        cell.IsPlayable = true;
                        break;
                }

                cells.Add(cell);
                counter++;
            }

            return cells;
        }
    }
}
