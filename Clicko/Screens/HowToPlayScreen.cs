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
    public class HowToPlayScreen : AGameScreen, IAAnimationEndListener
    {
        #region Public Fields
        #endregion

        #region Private Fields
        /// <summary>
        /// Content manager
        /// </summary>
        private ContentManager _content;
        /// <summary>
        /// Help
        /// </summary>
        private ATexture _help;
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor
        /// </summary>
        public HowToPlayScreen()
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
            _help = new ATexture(_content, "help");
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

            if (input.CurrentGamePadStates.Buttons.Back == ButtonState.Pressed
                || input.TouchState.Count > 0)
            {
                if (Player.Instance.SoundOn)
                {
                    ScreenManager.fx.PlaySfx("blop");
                }
                if (Player.Instance.VibraOn)
                {
                    ScreenManager.fx.VibrationCommonSelect();
                }
                LoadAndExitScreen(new MainMenuScreen());
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
            _help.Draw(sb, Vector2.Zero, Color.White);
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
