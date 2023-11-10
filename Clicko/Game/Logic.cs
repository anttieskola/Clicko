// Copyright © 2011 Antti Eskola. All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clicko
{
    /// <summary>
    /// This class contains logic of game.
    /// Purpose here is not to use any framework specific API's.
    /// Notes:
    /// - using int value range 0....255 should be more than enough
    /// - constructor contains table size atm
    /// </summary>
    class ALogic
    {
        /// <summary>
        /// Max number a cell can hold
        /// </summary>
        public const int cellMaxValue = AGlobals.GameTableCellMaxValue;

        /// <summary>
        /// Number of cells in x axis of game table
        /// </summary>
        public const int xSize = AGlobals.GameTableSizeX;

        /// <summary>
        /// Number of cells in y axis of game table
        /// </summary>
        public const int ySize = AGlobals.GameTableSizeY;

        /// <summary>
        /// Two dimensional game table
        /// should protect this from users
        /// </summary>
        public Int32[,] gameTable;

        /// <summary>
        /// Reduction history
        /// </summary>
        private List<int[]> _reductionHistory;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ALogic()
        {
            gameTable = new int[xSize,ySize];
            _reductionHistory = new List<int[]>();
            ResetTable();
        }
        
        /// <summary>
        /// Reset the game table
        /// </summary>
        public void ResetTable()
        {
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    gameTable[x,y] = 0;
                }
            }
        }

        /// <summary>
        /// Generate new game table
        /// </summary>
        /// <param name="additionCount">count of clicks</param>
        public void GenerateTable(int additionCount)
        {
            // Reset table as we generate new
            ResetTable();

            // Reset history
            _reductionHistory.Clear();

            // Just do random additions to table
            // according to count
            Random generator = new Random();
            for (int i = 0; i < additionCount; i++)
            {
                int xCoord = generator.Next(xSize);
                int yCoord = generator.Next(ySize);
                Addition(xCoord, yCoord);
            }
        }

        /// <summary>
        /// Do addition to given coordinate
        /// </summary>
        /// <param name="xCoordinate"></param>
        /// <param name="yCoordinate"></param>
        /// <returns>true if coordinate valid and max cell value's will not be reached</returns>
        public bool Addition(int xCoordinate, int yCoordinate)
        {
            bool maxValueFound = false;

            // Check coordinates are inside gametable
            if ((xCoordinate >= 0 && xCoordinate < xSize)
                && (yCoordinate >= 0 && yCoordinate < ySize))
            {
                // check values of cells we would modify
                // 0 v 0
                // v x v Here 'x' represent given coordinate 
                // 0 v 0 The 'v' represents value

                // First line
                if (yCoordinate >= 1)
                {
                    if ( gameTable[xCoordinate, yCoordinate - 1] >= cellMaxValue )
                    {
                        maxValueFound = true;
                    }
                }

                // Second line
                if (xCoordinate >= 1)
                {
                    if (gameTable[xCoordinate - 1, yCoordinate] >= cellMaxValue)
                    {
                        maxValueFound = true;
                    }
                }
                if (xCoordinate <= xSize - 2)
                {
                    if (gameTable[xCoordinate + 1, yCoordinate] >= cellMaxValue)
                    {
                        maxValueFound = true;
                    }
                }

                // Third line
                if (yCoordinate <= ySize - 2)
                {
                    if (gameTable[xCoordinate, yCoordinate + 1] >= cellMaxValue)
                    {
                        maxValueFound = true;
                    }
                }
            }
            // don't modify if max cell value found
            if (maxValueFound)
            {
                return false;
            }
            return ModifyTable(xCoordinate, yCoordinate, +1);
        }

        /// <summary>
        /// Check can you do reduction in given coordinate
        /// </summary>
        /// <param name="xCoordinate"></param>
        /// <param name="yCoordinate"></param>
        /// <returns></returns>
        public bool ReductionCheck(int xCoordinate, int yCoordinate)
        {
            // Return value
            bool ret = true;

            // Is it valid to do reduction ?
            // 0 c 0
            // c x c Here 'x' represent given coordinate 
            // 0 c 0 The 'c' represents spots to check

            // First line
            if (yCoordinate >= 1)
            {
                if (gameTable[xCoordinate, yCoordinate - 1] == 0)
                {
                    ret = false;
                }
            }
            // Second line
            if (xCoordinate >= 1)
            {
                if (gameTable[xCoordinate - 1, yCoordinate] == 0)
                {
                    ret = false;
                }
            }
            if (xCoordinate <= xSize - 2)
            {
                if (gameTable[xCoordinate + 1, yCoordinate] == 0)
                {
                    ret = false;
                }
            }
            // Third line
            if (yCoordinate <= ySize - 2)
            {
                if (gameTable[xCoordinate, yCoordinate + 1] == 0)
                {
                    ret = false;
                }
            }
            return ret;
        }

        /// <summary>
        /// Do reduction to given coordinate
        /// </summary>
        /// <param name="xCoordinate"></param>
        /// <param name="yCoordinate"></param>
        /// <returns>true if coordinate was valid</returns>
        public bool Reduction(int xCoordinate, int yCoordinate)
        {
            // check its valid
            bool ret = ReductionCheck(xCoordinate, yCoordinate);

            // modify table if it was valid spot
            if (ret)
            {
                ModifyTable(xCoordinate, yCoordinate, -1);
                int[] reduction = new int[2];
                reduction[0] = xCoordinate;
                reduction[1] = yCoordinate;
                _reductionHistory.Add(reduction);
            }
            return ret;
        }


        /// <summary>
        /// Check is the table been cleared, so fort is empty
        /// </summary>
        /// <returns>true if empty</returns>
        public bool IsTableEmpty()
        {
            bool ret = true;
            foreach (int i in gameTable)
            {
                if (i != 0)
                {
                    ret = false;
                    break;
                }
            }
            return ret;
        }

        /// <summary>
        /// Undo last reduction
        /// </summary>
        /// <returns>true if there was reduction to undo</returns>
        public bool Undo()
        {
            bool ret = false;
            if (_reductionHistory.Count > 0)
            {
                int[] lastMove = _reductionHistory.Last<int[]>();
                _reductionHistory.RemoveAt(_reductionHistory.Count-1);
                ret = Addition(lastMove[0], lastMove[1]);
            }
            return ret;
        }

        /// <summary>
        /// Checks is table locked so no more possible
        /// moves can be done anymore.
        /// </summary>
        /// <returns></returns>
        public bool IsTableLocked()
        {
            // just brutally check can we reduce any cells in game table
            bool tableLocked = true;
            // y axis
            for (int y = 0; y < ySize; y++)
            {
                // x axis
                for (int x = 0; x < xSize; x++)
                {
                    if (ReductionCheck(x, y))
                    {
                        tableLocked = false;
                        break;
                    }
                }
                if (!tableLocked)
                {
                    break;
                }
            }
            return tableLocked;
        }

        /// <summary>
        /// Modify table with given value
        /// </summary>
        /// <param name="xCoordinate"></param>
        /// <param name="yCoordinate"></param>
        /// <param name="value"></param>
        /// <returns>true if coordinates were valid</returns>
        private bool ModifyTable(int xCoordinate, int yCoordinate, int value)
        {
            // Return value
            bool ret = false;

            // Check coordinates are inside gametable
            if ((xCoordinate >= 0 && xCoordinate < xSize)
                && (yCoordinate >= 0 && yCoordinate < ySize))
            {
                ret = true;
                // Now we need to do modify values like
                // 0 v 0
                // v x v Here 'x' represent given coordinate 
                // 0 v 0 The 'v' represents value
                
                // First line
                if (yCoordinate >= 1)
                {
                    gameTable[xCoordinate, yCoordinate - 1] += value;
                }

                // Second line
                if (xCoordinate >= 1)
                {
                    gameTable[xCoordinate - 1, yCoordinate] += value;
                }
                if (xCoordinate <= xSize - 2)
                {
                    gameTable[xCoordinate + 1, yCoordinate] += value;
                }

                // Third line
                if (yCoordinate <= ySize - 2)
                {
                    gameTable[xCoordinate, yCoordinate + 1] += value;
                }
            }
            return ret;
        }
    }
}
