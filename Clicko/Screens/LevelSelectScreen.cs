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
    using GameStateManagement;
    using AEngine;
    #endregion

    /// <summary>
    /// 
    /// </summary>
    public class LevelSelectScreen : AGameScreen, IAAnimationEndListener
    {
        #region Private Constants
        private static int _levelBlockXStart = 50;
        private static int _levelBlockYStart = 200;
        private static int _levelBlockYSpacing = 50;
        private static int _levelBlockWidth = 100;
        private static int _levelBlockHeight = 100;
        private static int _levelTextSpacing = 25 + _levelBlockWidth;
        private static int _levelTextWidth = 200;
        private static float _scrollingMaxSpeed = 25f;
        private static float _scrollingResist = 0.5f;
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
        /// Background of screen
        /// </summary>
        private ATexture _background;
        /// <summary>
        /// Background textures rectangle
        /// </summary>
        private static Rectangle _rectangleBackground = new Rectangle(0, 0, 480, 800);
        /// <summary>
        /// Title colum one
        /// </summary>
        private static Rectangle _rectangleLevelTitleColumOne = new Rectangle(_levelBlockXStart, 20, _levelBlockWidth, 200);
        /// <summary>
        /// Title colum two
        /// </summary>
        private static Rectangle _rectangleLevelTitleColumTwo = new Rectangle(_levelBlockXStart + _levelBlockWidth + _levelBlockYSpacing, 20, _levelTextWidth, 200);
        /// <summary>
        /// Level block starting position
        /// </summary>
        private static Rectangle _rectangleLevelBlockStart = new Rectangle(_levelBlockXStart, _levelBlockYStart, _levelBlockWidth, _levelBlockHeight);
        /// <summary>
        /// Level time text position
        /// </summary>
        private static Rectangle _rectangleLevelTimeStart = new Rectangle(_levelBlockXStart + _levelBlockWidth + _levelBlockYSpacing, _levelBlockYStart, _levelTextWidth, _levelBlockHeight);
        /// <summary>
        /// scrolling negative max is complicated, checked to proper value in constructor
        /// </summary>
        private int _scrollingNegativeMax =
            (((_levelBlockHeight + _levelBlockYSpacing) * MyGlobals.GameMaxLevel) - MyGlobals.ScreenSizeVertical + _rectangleLevelTitleColumOne.Height + 20) * -1;
        /// <summary>
        /// Level block texture
        /// </summary>
        private ATexture _levelBlock;
        /// <summary>
        /// Level lock texture
        /// </summary>
        private ATexture _levelLock;
        /// <summary>
        /// Scrolling speed
        /// </summary>
        private float _scrollingSpeed;
        /// <summary>
        /// Offset of scrolling
        /// [] <- 0
        /// [] <- +1
        /// [] <- +2
        /// </summary>
        private int _scrollingOffset
        {
            get { return _scrollingOffSet; }
            set { _scrollingOffSet = value; }
        }
        private int _scrollingOffSet;
        /// <summary>
        /// Level highligh, selection
        /// </summary>
        private AAnimatedTexture _levelHighLight;
        private bool _levelHighLightEnabled;
        private Rectangle _rectangleLevelHighLight;
        private int _levelSelected;
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor
        /// </summary>
        public LevelSelectScreen()
        {
            TransitionOnTime = MyGlobals.screenTransitionTime;
            TransitionOffTime = MyGlobals.screenTransitionTime;
            // if we don't need more space than one screen disable scrolling
            if (_scrollingNegativeMax >= 0)
            {
                _scrollingNegativeMax = 0;
            }
            _scrollingOffset = 0;
            EnabledGestures = GestureType.VerticalDrag | GestureType.Tap;
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
            _background = new ATexture(_content, "main_menu");
            _levelBlock = new ATexture(_content, "button_level");
            _levelLock = new ATexture(_content, "button_level_lock");
            _levelHighLight = new AAnimatedTexture(_content, "button_level_highlight", 100, false, false, this, 1);
            _rectangleLevelHighLight = new Rectangle(_levelBlockXStart, _levelBlockYStart, _levelBlockWidth, _levelBlockHeight);
            _levelHighLightEnabled = false;
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
                    // level selected and go
                    Player.Instance.CurrentLevel = _levelSelected;
                    LoadAndExitScreen(new GamePlayScreen());
                    break;
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
            // speed change
            if (_scrollingSpeed > 0)
            {
                _scrollingSpeed -= _scrollingResist;
            }
            else if (_scrollingSpeed < 0)
            {
                _scrollingSpeed += _scrollingResist;
            }
            // offset modification
            _scrollingOffset += (int)_scrollingSpeed;
            // we reached either end, top or bottom
            if (_scrollingOffSet > 0)
            {
                _scrollingOffset = 0;
                _scrollingSpeed = 0;
            }
            else if (_scrollingOffSet < _scrollingNegativeMax)
            {
                _scrollingOffset = _scrollingNegativeMax;
                _scrollingSpeed = 0;
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

            if (input.CurrentGamePadStates.Buttons.Back == ButtonState.Pressed)
            {
                LoadAndExitScreen(new MainMenuScreen());
            }

            foreach (GestureSample gs in input.Gestures)
            {
                // scroll list
                if (gs.GestureType == GestureType.VerticalDrag)
                {
                    float delta = _scrollingSpeed + (gs.Delta.Y / 4);
                    if (delta > 0)
                    {
                        if (delta > _scrollingMaxSpeed)
                        {
                            _scrollingSpeed = _scrollingMaxSpeed;
                        }
                        else
                        {
                            _scrollingSpeed = delta;
                        }
                    }
                    else
                    {
                        if (delta < (-1 * _scrollingMaxSpeed))
                        {
                            _scrollingSpeed = (-1 * _scrollingMaxSpeed);
                        }
                        else
                        {
                            _scrollingSpeed = delta;
                        }
                    }
                }
                // selection
                else if (gs.GestureType == GestureType.Tap)
                {
                    // stop scrolling always
                    _scrollingSpeed = 0;

                    // pickup position
                    Point position = new Point();
                    position.X = (int)gs.Position.X;
                    position.Y = (int)gs.Position.Y;

                    // search did we draw any level to that position
                    int levelSelected = 1;
                    Rectangle levelRectangle = _rectangleLevelBlockStart;
                    levelRectangle.Y += _scrollingOffSet;
                    for (int y = 0; y < MyGlobals.GameMaxLevel; y++)
                    {
                        if (levelRectangle.Contains(position))
                            {
                                if (levelSelected <= Player.Instance.MaxLevel)
                                {
                                    if (Player.Instance.SoundOn)
                                    {
                                        ScreenManager.fx.PlaySfx("blop");
                                    }
                                    if (Player.Instance.VibraOn)
                                    {
                                        ScreenManager.fx.VibrationCommonSelect();
                                    }
                                    _levelSelected = levelSelected;
                                    _rectangleLevelHighLight = levelRectangle;
                                    _levelHighLight.StartAnimation();
                                    _levelHighLightEnabled = true;
                                    _disableInput = true;
                                }
                            }
                        levelSelected++;
                        levelRectangle.Y += _levelBlockYSpacing + _levelBlockHeight;
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
            // background
            SpriteBatch sb = ScreenManager.SpriteBatch;
            sb.Begin();
            _background.Draw(sb, _rectangleBackground, Color.White);

            // title
            Rectangle titleColumOnePosition = _rectangleLevelTitleColumOne;
            titleColumOnePosition.Y += _scrollingOffSet;
            ScreenManager.commonFont25.DrawCentered(sb, "Level", titleColumOnePosition, Color.White);
            Rectangle titleColumTwoPosition = _rectangleLevelTitleColumTwo;
            titleColumTwoPosition.Y += _scrollingOffset;
            ScreenManager.commonFont25.DrawCentered(sb, "Best time", titleColumTwoPosition, Color.White);

            // loop variables
            int level = 1;
            Rectangle position = _rectangleLevelBlockStart;
            Rectangle textPosition = _rectangleLevelTimeStart;
            position.Y += _scrollingOffSet;
            textPosition.Y += _scrollingOffSet;
            for (int y = 0; y < MyGlobals.GameMaxLevel; y++)
            {
                // level gfx
                _levelBlock.Draw(sb, position, Color.White);
                // highlight
                if (level == _levelSelected && _levelHighLightEnabled)
                {
                    _levelHighLight.Draw(sb, _rectangleLevelHighLight, Color.White);
                }

                // draw level text if opened or lock otherwise
                if (level > Player.Instance.MaxLevel)
                {
                    _levelLock.Draw(sb, position, Color.White);
                }
                else
                {

                    ScreenManager.commonFont25.DrawCentered(sb, level.ToString(), position, Color.White);
                }

                // best time
                ScreenManager.commonFont25.DrawCentered(sb, Player.Instance.HighScores.FastestTimeFormatted(level), textPosition, Color.White);
                
                // increment loop variables
                level++;
                position.Y += _levelBlockYSpacing + _levelBlockHeight;
                textPosition.Y += _levelBlockYSpacing + _levelBlockHeight;
            }
            sb.End();
        }

        public override void DrawTransitionOn(GameTime gameTime)
        {
            // draw menu
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
            // draw menu
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
    }
}
