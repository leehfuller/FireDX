using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FireDX2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class FireDX2 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont font1;
        Vector2 fontPos;
        Texture2D tLine;
        Texture2D tFire;

        Texture2D spriteFire1;
        Texture2D spriteFire2;
        Texture2D spriteSmoke1;
        Texture2D spriteSmoke2;

        double fps = 0.0;
        DrawFire drawFire = null;

        public FireDX2()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;

            //graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font1 = Content.Load<SpriteFont>("default");

            tLine = new Texture2D(GraphicsDevice, 1, 1);
            tLine.SetData<Color>(new Color[] { Color.White });

            tFire = new Texture2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            drawFire = new DrawFire(tFire);

            //fontPos = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2);
            fontPos = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, 10);

            spriteFire1 = Content.Load<Texture2D>("fire1");
            spriteFire2 = Content.Load<Texture2D>("fire2");
            spriteSmoke1 = Content.Load<Texture2D>("smoke1");
            spriteSmoke2 = Content.Load<Texture2D>("smoke2");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape) || Mouse.GetState().LeftButton == ButtonState.Pressed)
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end, Color lineColor)
        {
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            sb.Draw(tLine, new Rectangle((int)start.X,(int)start.Y,(int)edge.Length(),1),null, lineColor, angle,new Vector2(0, 0),SpriteEffects.None,0);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (gameTime.ElapsedGameTime.TotalSeconds > 0)
            {
                fps = (1 / gameTime.ElapsedGameTime.TotalSeconds);
            }

            //spriteBatch.Begin();
            spriteBatch.Begin(SpriteSortMode.Deferred);

            //spriteBatch.Draw(spriteFire1, new Vector2(0, tFire.Height-spriteFire1.Height-1), Color.White);

            drawFire.updateFire();

            Vector2 s = new Vector2(0, 0);
            spriteBatch.Draw(tFire, s, Color.White);

            string outputText = "FPS: " + Math.Round(fps, 1) + " ("+tFire.Width+","+tFire.Height+")";
            Vector2 fontOrigin = font1.MeasureString(outputText) / 2;

            spriteBatch.DrawString(font1, outputText, fontPos, Color.White, 0, fontOrigin, 1.0f, SpriteEffects.None, 0.5f);

            //DrawLine(spriteBatch, new Vector2(200, 200 + randomNumber), new Vector2(100, 50), Color.Black);
            //DrawLine(spriteBatch, new Vector2(200 + randomNumber, 200), new Vector2(100, 50), Color.White);
            //DrawLine(spriteBatch, new Vector2(200, 200), new Vector2(100, 50 + randomNumber), Color.Blue);
            //DrawLine(spriteBatch, new Vector2(200, 200), new Vector2(100 + randomNumber, 50), Color.Purple);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
