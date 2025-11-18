using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grubby_Escape.Content
{
    internal class Lumafly
    {
        private List<Texture2D> _textures;
        private Rectangle _hitbox;
        private Vector2 _position;
        private Vector2 _velocity;
        private Vector2 _stopPos;
        private bool _reachedTarget;

        private float _animTimer = 0;
        private int _currentFrame = 0;

        public Lumafly(List<Texture2D> textures, Vector2 startingPos)
        {
            _textures = textures;
            _position = startingPos;
            _stopPos = startingPos;
            _velocity = Vector2.Zero;
            _hitbox = new Rectangle((int)_position.X, (int)_position.Y, _textures[0].Width, _textures[0].Height);
            _reachedTarget = true;
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _animTimer += dt;

            if (_animTimer > 1f / 12f)
            {
                _currentFrame++;
                _animTimer = 0;
                if (_currentFrame >=  _textures.Count)
                {
                    _currentFrame = 0;
                }
            }


            Vector2 toTarget = _stopPos - _position;
            float distance = toTarget.Length();

            if (distance < 2f && !_reachedTarget)
            {
                _position = _stopPos;
                _velocity = Vector2.Zero;
                _reachedTarget = true;
            }


            _position += _velocity * dt;

            _hitbox.X = (int)_position.X;
            _hitbox.Y = (int)_position.Y;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_textures[_currentFrame], _position, Color.White);
        }
        public void Move(Vector2 newPos, float speed)
        {
            _reachedTarget = false;
            _stopPos = newPos;
            Vector2 distance = _stopPos - _position;

            if (distance != Vector2.Zero)
            {
                distance.Normalize();
                _velocity = distance * speed;
            }
        }
        public Rectangle Hitbox
        {
            get { return _hitbox; }
        }
        public Vector2 Position
        {
            get { return _position; }
        }
        public bool ReachedTarget
        {
            get { return _reachedTarget; }
        }
    }
}
