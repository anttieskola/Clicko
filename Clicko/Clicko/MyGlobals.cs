#region copyright
// Copyright © 2011-2012 Antti Eskola. All rights reserved.
#endregion
namespace Clicko
{
    #region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
    #endregion

    /// <summary>
    /// Helper for storing global static definitions, can't be instanciated.
    /// Declare all as public constants or if instances as statics.
    /// </summary>
    public class MyGlobals
    {
        /// <summary>
        /// Size of game table in cells in x
        /// </summary>
        public const int GameTableSizeX = 8;

        /// <summary>
        /// Size of game table in cells in y
        /// </summary>
        public const int GameTableSizeY = 10;

        /// <summary>
        /// Max "legal" value of game table cell
        /// </summary>
        public const int GameTableCellMaxValue = 7;

        /// <summary>
        /// Size of game table in cells in y
        /// </summary>
        public const int GameTableCellSide = 60;

        /// <summary>
        /// Max level of the game (0...x)
        /// </summary>
        public const int GameMaxLevel = 10;
                
        /// <summary>
        /// Screen size x
        /// </summary>
        public static int ScreenSizeHorizontal = 480;

        /// <summary>
        /// Screen size y
        /// </summary>
        public static int ScreenSizeVertical = 800;

        /// <summary>
        /// Full screen rectanle
        /// </summary>
        public static Rectangle RectangleFullScreen = new Rectangle(0, 0, 480, 800);

        /// <summary>
        /// Colors from palette
        /// </summary>
        public static Color colorPrimary = new Color(67, 153, 116);
        public static Color colorSecondaryA = new Color(68, 111, 144);
        public static Color colorSecondaryB = new Color(138, 199, 87);
        public static Color colorComplementary = new Color(223, 134, 98);

        /// <summary>
        /// Common transition time of screens
        /// </summary>
        public static TimeSpan screenTransitionTime = TimeSpan.FromMilliseconds(333);

        /// Private constructor
        /// </summary>
        private MyGlobals() { }
    }
}
