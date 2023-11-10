#region copyright
// Copyright © 2011-2012 Antti Eskola. All rights reserved.
#endregion
namespace Clicko
{
    #region Using Statements
    using System;
    using System.IO;
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
    using GameStateManagement;
    using AEngine;
    #endregion

    /// <summary>
    /// 
    /// </summary>
    class GamePlayScreen : AGameScreen, IAAnimationEndListener, IBlockEventListener
    {
        #region Private Constants
        /// <summary>
        /// UI location rectangles in game
        /// </summary>
        private static Rectangle _rectangleGameTableBackground = new Rectangle(0, 0, 480, 600);
        private static Rectangle _rectangleGameTable = new Rectangle(0, 0, 480, 600);
        private static Point _pointMessageOne = new Point(240, 150);
        private static Point _pointMessageTwo = new Point(240, 250);
        private static Point _pointMessageThree = new Point(240, 300);
        private static Rectangle _rectangleGameMenu = new Rectangle(0, 600, 480, 200);
        private static Rectangle _rectangleGameMenuItem1 = new Rectangle(20, 610, 200, 80);
        private static Rectangle _rectangleGameMenuItem2 = new Rectangle(260, 610, 200, 80);
        private static Rectangle _rectangleGameMenuItem3 = new Rectangle(20, 700, 200, 80);
        private static Rectangle _rectangleGameMenuItem4 = new Rectangle(260, 700, 200, 80);
        /// <summary>
        /// Game Table Background tint color
        /// </summary>
        public static Color _gameTableBackgroundColor = new Color(200, 200, 200, 255);
        /// <summary>
        /// Background number max
        /// </summary>
        private const int _maxBackGroundNumber = 10;
        /// <summary>
        /// Click sound number max
        /// </summary>
        private const int _maxclickSoundNumber = 3;
        #endregion

        #region Private Fields
        /// <summary>
        /// Content manager
        /// </summary>
        private ContentManager _content;
        /// <summary>
        /// Disabled all input?
        /// </summary>
        private bool _disableInput;
        /// <summary>
        /// Blocks
        /// </summary>
        private Block[,] _blocks;
        /// <summary>
        /// Crosshair
        /// </summary>
        private bool _crossHairEnabled;
        private bool _crossHairOk;
        private Rectangle _rectangleCrossHair;
        /// <summary>
        /// What game table background are we currently using
        /// </summary>
        private int _gameTableBackGroundNumber;
        /// <summary>
        /// Which click sound are we currently using
        /// </summary>
        private int _gameClickSoundNumber;
        /// <summary>
        /// To store what block was clicked as we use single animation event
        /// </summary>
        private int[] _blockReductionMemory = new int[2];
        /// <summary>
        /// Helper variable to tell us did we do reduction or not as we use single animation event
        /// </summary>
        private bool _blockReductionDone = true;
        /// <summary>
        /// 
        /// </summary>
        private AAnimatedTexture _gameButtonUndo;
        /// <summary>
        /// 
        /// </summary>
        private AAnimatedTexture _gameButtonPause;
        /// <summary>
        /// 
        /// </summary>
        private AAnimatedTexture _gameCrossHairRed;
        /// <summary>
        /// 
        /// </summary>
        private AAnimatedTexture _gameCrossHairGreen;
        /// <summary>
        /// 
        /// </summary>
        private ASpriteFont _gameMenuFont;
        /// <summary>
        /// 
        /// </summary>
        private AAnimatedSpriteFont _gameMessageLevelComplete;
        /// <summary>
        /// 
        /// </summary>
        private AAnimatedSpriteFont _gameMessageLevelFastestTime;
        /// <summary>
        /// 
        /// </summary>
        private AAnimatedSpriteFont _gameMessageNoMoreClicks;
        /// <summary>
        /// 
        /// </summary>
        private AAnimatedSpriteFont _gameMessagePlusSeconds;
        /// <summary>
        /// 
        /// </summary>
        private AAnimatedSpriteFont _gameMessageGameComplete;
        /// <summary>
        /// Backgrounds
        /// </summary>
        private ATexture[] _backGroundTable;
        /// <summary>
        /// Game menu texture (base)
        /// </summary>
        private ATexture _gameMenu;
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor
        /// </summary>
        public GamePlayScreen()
        {
            TransitionOnTime = MyGlobals.screenTransitionTime;
            TransitionOffTime = MyGlobals.screenTransitionTime;
         }
        /// <summary>
        /// Load all content for the screen
        /// </summary>
        public override void LoadContent()
        {
            // content manager
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");
            _disableInput = false;

            _blocks = new Block[MyGlobals.GameTableSizeX, MyGlobals.GameTableSizeY];
            Rectangle position = new Rectangle(_rectangleGameTable.X, _rectangleGameTable.Y, MyGlobals.GameTableCellSide, MyGlobals.GameTableCellSide);
            for (int y = 0; y < MyGlobals.GameTableSizeY; y++)
            {
                for (int x = 0; x < MyGlobals.GameTableSizeX; x++)
                {
                    _blocks[x, y] = new Block(_content, this, position);
                    position.X += MyGlobals.GameTableCellSide;
                }
                position.X = _rectangleGameTable.X;
                position.Y += MyGlobals.GameTableCellSide;
            }

            // in game button anim
            _gameButtonUndo = new AAnimatedTexture(_content, "button_200", 200, false, false);
            _gameButtonPause = new AAnimatedTexture(_content, "button_200", 200, false, false);

            // crosshairs
            _gameCrossHairRed = new AAnimatedTexture(_content, "crosshair_red", 180, true, true);
            _gameCrossHairGreen = new AAnimatedTexture(_content, "crosshair_green", 180, true, true);
            _crossHairEnabled = false;
            _crossHairOk = false;
            _rectangleCrossHair = new Rectangle(0, 0, 180, 180);

            // in game font
            _gameMenuFont = new ASpriteFont(_content, "gameFont");

            // Game messages
            _gameMessageLevelComplete = new AAnimatedSpriteFont(_content, "gameMessage35", this, 1);
            _gameMessageGameComplete = new AAnimatedSpriteFont(_content, "gameMessage30", this, 2);
            _gameMessageLevelFastestTime = new AAnimatedSpriteFont(_content, "gameMessage35");
            _gameMessageNoMoreClicks = new AAnimatedSpriteFont(_content, "gameMessage35");
            _gameMessagePlusSeconds = new AAnimatedSpriteFont(_content, "gameMessage35");

            // Load all backgrounds (simple table)
            _backGroundTable = new ATexture[_maxBackGroundNumber+1];
            const String backGroundPrefix = "bg_";
            for (int i = 0; i <= _maxBackGroundNumber; i++)
            {
                String backGroundString = backGroundPrefix + i.ToString();
                _backGroundTable[i] = new ATexture(_content, backGroundString);
            }

            // bg and sound randomization
            RandomBackground();
            RandomClickSound();

            // in game menu
            _gameMenu = new ATexture(_content, "game_menu");
        }
        /// <summary>
        /// Unloading of screen content
        /// </summary>
        public override void UnloadContent()
        {
            _content.Unload();
        }
        #endregion

