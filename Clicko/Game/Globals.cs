// Copyright © 2011 Antti Eskola. All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Clicko
{
    /// <summary>
    /// Helper for storing global static definitions, can't be instanciated.
    /// Declare all as public constants or if instances as statics.
    /// </summary>
    class AGlobals
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
        /// Size of game table in cells in y
        /// </summary>
        public const int GameTableCellSide = 60;

        /// <summary>
        /// Max "legal" value of game table cell
        /// </summary>
        public const int GameTableCellMaxValue = 20;

        /// <summary>
        /// Game Table Background tint color
        /// </summary>
        public static Color GameTableBackgroundColor = new Color( 200, 200, 200, 255 );

        /// <summary>
        /// Gane table cell tint color
        /// </summary>
        public static Color GameTableCellColor = new Color(255, 255, 255, 200);


        /// <summary>
        /// Color to draw disabled button in main menu
        /// </summary>
        public static Color mainMenuDisabledColor = new Color(255, 255, 255, 255);

        /// <summary>
        /// Private constructor
        /// </summary>
        private AGlobals() { }
    }
}
