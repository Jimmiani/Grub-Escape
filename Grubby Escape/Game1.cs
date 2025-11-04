using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Grubby_Escape
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        MouseState mouseState, prevMouseState;

        Grub grubby;

        List<Texture2D> grubIdle;
        List<Texture2D> grubAlert;
        List<Texture2D> grubJump;
        List<Texture2D> grubBounce;
        List<Texture2D> grubWave;

        List<SoundEffect> idleEffect;
        List<SoundEffect> alertEffect;
        List<SoundEffect> jumpEffect;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            grubIdle = new List<Texture2D>();
            grubAlert = new List<Texture2D>();
            grubJump = new List<Texture2D>();
            grubBounce = new List<Texture2D>();
            grubWave = new List<Texture2D>();

            idleEffect = new List<SoundEffect>();
            alertEffect = new List<SoundEffect>();
            jumpEffect = new List<SoundEffect>();

            base.Initialize();
            
            grubby = new Grub(grubIdle, idleEffect, grubAlert, alertEffect, grubJump, jumpEffect);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

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

            // --------------------------- Music -------------------------------

            for (int i = 1; i <= 3; i++)
                alertEffect.Add(Content.Load<SoundEffect>("Grubby Escape/Audio/Sound Effects/Grubs/Alert/grub_alert_" + i));
            for (int i = 1; i <= 2; i++)
                jumpEffect.Add(Content.Load<SoundEffect>("Grubby Escape/Audio/Sound Effects/Grubs/Freed/grub_free_" + i));
            for (int i = 1; i <= 3; i++)
                idleEffect.Add(Content.Load<SoundEffect>("Grubby Escape/Audio/Sound Effects/Grubs/Sad/grub_sad_" + i));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            prevMouseState = mouseState;
            mouseState = Mouse.GetState();

            grubby.Update(mouseState, prevMouseState, gameTime);

            if (mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
                grubby.Happy();

            if (mouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton == ButtonState.Released)
                grubby.Jump();


            grubby.Position = mouseState.Position.ToVector2();


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            grubby.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