        #region Animation end listener
        /// <summary>
        /// Listener implementation for animations
        /// </summary>
        /// <param name="e"></param>
        public void AAnimationEnded(int e)
        {
            switch (e)
            {
                case 1:
                    // level complete
                    if (Player.Instance.NextLevel())
                    {
                        RandomBackground();
                        RandomClickSound();
                        _disableInput = false;
                    }
                    // game complete
                    else
                    {
                        _gameMessageGameComplete.StartFadeInAndOutAnimation("CONGRATULATIONS\nMASTER CLICKER\nGAME COMPLETE!", _pointMessageOne, MyGlobals.colorComplementary, 180);
                    }
                    break;
                case 2:
                    // game complete
                    RandomBackground();
                    RandomClickSound();
                    _disableInput = false;
                    Player.Instance.FirstLevel();
                    LoadAndExitAllScreens(new MainMenuScreen());
                    break;
                default:
                    throw new Exception("Invalid animation end event");
            }
        }
        #endregion

        #region Block even listener
        /// <summary>
        /// Listener for block events
        /// </summary>
        /// <param name="e"></param>
        public void BlockEventSignal(BlockEvent e)
        {
            switch (e)
            {
                case BlockEvent.StateChange:
                    if (!_blockReductionDone)
                    {
                        // We now do reduction
                        _blockReductionDone = true;

                        // do reduction into logic
                        Player.Instance.Logic.Reduction(_blockReductionMemory[0], _blockReductionMemory[1]);

                        // Check is level complete
                        if (Player.Instance.Logic.IsTableEmpty())
                        {
                            Player.Instance.LevelTimer.Pause();
                            _gameMessageLevelComplete.StartFadeInAndOutAnimation("LEVEL COMPLETE", _pointMessageOne, MyGlobals.colorComplementary);
                            if (Player.Instance.HighScores.NewTime(Player.Instance.CurrentLevel, Player.Instance.LevelTimer.Time))
                            {
                                _gameMessageLevelFastestTime.StartFadeInAndOutAnimation("NEW BEST TIME!", _pointMessageTwo, MyGlobals.colorComplementary);
                                if (Player.Instance.SoundOn)
                                {
                                    ScreenManager.fx.PlaySfx("complete_best_time");
                                }
                            }
                            else
                            {
                                if (Player.Instance.SoundOn)
                                {
                                    ScreenManager.fx.PlaySfx("complete");
                                }
                            }
                            // disable input for complete animations
                            _disableInput = true;
                        }
                        // Is level locked
                        else if (Player.Instance.Logic.IsTableLocked())
                        {
                            if (Player.Instance.SoundOn)
                            {
                                ScreenManager.fx.PlaySfx("error");
                            }
                            // vibration
                            if (Player.Instance.VibraOn)
                            {
                                ScreenManager.fx.VibrationCommonErrorBig();
                            }
                            // vibration
                            if (Player.Instance.VibraOn)
                            {
                                ScreenManager.fx.VibrationCommonError();
                            }
                            // launch no more moves text animation
                            _gameMessageNoMoreClicks.StartFadeInAnimation("NO MORE MOVES\nTry undo or restart.", _pointMessageOne, MyGlobals.colorComplementary);
                        }
                    }
                    break;
                default:
                    throw new Exception("AEngine::AEntityEventChange unknown event");
            }
        }
        #endregion

