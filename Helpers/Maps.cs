using Entities.Enums;
using System;
using System.Collections.Generic;

namespace Helpers
{
    public static class Maps
    {
        public static IEnumerable<string> GetFileLines(MapNames mapName)
        {
            string mapFilename = Enum.GetName(typeof(MapNames), mapName);
            string rawMap = Properties.Maps.ResourceManager.GetString(mapFilename);
            
            IEnumerable<string> lines = rawMap.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            return lines;
        }
    }
}