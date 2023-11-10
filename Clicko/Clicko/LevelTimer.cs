#region copyright
// Copyright © 2011-2012 Antti Eskola. All rights reserved.
#endregion
namespace Clicko
{
    #region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.GamerServices;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.Input.Touch;
    using Microsoft.Xna.Framework.Media;
    using AEngine;
    #endregion
    
    public class LevelTimer
    {
        #region Properties
        /// <summary>
        /// If want to save,load time from file or
        /// for fastest time screen
        /// </summary>
        public int Time
        {
            get
            {
                return (_currentTime - _startTime) + _time;
            }
            set
            {
                _currentTime = 0;
                _startTime = 0;
                _started = false;
                _paused = false;
                _time = value;
            }
        }
        int _time;
        #endregion

        #region Private Fields
        int _currentTime;
        int _startTime;
        bool _started;
        bool _paused;
        #endregion

        #region Initialization
        public LevelTimer()
        {
            Time = 0;
            _currentTime = 0;
            _startTime = 0;
            _started = false;
            _paused = false;
        }
        #endregion

        #region Public Methods
        public string FormattedTimeString()
        {
            int elapsed = (_currentTime - _startTime) + _time;
            double mins = elapsed / 60;
            double seconds = elapsed % 60;
            String str = String.Empty;
            if (mins == 0)
            {
                str += "00";
            }
            else if (mins < 10)
            {
                str += "0";
                str += mins.ToString();
            }
            else
            {
                str += mins.ToString();
            }
            str += ":";
            if (seconds < 10)
            {
                str += "0";
            }
            str += seconds.ToString();
            return str;
            
        }
        public void Pause()
        {
            _paused = true;
            _time += (_currentTime - _startTime);
            _currentTime = 0;
            _startTime = 0;
            _started = false;
        }
        public void Continue()
        {
            _paused = false;
        }
        public void Update(GameTime time)
        {
            if (!_paused)
            {
                if (!_started)
                {
                    _startTime = (int)(time.TotalGameTime.Ticks / 10000000);
                    _started = true;
                }
                _currentTime = (int)(time.TotalGameTime.Ticks / 10000000);
            }
        }
        public void Reset()
        {
            Time = 0;
            _currentTime = 0;
            _startTime = 0;
            _started = false;
            _paused = false;
        }
        public void Add(int seconds)
        {
            _startTime = _startTime - seconds;
        }
        #endregion
    }
}