        #region Update, Draw and HandleInput
        /// <summary>
        /// Update, called always
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="otherScreenHasFocus"></param>
        /// <param name="coveredByOtherScreen"></param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            if (!Player.Instance.Logic.IsTableLocked())
            {
                _gameMessageNoMoreClicks.Active = false;
            }
        }
        /// <summary>
        /// Handle user input, called when active
        /// </summary>
        /// <param name="input"></param>
        public override void HandleInput(AInputState input)
        {
            // during transition don't accept input
            if (this.ScreenState != ScreenState.Active)
            {
                return;
            }

            // skip all, if input is disabled
            if (_disableInput)
            {
                return;
            }

            // back button
            if (input.CurrentGamePadStates.Buttons.Back == ButtonState.Pressed)
            {
                Player.Instance.LevelTimer.Pause();
                LoadScreen(new GamePlayPauseScreen());
            }

            // this screen has popup pause menu also, so have to reset animations when no input
            if (input.TouchState.Count == 0)
            {
                _crossHairEnabled = false;
                _gameButtonPause.StopAnimationAndRewind();
                _gameButtonUndo.StopAnimationAndRewind();
            }

            for (int i = 0; i < input.TouchState.Count; i++)
            {
                TouchLocation touchLocation = input.TouchState[i];
                int x = (int)touchLocation.Position.X;
                int y = (int)touchLocation.Position.Y;
                Point xy = new Point(x, y);

                if (touchLocation.State == TouchLocationState.Pressed
                    || touchLocation.State == TouchLocationState.Moved)
                {
                    // enable crosshair and button animations
                    if (_rectangleGameTable.Contains(xy))
                    {
                        // easiest way to calculate cell what input clicks
                        // is to just divide x,y values with side lenght of cell texture
                        int xCellCoord = x / MyGlobals.GameTableCellSide;
                        int yCellCoord = (y - _rectangleGameTable.Y) / MyGlobals.GameTableCellSide;
                        // set crosshair rectangle
                        //  |
                        // -x-
                        //  |
                        _rectangleCrossHair.X = xCellCoord * MyGlobals.GameTableCellSide - MyGlobals.GameTableCellSide;
                        _rectangleCrossHair.Y = yCellCoord * MyGlobals.GameTableCellSide - MyGlobals.GameTableCellSide;
                        // draw green crosshair if reduction ok otherwise red
                        if (Player.Instance.Logic.ReductionCheck(xCellCoord, yCellCoord))
                        {
                            _crossHairOk = true;
                            _crossHairEnabled = true;
                        }
                        else
                        {
                            _crossHairOk = false;
                            _crossHairEnabled = true;
                        }
                    }
                    // undo
                    else if (_rectangleGameMenuItem3.Contains(xy) && Player.Instance.Logic.IsUndoAvailable())
                    {
                        _gameButtonUndo.PlayAnimation(true);
                        _gameButtonPause.StopAnimationAndRewind();
                    }
                    // pause
                    else if (_rectangleGameMenuItem4.Contains(xy))
                    {
                        _gameButtonUndo.StopAnimationAndRewind();
                        _gameButtonPause.PlayAnimation(true);
                    }
                    else
                    {
                        _crossHairEnabled = false;
                        _gameButtonPause.StopAnimationAndRewind();
                        _gameButtonUndo.StopAnimationAndRewind();
                    }
                }
                else
                {
                    // always disable crosshair when finger released
                    _crossHairEnabled = false;

                    if (_rectangleGameTable.Contains(xy))
                    {
                        // easiest way to calculate cell what input clicks
                        // is to just divide x,y values with side lenght of cell texture
                        int xCellCoord = x / MyGlobals.GameTableCellSide;
                        int yCellCoord = (y - _rectangleGameTable.Y) / MyGlobals.GameTableCellSide;

                        // launch animations if position is valid
                        if (Player.Instance.Logic.ReductionCheck(xCellCoord, yCellCoord))
                        {
                            // sound effect
                            if (Player.Instance.SoundOn)
                            {
                                ScreenManager.fx.PlaySfx(String.Format("click_{0}", _gameClickSoundNumber));
                            }
                            // vibration
                            if (Player.Instance.VibraOn)
                            {
                                ScreenManager.fx.VibrationVeryCommonSelect();
                            }

                            // Set reduction not done
                            _blockReductionDone = false;

                            // save reduction spot
                            _blockReductionMemory[0] = xCellCoord;
                            _blockReductionMemory[1] = yCellCoord;

                            // First line
                            if (yCellCoord >= 1)
                                _blocks[xCellCoord, yCellCoord - 1].SetStateAndDirection(BlockState.Reduction, BlockDirection.Up);

                            // Second line
                            if (xCellCoord >= 1)
                                _blocks[xCellCoord - 1, yCellCoord].SetStateAndDirection(BlockState.Reduction, BlockDirection.Left);

                            if (xCellCoord <= MyGlobals.GameTableSizeX - 2)
                                _blocks[xCellCoord + 1, yCellCoord].SetStateAndDirection(BlockState.Reduction, BlockDirection.Right);

                            // Third line
                            if (yCellCoord <= MyGlobals.GameTableSizeY - 2)
                                _blocks[xCellCoord, yCellCoord + 1].SetStateAndDirection(BlockState.Reduction, BlockDirection.Down);
                        }
                        // launch error animations
                        else
                        {
                            // sound effect
                            if (Player.Instance.SoundOn)
                            {
                                ScreenManager.fx.PlaySfx("error");
                            }
                            // vibration
                            if (Player.Instance.VibraOn)
                            {
                                ScreenManager.fx.VibrationCommonError();
                            }

                            // First line
                            if (yCellCoord >= 1)
                            {
                                if (Player.Instance.Logic.GameTable[xCellCoord, yCellCoord - 1] < 1)
                                    _blocks[xCellCoord, yCellCoord - 1].SetStateAndDirection(BlockState.FalseReduction, BlockDirection.Up);
                            }

                            // Second line
                            if (xCellCoord >= 1)
                            {
                                if (Player.Instance.Logic.GameTable[xCellCoord - 1, yCellCoord] < 1)
                                    _blocks[xCellCoord - 1, yCellCoord].SetStateAndDirection(BlockState.FalseReduction, BlockDirection.Left);
                            }

                            if (xCellCoord <= MyGlobals.GameTableSizeX - 2)
                            {
                                if (Player.Instance.Logic.GameTable[xCellCoord + 1, yCellCoord] < 1)
                                    _blocks[xCellCoord + 1, yCellCoord].SetStateAndDirection(BlockState.FalseReduction, BlockDirection.Right);
                            }

                            // Third line
                            if (yCellCoord <= MyGlobals.GameTableSizeY - 2)
                            {
                                if (Player.Instance.Logic.GameTable[xCellCoord, yCellCoord + 1] < 1)
                                    _blocks[xCellCoord, yCellCoord + 1].SetStateAndDirection(BlockState.FalseReduction, BlockDirection.Down);
                            }
                        }
                    }
                    // undo
                    else if (_rectangleGameMenuItem3.Contains(xy) && Player.Instance.Logic.IsUndoAvailable())
                    {
                        SelectEffect();                        
                        _gameMessagePlusSeconds.StartFadeInAndOutAnimation(String.Format("+{0} secs", Player.UndoSeconds), _pointMessageThree, MyGlobals.colorComplementary);
                        Player.Instance.LevelTimer.Add(Player.UndoSeconds);
                        Player.Instance.Logic.Undo();
                    }
                    // pause
                    else if (_rectangleGameMenuItem4.Contains(xy))
                    {
                        SelectEffect();
                        Player.Instance.LevelTimer.Pause();
                        LoadScreen(new GamePlayPauseScreen());
                        _gameButtonPause.StopAnimationAndRewind();
                    }
                }
            }
        }
        /// <summary>
        /// Draw screen, called when active
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            // update timer
            Player.Instance.LevelTimer.Update(gameTime);

