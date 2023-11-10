#region copyright
// Copyright © 2011-2012 Antti Eskola. All rights reserved.
//-----------------------------------------------------------------------------
// Modifications to original by Antti.E
// - 
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// ScreenManager.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO;
using System.IO.IsolatedStorage;
using AEngine;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// The screen manager is a component which manages one or more GameScreen
    /// instances. It maintains a stack of screens, calls their Update and Draw
    /// methods at the appropriate times, and automatically routes input to the
    /// topmost active screen.
    /// </summary>
    public class AScreenManager : DrawableGameComponent
    {
        #region Public common Fields
        public ASpriteFont commonFont25;
        public ASpriteFont commonFont20;
        public AEffects fx;
        #endregion

        #region Public Fields
        public SpriteBatch spriteBatch;
        public AInputState input = new AInputState();
        #endregion

        #region Private Fields
        private List<AGameScreen> _screens = new List<AGameScreen>();
        private List<AGameScreen> _screensLoading = new List<AGameScreen>();
        private List<AGameScreen> _screensUnloading = new List<AGameScreen>();
        private List<AGameScreen> _screensToUpdate = new List<AGameScreen>();
        private Texture2D _blankTexture;
        private bool _isInitialized;
        #endregion

        #region Properties
        /// <summary>
        /// A default SpriteBatch shared by all the screens. This saves
        /// each screen having to bother creating their own local instance.
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Constructs a new screen manager component.
        /// </summary>
        public AScreenManager(Game game)
            : base(game)
        {
            // we must set EnabledGestures before we can query for them, but
            // we don't assume the game wants to read them.
            TouchPanel.EnabledGestures = GestureType.None;
        }
        /// <summary>
        /// Initializes the screen manager component.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            _isInitialized = true;
        }
        /// <summary>
        /// Load your graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load content belonging to the screen manager.
            ContentManager content = Game.Content;

            // Effects content manager
            ContentManager effectContent = new ContentManager(Game.Services, "Content");
            fx = new AEffects(effectContent);

            spriteBatch = new SpriteBatch(GraphicsDevice);
            _blankTexture = content.Load<Texture2D>("blank");

            // Tell each of the screens to load their content.
            foreach (AGameScreen screen in _screens)
            {
                screen.LoadContent();
            }

            // Load common content
            commonFont25 = new ASpriteFont(content, "commonFont25");
            commonFont20 = new ASpriteFont(content, "commonFont20");
        }
        /// <summary>
        /// Unload your graphics content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Tell each of the screens to unload their content.
            foreach (AGameScreen screen in _screens)
            {
                screen.UnloadContent();
            }
        }
        #endregion

        #region Update and Draw
        /// <summary>
        /// Allows each screen to run logic.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // handle loading / unloading of screen
            foreach (AGameScreen screen in _screensUnloading)
            {
                RemoveScreen(screen);
            }
            _screensUnloading.Clear();
            foreach (AGameScreen screen in _screensLoading)
            {
                AddScreen(screen);
            }
            _screensLoading.Clear();

            // Read the keyboard and gamepad.
            input.Update();

            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            _screensToUpdate.Clear();

            foreach (AGameScreen screen in _screens)
                _screensToUpdate.Add(screen);

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            while (_screensToUpdate.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                AGameScreen screen = _screensToUpdate[_screensToUpdate.Count - 1];

                _screensToUpdate.RemoveAt(_screensToUpdate.Count - 1);

                // Update the screen.
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(input);

                        otherScreenHasFocus = true;
                    }

                    // If this is an active non-popup, inform any subsequent
                    // screens that they are covered by it.
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }
        }
        /// <summary>
        /// Tells each screen to draw itself.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            foreach (AGameScreen screen in _screens)
            {
                switch (screen.ScreenState)
                {
                    case ScreenState.Hidden:
                        continue;
                    case ScreenState.Active:
                        screen.Draw(gameTime);
                        break;
                    case ScreenState.TransitionOn:
                        screen.DrawTransitionOn(gameTime);
                        break;
                    case ScreenState.TransitionOff:
                        screen.DrawTransitionOff(gameTime);
                        break;
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Load a new screen and make it active
        /// </summary>
        /// <param name="screen"></param>
        public void LoadScreen(AGameScreen screen)
        {
            _screensLoading.Add(screen);
        }
        /// <summary>
        /// Unload given screen and remove it from content
        /// </summary>
        /// <param name="screen"></param>
        public void UnLoadScreen(AGameScreen screen)
        {
            _screensUnloading.Add(screen);
        }
        /// <summary>
        /// Unload all screens and remove their content
        /// (carefull when calling this)
        /// </summary>
        public void ExitAllScreens()
        {
            foreach (AGameScreen screen in _screens)
            {
                screen.ExitScreen();
            }
        }
        /// <summary>
        /// Informs the screen manager to serialize its state to disk.
        /// </summary>
        public void SerializeState()
        {
            // open up isolated storage
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // if our screen manager directory already exists, delete the contents
                if (storage.DirectoryExists("ScreenManager"))
                {
                    DeleteState(storage);
                }

                // otherwise just create the directory
                else
                {
                    storage.CreateDirectory("ScreenManager");
                }

                // create a file we'll use to store the list of screens in the stack
                using (IsolatedStorageFileStream stream = storage.CreateFile("ScreenManager\\ScreenList.dat"))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        // write out the full name of all the types in our stack so we can
                        // recreate them if needed.
                        foreach (AGameScreen screen in _screens)
                        {
                            writer.Write(screen.GetType().AssemblyQualifiedName);
                        }
                    }
                }
            }
        }
        public bool DeserializeState()
        {
            // open up isolated storage
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // see if our saved state directory exists
                if (storage.DirectoryExists("ScreenManager"))
                {
                    try
                    {
                        // see if we have a screen list
                        if (storage.FileExists("ScreenManager\\ScreenList.dat"))
                        {
                            // load the list of screen types
                            using (IsolatedStorageFileStream stream = storage.OpenFile("ScreenManager\\ScreenList.dat", FileMode.Open, FileAccess.Read))
                            {
                                using (BinaryReader reader = new BinaryReader(stream))
                                {
                                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                                    {
                                        // read a line from our file
                                        string line = reader.ReadString();

                                        // if it isn't blank, we can create a screen from it
                                        if (!string.IsNullOrEmpty(line))
                                        {
                                            Type screenType = Type.GetType(line);
                                            AGameScreen screen = Activator.CreateInstance(screenType) as AGameScreen;
                                            AddScreen(screen);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // if an exception was thrown while reading, odds are we cannot recover
                        // from the saved state, so we will delete it so the game can correctly
                        // launch.
                        DeleteState(storage);
                    }
                }
            }

            if (_screens.Count > 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Deletes the saved state files from isolated storage.
        /// </summary>
        private void DeleteState(IsolatedStorageFile storage)
        {
            // get all of the files in the directory and delete them
            string[] files = storage.GetFileNames("ScreenManager\\*");
            foreach (string file in files)
            {
                storage.DeleteFile(Path.Combine("ScreenManager", file));
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Adds a new screen to the screen manager.
        /// </summary>
        private void AddScreen(AGameScreen screen)
        {
            screen.ScreenManager = this;
            screen.IsExiting = false;

            // If we have a graphics device, tell the screen to load content.
            if (_isInitialized)
            {
                screen.LoadContent();
            }
            _screens.Add(screen);

            // update the TouchPanel to respond to gestures this screen is interested in
            TouchPanel.EnabledGestures = screen.EnabledGestures;
        }
        /// <summary>
        /// Removes a screen from the screen manager. You should normally
        /// use GameScreen.ExitScreen instead of calling this directly, so
        /// the screen can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        private void RemoveScreen(AGameScreen screen)
        {
            // If we have a graphics device, tell the screen to unload content.
            if (_isInitialized)
            {
                screen.UnloadContent();
            }

            _screens.Remove(screen);
            _screensToUpdate.Remove(screen);

            // if there is a screen still in the manager, update TouchPanel
            // to respond to gestures that screen is interested in.
            if (_screens.Count > 0)
            {
                TouchPanel.EnabledGestures = _screens[_screens.Count - 1].EnabledGestures;
            }
        }
        /// <summary>
        /// Expose an array holding all the screens. We return a copy rather
        /// than the real master list, because screens should only ever be added
        /// or removed using the AddScreen and RemoveScreen methods.
        /// </summary>
        private AGameScreen[] GetScreens()
        {
            return _screens.ToArray();
        }
        #endregion
    }
}
