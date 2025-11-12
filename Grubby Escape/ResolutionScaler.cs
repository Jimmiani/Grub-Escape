using Microsoft.Xna.Framework;
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
    public class ResolutionScaler
    {
        private RenderTarget2D _renderTarget;
        private GraphicsDevice _graphicsDevice;
        private int _targetWidth;
        private int _targetHeight;

        public int TargetWidth => _targetWidth;
        public int TargetHeight => _targetHeight;

        public ResolutionScaler(GraphicsDevice graphicsDevice, int width, int height)
        {
            _graphicsDevice = graphicsDevice;
            _targetWidth = width;
            _targetHeight = height;

            _renderTarget = new RenderTarget2D(_graphicsDevice, _targetWidth, _targetHeight);
        }

        public void DrawToCanvas()
        {
            _graphicsDevice.SetRenderTarget(_renderTarget);
            _graphicsDevice.Clear(Color.CornflowerBlue);
        }

        public void DrawToScreen(SpriteBatch spriteBatch)
        {
            _graphicsDevice.SetRenderTarget(null); // Back to screen
            _graphicsDevice.Clear(Color.Black);

            // Calculate scale to fit screen
            float scaleX = _graphicsDevice.Viewport.Width / 1280f;
            float scaleY = _graphicsDevice.Viewport.Height / 720f;
            float scale = Math.Min(scaleX, scaleY); // Keeps aspect ratio

            int width = (int)(1280 * scale);
            int height = (int)(720 * scale);

            Rectangle destination = new Rectangle(
                (_graphicsDevice.Viewport.Width - width) / 2,  // Center X
                (_graphicsDevice.Viewport.Height - height) / 2, // Center Y
                width,
                height);

            spriteBatch.Begin();
            spriteBatch.Draw(_renderTarget, destination, Color.White);
            spriteBatch.End();
        }

        public Vector2 GetMouseWorldPosition()
        {
            // Get the raw mouse position in window coordinates
            MouseState mouseState = Mouse.GetState();
            Vector2 mouseScreen = new Vector2(mouseState.X, mouseState.Y);

            // Calculate the scale factor and offset (same as in DrawToScreen)
            float scaleX = _graphicsDevice.Viewport.Width / (float)_targetWidth;
            float scaleY = _graphicsDevice.Viewport.Height / (float)_targetHeight;
            float scale = Math.Min(scaleX, scaleY);

            int width = (int)(_targetWidth * scale);
            int height = (int)(_targetHeight * scale);

            // Calculate black bar offsets (letterboxing/pillarboxing)
            float offsetX = (_graphicsDevice.Viewport.Width - width) / 2f;
            float offsetY = (_graphicsDevice.Viewport.Height - height) / 2f;

            // Subtract offset and scale down to RT coordinates
            Vector2 mouseRT = (mouseScreen - new Vector2(offsetX, offsetY)) / scale;

            // Optional: clamp to RT bounds
            mouseRT.X = MathHelper.Clamp(mouseRT.X, 0, _targetWidth);
            mouseRT.Y = MathHelper.Clamp(mouseRT.Y, 0, _targetHeight);

            return mouseRT;
        }
    }
}
