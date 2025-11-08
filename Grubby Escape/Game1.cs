using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;

namespace Grubby_Escape
{
    enum GameState
    {
        Start,

    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        MouseState mouseState, prevMouseState;
        KeyboardState keyboardState, prevKeyboardState;
        GameState gameState;
        Camera2D camera;

        // Grubby

        Grub grubby;

        List<Texture2D> grubIdle;
        List<Texture2D> grubAlert;
        List<Texture2D> grubJump;
        List<Texture2D> grubBounce;
        List<Texture2D> grubWave;

        List<SoundEffect> sadEffect;
        List<SoundEffect> alertEffect;
        List<SoundEffect> jumpEffect;
        List<SoundEffect> idleEffect;

        bool isOnCart;
        bool isDragging;

        // Cart

        Cart cart;
        Texture2D cartTexture;
        Texture2D wheelTexture;
        SoundEffect startSfx, movingSfx, stopSfx;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();

            gameState = GameState.Start;
            camera = new Camera2D(GraphicsDevice.Viewport);

            grubIdle = new List<Texture2D>();
            grubAlert = new List<Texture2D>();
            grubJump = new List<Texture2D>();
            grubBounce = new List<Texture2D>();
            grubWave = new List<Texture2D>();

            sadEffect = new List<SoundEffect>();
            alertEffect = new List<SoundEffect>();
            jumpEffect = new List<SoundEffect>();
            idleEffect = new List<SoundEffect>();

            isOnCart = false;
            isDragging = false;

            base.Initialize();
            
            grubby = new Grub(grubIdle, idleEffect, sadEffect, grubAlert, alertEffect, grubJump, jumpEffect);
            cart = new Cart(cartTexture, wheelTexture, new Vector2(300, 600), startSfx, movingSfx, stopSfx);

            grubby.Position = new Vector2(0, 535);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // --------------------------- Images -------------------------------

            // Cart

            wheelTexture = Content.Load<Texture2D>("Grubby Escape/Images/Rails/mines_flip_platform_BG");
            cartTexture = Content.Load<Texture2D>("Grubby Escape/Images/Rails/Mines_Layered_0008_a");

            // Grubby

            for (int i = 0; i <= 6; i++)
                grubAlert.Add(Content.Load<Texture2D>("Grubby Escape/Images/Grubs/Alert 12/Cry_00" + i));
            for (int i = 0; i <= 18; i++)
                grubJump.Add(Content.Load<Texture2D>("Grubby Escape/Images/Grubs/Jump 10/Freed_" + i.ToString("D3")));
            for (int i = 0; i <= 37; i++)
                grubIdle.Add(Content.Load<Texture2D>("Grubby Escape/Images/Grubs/Idle 12/Idle_" + i.ToString("D3")));
            for (int i = 0; i <= 3; i++)
                grubBounce.Add(Content.Load<Texture2D>("Grubby Escape/Images/Grubs/Home Idle 12/Home Bounce_" + i.ToString("D3")));
            for (int i = 0; i <= 22; i++)
                grubWave.Add(Content.Load<Texture2D>("Grubby Escape/Images/Grubs/Home Wave 12/Home Wave_" + i.ToString("D3")));

            // --------------------------- Audio -------------------------------

            // Grubby

            for (int i = 1; i <= 3; i++)
                alertEffect.Add(Content.Load<SoundEffect>("Grubby Escape/Audio/Sound Effects/Grubs/Alert/grub_alert_" + i));
            for (int i = 1; i <= 2; i++)
                jumpEffect.Add(Content.Load<SoundEffect>("Grubby Escape/Audio/Sound Effects/Grubs/Freed/grub_free_" + i));
            for (int i = 1; i <= 3; i++)
                sadEffect.Add(Content.Load<SoundEffect>("Grubby Escape/Audio/Sound Effects/Grubs/Sad/grub_sad_" + i));
            for (int i = 1; i <= 4; i++)
                idleEffect.Add(Content.Load<SoundEffect>("Grubby Escape/Audio/Sound Effects/Grubs/Sad Idle/Grub_sad_idle_0" + i));

            // Cart

            movingSfx = Content.Load<SoundEffect>("Grubby Escape/Audio/Sound Effects/Rails/mines_conveyor_loop");
            startSfx = Content.Load<SoundEffect>("Grubby Escape/Audio/Sound Effects/Rails/lift_activate");
            stopSfx = Content.Load<SoundEffect>("Grubby Escape/Audio/Sound Effects/Rails/lift_arrive");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            prevMouseState = mouseState;
            mouseState = Mouse.GetState();

            prevKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();

            grubby.Update(mouseState, prevMouseState, gameTime);
            cart.Update(gameTime);
            camera.Update(gameTime);

            if (gameState == GameState.Start)
            {
                //camera.Follow(new Vector2(cart.Hitbox.Center.X + 500, cart.Hitbox.Center.Y - 100));
                
                if (!isDragging && grubby.Hitbox.Contains(mouseState.Position) && mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
                {
                    isDragging = true;
                }
                if (isDragging && mouseState.LeftButton == ButtonState.Pressed)
                {
                    grubby.Position = new Vector2(mouseState.X, mouseState.Y);
                }
                if (isDragging && mouseState.LeftButton == ButtonState.Released)
                {
                    isDragging = false;
                    grubby.Position = new Vector2(0, 535);
                }

                if (isOnCart)
                    grubby.Position = new Vector2(cart.Position.X + 30, cart.Position.Y - 65);
            }
            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(transformMatrix: camera.Transform);

            cart.Draw(_spriteBatch);
            grubby.Draw(_spriteBatch, true);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
