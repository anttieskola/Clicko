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
    using AEngine;
    #endregion

    enum BlockEvent
    {
        StateChange
    }

    enum BlockState
    {
        Idle,
        Reduction,
        FalseReduction
    }

    enum BlockDirection
    {
        Left,
        Right,
        Up,
        Down
    }

    /// <summary>
    /// Implement this listener to receive events from entity
    /// </summary>
    interface IBlockEventListener
    {
        void BlockEventSignal(BlockEvent e);
    }

    /// <summary>
    /// Static entity with only state
    /// </summary>
    class Block : IAAnimationEndListener
    {

        /// <summary>
        /// Listener for our events
        /// </summary>
        private readonly IBlockEventListener _entityEventListener;

        /// <summary>
        /// Block textures
        /// </summary>
        private readonly AAnimatedTexture _idleTexture;
        private readonly AAnimatedTexture _reductionTexture;
        private readonly AAnimatedTexture _falseReductionTexture;

        /// <summary>
        /// Current texture (helper)
        /// </summary>
        private AAnimatedTexture _currentTexture;

        /// <summary>
        /// Block font
        /// </summary>
        private readonly ASpriteFont _blockFont;

        /// <summary>
        /// Position (static)
        /// </summary>
        private readonly Rectangle _positionRectangle;

        /// <summary>
        /// Center of our position rectangle
        /// </summary>
        private Vector2 _origin;

        /// <summary>
        /// Our state
        /// </summary>
        private BlockState _entityState;

        /// <summary>
        /// Our rotation (right == 0 rad)
        /// </summary>
        private float _rotation;

        /// <summary>
        /// Currently static helpers
        /// </summary>
        private static Color _color = Color.White;

        public Block(ContentManager _content, IBlockEventListener entityEventListener, Rectangle positionRectangle)
        {
            _entityEventListener = entityEventListener;
            _positionRectangle = positionRectangle;


            _idleTexture = new AAnimatedTexture(_content, "block_idle", MyGlobals.GameTableCellSide, false, false, this, 1);
            _reductionTexture = new AAnimatedTexture(_content, "block_ani", MyGlobals.GameTableCellSide, false, false, this, 2);
            _falseReductionTexture = new AAnimatedTexture(_content, "block_x_ani", MyGlobals.GameTableCellSide, false, false, this, 3);
            _blockFont = new ASpriteFont(_content, "blockFont");

            _entityState = BlockState.Idle;
            _currentTexture = _idleTexture;
            _rotation = 0f;
            _origin.X = 0f;
            _origin.Y = 0f;
        }

        /// <summary>
        /// Listens for animation end events
        /// </summary>
        /// <param name="animationEnd"></param>
        public void AAnimationEnded(int animationEnd)
        {
            switch (animationEnd)
            {
                case 1:
                    // Idle texture animations not used so far
                    break;
                case 2:
                    SetStateAndDirection(BlockState.Idle, BlockDirection.Right);
                    _entityEventListener.BlockEventSignal(BlockEvent.StateChange);
                    break;
                case 3:
                    SetStateAndDirection(BlockState.Idle, BlockDirection.Right);
                    break;
            }
        }

        public void SetStateAndDirection(BlockState entityState, BlockDirection entityDirection)
        {
            setState(entityState);
            setDirection(entityDirection);
        }

        private void setState(BlockState entityState)
        {
            _entityState = entityState;
            switch (_entityState)
            {
                case BlockState.Idle:
                    _idleTexture.StartAnimation();
                    _currentTexture = _idleTexture;
                    break;
                case BlockState.Reduction:
                    _reductionTexture.StartAnimation();
                    _currentTexture = _reductionTexture;
                    break;
                case BlockState.FalseReduction:
                    _falseReductionTexture.StartAnimation();
                    _currentTexture = _falseReductionTexture;
                    break;
            }
        }

        private void setDirection(BlockDirection direction)
        {
            switch (direction)
            {
                case BlockDirection.Right:
                    _rotation = 0f;
                    _origin.X = 0f;
                    _origin.Y = 0f;
                    break;
                case BlockDirection.Up:
                    _rotation = (3 * MathHelper.Pi) / 2;
                    _origin.X = _positionRectangle.Width;
                    _origin.Y = 0f;
                    break;
                case BlockDirection.Left:
                    _rotation = MathHelper.Pi;
                    _origin.X = _positionRectangle.Width;
                    _origin.Y = _positionRectangle.Height;
                    break;
                case BlockDirection.Down:
                    _rotation = MathHelper.Pi / 2;
                    _origin.X = 0f;
                    _origin.Y = _positionRectangle.Height;
                    break;
                default:
                    throw new Exception("AEntity::SetDirection unsupported direction");
            }
        }

        public void Draw(SpriteBatch sb, int value)
        {
            // draw texture if value > 1 or if we drawing error animation
            if (value > 0 || _entityState == BlockState.FalseReduction)
            {
                _currentTexture.Draw(sb, _positionRectangle, _color, _rotation, _origin);
            }

            if (value > 0)
            {

                // draw text
                Color fontColor = Color.White;

                // Tweak color depending on value
                fontColor.G = (byte)(255 - (value * (255 / MyGlobals.GameTableCellMaxValue)));
                fontColor.B = (byte)(255 - (value * (255 / MyGlobals.GameTableCellMaxValue)));

                if (value < 10)
                {
                    _blockFont.DrawCentered(sb, value.ToString(), _positionRectangle, fontColor);
                }
                else
                {
                    _blockFont.DrawCentered(sb, value.ToString(), _positionRectangle, fontColor, 0.9f);
                }

            }
        }
    }
}
