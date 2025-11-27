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
    public class DrawingCanvas
    {
        private struct DrawnPoint
        {
            public Vector2 Position;
            public Color Color;
        }

        private List<DrawnPoint> drawnPoints = new List<DrawnPoint>();
        private Texture2D pixelTexture; // A 1x1 white texture used as the brush
        private Color currentColor = Color.Red;
        private ResolutionScaler _resolutionScaler;

        // Fields for smooth drawing interpolation and input debouncing
        private MouseState previousMouseState;
        private Vector2 previousMousePosition;
        private KeyboardState previousKeyboardState;
        private const float INTERPOLATION_STEP = 2.0f; // The distance between inserted points (in pixels)
        private float scale = 4.0f;

        // Properties for simple color cycling
        private List<Color> availableColors = new List<Color>
        {
            Color.Red,
            Color.Blue,
            Color.Black
        };
        private int colorIndex = 0;


        public DrawingCanvas(GraphicsDevice graphicsDevice, ResolutionScaler resolutionScaler)
        {
            // Create a 1x1 white texture to use as our drawing "pixel" or brush tip
            pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            pixelTexture.SetData(new[] { Color.White });

            // Initialize state
            previousMouseState = Mouse.GetState();
            previousMousePosition = new Vector2(previousMouseState.X, previousMouseState.Y);
            _resolutionScaler = resolutionScaler;
        }
        public void Update(GameTime gameTime)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();
            MouseState currentMouseState = Mouse.GetState();
            Vector2 currentMousePosition = _resolutionScaler.GetMouseWorldPosition();

            // 1. Check for color change (Right-Click Debounce)
            // Only cycle color on the frame the button is pressed down (not every frame it is held)
            if (currentKeyboardState.IsKeyDown(Keys.C) && previousKeyboardState.IsKeyUp(Keys.C))
            {
                CycleColor();
            }

            if (currentKeyboardState.IsKeyDown(Keys.OemMinus) && previousKeyboardState.IsKeyUp(Keys.OemMinus))
            {
                scale--;
            }

            if (currentKeyboardState.IsKeyDown(Keys.OemPlus) && previousKeyboardState.IsKeyUp(Keys.OemPlus))
            {
                scale++;
            }

            // 2. Handle actual drawing with the left mouse button
            if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                // Case 1: Beginning of a new stroke (Left button just pressed)
                if (previousMouseState.LeftButton == ButtonState.Released)
                {
                    // Start the line at the initial position
                    AddDrawnPoint(currentMousePosition);
                }
                // Case 2: Continuing an existing stroke (Left button held down)
                else
                {
                    // Calculate the distance the mouse has moved
                    float distance = Vector2.Distance(previousMousePosition, currentMousePosition);

                    // If the distance is greater than our step size, we need to interpolate
                    if (distance > INTERPOLATION_STEP)
                    {
                        // Calculate how many intermediate points to insert
                        int steps = (int)Math.Ceiling(distance / INTERPOLATION_STEP);

                        for (int i = 1; i <= steps; i++)
                        {
                            // Calculate the position for the intermediate point using Linear Interpolation (Lerp)
                            float t = (float)i / steps;
                            Vector2 interpolatedPosition = Vector2.Lerp(previousMousePosition, currentMousePosition, t);

                            AddDrawnPoint(interpolatedPosition);
                        }
                    }
                    // Always add the current position to ensure the line ends correctly this frame
                    // (This is redundant if steps > 0 but harmless)
                    AddDrawnPoint(currentMousePosition);
                }
            }

            // 3. Store current state for the next frame's comparison
            previousMouseState = currentMouseState;
            previousMousePosition = currentMousePosition;
            previousKeyboardState = currentKeyboardState;
        }

        private void AddDrawnPoint(Vector2 position)
        {
            DrawnPoint newPoint = new DrawnPoint
            {
                Position = position,
                Color = currentColor
            };
            drawnPoints.Add(newPoint);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var point in drawnPoints)
            {
                // Draw the 1x1 texture at the point's position, using the point's stored color.
                // Scale the pixel up (e.g., 5x5) to make the line thicker and easier to see.
                spriteBatch.Draw(
                    pixelTexture,
                    point.Position,
                    null,
                    point.Color,
                    0f,
                    Vector2.Zero,
                    scale, // Scale factor to make the brush 5x5 pixels wide
                    SpriteEffects.None,
                    0f
                );
            }
        }
        public void Clear()
        {
            drawnPoints.Clear();
        }
        public void CycleColor()
        {
            colorIndex = (colorIndex + 1) % availableColors.Count;
            currentColor = availableColors[colorIndex];
        }
        public string GetCurrentColorName()
        {
            return currentColor.ToString();
        }
    }
}