            // Get the batch
            SpriteBatch sb = ScreenManager.spriteBatch;
            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            // Draw pictures first

            // Game table background
            _backGroundTable[_gameTableBackGroundNumber].Draw(sb,
                _rectangleGameTableBackground, _gameTableBackgroundColor);

            // Game table content
            for (int y = 0; y < Logic.ySize; y++)
            {
                for (int x = 0; x < Logic.xSize; x++)
                {
                    _blocks[x, y].Draw(sb, Player.Instance.Logic.GameTable[x, y]);
                }
            }

            // Crosshair
            if (_crossHairEnabled)
            {
                if (_crossHairOk)
                {
                    _gameCrossHairGreen.Draw(sb, _rectangleCrossHair, Color.White);
                }
                else
                {
                    _gameCrossHairRed.Draw(sb, _rectangleCrossHair, Color.White);
                }
            }

            // Game menu
            _gameMenu.Draw(sb, _rectangleGameMenu, Color.White);
            if (Player.Instance.Logic.IsUndoAvailable())
                _gameButtonUndo.Draw(sb, _rectangleGameMenuItem3, Color.White);
            _gameButtonPause.Draw(sb, _rectangleGameMenuItem4, Color.White);

            // texts
            _gameMenuFont.DrawCentered(sb,
                Player.Instance.LevelString(),
                _rectangleGameMenuItem1,
                Color.White);

