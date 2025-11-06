using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grubby_Escape
{
    internal class Cart
    {
        private Rectangle _hitbox;
        private Vector2 _position;
        private Texture2D _cartTex;
        private Texture2D _wheelTex;
        private float _rotationAmt;

        public Cart(Texture2D cartTexture, Texture2D wheelTexture, Vector2 startingPos)
        {
            _cartTex = cartTexture;
            _wheelTex = wheelTexture;
            _position = startingPos;

            _hitbox = new Rectangle();
        }

        public void Update(GameTime gameTime)
        {
            _hitbox.X = (int)_position.X;
            _hitbox.Y = (int)_position.Y;
        }
    }
}
