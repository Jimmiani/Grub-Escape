using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grubby_Escape
{
    internal class Concealer
    {
        private Texture2D texture;
        private Rectangle _hitbox;
        private float _color;
        private bool _fadeOut;
        private float _fadeAmount;
        public Concealer(GraphicsDevice graphicsDevice, Rectangle hitbox)
        {
            _hitbox = hitbox;
            _color = 1;
            _fadeOut = false;
            _fadeAmount = 0;
            texture = new Texture2D(graphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_fadeOut && _color > 0)
            {
                _fadeAmount += dt;
                _color = MathHelper.Lerp(1, 0, _fadeAmount);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, _hitbox, Color.White * _color);
        }
        public void Remove()
        {
            _fadeOut = true;
        }
        public Rectangle Hitbox
        {
            get { return _hitbox; }
        }
    }
}
