using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grubby_Escape
{

    enum CartState
    {
        Start,
        Moving,
        Stop,
        Stopped
    }

    internal class Cart
    {
        private CartState _cartState;
        private Rectangle _hitbox;
        private Vector2 _position;
        private Vector2 _velocity;
        private Texture2D _cartTex;
        private Texture2D _wheelTex;
        private float _rotationAmt;
        private SoundEffect _movingSfx;
        private SoundEffectInstance _movingSfxInstance;
        private SoundEffect _startSfx;
        private SoundEffect _stopSfx;
        private float _startTimer;
        private float _moveSpeed;

        public Cart(Texture2D cartTexture, Texture2D wheelTexture, Vector2 startingPos, SoundEffect startSfx, SoundEffect movingSfx, SoundEffect stopSfx)
        {
            _cartTex = cartTexture;
            _wheelTex = wheelTexture;
            _position = startingPos;
            _startSfx = startSfx;
            _stopSfx = stopSfx;
            _movingSfx = movingSfx;
            _movingSfxInstance = _movingSfx.CreateInstance();

            _cartState = CartState.Stopped;
            _velocity = new Vector2(0, 0);
            _hitbox = new Rectangle((int)_position.X, (int)_position.Y, cartTexture.Width, cartTexture.Height);
            _moveSpeed = 6;
        }

        public void Update(GameTime gameTime)
        {
            if (_cartState == CartState.Start)
            {
                _startTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (_velocity.X < _moveSpeed)
                {
                    _velocity.X = (_startTimer * _startTimer) * (_moveSpeed / 2);
                }
                else
                {
                    _startTimer = 0;
                    _cartState = CartState.Moving;
                }
            }
            else if (_cartState == CartState.Moving)
            {
                _velocity.X = _moveSpeed;

                if (_movingSfxInstance.State != SoundState.Playing)
                {
                    _movingSfxInstance.IsLooped = true;
                    _movingSfxInstance.Play();
                }
            }
            else if (_cartState == CartState.Stop)
            {
                _startTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (_velocity.X > 0)
                {
                    _velocity.X = _moveSpeed - ((_startTimer * _startTimer) * (_moveSpeed / 2));
                }
                else
                {
                    _startTimer = 0;
                    _cartState = CartState.Stopped;
                }
            }
            else if (_cartState == CartState.Stopped)
            {
                _velocity = Vector2.Zero;
            }

            _rotationAmt += _velocity.X * 0.02f;
            _hitbox.X = (int)_position.X;
            _hitbox.Y = (int)_position.Y;
            _position += _velocity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_cartTex, _hitbox, Color.White);

            spriteBatch.Draw(_wheelTex, new Rectangle(_hitbox.Left + 40, _hitbox.Bottom - 30, 80, 80), null, Color.White, _rotationAmt, new Vector2(_wheelTex.Width / 2, _wheelTex.Height / 2), SpriteEffects.None, 0f);
            spriteBatch.Draw(_wheelTex, new Rectangle(_hitbox.Right - 40, _hitbox.Bottom - 30, 80, 80), null, Color.White, _rotationAmt, new Vector2(_wheelTex.Width / 2, _wheelTex.Height / 2), SpriteEffects.None, 0f);
        }

        public void Start(float speed)
        {
            _moveSpeed = speed;
            _startSfx.Play();
            _cartState = CartState.Start;
        }
        public void Move(float speed)
        {
            _moveSpeed = speed;

            _cartState = CartState.Moving;
        }
        public void Stop()
        {
            _stopSfx.Play();
            _movingSfxInstance.Stop();
            _cartState = CartState.Stop;
        }

        public Rectangle Hitbox
        {
            get { return _hitbox; }
        }

        public Vector2 Position
        {
            get { return _position; }
        }
    }
}
