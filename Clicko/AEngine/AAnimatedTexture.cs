#region copyright
// Copyright © 2011-2012 Antti Eskola. All rights reserved.
#endregion
namespace AEngine
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
    #endregion

    #region Animation listener interface
    /// <summary>
    /// Implement this listener to receive animation end events
    /// </summary>
    public interface IAAnimationEndListener
    {
        /// <summary>
        /// Animation has ended
        /// </summary>
        /// <param name="e">which animation</param>
        void AAnimationEnded(int e);
    }
    #endregion


    public class AAnimatedTexture
    {
        #region Public Fields
        /// <summary>
        /// Basic texture values, all are set in constructor.
        /// </summary>
        public readonly string Name;
        public readonly int Width;
        public readonly int Height;
        public readonly int Frames;
        /// <summary>
        /// Is animation on?
        /// </summary>
        public bool AnimationEnabled;
        /// <summary>
        /// Do we loop after last frame?
        /// </summary>
        public bool Loop;
        #endregion

        #region Private Fields
        private readonly ContentManager _content; 
        private IAAnimationEndListener _animationEndListener;
        private int _animationEnd;
        private bool _stopAtEnd;
        private int _currentFrame;
        private uint _frameLifeTime;
        private uint _frameLifeTimeCurrent;
        private Rectangle _rectangleSource;
        #endregion

        #region Initialization
        /// <summary>
        /// Animated texture constructor without listener
        /// </summary>
        /// <param name="content"></param>
        /// <param name="name"></param>
        /// <param name="frameWidth"></param>
        /// <param name="animationEnabled"></param>
        /// <param name="loop"></param>
        public AAnimatedTexture(ContentManager content, string name, int frameWidth,
            bool animationEnabled, bool loop)
        {
            _content = content;
            _animationEndListener = null;
            _stopAtEnd = false;
            Name = name;
            Texture2D texture = _content.Load<Texture2D>(name);
            Width = texture.Width;
            Height = texture.Height;

            AnimationEnabled = animationEnabled;
            Loop = loop;
  
            Frames = texture.Width / frameWidth;
            _rectangleSource.X = 0;
            _rectangleSource.Y = 0;
            _rectangleSource.Width = frameWidth;
            _rectangleSource.Height = Height;
            _frameLifeTime = 1;
            _frameLifeTimeCurrent = _frameLifeTime;
        }

        /// <summary>
        /// Animated texture constructor without listener but custom frame life time
        /// </summary>
        /// <param name="content"></param>
        /// <param name="name"></param>
        /// <param name="frameWidth"></param>
        /// <param name="animationEnabled"></param>
        /// <param name="loop"></param>
        /// <param name="frameLifeTime"></param>
        public AAnimatedTexture(ContentManager content, string name, int frameWidth,
            bool animationEnabled, bool loop, uint frameLifeTime)
        {
            _content = content;
            _animationEndListener = null;
            _stopAtEnd = false;
            Name = name;
            Texture2D texture = _content.Load<Texture2D>(name);
            Width = texture.Width;
            Height = texture.Height;

            AnimationEnabled = animationEnabled;
            Loop = loop;

            Frames = texture.Width / frameWidth;
            _rectangleSource.X = 0;
            _rectangleSource.Y = 0;
            _rectangleSource.Width = frameWidth;
            _rectangleSource.Height = Height;
            _frameLifeTime = frameLifeTime;
            _frameLifeTimeCurrent = _frameLifeTime;
        }

        /// <summary>
        /// Animated texture constructor with listener
        /// </summary>
        /// <param name="content"></param>
        /// <param name="name"></param>
        /// <param name="frameWidth"></param>
        /// <param name="animationEnabled"></param>
        /// <param name="loop"></param>
        /// <param name="resetAtEnd"></param>
        /// <param name="animationEndListener"></param>
        /// <param name="animationEnd"></param>
        public AAnimatedTexture(ContentManager content, string name, int frameWidth,
            bool animationEnabled, bool loop, IAAnimationEndListener animationEndListener, int animationEnd)
            : this(content, name, frameWidth, animationEnabled, loop)
        {
            _animationEndListener = animationEndListener;
            _animationEnd = animationEnd;
        }
        #endregion

        #region Control animation
        /// <summary>
        /// (Re)Start animation
        /// </summary>
        public void StartAnimation()
        {
            _currentFrame = 0;
            AnimationEnabled = true;
        }

        /// <summary>
        /// (Re)Start animation with option to stop at last frame
        /// </summary>
        public void StartAnimation(bool stopAtEnd)
        {
            StartAnimation();
            _stopAtEnd = stopAtEnd;
        }

        /// <summary>
        /// (Re)Start animation with option for frame fps lifetime
        /// </summary>
        /// <param name="frameLifeTime">How many draws/fps does a single animation frame live</param>
        public void StartAnimation(uint frameLifeTime)
        {
            if (frameLifeTime > 0)
            {
                _frameLifeTime = frameLifeTime;
            }
            StartAnimation();
        }

        /// <summary>
        /// (Re)Start animation with option for frame fps lifetime and to stop at last frame
        /// </summary>
        /// <param name="frameLifeTime">How many draws/fps does a single animation frame live</param>
        public void StartAnimation(uint frameLifeTime, bool stopAtEnd)
        {
            if (frameLifeTime > 0)
            {
                _frameLifeTime = frameLifeTime;
            }
            StartAnimation(true);
        }

        /// <summary>
        /// Stop and reset
        /// </summary>
        public void StopAnimationAndRewind()
        {
            AnimationEnabled = false;
            _currentFrame = 0;
        }

        /// <summary>
        /// Rewind animation
        /// </summary>
        public void Rewind()
        {
            _currentFrame = 0;
        }

        /// <summary>
        /// Pause animation
        /// </summary>
        public void PauseAnimation()
        {
            AnimationEnabled = false;
        }

        /// <summary>
        /// Continue Animation from current position
        /// </summary>
        public void PlayAnimation()
        {
            AnimationEnabled = true;
        }

        /// <summary>
        /// Continue Animation from current position with option to stop at last frame
        /// </summary>
        public void PlayAnimation(bool stopAtEnd)
        {
            PlayAnimation();
            _stopAtEnd = stopAtEnd;
        }
        #endregion

        #region Draw
        /// <summary>
        /// Draw texture to wanted position
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="position"></param>
        /// <param name="color"></param>
        public void Draw(SpriteBatch sb, Rectangle position, Color color)
        {
            sb.Draw(_content.Load<Texture2D>(Name),
                position,
                getNextFrameSource(),
                color);
        }

        /// <summary>
        /// Draw texture to wanted position with rotation
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="position"></param>
        /// <param name="color"></param>
        /// <param name="rotation"></param>
        public void Draw(SpriteBatch sb, Rectangle position, Color color, float rotation, Vector2 origin)
        {
            sb.Draw(_content.Load<Texture2D>(Name),
                position,
                getNextFrameSource(),
                color,
                rotation,
                origin,
                SpriteEffects.None,
                0f);
        }


        /// <summary>
        /// Helper to ask texture height
        /// </summary>
        /// <returns></returns>
        public int TextureHeight()
        {
            return Height;
        }

        /// <summary>
        /// Helper to ask texture width
        /// </summary>
        /// <returns></returns>
        public int TextureWidth()
        {
            return Width / Frames;
        }
        #endregion

        #region Private functions
        /// <summary>
        /// Returns the source rectangle
        /// </summary>
        /// <returns></returns>
        private Rectangle getNextFrameSource()
        {
            _rectangleSource.X = _currentFrame * (Width / Frames);
            if (AnimationEnabled)
            {
                if (!NextFrame())
                {
                    if (_animationEndListener != null)
                    {
                        _animationEndListener.AAnimationEnded(_animationEnd);
                    }

                    if (Loop)
                    {
                        StartAnimation();
                    }
                    else if (_stopAtEnd)
                    {
                        PauseAnimation();
                    }
                    else
                    {
                        StopAnimationAndRewind();
                    }
                }
            }
            return _rectangleSource;
        }

        /// <summary>
        /// Checks is there new frames available
        /// </summary>
        /// <returns></returns>
        private bool NextFrame()
        {
            _frameLifeTimeCurrent--;
            if (_frameLifeTimeCurrent == 0)
            {
                _frameLifeTimeCurrent = _frameLifeTime; // reset life
                if (_currentFrame + 1 < Frames)
                {
                    _currentFrame++; // set next frame
                    return true;
                }
                return false;
            }
            return true;
        }
        #endregion
    }
}
