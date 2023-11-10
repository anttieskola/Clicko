#region copyright
// Copyright © 2011-2012 Antti Eskola. All rights reserved.
#endregion
namespace AEngine
{
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

    public class AAnimatedSpriteFont
    {
        #region Public Fields
        /// <summary>
        /// Is animation active
        /// </summary>
        public bool Active;
        #endregion

        #region Private Fields
        /// <summary>
        /// Basic members
        /// </summary>
        private readonly ContentManager _content;
        private readonly string _name;
        private readonly IAAnimationEndListener _listener;
        private readonly int _animationEnd;
        private bool _animationFadeIn;
        private bool _animationFadeOut;
        private string _text;
        private Color _color;
        private Vector2 _origin;
        /// <summary>
        /// Position related
        /// </summary>
        private Vector2 _position;
        /// <summary>
        /// Length of the animation, either in or out.
        /// </summary> 
        private int _animationLength;
        private int _currentFrame;
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor without listener
        /// </summary>
        /// <param name="content"></param>
        /// <param name="name"></param>
        public AAnimatedSpriteFont(ContentManager content, string name)
        {
            Active = false;
            _content = content;
            _name = name;
            _listener = null;
        }
        /// <summary>
        /// Constructor with listener
        /// </summary>
        /// <param name="content"></param>
        /// <param name="name"></param>
        /// <param name="listener"></param>
        /// <param name="animationEnd"></param>
        public AAnimatedSpriteFont(ContentManager content, string name, IAAnimationEndListener listener, int animationEnd)
            : this(content, name)
        {
            _listener = listener;
            _animationEnd = animationEnd;
        }
        #endregion

        #region Animation
        /// <summary>
        /// Draw font
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            if (Active)
            {
                NextFrame();
                sb.DrawString(_content.Load<SpriteFont>(_name),
                    _text,
                    _position,
                    _color,
                    0.0f,
                    _origin,
                    1.0f,
                    SpriteEffects.None,
                    1.0f);
            }
        }
        /// <summary>
        /// Just a helper to disable text drawing
        /// </summary>
        public void DisableAnimation()
        {
            Active = false;
        }
        /// <summary>
        /// Fade animation
        /// </summary>
        /// <param name="text"></param>
        /// <param name="centerPosition"></param>
        /// <param name="color"></param>
        public void StartFadeInAndOutAnimation(string text, Point centerPosition, Color color)
        {
            Active = true;
            _text = text;
            _position.X = centerPosition.X;
            _position.Y = centerPosition.Y;
            _color = color;

            // calculate string size as "bitmap" and pickup center of it
            Vector2 stringSize = _content.Load<SpriteFont>(_name).MeasureString(_text);
            _origin.X = stringSize.X / 2;
            _origin.Y = stringSize.Y / 2;
            
            // set alpha to max
            _color.A = 0;

            // animation stage fading in
            _animationFadeIn = true;
            // also fade out after
            _animationFadeOut = true;
            // common length
            _animationLength = 30;
            // reset current frame
            _currentFrame = 0;
        }
        /// <summary>
        /// Fade animation with custom length
        /// </summary>
        /// <param name="text"></param>
        /// <param name="centerPosition"></param>
        /// <param name="color"></param>
        /// <param name="animationLength"></param>
        public void StartFadeInAndOutAnimation(string text, Point centerPosition, Color color, int animationLength)
        {
            StartFadeInAndOutAnimation(text, centerPosition, color);
            // custom length
            _animationLength = animationLength;
        }
        /// <summary>
        /// Fade animation in but never out
        /// </summary>
        /// <param name="text"></param>
        /// <param name="centerPosition"></param>
        /// <param name="color"></param>
        public void StartFadeInAnimation(string text, Point centerPosition, Color color)
        {
            StartFadeInAndOutAnimation(text, centerPosition, color);
            // don't fade out
            _animationFadeOut = false;
        }
        /// <summary>
        /// Fade animation from max visibility to invisible
        /// </summary>
        /// <param name="text"></param>
        /// <param name="centerPosition"></param>
        /// <param name="color"></param>
        public void StartFadeOutAnimation(string text, Point centerPosition, Color color)
        {
            StartFadeInAndOutAnimation(text, centerPosition, color);
            // set alpha to max
            _color.A = 255;
            // animation stage fading in
            _animationFadeIn = false;
            // we just fade out so does not matter really
            _animationFadeOut = true;
        }
        #endregion

        #region Private Methods
        private void NextFrame()
        {
            // text coming to visible
            if (_animationFadeIn)
            {
                // increment frame
                _currentFrame++;
                // new alpa value, exponential
                double value = Math.Pow(_currentFrame, 2);
                if (value > 255)
                {
                    _color.A = 255;
                }
                else
                {
                    _color.A = (byte)value;
                }
                // we completed animation?
                if (_currentFrame == _animationLength)
                {
                    // complete
                    _animationFadeIn = false;
                    _currentFrame = 0;
                    // send signal if to listener if no fade out
                    if (!_animationFadeOut
                        && _listener != null)
                    {
                        _listener.AAnimationEnded(_animationEnd);
                    }
                }
            }
            // text fading out
            else if (_animationFadeOut)
            {
                // increment frame
                _currentFrame++;
                // new alpha value, exponentially but inverted
                double value = Math.Pow((_animationLength - _currentFrame), 2);
                if (value > 255)
                {
                    _color.A = 255;
                }
                else
                {
                    _color.A = (byte)value;
                }
                // We completed animation?
                if (_currentFrame == _animationLength)
                {
                    // complete
                    _animationFadeOut = false;
                    _currentFrame = 0;
                    Active = false;
                    // send signal to listener
                    if (_listener != null)
                    {
                        _listener.AAnimationEnded(_animationEnd);
                    }
                }
            }
        }
        #endregion
    }
}
