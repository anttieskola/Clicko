#region copyright
// Copyright © 2011-2012 Antti Eskola. All rights reserved.
#endregion

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

namespace Clicko
{
    /// <summary>
    /// GameMain, is the game entry point
    /// </summary>
    public class GameMain : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        AScreenManager screenManager;

        public GameMain()
        {
            // graphic device
            graphics = new GraphicsDeviceManager(this);

            // window settings
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;
            graphics.SupportedOrientations = DisplayOrientation.Portrait;
            graphics.IsFullScreen = true;

            // backbuffer color
            graphics.PreferredBackBufferFormat = SurfaceFormat.Color;
            // stencil format and value
            graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
            // no multisampling
            graphics.PreferMultiSampling = true;

            // content
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Player
            Player.Instance.Load();

            // Screens
            screenManager = new AScreenManager(this);
            Components.Add(screenManager);

            // load where we were at exit
            if (!screenManager.DeserializeState())
            {
                // no saved screen setup found
                screenManager.LoadScreen(new MainMenuScreen());
            }
        }

        /// <summary>
        /// *Stuff here must be fast*
        /// Create required objects
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }
         
        /// <summary>
        /// *Stuff here can take a while to complete*
        /// Load textures and sounds
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Add it as service for the engine
            Services.AddService(typeof(SpriteBatch), spriteBatch);
        }

        /// <summary>
        /// Game process killed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnExiting(object sender, EventArgs args)
        {
            // Player
            Player.Instance.Save();

            // Screens
            screenManager.SerializeState();

            base.OnExiting(sender, args);
        }

        /// <summary>
        /// Game process starts up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnActivated(object sender, EventArgs args)
        {
            base.OnActivated(sender, args);
        }

        /// <summary>
        /// Nothing needed yet.
        /// </summary>
        protected override void UnloadContent()
        {

        }

        /// <summary>
        /// Main game loop
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }
    }
}
