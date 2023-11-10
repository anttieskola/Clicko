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

    /// <summary>
    /// Class helping to draw strings with fabulous ways
    /// </summary>
    public class ASpriteFont
    {
        #region Private Fields
        private readonly ContentManager _content;
        private readonly string _name;
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="game"></param>
        public ASpriteFont(ContentManager content, String name)
        {
            // font name
            _name = name;

            // set content manager
            _content = content;

            // Load it so its cached in content manager
            SpriteFont font = _content.Load<SpriteFont>(name);
        }
        #endregion

        #region Draw
        /// <summary>
        /// Default way to draw "simple"
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="text"></param>
        /// <param name="position"></param>
        /// <param name="color"></param>
        public void Draw(SpriteBatch sb, string text, Vector2 position, Color color)
        {
            sb.DrawString(_content.Load<SpriteFont>(_name), text, position, color);
        }
        /// <summary>
        /// Draw text centered to given rectangle
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="text"></param>
        /// <param name="rectangle"></param>
        /// <param name="color"></param>
        public void DrawCentered(SpriteBatch sb, string text, Rectangle rectangle, Color color)
        {
            // calculate string size as "bitmap"
            Vector2 stringSize = _content.Load<SpriteFont>(_name).MeasureString(text);

            // rectangle center center spot
            Vector2 rectangleCenter = new Vector2(rectangle.Center.X, rectangle.Center.Y);

            // draw string without effects or etc...
            sb.DrawString(
                _content.Load<SpriteFont>(_name),
                text,
                rectangleCenter,
                color,
                0.0f,
                new Vector2(stringSize.X / 2, stringSize.Y / 2),
                1.0f,
                SpriteEffects.None,
                0.0f);
        }

        public void DrawCentered(SpriteBatch sb, string text, Rectangle rectangle, Color color, float scale)
        {
            // calculate string size as "bitmap"
            Vector2 stringSize = _content.Load<SpriteFont>(_name).MeasureString(text);

            // rectangle center center spot
            Vector2 rectangleCenter = new Vector2(rectangle.Center.X, rectangle.Center.Y);

            // draw string without effects or etc...
            sb.DrawString(
                _content.Load<SpriteFont>(_name),
                text,
                rectangleCenter,
                color,
                0.0f,
                new Vector2(stringSize.X / 2, stringSize.Y / 2),
                scale,
                SpriteEffects.None,
                0.0f);
        }
        /// <summary>
        /// Draw text vertically centered to given rectangle
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="text"></param>
        /// <param name="rectangle"></param>
        /// <param name="color"></param>
        public void DrawVerticalCentered(SpriteBatch sb, string text, Rectangle rectangle, Color color)
        {
            // calculate string size as "bitmap"
            Vector2 stringSize = _content.Load<SpriteFont>(_name).MeasureString(text);

            // rectangle center center spot
            Vector2 rectangleCenter = new Vector2(rectangle.X, rectangle.Center.Y);

            // draw string without effects or etc...
            sb.DrawString(
                _content.Load<SpriteFont>(_name),
                text,
                rectangleCenter,
                color,
                0.0f,
                new Vector2(stringSize.X / 2, stringSize.Y / 2),
                1.0f,
                SpriteEffects.None,
                0.0f);
        }
        #endregion
    }
}
