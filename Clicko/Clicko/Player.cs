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
    using System.IO;
    using System.IO.IsolatedStorage;
    #endregion
    /// <summary>
    /// Class representing player and all related
    /// Implemented using singleton pattern
    /// </summary>
    public class Player
    {
        #region Difficulty Settings
        /// <summary>
        /// Get number of clicks for given level
        /// "here we have difficulty formula"
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private static int Clicks(int level)
        {
            double clicks = Math.Pow((level + 1), 2);
            return (int)clicks;
        }
        /// <summary>
        /// How many seconds undo adds to clock
        /// </summary>
        public static int UndoSeconds
        {
            get { return 3; }
            set { }
        }
        #endregion

        #region Singleton Implementation
        /// <summary>
        /// Player instance
        /// </summary>
        public static Player Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Player();
                }
                return instance;
            }
        }
        private static Player instance;
        #endregion

        #region Properties

        /// <summary>
        /// Current Level
        /// </summary>
        public Int32 CurrentLevel
        {
            get { return _currentLevel; }
            set
            {
                _currentLevel = value;
                if (value > MaxLevel)
                {
                    MaxLevel = value;
                }
                ResetLevel();
            }
        }
        Int32 _currentLevel;
        
        /// <summary>
        /// Max level achieved
        /// </summary>
        public Int32 MaxLevel
        {
            get { return _maxLevel; }
            private set { _maxLevel = value; }
        }
        Int32 _maxLevel;

        /// <summary>
        /// Sound on?
        /// </summary>
        public bool SoundOn
        {
            get { return _soundOn; }
            set { _soundOn = value; }
        }
        bool _soundOn;

        /// <summary>
        /// Vibra on?
        /// </summary>
        public bool VibraOn
        {
            get { return _vibraOn; }
            set { _vibraOn = value; }
        }
        bool _vibraOn;
        
        /// <summary>
        /// Save available?
        /// </summary>
        public bool SaveAvailable
        {
            get { return _saveAvailable; }
            private set { _saveAvailable = value; }
        }
        bool _saveAvailable;
        
        /// <summary>
        /// Players game's logic
        /// </summary>
        public Logic Logic
        {
            get { return _logic; }
            private set { _logic = value; }
        }
        Logic _logic;
        
        /// <summary>
        /// Player's game level timer
        /// </summary>
        public LevelTimer LevelTimer
        {
            get { return _levelTimer; }
            private set { _levelTimer = value; }
        }
        LevelTimer _levelTimer;

        public HighScores HighScores
        {
            get { return _highScores; }
            private set { _highScores = value; }
        }
        HighScores _highScores;
        #endregion

        #region Initialization
        public Player()
        {
            // logic
            Logic = new Logic();
            // Scores
            HighScores = new HighScores();
            // timer
            LevelTimer = new LevelTimer();
            // stats
            MaxLevel = 1;
            CurrentLevel = 1;
            // Setup
            VibraOn = true;
            SoundOn = true;
        }
        #endregion

        #region Public Methods
        public void ResetLevel()
        {
            Logic.GenerateTable(Clicks(CurrentLevel));
            LevelTimer.Reset();
        }
        public bool NextLevel()
        {
            if (CurrentLevel + 1 > MyGlobals.GameMaxLevel)
            {
                return false;
            }
            CurrentLevel++;
            ResetLevel();
            return true;
        }
        public bool FirstLevel()
        {
            CurrentLevel = 1;
            ResetLevel();
            return true;
        }
        public string LevelString()
        {
            string str = "Level ";
            str += _currentLevel.ToString();
            return str;
        }
        public void Save()
        {
            SaveStats();
            SaveGame();
            SaveHighScores();
            SaveSetup();
        }
        public void Load()
        {
            LoadStats();
            LoadGame();
            LoadHighScores();
            LoadSetup();
        }
        #endregion

        #region Save/Load Game
        private void SaveGame()
        {
            using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (BinaryWriter writer = new BinaryWriter(file.CreateFile("playerSave")))
                {
                    SaveAvailable = true;
                    try
                    {
                        writer.Write(LevelTimer.Time);
                        for (int y = 0; y < Logic.ySize; y++)
                        {
                            for (int x = 0; x < Logic.xSize; x++)
                            {
                                writer.Write(Logic.GameTable[x, y]);
                            }
                        }
                        Int32 historyLength = Logic.ReductionHistory.Count;
                        writer.Write(historyLength);
                        for (int x = 0; x < Logic.ReductionHistory.Count; x++)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Int32 value = Logic.ReductionHistory[x][i];
                                writer.Write(value);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        SaveAvailable = false;
                    }
                }
            }
        }
        private void LoadGame()
        {
            using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (file.FileExists("playerSave"))
                {
                    SaveAvailable = true;
                    using (BinaryReader reader = new BinaryReader(file.OpenFile("playerSave", FileMode.Open)))
                    {
                        try
                        {
                            // Saved time
                            LevelTimer.Time = reader.ReadInt32();
                            // Logic
                            for (int y = 0; y < Logic.ySize; y++)
                            {
                                for (int x = 0; x < Logic.xSize; x++)
                                {
                                    Logic.GameTable[x, y] = reader.ReadInt32();
                                }
                            }
                            int historyLength = reader.ReadInt32();
                            for (int x = 0; x < historyLength; x++)
                            {
                                int[] reduction = new int[2];
                                reduction[0] = reader.ReadInt32();
                                reduction[1] = reader.ReadInt32();
                                Logic.ReductionHistory.Add(reduction);
                            }
                        }
                        catch (Exception)
                        {
                            SaveAvailable = false;
                        }
                    }
                }
            }
        }
        #endregion

        #region Save/Load Stats
        private void SaveStats()
        {
            using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (BinaryWriter writer = new BinaryWriter(file.CreateFile("playerStats")))
                {
                    try
                    {
                        writer.Write(_currentLevel);
                        writer.Write(_maxLevel);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
        private void LoadStats()
        {
            using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (file.FileExists("playerStats"))
                {
                    using (BinaryReader reader = new BinaryReader(file.OpenFile("playerStats", FileMode.Open)))
                    {
                        try
                        {
                            _currentLevel = reader.ReadInt32();
                            _maxLevel = reader.ReadInt32();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
        }
        #endregion

        #region Save/Load High Scores
        private void SaveHighScores()
        {
            using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (BinaryWriter writer = new BinaryWriter(file.CreateFile("highScores")))
                {
                    try
                    {
                        for (int i = 0; i < MyGlobals.GameMaxLevel; i++)
                        {
                            writer.Write((Int32)HighScores.Times[i]);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
        private void LoadHighScores()
        {
            using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (file.FileExists("highScores"))
                {
                    using (BinaryReader reader = new BinaryReader(file.OpenFile("highScores", FileMode.Open)))
                    {
                        try
                        {
                            for (int i = 0; i < MyGlobals.GameMaxLevel; i++)
                            {
                                HighScores.Times[i] = reader.ReadInt32();
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
        }
        #endregion
        #region Save/Load setup
        private void SaveSetup()
        {
            using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (BinaryWriter writer = new BinaryWriter(file.CreateFile("playerSetup")))
                {
                    try
                    {
                        writer.Write(_soundOn);
                        writer.Write(_vibraOn);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
        private void LoadSetup()
        {
            using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (file.FileExists("playerSetup"))
                {
                    using (BinaryReader reader = new BinaryReader(file.OpenFile("playerSetup", FileMode.Open)))
                    {
                        try
                        {
                            _soundOn = reader.ReadBoolean();
                            _vibraOn = reader.ReadBoolean();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
        }
        #endregion
    }
}
