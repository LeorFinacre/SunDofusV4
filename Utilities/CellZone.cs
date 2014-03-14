using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunDofus.World.Game.Maps;

namespace SunDofus.Utilities
{
    class CellZone
    {
        public static List<int> GetAdjacentCells(Map map, int cell)
        {
            var cells = new List<int>();

            for (var i = 1; i < 8; i += 2)
                cells.Add(Pathfinding.NextCell(map, cell, i));

            return cells;
        }

        public static List<int> GetLineCells(Map map, int cell, int direction, int length)
        {
            var cells = new List<int>();
            var lastCell = cell;

            for (var i = 0; i < length; i++)
            {
                cells.Add(Pathfinding.NextCell(map, lastCell, direction));
                lastCell = cells[i];
            }

            return cells;
        }

        public static List<int> GetCircleCells(Map map, int currentCell, int radius)
        {
            var cells = new List<int>() { currentCell };

            for (var i = 0; i < radius; i++)
            {
                var copy = cells.ToArray();

                foreach (var cell in copy)
                    cells.AddRange(from Item in GetAdjacentCells(map, cell) where !cells.Contains(Item) select Item);
            }

            return cells;
        }

        public static List<int> GetCrossCells(Map map, int currentCell, int radius)
        {
            var cells = new List<int>();

            foreach (var cell in GetCircleCells(map, currentCell, radius))
            {
                if (Pathfinding.InLine(map, currentCell, cell))
                    cells.Add(cell);
            }

            return cells;
        }

        public static List<int> GetTLineCells(Map map, int cell, int direction, int length)
        {
            var lineDirection = direction <= 5 ? direction + 2 : direction - 6;
            var cells = new List<int>();

            cells.AddRange(GetLineCells(map, cell, lineDirection, length));
            cells.AddRange(GetLineCells(map, cell, Pathfinding.OppositeDirection(lineDirection), length));

            return cells;
        }

        public static List<int> GetCells(Map map, int cell, int currentCell, string range)
        {
            switch (range[0])
            {
                case 'C':
                    return GetCircleCells(map, cell, Basic.HASH.IndexOf(range[1]));

                case 'X':
                    return GetCrossCells(map, cell, Basic.HASH.IndexOf(range[1]));

                case 'T':
                    var cells1 = new List<int> { cell };
                    cells1.AddRange(GetTLineCells(map, cell, Pathfinding.GetDirection(map, currentCell, cell), Basic.HASH.IndexOf(range[1])));
                    return cells1;

                case 'L':
                    var cells2 = new List<int> { cell };
                    cells2.AddRange(GetLineCells(map, cell, Pathfinding.GetDirection(map, currentCell, cell), Basic.HASH.IndexOf(range[1])));
                    return cells2;
            }

            return new List<int>() { cell };
        }
    }
}