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
    public class GamePlayPauseScreen : AGameScreen
    {
        #region Private Constants
        private static Rectangle _rectanglePauseTitle = new Rectangle(115, 20, 250, 200);
        private static Rectangle _rectanglePauseContinue = new Rectangle(115, 200, 250, 80);
        private static Rectangle _rectanglePauseRestart = new Rectangle(115, 300, 250, 80);
        private static Rectangle _rectanglePauseMenu = new Rectangle(115, 400, 250, 80);
        private static Rectangle _rectanglePauseExit = new Rectangle(115, 500, 250, 100);
        private static Rectangle _rectanglePauseVibra = new Rectangle(115, 620, 100, 100);
        private static Rectangle _rectanglePauseSound = new Rectangle(265, 620, 100, 100);
        #endregion

        #region Private Fields
        /// <summary>
        /// Content manager
        /// </summary>
        private ContentManager _content;
        /// <summary>
        /// Background
        /// </summary>
        private ATexture _pauseBackGround;
        /// <summary>
        /// buttons
        /// </summary>
        private AAnimatedTexture _pauseContinue;
        private AAnimatedTexture _pauseRestart;
        private AAnimatedTexture _pauseMenu;
        private AAnimatedTexture _pauseExit;
        /// <summary>
        /// Vibra icon
        /// </summary>
        private AAnimatedTexture _pauseVibra;
        private ATexture _pauseVibraX;
        /// <summary>
        /// Sound icon
        /// </summary>
        private AAnimatedTexture _pauseSound;
        private ATexture _pauseSoundX;
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor
        /// </summary>
        public GamePlayPauseScreen()
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

            // bg
            _pauseBackGround = new ATexture(_content, "main_menu");

            // buttons
            _pauseContinue = new AAnimatedTexture(_content, "button_250", 250, false, false);
            _pauseExit = new AAnimatedTexture(_content, "button_250", 250, false, false);
            _pauseMenu = new AAnimatedTexture(_content, "button_250", 250, false, false);
            _pauseRestart = new AAnimatedTexture(_content, "button_250", 250, false, false);

            // vibra
            _pauseVibra = new AAnimatedTexture(_content, "vibra_ani", 100, true, true, 10);
            if (!Player.Instance.VibraOn)
            {
                _pauseVibra.StopAnimationAndRewind();
            }
            _pauseVibraX = new ATexture(_content, "vibra_x");

            // sound
            _pauseSound = new AAnimatedTexture(_content, "speaker_ani", 100, true, true, 10);
            if (!Player.Instance.SoundOn)
            {
                _pauseSound.StopAnimationAndRewind();
            }
            _pauseSoundX = new ATexture(_content, "speaker_x");
        }
        /// <summary>
        /// Unloading of screen content
        /// </summary>
        public override void UnloadContent()
        {
            _content.Unload();
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
            // back button
            if (input.CurrentGamePadStates.Buttons.Back == ButtonState.Pressed)
            {
                Player.Instance.LevelTimer.Continue();
                ExitScreen();
            }
            // touch states for menu selections
            for (int i = 0; i < input.TouchState.Count; i++)
            {
                TouchLocation touchLocation = input.TouchState[i];
                int x = (int)touchLocation.Position.X;
                int y = (int)touchLocation.Position.Y;
                Point xy = new Point(x, y);

                // finger pressed/moved to button (animations)
                if (touchLocation.State == TouchLocationState.Pressed
                    || touchLocation.State == TouchLocationState.Moved)
                {
                    // continue
                    if (_rectanglePauseContinue.Contains(xy))
                    {
                        _pauseContinue.PlayAnimation(true);
                        _pauseRestart.StopAnimationAndRewind();
                        _pauseMenu.StopAnimationAndRewind();
                        _pauseExit.StopAnimationAndRewind();
                    }
                    // restart level
                    else if (_rectanglePauseRestart.Contains(xy))
                    {
                        _pauseContinue.StopAnimationAndRewind();
                        _pauseRestart.PlayAnimation(true);
                        _pauseMenu.StopAnimationAndRewind();
                        _pauseExit.StopAnimationAndRewind();
                    }
                    // back to main menu
                    else if (_rectanglePauseMenu.Contains(xy))
                    {
                        _pauseContinue.StopAnimationAndRewind();
                        _pauseRestart.StopAnimationAndRewind();
                        _pauseMenu.PlayAnimation(true);
                        _pauseExit.StopAnimationAndRewind();
                    }
                    // exit game
                    else if (_rectanglePauseExit.Contains(xy))
                    {
                        _pauseContinue.StopAnimationAndRewind();
                        _pauseRestart.StopAnimationAndRewind();
                        _pauseMenu.StopAnimationAndRewind();
                        _pauseExit.PlayAnimation(true);
                    }
                    // vibra & sound have no click animation available
                    else
                    {
                        // finger moved out of scope (stop all animations)
                        _pauseContinue.StopAnimationAndRewind();
                        _pauseRestart.StopAnimationAndRewind();
                        _pauseMenu.StopAnimationAndRewind();
                        _pauseExit.StopAnimationAndRewind();
                    }
                }
                // finger released (action)
                else
                {
                    // continue
                    if (_rectanglePauseContinue.Contains(xy))
                    {
                        SelectEffect();
                        Player.Instance.LevelTimer.Continue();
                        ExitScreen();
                    }
                    // restart
                    else if (_rectanglePauseRestart.Contains(xy))
                    {
                        SelectEffect();
                        Player.Instance.ResetLevel();
                        ExitScreen();
                    }
                    // back to main menu
                    else if (_rectanglePauseMenu.Contains(xy))
                    {
                        SelectEffect();
                        Player.Instance.Save();
                        LoadAndExitAllScreens(new MainMenuScreen());
                    }
                    // exit game
                    else if (_rectanglePauseExit.Contains(xy))
                    {
                        SelectEffect();
                        ScreenManager.Game.Exit();
                    }
                    // vibra
                    else if (_rectanglePauseVibra.Contains(xy))
                    {
                        SelectEffect();
                        Player.Instance.VibraOn = !Player.Instance.VibraOn;
                        if (!Player.Instance.VibraOn)
                        {
                            _pauseVibra.StopAnimationAndRewind();
                        }
                        else
                        {
                            _pauseVibra.StartAnimation();
                        }
                    }
                    // sound
                    else if (_rectanglePauseSound.Contains(xy))
                    {
                        SelectEffect();
                        Player.Instance.SoundOn = !Player.Instance.SoundOn;
                        if (!Player.Instance.SoundOn)
                        {
                            _pauseSound.StopAnimationAndRewind();
                        }
                        else
                        {
                            _pauseSound.StartAnimation();
                        }
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
            SpriteBatch sb = ScreenManager.SpriteBatch;
            sb.Begin();
            _pauseBackGround.Draw(sb, MyGlobals.RectangleFullScreen, Color.White);
            // title
            ScreenManager.commonFont25.DrawCentered(sb, "Game paused", _rectanglePauseTitle, Color.White);
            // buttons
            _pauseContinue.Draw(sb, _rectanglePauseContinue, Color.White);
            _pauseRestart.Draw(sb, _rectanglePauseRestart, Color.White);
            _pauseMenu.Draw(sb, _rectanglePauseMenu, Color.White);
            _pauseExit.Draw(sb, _rectanglePauseExit, Color.White);
            // button text's
            ScreenManager.commonFont20.DrawCentered(sb, "Continue", _rectanglePauseContinue, Color.White);
            ScreenManager.commonFont20.DrawCentered(sb, "Restart level", _rectanglePauseRestart, Color.White);
            ScreenManager.commonFont20.DrawCentered(sb, "Menu", _rectanglePauseMenu, Color.White);
            ScreenManager.commonFont20.DrawCentered(sb, "Exit Game", _rectanglePauseExit, Color.White);
            // vibra
            _pauseVibra.Draw(sb, _rectanglePauseVibra, Color.White);
            if (!Player.Instance.VibraOn)
            {
                _pauseVibraX.Draw(sb, _rectanglePauseVibra, Color.White);
            }
            // sound
            _pauseSound.Draw(sb, _rectanglePauseSound, Color.White);
            if (!Player.Instance.SoundOn)
            {
                _pauseSoundX.Draw(sb, _rectanglePauseSound, Color.White);
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
        #region private helpers
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
