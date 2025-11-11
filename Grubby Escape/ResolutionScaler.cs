using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Grubby_Escape
{
    public class ResolutionScaler
    {

        private RenderTarget2D _renderTarget;
        private GraphicsDevice _graphicsDevice;
        private int _targetWidth;
        private int _targetHeight;

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
    }
}
