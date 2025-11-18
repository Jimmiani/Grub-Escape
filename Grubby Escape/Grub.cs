using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grubby_Escape
{
    enum GrubState
    {
        Idle,
        Alert,
        Jump,
        Freed,
        Gone,
        Bounce,
        Wave
    }

    internal class Grub
    {
        private GrubState grubState;
        private Random _generator;

        private List<SoundEffect> _alertEffect;
        private List<SoundEffect> _jumpEffect;
        private List<SoundEffect> _sadEffect;
        private List<SoundEffect> _idleEffect;

        private List<Texture2D> _idleAnim;
        private List<Texture2D> _jumpAnim;
        private List<Texture2D> _alertAnim;
        private List<Texture2D> _currentAnim;
        private List<Texture2D> _waveAnim;

        private Rectangle _grubRect;
        private Vector2 _position;

        private int _currentFrame;
        private float _animTimer;
        private float _bounceTimer;
        private float _idleTimer;
        private int _idleIndex;

        public Grub(List<Texture2D> idleAnim, List<SoundEffect> idleEffect, List<SoundEffect> sadEffect, List<Texture2D> alertAnim, List<SoundEffect> alertEffect, List<Texture2D> jumpAnim, List<SoundEffect> jumpEffect)
        {
            _idleEffect = idleEffect;
            _alertEffect = alertEffect;
            _jumpEffect = jumpEffect;
            _sadEffect = sadEffect;

            _idleAnim = idleAnim;
            _alertAnim = alertAnim;
            _jumpAnim = jumpAnim;

            _generator = new Random();
            grubState = GrubState.Idle;
            _currentAnim = _idleAnim;
            _currentFrame = 0;
            _animTimer = 0;
            _idleTimer = 0;
            _idleIndex = 0;
            _grubRect = new Rectangle(100, 100, 157, 171);
        }

        public Grub(List<Texture2D> idleAnim, List<Texture2D> waveAnim, int x, int y, int size)
        {
            _idleAnim = idleAnim;
            _waveAnim = waveAnim;
            _currentAnim = idleAnim;
            grubState = GrubState.Bounce;
            _grubRect = new Rectangle(x, y, size, size * (171 / 157));
            _bounceTimer = 0;
        }

        public void Update(MouseState mouseState, MouseState prevMouseState, GameTime gameTime)
        {
            _animTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (grubState == GrubState.Idle)
            {
                _idleTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                _currentAnim = _idleAnim;

                if (_animTimer >= 1.0 / 12.0)
                {
                    _currentFrame++;
                    _animTimer = 0;
                    if (_currentFrame >= _idleAnim.Count)
                    {
                        _currentFrame = 1;
                    }
                }

                if (_idleTimer > 3.5f)
                {
                    _idleEffect[_idleIndex].Play();
                    _idleTimer = 0;
                    _idleIndex += 1;
                    if (_idleIndex >= _idleEffect.Count)
                    {
                        _idleIndex = 0;
                    }
                }
            }
            else if (grubState == GrubState.Alert)
            {
                _currentAnim = _alertAnim;

                if (_animTimer >= 1.0 / 12.0)
                {
                    _currentFrame++;
                    _animTimer = 0;
                    if (_currentFrame >= _alertAnim.Count)
                    {
                        _currentFrame = 1;
                    }
                }
            }
            else if (grubState == GrubState.Jump)
            {
                _currentAnim = _jumpAnim;
                if (_animTimer >= 1.0 / 10.0)
                {
                    _currentFrame++;
                    _animTimer = 0;
                    if (_currentFrame >= _jumpAnim.Count)
                    {
                        grubState = GrubState.Alert;
                        _currentFrame = 1;
                    }
                }
            }

            _grubRect.X = (int)_position.X;
            _grubRect.Y = (int)_position.Y;
        }
        public void Update(GameTime gameTime)
        {
            _animTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (grubState == GrubState.Bounce)
            {
                _bounceTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                _currentAnim = _idleAnim;
                if (_animTimer >= 1.0 / 12.0)
                {
                    _currentFrame++;
                    _animTimer = 0;
                    if (_currentFrame >= _idleAnim.Count)
                    {
                        _currentFrame = 0;
                    }
                }
                if (_bounceTimer >= 5)
                {
                    _bounceTimer = 0;
                    _currentFrame = 0;
                    _animTimer = 0;
                    grubState = GrubState.Wave;
                }
            }
            else if (grubState == GrubState.Wave)
            {
                _currentAnim = _waveAnim;
                if (_animTimer >= 1.0 / 12.0)
                {
                    _currentFrame++;
                    _animTimer = 0;
                    if (_currentFrame >= _waveAnim.Count)
                    {
                        grubState = GrubState.Bounce;
                        _currentFrame = 0;
                        _animTimer = 0;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (grubState != GrubState.Gone)
            {
                spriteBatch.Draw(_currentAnim[_currentFrame], _grubRect, Color.White);
            }

        }

        public void Draw(SpriteBatch spriteBatch, bool flip)
        {
            if (flip)
            {
                spriteBatch.Draw(_currentAnim[_currentFrame], _grubRect, null, Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
            }
            else
            {
                spriteBatch.Draw(_currentAnim[_currentFrame], _grubRect, Color.White);
            }
        }

        public void Happy()
        {
            if (grubState != GrubState.Alert)
            {
                _alertEffect[_generator.Next(0, _alertEffect.Count)].Play();

                _currentFrame = 0;
                grubState = GrubState.Alert;
            }
        }

        public void Jump()
        {
            _jumpEffect[_generator.Next(0, _jumpEffect.Count)].Play();

            _currentFrame = 0;
            grubState = GrubState.Jump;
        }

        public void Sad()
        {
            _sadEffect[_generator.Next(0, _sadEffect.Count)].Play();

            _currentFrame = 0;
            grubState = GrubState.Idle;
            _idleTimer = 0;
        }

        public GrubState CurrentState
        {
            get { return grubState; }
            set { grubState = value; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Rectangle Hitbox
        {
            get { return _grubRect; }
        }
    }
}
