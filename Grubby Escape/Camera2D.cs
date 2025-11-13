using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        // RenderTarget2D internal resolution (used for clamping)
        public int InternalWidth = 1280;
        public int InternalHeight = 720;

        // Level bounds
        public Rectangle LevelBounds; // Set this to the rectangle of your level

        // ----- Screen Shake Fields -----

        private float _shakeDuration = 0f;
        private float _totalShakeDuration = 0f;
        private float _initialIntensity = 0f;
        private bool _shakeFade = false;
        private Random _generator = new Random();
        private Vector2 _shakeOffset = Vector2.Zero;
        private Vector2 _finalPosition = Vector2.Zero;

        // ----- Smooth Follow / Look-Ahead -----
        public float LerpSpeed = 5f;               // How fast camera catches up
        public float LookAheadDistanceX = 100f;    // How far ahead camera looks
        public float LookAheadDistanceY = 60f;    // How far ahead camera looks
        public float LookAheadLerp = 3f;           // How fast look-ahead adjusts
        private Vector2 _lookAhead = Vector2.Zero; // Current look-ahead offset

        public Camera2D(Viewport viewport, Rectangle levelBounds, Vector2 startingPos, int internalWidth, int internalHeight)
        {
            Viewport = viewport;
            LevelBounds = levelBounds;
            Position = startingPos;
            InternalWidth = internalWidth;
            InternalHeight = internalHeight;
        }
        public void Update(GameTime gameTime, Vector2 targetPosition, Vector2 targetVelocity)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // --- Calculate target look-ahead ---

            Vector2 targetLookAhead = Vector2.Zero;

            // Horizontal
            if (targetVelocity.X > 0)
                targetLookAhead.X = LookAheadDistanceX; // moving right

            else if (targetVelocity.X < 0)
                targetLookAhead.X = -LookAheadDistanceX; // moving left
            
            else
                targetLookAhead.X = 0; // no horizontal movement
            

            // Vertical
            if (targetVelocity.Y > 0)
                targetLookAhead.Y = LookAheadDistanceY; // moving down
            
            else if (targetVelocity.Y < 0)
                targetLookAhead.Y = -LookAheadDistanceY; // moving up
            
            else
                targetLookAhead.Y = 0; // no vertical movement
            

            // --- Smoothly interpolate look-ahead ---
            _lookAhead.X = MathHelper.Lerp(_lookAhead.X, targetLookAhead.X, LookAheadLerp * deltaTime);
            _lookAhead.Y = MathHelper.Lerp(_lookAhead.Y, targetLookAhead.Y, LookAheadLerp * deltaTime);

            // --- Smooth follow ---
            Vector2 desiredPosition = targetPosition + _lookAhead;

            // --- Clamp camera to level bounds using internal resolution ---
            float halfWidth = InternalWidth / 2f;
            float halfHeight = InternalHeight / 2f;

            float minX = LevelBounds.Left + halfWidth;
            float maxX = LevelBounds.Right - halfWidth;
            float minY = LevelBounds.Top + halfHeight;
            float maxY = LevelBounds.Bottom - halfHeight;

            // Only clamp if level is larger than viewport
            if (LevelBounds.Width < InternalWidth)
                desiredPosition.X = LevelBounds.Center.X;
            else
                desiredPosition.X = MathHelper.Clamp(desiredPosition.X, minX, maxX);

            if (LevelBounds.Height < InternalHeight)
                desiredPosition.Y = LevelBounds.Center.Y;
            else
                desiredPosition.Y = MathHelper.Clamp(desiredPosition.Y, minY, maxY);

            // Smoothly move camera center to desired position
            Position = Vector2.Lerp(Position, desiredPosition, LerpSpeed * deltaTime);


            // --- Screen Shake ---

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
            _finalPosition = Position + _shakeOffset;

            // Build transformation matrix
            _transform =
                Matrix.CreateTranslation(new Vector3(-_finalPosition, 0f)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(Zoom, Zoom, 1f) *
                Matrix.CreateTranslation(new Vector3(InternalWidth / 2f, InternalHeight / 2f, 0f));
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
            Vector2 cameraCenter = new Vector2(InternalWidth / 2f, InternalHeight / 2f);

            // The offset from the camera's center, scaled by (factor - 1)
            Vector2 parallaxOffset = (_finalPosition - cameraCenter) * (factor - 1f);

            return Matrix.CreateTranslation(new Vector3(-_finalPosition - parallaxOffset, 0f)) *
                   Matrix.CreateRotationZ(Rotation) *
                   Matrix.CreateScale(Zoom, Zoom, 1f) *
                   Matrix.CreateTranslation(new Vector3(cameraCenter, 0f));
        }
    }
}
