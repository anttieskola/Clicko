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
    /// Static texture
    /// </summary>
    public class ATexture
    {
        #region Public Fields
        /// <summary>
        /// Basic texture values, all are set in constructor.
        /// </summary>
        public readonly string Name;
        public readonly int Width;
        public readonly int Height;

        /// <summary>
        /// Active / Visible
        /// </summary>
        public bool Active;
        #endregion

        #region Private Fields
        private readonly ContentManager _content;
        private Vector2 _position = new Vector2();
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor for static texture
        /// </summary>
        /// <param name="name"></param>
        /// <param name="game"></param>
        public ATexture(ContentManager content, String name)
        {
            _content = content;
            Name = name;
            Texture2D texture = _content.Load<Texture2D>(name);
            Width = texture.Width;
            Height = texture.Height;
            Active = true;
        }
        #endregion

        #region Draw
        /// <summary>
        /// Draw texture to wanted position
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="position"></param>
        /// <param name="color"></param>
        public void Draw(SpriteBatch sb, Vector2 position, Color color)
        {
            if (Active)
            {
                sb.Draw(_content.Load<Texture2D>(Name), position, color);
            }
        }
        /// <summary>
        /// Draw texture to rectangles upper left corner
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="position"></param>
        /// <param name="color"></param>
        public void Draw(SpriteBatch sb, Rectangle position, Color color)
        {
            if (Active)
            {
                _position.X = position.X;
                _position.Y = position.Y;
                sb.Draw(_content.Load<Texture2D>(Name), _position, color);
            }
        }
        #endregion
    }
}
