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
        TowardsBrokenRail,
        Reverse
    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        MouseState mouseState, prevMouseState;
        KeyboardState keyboardState, prevKeyboardState;
        GameState gameState;
        Camera2D camera;

        // Backgrounds

        Texture2D BG1, BG2, BG3, BG4, BG5;
        Texture2D lightTex;

        // Music

        SoundEffect mainMusicSfx, bassMusicSfx, actionMusicSfx, machineryAtmosSfx;
        SoundEffectInstance mainMusic, bassMusic, actionMusic, machineryAtmos;

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
        float cartStartTimer, cartStopTimer;
        bool hasStarted, hasStopped;

        // Floors

        Texture2D woodFloor, railFloor, railBrokenL, railBrokenR;

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
            hasStarted = false;
            hasStopped = false;
            cartStartTimer = 0;
            cartStopTimer = 0;

            base.Initialize();
            
            grubby = new Grub(grubIdle, idleEffect, sadEffect, grubAlert, alertEffect, grubJump, jumpEffect);
            cart = new Cart(cartTexture, wheelTexture, new Vector2(300, 600), startSfx, movingSfx, stopSfx);

            grubby.Position = new Vector2(0, 630);

            mainMusic.Volume = 0.8f;
            mainMusic.IsLooped = true;
            mainMusic.Play();

            bassMusic.Volume = 0;
            bassMusic.IsLooped = true;
            bassMusic.Play();

            actionMusic.Volume = 0;
            actionMusic.IsLooped = true;
            actionMusic.Play();

            machineryAtmos.Volume = 0.7f;
            machineryAtmos.IsLooped = true;
            machineryAtmos.Play();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // --------------------------- Images -------------------------------

            // Backgrounds

            BG1 = Content.Load<Texture2D>("Grubby Escape/Images/Background/Extremely Far/BG_bone_01");
            BG2 = Content.Load<Texture2D>("Grubby Escape/Images/Background/Extremely Far/BG_bone_02");
            BG3 = Content.Load<Texture2D>("Grubby Escape/Images/Background/Extremely Far/BG_bone_03");
            BG4 = Content.Load<Texture2D>("Grubby Escape/Images/Background/Extremely Far/BG_egg_05");
            BG5 = Content.Load<Texture2D>("Grubby Escape/Images/Background/Extremely Far/BG_twist");
            lightTex = Content.Load<Texture2D>("Grubby Escape/Images/Lights/white_light");

            // Cart

            wheelTexture = Content.Load<Texture2D>("Grubby Escape/Images/Rails/mines_flip_platform_BG");
            cartTexture = Content.Load<Texture2D>("Grubby Escape/Images/Rails/Mines_Layered_0008_a");
            woodFloor = Content.Load<Texture2D>("Grubby Escape/Images/Floor/mine_floor_01");
            railFloor = Content.Load<Texture2D>("Grubby Escape/Images/Rails/mine_rail_01");
            railBrokenL = Content.Load<Texture2D>("Grubby Escape/Images/Rails/mine_rail_02");
            railBrokenR = Content.Load<Texture2D>("Grubby Escape/Images/Rails/mine_rail_03");

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

            // Music

            mainMusicSfx = Content.Load<SoundEffect>("Grubby Escape/Audio/Music/Songs/S26 Crystal MAIN");
            bassMusicSfx = Content.Load<SoundEffect>("Grubby Escape/Audio/Music/Songs/S26 Crystal BASS");
            actionMusicSfx = Content.Load<SoundEffect>("Grubby Escape/Audio/Music/Songs/S26 Crystal ACTION");
            machineryAtmosSfx = Content.Load<SoundEffect>("Grubby Escape/Audio/Music/Atmos/mines_machinery_atmos");

            mainMusic = mainMusicSfx.CreateInstance();
            bassMusic = bassMusicSfx.CreateInstance();
            actionMusic = actionMusicSfx.CreateInstance();
            machineryAtmos = machineryAtmosSfx.CreateInstance();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            prevMouseState = mouseState;
            mouseState = Mouse.GetState();
            Vector2 mouseWorldPos = Vector2.Transform(mouseState.Position.ToVector2(), Matrix.Invert(camera.Transform));

            prevKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();

            grubby.Update(mouseState, prevMouseState, gameTime);
            cart.Update(gameTime);
            camera.Update(gameTime);

            Debug.WriteLine(mouseWorldPos);

            if (isOnCart)
                grubby.Position = new Vector2(cart.Position.X + 30, cart.Position.Y - 65);

            if (gameState == GameState.Start)
            {
                camera.Follow(new Vector2(cart.Hitbox.Center.X + 500, cart.Hitbox.Center.Y - 100));

                if (!isDragging && grubby.Hitbox.Contains(mouseWorldPos) && mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
                {
                    isDragging = true;
                    grubby.Happy();
                }
                if (isDragging && mouseState.LeftButton == ButtonState.Pressed)
                {
                    grubby.Position = new Vector2(mouseWorldPos.X - grubby.Hitbox.Width / 2, mouseWorldPos.Y - grubby.Hitbox.Height / 2);
                }
                if (isDragging && mouseState.LeftButton == ButtonState.Released && !grubby.Hitbox.Intersects(cart.Hitbox))
                {
                    isDragging = false;
                    grubby.Position = new Vector2(0, 630);
                    grubby.Sad();
                }
                else if (isDragging && mouseState.LeftButton == ButtonState.Released && grubby.Hitbox.Intersects(cart.Hitbox))
                {
                    isDragging = false;
                    isOnCart = true;
                    gameState = GameState.TowardsBrokenRail;
                    grubby.Jump();
                }
            }
            else if (gameState == GameState.TowardsBrokenRail)
            {
                camera.Follow(new Vector2(cart.Hitbox.Center.X + 500, cart.Hitbox.Center.Y - 100));
                cartStartTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (cartStartTimer > 1 && !hasStarted)
                {
                    camera.Shake(3, 2, true);
                    cart.Start(8);
                    hasStarted = true;
                }

                if (cart.Position.X > 4900)
                {
                    if (!hasStopped)
                        cart.Stop();
                    hasStopped = true;
                    cartStopTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (cartStopTimer > 1.5f)
                    {
                        grubby.Sad();
                        gameState = GameState.Reverse;
                    }
                }
            }


                base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(transformMatrix: camera.GetParallaxTransform(0.2f));

            _spriteBatch.Draw(BG1, new Rectangle(0, 0, 1500, 1800), Color.White);
            _spriteBatch.Draw(BG2, new Rectangle(1500, 600, 1000, 1100), Color.White);
            _spriteBatch.Draw(BG3, new Rectangle(700, 100, 1900, 2000), Color.White);
            _spriteBatch.Draw(BG4, new Rectangle(2000, 900, 1700, 1900), Color.White);
            _spriteBatch.Draw(BG5, new Rectangle(-1000, -500, 1900, 2100), Color.White);
            _spriteBatch.Draw(lightTex, new Rectangle(-10000, -10000, 30000, 30000), Color.Pink * 0.2f);

            _spriteBatch.End();


            _spriteBatch.Begin(transformMatrix: camera.Transform);

            for (int i = 0; i < 25; i++)
            {
                _spriteBatch.Draw(woodFloor, new Vector2(-1500 + (290 * i), 780), Color.White);
            }
            for (int i = 0; i < 20; i++)
            {
                _spriteBatch.Draw(railFloor, new Vector2(150 + (270 * i), 770), Color.White);
            }
            _spriteBatch.Draw(railBrokenL, new Vector2(5560, 760), Color.White);
            cart.Draw(_spriteBatch);
            grubby.Draw(_spriteBatch, true);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
