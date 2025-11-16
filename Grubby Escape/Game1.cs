using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Grubby_Escape
{
    enum GameState
    {
        Waiting,
        CartFall,
        Start,
        TowardsBrokenRail,
        Reverse,
        Prepare,
        TransitionOut,
        Math,
        TransitionIn
    }

    enum MathState
    {
        TransitionIn,
        PictorialRepresentation
    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        MouseState mouseState, prevMouseState;
        KeyboardState keyboardState, prevKeyboardState;
        GameState gameState;
        MathState mathState;
        Camera2D camera;
        Vector2 cameraTarget;
        Random generator;
        ResolutionScaler resolutionScaler;

        // Transition

        float transitionTimer;
        Color transitionColor;
        SoundEffect transitionSfx;

        // Particles

        ParticleSystem smokeSystem;
        List<Texture2D> smokeTextures;

        ParticleSystem crystalSystemFG, crystalSystemMG, crystalSystemBG;
        List<Texture2D> crystalTextures;

        ParticleSystem dustSystem;
        List<Texture2D> dustTextures;

        // Backgrounds

        Texture2D BG1, BG2, BG3, BG4, BG5;
        Texture2D lightTex, vignette, pixel, lightEffect;
        Rectangle vignetteRect;
        Texture2D bankedCurve;
        List<Texture2D> backgrounds;

        // Foreground

        Texture2D crystalFG1, crystalFG2, blackFader;
        List<Texture2D> rocksFG;
        List<Vector2> rockPositions12, rockPositions14;

        // Music

        SoundEffect mainMusicSfx, bassMusicSfx, actionMusicSfx, machineryAtmosSfx, crystalAtmosSfx;
        SoundEffectInstance mainMusic, bassMusic, actionMusic, machineryAtmos, crystalAtmos;

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
        SoundEffect startSfx, movingSfx, stopSfx, landSfx, fallSfx;
        float cartStartTimer, cartStopTimer;
        bool hasStarted, hasStopped, hasFallen;

        // Floors

        Texture2D woodFloor, railFloor, railBrokenL, railBrokenR, crystalFloor1, crystalFloor2;
        List<Texture2D> bankedTextures;

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

            resolutionScaler = new ResolutionScaler(GraphicsDevice, 1920, 1080);

            gameState = GameState.Waiting;
            mathState = MathState.TransitionIn;
            generator = new Random();

            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            smokeTextures = new List<Texture2D>();
            crystalTextures = new List<Texture2D>();
            dustTextures = new List<Texture2D>();

            rocksFG = new List<Texture2D>();
            rockPositions12 = new List<Vector2>();
            rockPositions14 = new List<Vector2>();

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
            hasFallen = false;
            cartStartTimer = 0;
            cartStopTimer = 0;


            transitionTimer = 0;
            transitionColor = Color.Black * 0;

            bankedTextures = new List<Texture2D>();
            backgrounds = new List<Texture2D>();

            base.Initialize();

            grubby = new Grub(grubIdle, idleEffect, sadEffect, grubAlert, alertEffect, grubJump, jumpEffect);
            cart = new Cart(cartTexture, wheelTexture, new Vector2(300, -200), startSfx, movingSfx, stopSfx, landSfx, fallSfx);

            cameraTarget = new Vector2(600, 540);

            grubby.Position = new Vector2(0, 630);

            camera = new Camera2D(GraphicsDevice.Viewport, new Rectangle(-359, 0, 10000, 1080), cameraTarget, 1920, 1080);

            mainMusic.Volume = 0.5f;
            mainMusic.IsLooped = true;
            mainMusic.Play();

            machineryAtmos.Volume = 0.5f;
            machineryAtmos.IsLooped = true;
            machineryAtmos.Play();

            crystalAtmos.Volume = 1;
            crystalAtmos.IsLooped = true;
            crystalAtmos.Play();

            // Particles

            smokeSystem = new ParticleSystem(smokeTextures, new Rectangle(5750, 1150, 1000, 300), EmitterShape.Rectangle);

            smokeSystem.SetDefaults(
                Color.White,
                true,
                0.07f,
                1,
                0,
                0,
                -0.5f,
                -1,
                false,
                -15,
                15,
                6,
                6,
                3,
                4,
                0.9f,
                true,
                true,
                true,
                false,
                1,
                false,
                1);
            smokeSystem.RestoreDefaults();


            crystalSystemFG = new ParticleSystem(crystalTextures, new Rectangle(-1000, -1000, 20000, 3000), EmitterShape.Rectangle);

            crystalSystemFG.SetDefaults(
                Color.White,
                true,
                0.1f,
                1,
                0,
                0,
                -0.4f,
                -0.5f,
                false,
                -10,
                10,
                8,
                9,
                0.9f,
                1,
                0.5f,
                false,
                false,
                false,
                true,
                2,
                true,
                0.4f);
            crystalSystemFG.RestoreDefaults();


            crystalSystemMG = new ParticleSystem(crystalTextures, new Rectangle(-1000, -500, 10000, 2500), EmitterShape.Rectangle);

            crystalSystemMG.SetDefaults(
                Color.White,
                true,
                0.1f,
                1,
                0,
                0,
                -0.3f,
                -0.4f,
                false,
                -10,
                10,
                8,
                9,
                0.6f,
                0.7f,
                0.5f,
                false,
                false,
                false,
                true,
                2,
                true,
                0.4f);
            crystalSystemMG.RestoreDefaults();


            crystalSystemBG = new ParticleSystem(crystalTextures, new Rectangle(-500, -500, 6000, 2000), EmitterShape.Rectangle);

            crystalSystemBG.SetDefaults(
                Color.White,
                true,
                0.1f,
                1,
                0,
                0,
                -0.2f,
                -0.3f,
                false,
                -10,
                10,
                8,
                9,
                0.4f,
                0.5f,
                0.3f,
                false,
                false,
                false,
                true,
                2,
                true,
                0.4f);
            crystalSystemBG.RestoreDefaults();


            dustSystem = new ParticleSystem(dustTextures, new Rectangle(325, -300, 225, 100), EmitterShape.Rectangle);

            dustSystem.SetDefaults(
                Color.White,
                false,
                0.01f,
                1,
                -0.18f,
                0.18f,
                14,
                15,
                false,
                -360,
                360,
                0.7f,
                1f,
                1.2f,
                1.4f,
                0.15f,
                true,
                false,
                true,
                false,
                1,
                false,
                1);
            dustSystem.RestoreDefaults();

            int size = 50;
            vignetteRect = new Rectangle(vignette.Width * -size / 2, vignette.Height * -size / 2, vignette.Width * size, vignette.Height * size);


            // Rock positions

            for (int j = 0; j < 15; j++)
            {
                for (int i = 0; i < rocksFG.Count; i++)
                {
                    float x = -900 + (1200 * j) + (150 * i);
                    float y = -80;
                    rockPositions14.Add(new Vector2(x, y));
                }
            }

            for (int j = 0; j < 15; j++)
            {
                for (int i = 0; i < rocksFG.Count; i++)
                {
                    float x = -600 + (1200 * j) + (120 * i);
                    float y = -50;
                    rockPositions12.Add(new Vector2(x, y));
                }
            }

            // Backgrounds

            backgrounds.Add(BG1);
            backgrounds.Add(BG2);
            backgrounds.Add(BG3);
            backgrounds.Add(BG4);
            backgrounds.Add(BG5);

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // --------------------------- Images -------------------------------

            // Particles

            for (int i = 1; i <= 5; i++)
                smokeTextures.Add(Content.Load<Texture2D>($"Grubby Escape/Images/Particles/Smoke/abyss_smoke_0{i}"));
            for (int i = 1; i <= 3; i++)
                crystalTextures.Add(Content.Load<Texture2D>($"Grubby Escape/Images/Particles/Crystals/floating_crystals_0{i}"));
            for (int i = 1; i <= 1; i++)
                dustTextures.Add(Content.Load<Texture2D>($"Grubby Escape/Images/Particles/Dust/hot_spring_smoke"));
            vignette = Content.Load<Texture2D>("Grubby Escape/Images/Lights/vignette_large_v01");

            // Crystals

            crystalFG1 = Content.Load<Texture2D>("Grubby Escape/Images/Crystals/mine_crystal_04");
            crystalFG2 = Content.Load<Texture2D>("Grubby Escape/Images/Crystals/mine_crystal_06");
            crystalFloor1 = Content.Load<Texture2D>("Grubby Escape/Images/Crystals/mine_crystal_01");
            crystalFloor2 = Content.Load<Texture2D>("Grubby Escape/Images/Crystals/mine_crystal_08");

            // Foreground

            for (int i = 1; i <= 19; i++)
                rocksFG.Add(Content.Load<Texture2D>($"Grubby Escape/Images/Foreground/Rock_FG ({i})"));
            blackFader = Content.Load<Texture2D>("Grubby Escape/Images/Lights/black_fader_GG");

            // Backgrounds

            BG1 = Content.Load<Texture2D>("Grubby Escape/Images/Background/Extremely Far/BG_bone_01");
            BG2 = Content.Load<Texture2D>("Grubby Escape/Images/Background/Extremely Far/BG_bone_02");
            BG3 = Content.Load<Texture2D>("Grubby Escape/Images/Background/Extremely Far/BG_bone_03");
            BG4 = Content.Load<Texture2D>("Grubby Escape/Images/Background/Extremely Far/BG_egg_05");
            BG5 = Content.Load<Texture2D>("Grubby Escape/Images/Background/Extremely Far/BG_twist");
            lightTex = Content.Load<Texture2D>("Grubby Escape/Images/Lights/white_light");
            lightEffect = Content.Load<Texture2D>("Grubby Escape/Images/Lights/light_effect_v02");
            bankedCurve = Content.Load<Texture2D>("Grubby Escape/Images/Rails/banked_curve");

            // Cart

            wheelTexture = Content.Load<Texture2D>("Grubby Escape/Images/Rails/mines_flip_platform_BG");
            cartTexture = Content.Load<Texture2D>("Grubby Escape/Images/Rails/Mines_Layered_0008_a");
            woodFloor = Content.Load<Texture2D>("Grubby Escape/Images/Floor/mine_floor_01");
            railFloor = Content.Load<Texture2D>("Grubby Escape/Images/Rails/mine_rail_01");
            railBrokenL = Content.Load<Texture2D>("Grubby Escape/Images/Rails/mine_rail_02");
            railBrokenR = Content.Load<Texture2D>("Grubby Escape/Images/Rails/mine_rail_03");

            // Banked Textures

            bankedTextures.Add(Content.Load<Texture2D>("Grubby Escape/Images/Rails/mines_metal_thin_floors_0001_a"));
            bankedTextures.Add(Content.Load<Texture2D>("Grubby Escape/Images/Rails/mines_metal_thin_floors_0002_a"));
            bankedTextures.Add(Content.Load<Texture2D>("Grubby Escape/Images/Rails/mines_metal_thin_floors_0004_a"));

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
            landSfx = Content.Load<SoundEffect>("Grubby Escape/Audio/Sound Effects/Rails/false_knight_land_1st_time");
            fallSfx = Content.Load<SoundEffect>("Grubby Escape/Audio/Sound Effects/Rails/misc_rumble_loop");

            // Music

            mainMusicSfx = Content.Load<SoundEffect>("Grubby Escape/Audio/Music/Songs/S26 Crystal MAIN");
            bassMusicSfx = Content.Load<SoundEffect>("Grubby Escape/Audio/Music/Songs/S26 Crystal BASS");
            actionMusicSfx = Content.Load<SoundEffect>("Grubby Escape/Audio/Music/Songs/S26 Crystal ACTION");
            machineryAtmosSfx = Content.Load<SoundEffect>("Grubby Escape/Audio/Music/Atmos/mines_machinery_atmos");
            crystalAtmosSfx = Content.Load<SoundEffect>("Grubby Escape/Audio/Music/Atmos/mines_crystal_shimmer_loop");

            mainMusic = mainMusicSfx.CreateInstance();
            bassMusic = bassMusicSfx.CreateInstance();
            actionMusic = actionMusicSfx.CreateInstance();
            machineryAtmos = machineryAtmosSfx.CreateInstance();
            crystalAtmos = crystalAtmosSfx.CreateInstance();

            // Transition

            transitionSfx = Content.Load<SoundEffect>("Grubby Escape/Audio/Sound Effects/Start/spa_heal");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            prevMouseState = mouseState;
            mouseState = Mouse.GetState();
            Vector2 mouseRT = resolutionScaler.GetMouseWorldPosition();
            Vector2 mouseWorldPos = Vector2.Transform(mouseRT, Matrix.Invert(camera.Transform));

            prevKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();


            if (gameState != GameState.Math)
            {
                grubby.Update(mouseState, prevMouseState, gameTime);
                cart.Update(gameTime);
                camera.Update(gameTime, cameraTarget, cart.Velocity);
                smokeSystem.Update(gameTime);
                crystalSystemFG.Update(gameTime);
                crystalSystemMG.Update(gameTime);
                crystalSystemBG.Update(gameTime);
                dustSystem.Update(gameTime);
            }
            if (isOnCart)
                grubby.Position = new Vector2(cart.Position.X + 30, cart.Position.Y - 65);

            Debug.WriteLine(mouseWorldPos);
            if (gameState == GameState.Waiting)
            {
                if (keyboardState.IsKeyDown(Keys.Enter) && prevKeyboardState.IsKeyUp(Keys.Enter))
                {
                    dustSystem.CanSpawn = true;
                    cart.Fall(100, 600, 3);
                    camera.Shake(5, 4, false);
                    gameState = GameState.CartFall;
                    grubby.Happy();
                }
            }
            else if (gameState == GameState.CartFall)
            {

                if (cart.Position.Y > 610 && !hasFallen)
                {
                    hasFallen = true;
                    camera.Shake(25, 1, true);
                    dustSystem.SetVelocity(-1, 1, -0.5f, -0.2f);
                    dustSystem.SetSpawnInfo(1.5f, 15);
                    dustSystem.SetLifespan(1.4f, 1.6f);
                    dustSystem.SetAngularVelocity(-30, 30);
                    dustSystem.EmitterBoundary = new Rectangle(315, 780, 230, 20);
                }
                if (hasFallen)
                {
                    cartStartTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (cartStartTimer > 1.5f)
                    {
                        grubby.Jump();
                        gameState = GameState.Start;
                        cartStartTimer = 0;
                        dustSystem.CanSpawn = false;
                    }
                }
            }
            else if (gameState == GameState.Start)
            {
                cameraTarget = new Vector2(cart.Hitbox.Center.X, 0);

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
                cameraTarget = new Vector2(cart.Hitbox.Center.X + 500, 0);
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
                        hasStarted = false;
                    }
                }
            }

            else if (gameState == GameState.Reverse)
            {
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    cameraTarget = new Vector2(cart.Hitbox.Center.X + 1000, 0);
                }
                else
                {
                    cameraTarget = new Vector2(cart.Hitbox.Center.X + 500, 0);
                }

                if (keyboardState.IsKeyDown(Keys.Enter) && prevKeyboardState.IsKeyUp(Keys.Enter) && hasStarted == false)
                {
                    cart.Start(-4);
                    grubby.Happy();
                    hasStarted = true;
                }

                if (cart.Hitbox.X < 2800)
                {
                    cart.Stop();
                    gameState = GameState.Prepare;
                    grubby.Jump();
                    hasStarted = false;
                }
            }
            else if (gameState == GameState.Prepare)
            {
                cameraTarget = new Vector2(cart.Hitbox.Center.X + 500, 0);

                if (keyboardState.IsKeyDown(Keys.Enter) && prevKeyboardState.IsKeyUp(Keys.Enter))
                {
                    transitionSfx.Play();
                    gameState = GameState.TransitionOut;
                }
            }
            else if (gameState == GameState.TransitionOut)
            {
                transitionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                transitionColor = Color.Black * (transitionTimer / 2);

                if (mainMusic.Volume > 0)
                {
                    float newVolume = 0.5f - (transitionTimer / 2f);
                    mainMusic.Volume = Math.Clamp(newVolume, 0f, 1f);
                }
                if (machineryAtmos.Volume > 0)
                {
                    float newVolume = 0.5f - (transitionTimer / 2f);
                    machineryAtmos.Volume = Math.Clamp(newVolume, 0f, 1f);
                }
                if (crystalAtmos.Volume > 0)
                {
                    float newVolume = 1f - (transitionTimer / 2f);
                    crystalAtmos.Volume = Math.Clamp(newVolume, 0f, 1f);
                }

                if (transitionTimer > 5)
                {
                    gameState = GameState.Math;
                    transitionTimer = 0;
                }
            }
            else if (gameState == GameState.Math)
            {
                if (mathState == MathState.TransitionIn)
                {
                    transitionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    transitionColor = Color.Black * (1 - transitionTimer / 2);

                    if (mainMusic.Volume < 1)
                    {
                        float newVolume = transitionTimer / 2f;
                        mainMusic.Volume = Math.Clamp(newVolume, 0f, 1f);
                    }
                    if (crystalAtmos.Volume < 1)
                    {
                        float newVolume = transitionTimer / 2f;
                        crystalAtmos.Volume = Math.Clamp(newVolume, 0f, 1f);
                    }

                    if (keyboardState.IsKeyDown(Keys.Enter) && prevKeyboardState.IsKeyUp(Keys.Enter))
                    {
                        mathState = MathState.PictorialRepresentation;
                    }
                }
            }

                base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            resolutionScaler.DrawToCanvas();

            if (gameState != GameState.Math)
            {
                GraphicsDevice.Clear(Color.Black);

                // Background

                _spriteBatch.Begin(transformMatrix: camera.GetParallaxTransform(0.5f));

                //_spriteBatch.Draw(BG4, new Rectangle(200, -300, 1400, 1600), null, Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                //_spriteBatch.Draw(BG1, new Rectangle(-300, -500, 1000, 1500), Color.White);
                //_spriteBatch.Draw(BG2, new Rectangle(1500, 600, 1000, 1100), Color.White);
                //_spriteBatch.Draw(BG3, new Rectangle(-500, 400, 1500, 1800), Color.White);
                //_spriteBatch.Draw(BG5, new Rectangle(1400, -300, 1000, 1200), Color.White);
                //_spriteBatch.Draw(BG3, new Rectangle(2000, -100, 1500, 1800), Color.White);
                //_spriteBatch.Draw(BG5, new Rectangle(2400, 500, 1000, 1200), Color.White);


                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < backgrounds.Count; j++)
                    {
                        _spriteBatch.Draw(backgrounds[j], new Vector2(-200 * i + 40 * j, -200 * i + 40 * j), null, Color.White, 0, Vector2.Zero, 5, SpriteEffects.None, 1);
                    }
                }


                _spriteBatch.Draw(lightEffect, new Rectangle(-10000, -2500, 20000, 5000), Color.Pink * 0.45f);
                _spriteBatch.Draw(pixel, new Rectangle(-1000, -400, 10000, 450), Color.Black);
                _spriteBatch.Draw(blackFader, new Rectangle(-10000, -200, 30000, 420), Color.White);

                _spriteBatch.End();

                // Background particles

                _spriteBatch.Begin(transformMatrix: camera.GetParallaxTransform(0.6f));

                crystalSystemBG.Draw(_spriteBatch);

                _spriteBatch.End();

                // Mid ground

                _spriteBatch.Begin(transformMatrix: camera.Transform);

                crystalSystemMG.Draw(_spriteBatch);

                _spriteBatch.Draw(lightTex, new Rectangle(grubby.Hitbox.Center.X - 400, grubby.Hitbox.Center.Y - 400, 800, 800), Color.White * 0.2f);

                dustSystem.Draw(_spriteBatch);

                // Left side
                for (int i = 0; i < 4; i++)
                {
                    _spriteBatch.Draw(woodFloor, new Vector2(-359, 0 + (290 * i)), null, Color.White, MathHelper.ToRadians(90), new Vector2(woodFloor.Width / 2, woodFloor.Height / 2), 1, SpriteEffects.None, 0);
                }
                _spriteBatch.Draw(pixel, new Rectangle(-1000, 790, 6756, 1000), Color.Black);

                for (int i = 0; i < 25; i++)
                {
                    _spriteBatch.Draw(woodFloor, new Vector2(-1500 + (290 * i), 780), Color.White);
                }
                for (int i = 0; i < 20; i++)
                {
                    _spriteBatch.Draw(railFloor, new Vector2(150 + (270 * i), 770), Color.White);
                }
                for (int i = 0; i < 4; i++)
                {
                    _spriteBatch.Draw(woodFloor, new Vector2(5750, 940 + (290 * i)), null, Color.White, MathHelper.ToRadians(90), new Vector2(woodFloor.Width / 2, woodFloor.Height / 2), 1, SpriteEffects.None, 0);
                }
                _spriteBatch.Draw(railBrokenL, new Vector2(5560, 760), Color.White);

                _spriteBatch.Draw(crystalFG1, new Vector2(-440, 580), null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipVertically, 0);

                // Right side

                _spriteBatch.Draw(pixel, new Rectangle(6750, 790, 10000, 1000), Color.Black);
                for (int i = 0; i < 25; i++)
                {
                    _spriteBatch.Draw(woodFloor, new Vector2(6750 + (290 * i), 780), Color.White);
                }
                for (int i = 0; i < 15; i++)
                {
                    _spriteBatch.Draw(railFloor, new Vector2(6750 + (270 * i), 770), Color.White);
                }
                for (int i = 0; i < 4; i++)
                {
                    _spriteBatch.Draw(woodFloor, new Vector2(6750, 940 + (290 * i)), null, Color.White, MathHelper.ToRadians(270), new Vector2(woodFloor.Width / 2, woodFloor.Height / 2), 1, SpriteEffects.None, 0);
                }
                _spriteBatch.Draw(railBrokenR, new Vector2(6500, 770), Color.White);
                cart.Draw(_spriteBatch);
                grubby.Draw(_spriteBatch, true);

                // Middle

                _spriteBatch.Draw(blackFader, new Rectangle(3500, 900, 6000, 900), Color.White);
                smokeSystem.Draw(_spriteBatch);

                // Banked Curve

                for (int i = 0; i < 30; i++)
                {
                    for (int j = 0; j < bankedTextures.Count; j++)
                    {
                        _spriteBatch.Draw(bankedTextures[j], new Vector2((3000 + (60 * i)) + (20 * j), (910 + (18 * i)) + (6 * j)), null, Color.White, MathHelper.ToRadians(135), new Vector2(bankedTextures[j].Width / 2, bankedTextures[j].Height / 2), 1, SpriteEffects.FlipHorizontally, 1);
                    }
                }

                _spriteBatch.End();

                // Foregorund

                _spriteBatch.Begin(transformMatrix: camera.GetParallaxTransform(1.2f));

                int rockIndex = 0;

                for (int j = 0; j < 15; j++)
                {
                    for (int i = 0; i < rocksFG.Count; i++)
                    {
                        _spriteBatch.Draw(rocksFG[i], rockPositions12[rockIndex], Color.White);
                        rockIndex++;
                    }
                }

                _spriteBatch.End();

                _spriteBatch.Begin(transformMatrix: camera.GetParallaxTransform(1.4f));

                rockIndex = 0;

                for (int j = 0; j < 15; j++)
                {
                    for (int i = 0; i < rocksFG.Count; i++)
                    {
                        _spriteBatch.Draw(rocksFG[i], rockPositions14[rockIndex], Color.White);
                        rockIndex++;
                    }
                }

                _spriteBatch.End();

                // Crystal system FG

                _spriteBatch.Begin(transformMatrix: camera.GetParallaxTransform(2));

                crystalSystemFG.Draw(_spriteBatch);

                _spriteBatch.End();

                // Vignette

                _spriteBatch.Begin(transformMatrix: camera.Transform);

                _spriteBatch.Draw(vignette, new Rectangle(vignetteRect.X + grubby.Hitbox.Center.X, vignetteRect.Y + grubby.Hitbox.Center.Y, vignetteRect.Width, vignetteRect.Height), Color.White);

                _spriteBatch.End();
            }

            else if (gameState == GameState.Math)
            {
                GraphicsDevice.Clear(Color.Gray);
            }


            // Transitions

            _spriteBatch.Begin();

            _spriteBatch.Draw(pixel, new Rectangle(-500, -500, 4000, 3000), transitionColor);

            _spriteBatch.End();

            resolutionScaler.DrawToScreen(_spriteBatch);

            base.Draw(gameTime);
        }
    }
}
