using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FireDX3
{
    /// <summary>
    /// Custom particle system for creating a flame effect.
    /// </summary>
    class FireDXParticles : ParticleSystem
    {
        public FireDXParticles(Game game, ContentManager content) : base(game, content)
        { }
        
        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "fire";

            settings.MaxParticles = 10000;

            settings.Duration = TimeSpan.FromSeconds(5);
            settings.DurationRandomness = 1;

            settings.MinHorizontalVelocity = 2;
            settings.MaxHorizontalVelocity = 6;

            settings.MinVerticalVelocity = -5;
            settings.MaxVerticalVelocity = 10;

            // Set gravity upside down, so the flames will 'fall' upward.
            settings.Gravity = new Vector3(-10, 15, 10);

            settings.MinColor = new Color(255, 255, 255, 10);
            settings.MaxColor = new Color(255, 255, 255, 40);

            settings.MinStartSize = 5;
            settings.MaxStartSize = 10;

            settings.MinEndSize = 10;
            settings.MaxEndSize = 40;

            // Use additive blending.
            settings.BlendState = BlendState.Additive;
        }
    }
}

