//-----------------------------------------------------------------------------
// FireDX3Particles - yet another random fire app
// Based on XNA Example and Particle code
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FireDX3
{
    /// <summary>
    /// Sample showing how to implement a particle system entirely
    /// on the GPU, using the vertex shader to animate particles.
    /// </summary>
    public class FireDX3Particles : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        Model grid;

        ParticleSystem smokePlumeParticles;
        ParticleSystem fireDX;

        // The sample can switch between three different visual effects.
        enum ParticleState
        {
            FireDX,
            FireDXSmoke
        };

        ParticleState currentState = ParticleState.FireDXSmoke;

        // Random number generator for the fire effect.
        Random random = new Random();

        // Input state.
        KeyboardState currentKeyboardState;
        GamePadState currentGamePadState;

        KeyboardState lastKeyboardState;
        GamePadState lastGamePadState;

        // Camera state.
        float cameraArc = -5;
        float cameraRotation = 0;
        float cameraDistance = 200;

        /// <summary>
        /// Constructor.
        /// </summary>
        public FireDX3Particles()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.GraphicsProfile = GraphicsProfile.HiDef;

            //graphics.PreferredBackBufferWidth = 800;
            //graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.IsFullScreen = true;
 
            //graphics.ApplyChanges();

            // Construct our particle system components.
            smokePlumeParticles = new SmokePlumeParticleSystem(this, Content);
            fireDX = new FireDXParticles(this, Content);

            smokePlumeParticles.DrawOrder = 100;
            fireDX.DrawOrder = 200;

            // Register the particle system components.
            Components.Add(smokePlumeParticles);
            Components.Add(fireDX);
        }

        /// <summary>
        /// Load your graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            font = Content.Load<SpriteFont>("font");
            grid = Content.Load<Model>("grid");
        }

        /// <summary>
        /// Allows the game to run logic.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            cameraRotation+=0.01f;

            HandleInput();

            UpdateCamera(gameTime);

            switch (currentState)
            {
                case ParticleState.FireDX:
                    UpdateFireDX(gameTime);
                    break;

                case ParticleState.FireDXSmoke:
                    UpdateFireDX(gameTime);
                    break;
            }

            base.Update(gameTime);
        }

        void UpdateFireDX(GameTime gameTime)
        {
            int fireParticlesPerFrame = 30;
           
            for (int i = 0; i < fireParticlesPerFrame*2; i+=2)
            {
                fireDX.AddParticle(new Vector3(i-20, 0, 0), Vector3.Zero);
            }

            if (currentState == ParticleState.FireDXSmoke)
            {
                smokePlumeParticles.AddParticle(Vector3.Zero, Vector3.Zero);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice device = graphics.GraphicsDevice;
            device.Clear(Color.Black);

            // Compute camera matrices.
            float aspectRatio = (float)device.Viewport.Width /(float)device.Viewport.Height;

            Matrix view = Matrix.CreateTranslation(0, -25, 0) *
                          Matrix.CreateRotationY(MathHelper.ToRadians(cameraRotation)) *
                          Matrix.CreateRotationX(MathHelper.ToRadians(cameraArc)) *
                          Matrix.CreateLookAt(new Vector3(0, 0, -cameraDistance),
                                              new Vector3(0, 0, 0), Vector3.Up);

            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);

            // Pass camera matrices through to the particle system components.
            smokePlumeParticles.SetCamera(view, projection);
            fireDX.SetCamera(view, projection);

            // Draw our background grid and message text.
            DrawGrid(view, projection);
            DrawMessage();

            // This will draw the particle system components.
            base.Draw(gameTime);
        }

        /// <summary>
        /// Helper for drawing the background grid model.
        /// </summary>
        void DrawGrid(Matrix view, Matrix projection)
        {
            GraphicsDevice device = graphics.GraphicsDevice;

            device.BlendState = BlendState.Opaque;
            device.DepthStencilState = DepthStencilState.Default;
            device.SamplerStates[0] = SamplerState.LinearWrap;

            grid.Draw(Matrix.Identity, view, projection);
        }

        /// <summary>
        /// Helper for drawing our message text.
        /// </summary>
        void DrawMessage()
        {
            string message = string.Format("Particles: {0} - Arrows/WASD, Z/X, F, Esc", fireDX.countParticles());
            
            spriteBatch.Begin();
            spriteBatch.DrawString(font, message, new Vector2(50, 50), Color.White);
            spriteBatch.End();
        }

        /// <summary>
        /// Handles input for quitting the game and cycling
        /// through the different particle effects.
        /// </summary>
        void HandleInput()
        {
            lastKeyboardState = currentKeyboardState;
            lastGamePadState = currentGamePadState;

            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);

            // Check for exit.
            if (currentKeyboardState.IsKeyDown(Keys.Escape) || currentGamePadState.Buttons.Back == ButtonState.Pressed)
                Exit();

            // Check for changing the active particle effect.
            if (((currentKeyboardState.IsKeyDown(Keys.Space) && (lastKeyboardState.IsKeyUp(Keys.Space))) ||
                ((currentGamePadState.Buttons.A == ButtonState.Pressed)) && (lastGamePadState.Buttons.A == ButtonState.Released)))
            {
                currentState++;
                if (currentState > ParticleState.FireDXSmoke) currentState = 0;
            }
        }

        /// <summary>
        /// Handles input for moving the camera.
        /// </summary>
        void UpdateCamera(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (currentKeyboardState.IsKeyDown(Keys.F))
            {
                graphics.IsFullScreen = (graphics.IsFullScreen == true) ? false : true;

                if (graphics.IsFullScreen == false)
                {
                    graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2;
                    graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2;
                }               

                graphics.ApplyChanges();
            }

            // Check for input to rotate the camera up and down around the model.
            if (currentKeyboardState.IsKeyDown(Keys.Up) || currentKeyboardState.IsKeyDown(Keys.W))
                cameraArc += time * 0.025f;

            if (currentKeyboardState.IsKeyDown(Keys.Down) || currentKeyboardState.IsKeyDown(Keys.S))
                cameraArc -= time * 0.025f;

            cameraArc += currentGamePadState.ThumbSticks.Right.Y * time * 0.05f;

            // Limit the arc movement.
            if (cameraArc > 90.0f)          cameraArc = 90.0f;
            else if (cameraArc < -90.0f)    cameraArc = -90.0f;

            // Check for input to rotate the camera around the model.
            if (currentKeyboardState.IsKeyDown(Keys.Right) || currentKeyboardState.IsKeyDown(Keys.D))
                cameraRotation += time * 0.05f;

            if (currentKeyboardState.IsKeyDown(Keys.Left) || currentKeyboardState.IsKeyDown(Keys.A))
                cameraRotation -= time * 0.05f;

            cameraRotation += currentGamePadState.ThumbSticks.Right.X * time * 0.1f;

            // Check for input to zoom camera in and out.
            if (currentKeyboardState.IsKeyDown(Keys.Z))     cameraDistance += time * 0.25f;

            if (currentKeyboardState.IsKeyDown(Keys.X))     cameraDistance -= time * 0.25f;

            cameraDistance += currentGamePadState.Triggers.Left * time * 0.5f;
            cameraDistance -= currentGamePadState.Triggers.Right * time * 0.5f;

            // Limit the camera distance.
            if (cameraDistance > 500)       cameraDistance = 500;
            else if (cameraDistance < 10)   cameraDistance = 10;

            if (currentGamePadState.Buttons.RightStick == ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.R))
            {
                cameraArc = -5;
                cameraRotation = 0;
                cameraDistance = 200;
            }
        }
    }

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static class Program
    {
        static void Main()
        {
            using (FireDX3Particles game = new FireDX3Particles())
            {
                game.Run();
            }
        }
    }
}
