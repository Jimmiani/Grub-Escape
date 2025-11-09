using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grubby_Escape
{
    public class Camera2D
    {
        private Matrix _transform; // Used to move screen
        public Matrix Transform => _transform; // Makes matrix public

        public Vector2 Position { get; private set; } // Position of the camera
        public Viewport Viewport { get; } // Viewport (rectangle of visible window)
        public float Zoom { get; set; } = 1f; // Zoom for matrix
        public float Rotation { get; set; } = 0f; // Rotation for matrix

        // ----- Screen Shake Fields -----

        private float _shakeDuration = 0f;
        private float _totalShakeDuration = 0f;
        private float _initialIntensity = 0f;
        private bool _shakeFade = false;
        private Random _generator = new Random();
        private Vector2 _shakeOffset = Vector2.Zero;

        public Camera2D(Viewport viewport)
        {
            Viewport = viewport;
        }

        public void Follow(Vector2 targetPosition)
        {
            // Lock the camera to the target position
            Position = targetPosition - new Vector2(Viewport.Width / 2f, Viewport.Height / 2f); // Centers it so it doesn't end up in the top left corner
        }

        public void Update(GameTime gameTime)
        {
            // Reduce shake timer
            if (_shakeDuration > 0)
            {
                _shakeDuration -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (_shakeFade)
                {
                    float progress = _shakeDuration / _totalShakeDuration;
                    float currentIntensity = _initialIntensity * progress * progress;

                    // Random offset for shake effect
                    _shakeOffset.X = (float)(_generator.NextDouble() * 2 - 1) * currentIntensity;
                    _shakeOffset.Y = (float)(_generator.NextDouble() * 2 - 1) * currentIntensity;
                }
                else
                {
                    _shakeOffset.X = (float)(_generator.NextDouble() * 2 - 1) * _initialIntensity;
                    _shakeOffset.Y = (float)(_generator.NextDouble() * 2 - 1) * _initialIntensity;
                }
            }
            else
            {
                _shakeOffset = Vector2.Zero;
            }

            // Combine shake offset with camera position
            Vector2 finalPosition = Position + _shakeOffset;

            // Build transformation matrix
            _transform =
                Matrix.CreateTranslation(new Vector3(-finalPosition, 0f)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(Zoom, Zoom, 1f);
        }
        public void Shake(float intensity, float duration, bool fade)
        {
            _initialIntensity = intensity;
            _shakeDuration = duration;
            _totalShakeDuration = duration;
            _shakeFade = fade;
        }

        public Matrix GetParallaxTransform(float factor)
        {
            return Matrix.CreateTranslation(-Position.X * factor, -Position.Y * factor, 0f) *
                   Matrix.CreateRotationZ(Rotation) *
                   Matrix.CreateScale(Zoom, Zoom, 1f);
        }
    }
}
