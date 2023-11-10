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
    #endregion

    public class HighScores
    {
        #region Public Fields
        public int[] Times;
        #endregion

        #region Initialization
        public HighScores()
        {
            Times = new int[MyGlobals.GameMaxLevel];
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Get given level fastest time formatted as mins::seconds
        /// Note! levels start at 1
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public String FastestTimeFormatted(int level)
        {
            level--;

            int minutes = Times[level] / 60;
            int seconds = Times[level] % 60;
            String text = String.Empty;

            if (minutes != 0 || seconds != 0)
            {
                if (minutes < 10)
                {
                    text += "0";
                }
                text += minutes.ToString();

                text += ":";

                if (seconds < 10)
                {
                    text += "0";
                }
                text += seconds.ToString();
            }
            else
            {
                text += "--:--";
            }

            return text;
        }

        public bool NewTime(int level, int time)
        {
            if (time < Times[level - 1] || Times[level - 1] == 0)
            {
                Times[level - 1] = time;
                return true;
            }
            return false;
        }
        #endregion
    }
}