            _gameMenuFont.DrawCentered(sb,
                Player.Instance.LevelTimer.FormattedTimeString(),
                _rectangleGameMenuItem2,
                Color.White);

            if (Player.Instance.Logic.IsUndoAvailable())
            {
                _gameMenuFont.DrawCentered(sb,
                    "UNDO",
                    _rectangleGameMenuItem3,
                    Color.White);
            }

            _gameMenuFont.DrawCentered(sb,
                "PAUSE",
                _rectangleGameMenuItem4,
                Color.White);

            _gameMessageLevelComplete.Draw(sb);
            _gameMessageGameComplete.Draw(sb);
            _gameMessageLevelFastestTime.Draw(sb);
            _gameMessageNoMoreClicks.Draw(sb);
            _gameMessagePlusSeconds.Draw(sb);

            // End drawing of text
            sb.End();
        }

        public override void DrawTransitionOn(GameTime gameTime)
        {
            // draw
            Draw(gameTime);

            // place tinting on top of it
            Color color = Color.Black;
            color.A = (byte)(TransitionPosition * 255);
            SpriteBatch sb = ScreenManager.SpriteBatch;
            sb.Begin();
            Texture2D texture = new Texture2D(ScreenManager.GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.Black });
            sb.Draw(texture, MyGlobals.RectangleFullScreen, color);
            sb.End();
        }

        public override void DrawTransitionOff(GameTime gameTime)
        {
            // draw
            Draw(gameTime);

            // place tinting on top of it
            Color color = Color.Black;
            color.A = (byte)(255f - (TransitionPosition * 255));
            SpriteBatch sb = ScreenManager.SpriteBatch;
            sb.Begin();
            Texture2D texture = new Texture2D(ScreenManager.GraphicsDevice, 1, 1);
            texture.SetData(new[] { Color.Black });
            sb.Draw(texture, MyGlobals.RectangleFullScreen, color);
            sb.End();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Change background to random
        /// </summary>
        private void RandomBackground()
        {
            Random generator = new Random();
            _gameTableBackGroundNumber = generator.Next(_maxBackGroundNumber+1);
        }
        /// <summary>
        /// Change click sound to random
        /// </summary>
        private void RandomClickSound()
        {
            Random generator = new Random();
            _gameClickSoundNumber = generator.Next(_maxclickSoundNumber);
        }
        private void SelectEffect()
        {
            if (Player.Instance.SoundOn)
            {
                ScreenManager.fx.PlaySfx("blop");
            }
            if (Player.Instance.VibraOn)
            {
                ScreenManager.fx.VibrationCommonSelect();
            }
        }
        #endregion
    }
}
