using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grubby_Escape
{
    internal class Particle
    {
        public Texture2D Texture { get; set; }        // The texture that will be drawn to represent the particle
        public Vector2 Position { get; set; }        // The current position of the particle        
        public Vector2 Velocity { get; set; }        // The speed of the particle at the current instance
        public float Angle { get; set; }            // The current angle of rotation of the particle
        public float AngularVelocity { get; set; }    // The speed that the angle is changing
        public Color Color { get; set; }            // The color of the particle
        public float Size { get; set; }                // The size of the particle
        public float TTL { get; set; }                // The 'time to live' of the particle
        public float Opacity { get; set; }
        public float InitialTTL { get; set; }
        public float InitialSize { get; set; }
        private float _lifePercent;
        private float _maxOpacity;
        


        public Particle(Texture2D texture, Vector2 position, Vector2 velocity,
            float angle, float angularVelocity, Color color, float size, float ttl, float opacity)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            Angle = angle;
            AngularVelocity = angularVelocity;
            Color = color;
            Size = size;
            InitialSize = size;
            TTL = ttl;
            InitialTTL = ttl;
            _lifePercent = 1;
            _maxOpacity = opacity;
            Opacity = opacity;
        }

        public void Update(bool fade, bool fadeIn, bool fadeOut)
        {
            TTL--;
            Position += Velocity;
            Angle += AngularVelocity;

            if (fade)
            {
                _lifePercent = 1f - (TTL / InitialTTL);
                if (_lifePercent < 0.3f && fadeIn)
                {
                    Opacity = (_lifePercent / 0.3f) * _maxOpacity;
                }
                else if (_lifePercent > 0.7f && fadeOut)
                {
                    Opacity = ((1f - _lifePercent) / 0.3f) * _maxOpacity;
                }
                else
                {
                    Opacity = _maxOpacity;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

            spriteBatch.Draw(Texture, Position, sourceRectangle, Color * Opacity,
                Angle, origin, Size, SpriteEffects.None, 0f);
        }
    }
}
