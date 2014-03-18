/*
 * ORIGINAL CLASS BY NIGHTWOLF FROM THE SNOWING's PROJECT ! All rights reserved !
*/

using SunDofus.DataRecords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.Game.Maps
{
    class Pathfinding
    {
        public int Destination, Direction;

        private string m_StrPath;
        private int m_StartCell;
        private int m_StartDir;
        private bool m_IsTrigger;

        private DB_Map map;

        public Pathfinding(string path, DB_Map map, int startCell, int startDir, bool trigger = false)
        {
            m_StrPath = path;
            this.map = map;
            this.m_StartCell = startCell;
            this.m_StartDir = startDir;
            this.m_IsTrigger = trigger;
        }

        public void UpdatePath(string path)
        {
            m_StrPath = path;
        }

        public string GetStartPath
        {
            get
            {
                return GetDirChar(m_StartDir) + GetCellChars(m_StartCell);
            }
        }

        public int GetCaseIDFromDirection(int caseID, char direction, bool fight)
        {
            switch (direction)
            {
                case 'a':
                    return fight ? -1 : caseID + 1;
                case 'b':
                    return caseID + map.Width;
                case 'c':
                    return fight ? -1 : caseID + (map.Width * 2 - 1);
                case 'd':
                    return caseID + (map.Width - 1);
                case 'e':
                    return fight ? -1 : caseID - 1;
                case 'f':
                    return caseID - map.Width;
                case 'g':
                    return fight ? -1 : caseID - (map.Width * 2 - 1);
                case 'h':
                    return caseID - map.Width + 1;
            }

            return -1;
        }

        public static int GetCellNum(string cellChars)
        {
            var hash = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";

            var numChar1 = hash.IndexOf(cellChars[0]) * hash.Length;
            var numChar2 = hash.IndexOf(cellChars[1]);

            return numChar1 + numChar2;
        }

        public static string GetCellChars(int cellNum)
        {
            var hash = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";

            var charCode2 = (cellNum % hash.Length);
            var charCode1 = (cellNum - charCode2) / hash.Length;

            return hash[charCode1].ToString() + hash[charCode2].ToString();
        }

        public static string GetDirChar(int dirNum)
        {
            var hash = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";

            if (dirNum >= hash.Length)
                return "";

            return hash[dirNum].ToString();
        }

        public static int GetDirNum(string dirChar)
        {
            var hash = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";
            return hash.IndexOf(dirChar);
        }

        public static int GetDistanceBetween(DB_Map map, int id1, int id2)
        {
            if (id1 == id2 || map == null)
                return 0;

            int diffX = Math.Abs(GetCellXCoord(map, id1) - GetCellXCoord(map, id2));
            int diffY = Math.Abs(GetCellYCoord(map, id1) - GetCellYCoord(map, id2));

            return (diffX + diffY);
        }

        public static int GetCellXCoord(DB_Map map, int cellID)
        {
            int width = map.Width;
            return ((cellID - (width - 1) * GetCellYCoord(map, cellID)) / width);
        }

        public static int GetCellYCoord(DB_Map map, int cellID)
        {
            int width = map.Width;
            int loc5 = (int)(cellID / ((width * 2) - 1));
            int loc6 = cellID - loc5 * ((width * 2) - 1);
            int loc7 = loc6 % width;

            return (loc5 - loc7);
        }

        public static int GetDirection(DB_Map map, int cell1, int cell2)
        {
            int[] ListChange = new int[] { 1, map.Width, map.Width * 2 - 1, map.Width - 1, -1, -map.Width, -map.Width * 2 + 1, -(map.Width - 1) };
            int Result = cell2 - cell1;

            for (int i = 7; i > -1; i--)
                if (Result == ListChange[i])
                    return i;

            int ResultX = GetCellXCoord(map, cell2) - GetCellXCoord(map, cell1);
            int ResultY = GetCellYCoord(map, cell2) - GetCellYCoord(map, cell1);

            if (ResultX == 0)
                if (ResultY > 0)
                    return 3;
                else
                    return 7;
            else if (ResultX > 0)
                return 1;
            else
                return 5;
        }

        public static int OppositeDirection(int direction)
        {
            return (direction >= 4 ? direction - 4 : direction + 4);
        }

        public static bool InLine(DB_Map map, int cell1, int cell2)
        {
            bool isX = GetCellXCoord(map, cell1) == GetCellXCoord(map, cell2);
            bool isY = GetCellYCoord(map, cell1) == GetCellYCoord(map, cell2);

            return isX || isY;
        }

        public static int NextCell(DB_Map map, int cell, int dir)
        {
            switch (dir)
            {
                case 0:
                    return cell + 1;

                case 1:
                    return cell + map.Width;

                case 2:
                    return cell + (map.Width * 2) - 1;

                case 3:
                    return cell + map.Width - 1;

                case 4:
                    return cell - 1;

                case 5:
                    return cell - map.Width;

                case 6:
                    return cell - (map.Width * 2) + 1;

                case 7:
                    return cell - map.Width + 1;

            }

            return -1;
        }

        public string RemakeLine(int lastCell, string cell, int finalCell)
        {
            var direction = GetDirNum(cell[0].ToString());
            var toCell = GetCellNum(cell.Substring(1));
            var lenght = 0;

            if (InLine(map, lastCell, toCell))
                lenght = GetEstimateDistanceBetween(lastCell, toCell);
            else
                lenght = int.Parse(Math.Truncate((GetEstimateDistanceBetween(lastCell, toCell) / 1.4)).ToString());

            var backCell = lastCell;
            var actuelCell = lastCell;

            for (var i = 1; i <= lenght; i++)
            {
                actuelCell = NextCell(map, actuelCell, direction);
                backCell = actuelCell;

                if (m_IsTrigger & map.Triggers.Any(x => x.CellID == backCell))
                    return GetDirChar(direction) + GetCellChars(backCell) + ",0";
            }

            return cell + ",1";
        }

        public string RemakePath()
        {
            var newPath = "";
            var newCell = GetCellNum(m_StrPath.Substring(m_StrPath.Length - 2, 2));
            var lastCell = m_StartCell;

            for (var i = 0; i <= m_StrPath.Length - 1; i += 3)
            {
                var actualCell = m_StrPath.Substring(i, 3);
                var lineData = RemakeLine(lastCell, actualCell, newCell).Split(',');
                newPath += lineData[0];

                if (lineData[1] == null)
                    return newPath;

                lastCell = GetCellNum(actualCell.Substring(1));
            }

            Destination = GetCellNum(newPath.Substring(newPath.Length - 2, 2));
            Direction = GetDirNum(newPath.Substring(newPath.Length - 3, 1));
            m_StrPath = newPath;

            return GetStartPath + newPath;
        }

        public int GetEstimateDistanceBetween(int id1, int id2)
        {
            if (id1 == id2 || map == null)
                return 0;

            var diffX = Math.Abs(GetCellXCoord(map, id1) - GetCellXCoord(map, id2));
            var diffY = Math.Abs(GetCellYCoord(map, id1) - GetCellYCoord(map, id2));

            return int.Parse(Math.Truncate(Math.Sqrt(Math.Pow(diffX, 2) + Math.Pow(diffY, 2))).ToString());
        }

        public int GetLength()
        {
            var length = 0;
            var lastCell = m_StartCell;
            var actualCell = -1;

            for (var i = 0; i <= m_StrPath.Length - 1; i += 3)
            {
                actualCell = GetCellNum(m_StrPath.Substring(i, 3).Substring(1));
                length += GetDistanceBetween(map, lastCell, actualCell);
                lastCell = actualCell;
            }

            return length;
        }
    }
}