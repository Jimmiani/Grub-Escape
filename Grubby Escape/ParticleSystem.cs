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
    public enum EmitterShape
    {
        Point,
        Rectangle
    }
    public class ParticleSystem
    {
        private Random random;
        public Vector2 EmitterLocation { get; set; }
        public Rectangle EmitterBoundary { get; set; }
        private List<Particle> _particles;
        private List<Texture2D> _textures;
        private EmitterShape _emitterShape;
        private float _spawnTimer;
        private float _velocityAngle;


        public Color Color { get; set; } = Color.White;
        public bool ColorChange { get; set; } = false;
        public bool CanSpawn { get; set; } = true;
        public float SpawnRate { get; set; } = 1.0f;
        public int SpawnAmount { get; set; } = 1;
        public float MinYVelocity { get; set; } = -1f;
        public float MaxYVelocity { get; set; } = 1f;
        public float MinXVelocity { get; set; } = -1f;
        public float MaxXVelocity { get; set; } = 1f;
        public bool HighVelocityMode { get; set; } = false;

        public float MinAngularVelocity { get; set; } = -0.02f;
        public float MaxAngularVelocity { get; set; } = 0.02f;

        public float MinTTL { get; set; } = 120f;
        public float MaxTTL { get; set; } = 240f;

        public float MinSize {  get; set; } = 0.5f;
        public float MaxSize { get; set; } = 2f;
        public float MaxOpacity { get; set; } = 1f;
        public bool Fade { get; set; } = true;
        public bool FadeIn { get; set; } = true;
        public bool FadeOut {  get; set; } = true;
        public bool Shrink { get; set; } = false;
        public float TimeUntilShrink { get; set; } = 1;
        public bool Grow {  get; set; } = false;
        public float GrowTime { get; set; } = 1;

        // ---------- Defaults -----------

        private Color _defaultColor = Color.White;
        private bool _defaultCanSpawn = true;
        private float _defaultSpawnRate = 1.0f;
        private int _defaultSpawnAmount = 1;
        private float _defaultMinYVelocity = -1f;
        private float _defaultMaxYVelocity = 1f;
        private float _defaultMinXVelocity = -1f;
        private float _defaultMaxXVelocity = 1f;
        private bool _defaultHighVelocityMode = false;

        private float _defaultMinAngularVelocity = -0.02f;
        private float _defaultMaxAngularVelocity = 0.02f;

        private float _defaultMinTTL = 120f;
        private float _defaultMaxTTL = 240f;

        private float _defaultMinSize = 0.5f;
        private float _defaultMaxSize = 2f;
        private float _defaultMaxOpacity = 1f;
        private bool _defaultFade = true;
        private bool _defaultFadeIn = true;
        private bool _defaultFadeOut = true;

        private bool _defaultShrink = false;
        private float _defaultShrinkTime = 1;
        private bool _defaultGrow = false;
        private float _defaultGrowTime = 1;

        public ParticleSystem(List<Texture2D> textures, Vector2 location, EmitterShape shape)
        {
            EmitterLocation = location;
            _textures = textures;
            _particles = new List<Particle>();
            random = new Random();
            _emitterShape = shape;
            _spawnTimer = 0;
            _velocityAngle = MathHelper.TwoPi;
        }

        public ParticleSystem(List<Texture2D> textures, Rectangle boundary, EmitterShape shape)
        {
            EmitterBoundary = boundary;
            _textures = textures;
            _particles = new List<Particle>();
            random = new Random();
            _emitterShape = shape;
            _spawnTimer = 0;
            _velocityAngle = MathHelper.TwoPi;
        }

        public void Update(GameTime gameTime)
        {
            if (CanSpawn)
            {
                _spawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_spawnTimer >= SpawnRate)
                {
                    _spawnTimer -= SpawnRate;
                    for (int i = 0; i < SpawnAmount; i++)
                    {
                        _particles.Add(GenerateNewParticle());
                    }
                }
            }
            

            for (int i = 0; i < _particles.Count; i++)
            {
                _particles[i].Update(Fade, FadeIn, FadeOut);
                if (_particles[i].Color != Color && ColorChange)
                {
                    _particles[i].Color = Color;
                }
                if (Grow && GrowTime > (_particles[i].InitialTTL / 60) - (_particles[i].TTL / 60))
                {
                    float t = ((_particles[i].InitialTTL / 60) - (_particles[i].TTL / 60)) / GrowTime;
                    _particles[i].Size = MathHelper.Lerp(0, _particles[i].InitialSize, t);
                }
                if ((_particles[i].TTL / 60) < TimeUntilShrink && Shrink)
                {
                    float t = 1 - (_particles[i].TTL / 60 / TimeUntilShrink);
                    _particles[i].Size = MathHelper.Lerp(_particles[i].Size, 0, t);
                }
                if (_particles[i].TTL <= 0)
                {
                    _particles.RemoveAt(i);
                    i--;
                }
            }
        }

        private Particle GenerateNewParticle()
        {
            Texture2D texture = _textures[random.Next(_textures.Count)];
            Vector2 position = EmitterLocation;

            if (_emitterShape == EmitterShape.Rectangle)
            {
                position = new Vector2(RandomRange(EmitterBoundary.Left, EmitterBoundary.Right), RandomRange(EmitterBoundary.Top, EmitterBoundary.Bottom));
            }


            float vx, vy; // Velocity Components
            
            vx = RandomRange(MinXVelocity, MaxXVelocity);
            vy = RandomRange(MinYVelocity, MaxYVelocity);
            
            _velocityAngle = (float)random.NextDouble() * MathHelper.TwoPi;
            Vector2 velocity = new Vector2(vx, vy);
            if (HighVelocityMode)
            {
                velocity = new Vector2(vx * (float)Math.Cos(_velocityAngle), vy * (float)(Math.Sin(_velocityAngle)));
            }
            float angle = 0;
            float angularVelocity = RandomRange(MinAngularVelocity, MaxAngularVelocity);
            Color color = Color;
            float size = RandomRange(MinSize, MaxSize);
            float ttl = RandomRange(MinTTL, MaxTTL);
            float opacity = MaxOpacity;

            return new Particle(texture, position, velocity, angle, angularVelocity, color, size, ttl, opacity);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int index = 0; index < _particles.Count; index++)
            {
                _particles[index].Draw(spriteBatch);
            }
        }
        private float RandomRange(float min, float max)
        {
            return (float)(random.NextDouble() * (max - min) + min);
        }
        public void SetVelocity(float xMin, float xMax, float yMin, float yMax)
        {
            MinXVelocity = xMin;
            MinYVelocity = yMin;
            MaxXVelocity = xMax;
            MaxYVelocity = yMax;
        }
        public void SetLifespan(float min, float max)
        {
            MaxTTL = max * 60;
            MinTTL = min * 60;
        }
        public void SetSize(float min, float max)
        {
            MaxSize = max;
            MinSize = min;
        }
        public void SetAngularVelocity(float min, float max)
        {
            MaxAngularVelocity = MathHelper.ToRadians(max / 60);
            MinAngularVelocity = MathHelper.ToRadians(min / 60);
        }
        public void SetSpawnInfo(float rate, int amount)
        {
            SpawnRate = rate;
            SpawnAmount = amount;
        }
        public void SetShrinkInfo(bool shrink, float timeUntilShrink)
        {
            Shrink = shrink;
            TimeUntilShrink = timeUntilShrink;
        }
        public void SetOpacityInfo(float maxOpacity, bool fade, bool fadeIn, bool fadeOut)
        {
            MaxOpacity = maxOpacity;
            Fade = fade;
            FadeIn = fadeIn;
            FadeOut = fadeOut;
        }
        public void SetDefaults(Color color, bool canSpawn, float spawnRate, int spawnAmount, float minXVel, float maxXVel, float minYVel, float maxYVel, bool highVelocityMode, float minAngVel, float maxAngVel, float minTTL, float maxTTL, float minSize, float maxSize, float maxOpacity, bool fade, bool fadeIn, bool fadeOut, bool shrink, float timeUntilShrink, bool grow, float growTime)
        {
            _defaultColor = color;
            _defaultCanSpawn = canSpawn;
            _defaultSpawnRate = spawnRate;
            _defaultSpawnAmount = spawnAmount;
            _defaultMinYVelocity = minYVel;
            _defaultMaxYVelocity = maxYVel;
            _defaultMinXVelocity = minXVel;
            _defaultMaxXVelocity = maxXVel;
            _defaultHighVelocityMode = highVelocityMode;

            _defaultMinAngularVelocity = MathHelper.ToRadians(minAngVel / 60);
            _defaultMaxAngularVelocity = MathHelper.ToRadians(maxAngVel / 60);

            _defaultMinTTL = minTTL * 60;
            _defaultMaxTTL = maxTTL * 60;

            _defaultMinSize = minSize;
            _defaultMaxSize = maxSize;

            _defaultMaxOpacity = maxOpacity;
            _defaultFade = fade;
            _defaultFadeIn = fadeIn;
            _defaultFadeOut = fadeOut;

            _defaultShrink = shrink;
            _defaultShrinkTime = timeUntilShrink;
            _defaultGrow = grow;
            _defaultGrowTime = growTime;
        }
        public void RestoreDefaults()
        {
            Color = _defaultColor;
            CanSpawn = _defaultCanSpawn;
            SpawnRate = _defaultSpawnRate;
            SpawnAmount = _defaultSpawnAmount;
            MinYVelocity = _defaultMinYVelocity;
            MaxYVelocity = _defaultMaxYVelocity;
            MinXVelocity = _defaultMinXVelocity;
            MaxXVelocity = _defaultMaxXVelocity;
            HighVelocityMode = _defaultHighVelocityMode;

            MinAngularVelocity = _defaultMinAngularVelocity;
            MaxAngularVelocity = _defaultMaxAngularVelocity;

            MinTTL = _defaultMinTTL;
            MaxTTL = _defaultMaxTTL;

            MinSize = _defaultMinSize;
            MaxSize = _defaultMaxSize;

            MaxOpacity = _defaultMaxOpacity;
            Fade = _defaultFade;
            FadeIn = _defaultFadeIn;
            FadeOut = _defaultFadeOut;

            Shrink = _defaultShrink;
            TimeUntilShrink = _defaultShrinkTime;
            Grow = _defaultGrow;
            GrowTime = _defaultGrowTime;
        }
    }
}
