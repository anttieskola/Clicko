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
    /// Main menu
    /// </summary>
    public class MainMenuScreen : AGameScreen
    {
        #region Private Constants
        /// <summary>
        /// UI location rectangles in main menu
        /// </summary>
        private static Rectangle _rectangleMainMenuTitle = new Rectangle(65, 20, 350, 155);
        private static Rectangle _rectangleMainMenuContinue = new Rectangle(115, 200, 250, 80);
        private static Rectangle _rectangleMainMenuNewGame = new Rectangle(115, 300, 250, 80);
        private static Rectangle _rectangleMainMenuHowToPlay = new Rectangle(115, 400, 250, 80);
        private static Rectangle _rectangleMainMenuVibra = new Rectangle(115, 520, 100, 100);
        private static Rectangle _rectangleMainMenuSpeaker = new Rectangle(265, 520, 100, 100);
        private enum _button
        {
            continueGame = 1,
            newGame = 2,
            howToPlay = 3,
            vibra = 4,
            sound = 5
        }
        #endregion

        #region Private Fields
        /// <summary>
        /// Content manager
        /// </summary>
        private ContentManager _content;

        /// <summary>
        /// Main menu
        /// </summary>
        private ATexture _mainMenu;

        /// <summary>
        /// Title animation
        /// </summary>
        private AAnimatedTexture _mainMenuTitle;

        /// <summary>
        /// Main menu buttons
        /// </summary>
        private AAnimatedTexture _mainMenuContinue;
        private AAnimatedTexture _mainMenuNewGame;
        private AAnimatedTexture _mainMenuHowToPlay;
        private AAnimatedTexture _mainMenuVibra;
        private ATexture _mainMenuVibraX;
        private AAnimatedTexture _mainMenuSpeaker;
        private ATexture _mainMenuSpeakerX;
        #endregion

        #region Initialization
        public MainMenuScreen()
        {
            TransitionOnTime = MyGlobals.screenTransitionTime;
            TransitionOffTime = MyGlobals.screenTransitionTime;
        }

        public override void LoadContent()
        {
            // content manager
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            // main menu background
            _mainMenu = new ATexture(_content, "main_menu");

            // title
            _mainMenuTitle = new AAnimatedTexture(_content, "main_menu_title", 350, true, true, 15);

            // buttons
            _mainMenuContinue = new AAnimatedTexture(_content, "button_250", 250, false, false);
            _mainMenuNewGame = new AAnimatedTexture(_content, "button_250", 250, false, false);
            _mainMenuHowToPlay = new AAnimatedTexture(_content, "button_250", 250, false, false);
            _mainMenuVibra = new AAnimatedTexture(_content, "vibra_ani", 100, true, true, 10);

            // vibra
            if (!Player.Instance.VibraOn)
            {
                _mainMenuVibra.StopAnimationAndRewind();
            }
            _mainMenuVibraX = new ATexture(_content, "vibra_x");

            // sound
            _mainMenuSpeaker = new AAnimatedTexture(_content, "speaker_ani", 100, true, true, 10);
            if (!Player.Instance.SoundOn)
            {
                _mainMenuSpeaker.StopAnimationAndRewind();
            }
            _mainMenuSpeakerX = new ATexture(_content, "speaker_x");
        }

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
                ScreenManager.Game.Exit();
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
                    if (_rectangleMainMenuContinue.Contains(xy))
                    {
                        if (Player.Instance.SaveAvailable)
                        {
                            _mainMenuContinue.PlayAnimation(true);
                            _mainMenuHowToPlay.StopAnimationAndRewind();
                            _mainMenuNewGame.StopAnimationAndRewind();
                        }
                    }
                    // new game
                    else if (_rectangleMainMenuNewGame.Contains(xy))
                    {
                        _mainMenuNewGame.PlayAnimation(true);
                        _mainMenuContinue.StopAnimationAndRewind();
                        _mainMenuHowToPlay.StopAnimationAndRewind();
                    }

                    // help
                    else if (_rectangleMainMenuHowToPlay.Contains(xy))
                    {
                        _mainMenuHowToPlay.PlayAnimation(true);
                        _mainMenuContinue.StopAnimationAndRewind();
                        _mainMenuNewGame.StopAnimationAndRewind();
                    }
                    // vibra & sound have no click animation available
                    else
                    {
                        // finger moved out of scope (stop all animations)
                        _mainMenuContinue.StopAnimationAndRewind();
                        _mainMenuHowToPlay.StopAnimationAndRewind();
                        _mainMenuNewGame.StopAnimationAndRewind();
                    }
                }
                // finger released (action)
                else
                {
                    // continue
                    if (_rectangleMainMenuContinue.Contains(xy))
                    {
                        if (Player.Instance.SaveAvailable)
                        {
                            SelectEffect();
                            LoadAndExitScreen(new GamePlayScreen());
                            Player.Instance.LevelTimer.Continue();
                        }
                    }
                    // new game
                    else if (_rectangleMainMenuNewGame.Contains(xy))
                    {
                        SelectEffect();
                        LoadAndExitScreen(new LevelSelectScreen());
                    }
                    // help
                    else if (_rectangleMainMenuHowToPlay.Contains(xy))
                    {
                        SelectEffect();
                        LoadAndExitScreen(new HowToPlayScreen());
                    }
                    // vibra
                    else if (_rectangleMainMenuVibra.Contains(xy))
                    {
                        SelectEffect();
                        Player.Instance.VibraOn = !Player.Instance.VibraOn;
                        if (!Player.Instance.VibraOn)
                        {
                            _mainMenuVibra.StopAnimationAndRewind();
                        }
                        else
                        {
                            _mainMenuVibra.StartAnimation();
                        }
                    }
                    // sound
                    else if (_rectangleMainMenuSpeaker.Contains(xy))
                    {
                        SelectEffect();
                        Player.Instance.SoundOn = !Player.Instance.SoundOn;
                        if (!Player.Instance.SoundOn)
                        {
                            _mainMenuSpeaker.StopAnimationAndRewind();
                        }
                        else
                        {
                            _mainMenuSpeaker.StartAnimation();
                        }
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // Get the batch
            SpriteBatch sb = ScreenManager.SpriteBatch;
            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            // Pictures

            // main menu
            _mainMenu.Draw(sb, Vector2.Zero, Color.White);

            // title
            _mainMenuTitle.Draw(sb, _rectangleMainMenuTitle, Color.White);

            // menu continue
            if (Player.Instance.SaveAvailable)
            {
                _mainMenuContinue.Draw(sb, _rectangleMainMenuContinue, Color.White);
            }

            // menu new game
            _mainMenuNewGame.Draw(sb, _rectangleMainMenuNewGame, Color.White);

            // menu highscores
            _mainMenuHowToPlay.Draw(sb, _rectangleMainMenuHowToPlay, Color.White);

            // vibra
            _mainMenuVibra.Draw(sb, _rectangleMainMenuVibra, Color.White);

            // vibra x
            if (!Player.Instance.VibraOn)
            {
                _mainMenuVibraX.Draw(sb, _rectangleMainMenuVibra, Color.White);
            }

            // speaker
            _mainMenuSpeaker.Draw(sb, _rectangleMainMenuSpeaker, Color.White);

            // speaker X
            if (!Player.Instance.SoundOn)
            {
                _mainMenuSpeakerX.Draw(sb, _rectangleMainMenuSpeaker, Color.White);
            }

            // Texts
            // menu continue
            if (Player.Instance.SaveAvailable)
            {
                ScreenManager.commonFont20.DrawCentered(sb, "CONTINUE", _rectangleMainMenuContinue, Color.White);
            }

            // menu puzzle
            ScreenManager.commonFont20.DrawCentered(sb, "NEW GAME", _rectangleMainMenuNewGame, Color.White);

            // menu highscores
            ScreenManager.commonFont20.DrawCentered(sb, "HOW TO PLAY", _rectangleMainMenuHowToPlay, Color.White);
            // end our drawing
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
