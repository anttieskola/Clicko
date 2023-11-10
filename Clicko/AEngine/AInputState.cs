#region copyright
// Copyright © 2011-2012 Antti Eskola. All rights reserved.
//-----------------------------------------------------------------------------
// Modifications to original by Antti.E
// - Removed support for multiple players
// - Removed menu/cancel supports
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// InputState.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// Helper for reading input from keyboard, gamepad, and touch input. This class 
    /// tracks both the current and previous state of the input devices, and implements 
    /// query methods for high level input actions such as "move up through the menu"
    /// or "pause the game".
    /// </summary>
    public class AInputState
    {
        #region Public Fields
        public KeyboardState CurrentKeyboardStates;
        public GamePadState CurrentGamePadStates;
        public KeyboardState LastKeyboardStates;
        public GamePadState LastGamePadStates;
        public TouchCollection TouchState;
        public readonly List<GestureSample> Gestures = new List<GestureSample>();
        #endregion

        #region Initialization
        /// <summary>
        /// Constructs a new input state.
        /// </summary>
        public AInputState()
        {
            LastKeyboardStates = Keyboard.GetState(PlayerIndex.One);
            LastGamePadStates = GamePad.GetState(PlayerIndex.One);
            CurrentKeyboardStates = Keyboard.GetState(PlayerIndex.One);
            CurrentGamePadStates = GamePad.GetState(PlayerIndex.One);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Reads the latest state of the keyboard and gamepad.
        /// </summary>
        public void Update()
        {
            LastKeyboardStates = CurrentKeyboardStates;
            LastGamePadStates = CurrentGamePadStates;

            CurrentKeyboardStates = Keyboard.GetState(PlayerIndex.One);
            CurrentGamePadStates = GamePad.GetState(PlayerIndex.One);

            TouchState = TouchPanel.GetState();

            Gestures.Clear();
            while (TouchPanel.IsGestureAvailable)
            {
                Gestures.Add(TouchPanel.ReadGesture());
            }
        }
        /// <summary>
        /// Helper for checking if a key was newly pressed during this update.
        /// </summary>
        public bool IsNewKeyPress(Keys key)
        {
             return (CurrentKeyboardStates.IsKeyDown(key)
                 && LastKeyboardStates.IsKeyUp(key));
        }
        /// <summary>
        /// Helper for checking if a button was newly pressed during this update.
        /// </summary>
        public bool IsNewButtonPress(Buttons button)
        {
            return (CurrentGamePadStates.IsButtonDown(button) &&
                    LastGamePadStates.IsButtonUp(button));
        }
        #endregion
    }
}
