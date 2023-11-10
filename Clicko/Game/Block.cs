// Copyright © 2011 Antti Eskola. All rights reserved.
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

namespace Clicko
{
    enum ABlockEvent
    {
        StateChange
    };

    enum ABlockState
    {
        Idle,
        Reduction,
        FalseReduction
    };

    enum ABlockDirection
    {
        Left,
        Right,
        Up,
        Down
    };

    /// <summary>
    /// Implement this listener to receive events from entity
    /// </summary>
    interface IABlockEventListener
    {
        void ABlockEventSignal(ABlockEvent e);
    };

    /// <summary>
    /// Static entity with only state
    /// </summary>
    class ABlock : IAAnimationEndListener
    {

        /// <summary>
        /// Listener for our events
        /// </summary>
        private readonly IABlockEventListener _entityEventListener;

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
        private ABlockState _entityState;

        /// <summary>
        /// Our rotation (right == 0 rad)
        /// </summary>
        private float _rotation;

        /// <summary>
        /// Currently static helpers
        /// </summary>
        private static Color _color = Color.White;

        public ABlock(ContentManager _content, IABlockEventListener entityEventListener, Rectangle positionRectangle)
        {
            _entityEventListener = entityEventListener;
            _positionRectangle = positionRectangle;


            _idleTexture = new AAnimatedTexture(_content, "block_idle", AGlobals.GameTableCellSide, false, false, this, AAnimationEnd.GameBlockReduction);
            _reductionTexture = new AAnimatedTexture(_content, "block_arrow_ani", AGlobals.GameTableCellSide, false, false, this, AAnimationEnd.GameBlockReduction);
            _falseReductionTexture = new AAnimatedTexture(_content, "block_x_ani", AGlobals.GameTableCellSide, false, false, this, AAnimationEnd.GameBlockFalseReduction);
            _blockFont = new ASpriteFont(_content, "blockFont");

            _entityState = ABlockState.Idle;
            _currentTexture = _idleTexture;
            _rotation = 0f;
            _origin.X = 0f;
            _origin.Y = 0f;
        }

        /// <summary>
        /// Listens for animation end events
        /// </summary>
        /// <param name="animationEnd"></param>
        public void AAnimationEnded(AAnimationEnd animationEnd)
        {
            switch (animationEnd)
            {
                case AAnimationEnd.GameBlockReduction:
                    SetStateAndDirection(ABlockState.Idle, ABlockDirection.Right);
                    _entityEventListener.ABlockEventSignal(ABlockEvent.StateChange);
                    break;
                case AAnimationEnd.GameBlockFalseReduction:
                    SetStateAndDirection(ABlockState.Idle, ABlockDirection.Right);
                    break;
                default:
                    throw new Exception("AEntity:AAnimationEnded invalid animation end event");
            }
        }

        public void SetStateAndDirection(ABlockState entityState, ABlockDirection entityDirection)
        {
            setState(entityState);
            setDirection(entityDirection);
        }

        private void setState(ABlockState entityState)
        {
            _entityState = entityState;
            switch (_entityState)
            {
                case ABlockState.Idle:
                    _idleTexture.StartAnimation();
                    _currentTexture = _idleTexture;
                    break;
                case ABlockState.Reduction:
                    _reductionTexture.StartAnimation();
                    _currentTexture = _reductionTexture;
                    break;
                case ABlockState.FalseReduction:
                    _falseReductionTexture.StartAnimation();
                    _currentTexture = _falseReductionTexture;
                    break;
            }
        }

        private void setDirection(ABlockDirection direction)
        {
            switch (direction)
            {
                case ABlockDirection.Right:
                    _rotation = 0f;
                    _origin.X = 0f;
                    _origin.Y = 0f;
                    break;
                case ABlockDirection.Up:
                    _rotation = (3*MathHelper.Pi)/2;
                    _origin.X = _positionRectangle.Width;
                    _origin.Y = 0f;
                    break;
                case ABlockDirection.Left:
                    _rotation = MathHelper.Pi;
                    _origin.X = _positionRectangle.Width;
                    _origin.Y = _positionRectangle.Height;
                    break;
                case ABlockDirection.Down:
                    _rotation = MathHelper.Pi/2;
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
            if (value > 0 || _entityState == ABlockState.FalseReduction)
            {
                _currentTexture.Draw(sb, _positionRectangle, _color, _rotation, _origin);
            }

            if (value > 0)
            {

                // draw text
                Color fontColor = Color.White;

                // Tweak color depending on value
                fontColor.G = (byte)(255 - (value * (255 / AGlobals.GameTableCellMaxValue)));
                fontColor.B = (byte)(255 - (value * (255 / AGlobals.GameTableCellMaxValue)));

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
