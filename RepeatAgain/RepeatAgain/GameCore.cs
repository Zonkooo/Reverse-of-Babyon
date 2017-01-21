using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace RepeatAgain
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameCore : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        public MenuManager menu;
        public LevelManager levelMgr;
        public Ground Ground;
        public Ground Ceiling;
        public Character charac;
        public SoundManager soundMgr;
        public Transitioner Transitioner;
        private TimeCounter _timer;
        

        public Texture2D OneWhitePixel;

        public const double Angle = -0.0872; //5 degrees in radians
        public readonly Vector2 XVector = new Vector2((float)Math.Cos(Angle), (float)Math.Sin(Angle));
        public readonly Vector2 YVector = new Vector2((float)Math.Cos(Angle - Math.PI / 2), (float)Math.Sin(Angle - Math.PI / 2));

        private bool _load;
        public bool _menu;
        public bool endGame;
        public int timerRetry;

        private List<GameComponent> listComponent;
        private List<DrawableGameComponent> listDrawable;

        /// <summary>
        /// 
        /// </summary>
        public GameCore()
        {
            graphics = new GraphicsDeviceManager(this)
                           {
                               PreferredBackBufferHeight = 720, 
                               PreferredBackBufferWidth = 1280,
                           };
            IsMouseVisible = true;

            Content.RootDirectory = "Content";
            _menu = true;
            endGame = false;

            listComponent = new List<GameComponent>();
            listDrawable = new List<DrawableGameComponent>();
            timerRetry = 0;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //new LevelDisplay(this);
            _timer = new TimeCounter(this);
            this.Transitioner = new Transitioner(this);

            Ground = new Ground(this, new Vector2(90, 690), new Vector2(0, 0), 28)
                         {HitBox = {Width = 40*28, Height = 40,}};
            Ceiling = new Ground(this, Ground.Position + new Vector2(0, -240 - 40), new Vector2(0, 0), 29)
                          {HitBox = {Width = 40*28, Height = 40,},
                          DrawOrder = 210};

            var levelBg = new BackGround(this, "TowerElements/tower_bg", Ceiling.Position + new Vector2(0, 40), 15, 1,true);
            levelBg.DrawOrder = 10;
            var towerBg = new BackGround(this, "TowerElements/tower_wall", new Vector2(), 6, 4, false);
            towerBg.DrawOrder = 0;
            var skyLeft = new BackGround(this, "TowerElements/sky", new Vector2(), 1, 1, false);
            skyLeft.DrawOrder = 300;
            var skyRight = new BackGround(this, "TowerElements/sky", new Vector2(GraphicsDevice.Viewport.Width - 100, 0), 1, 1, false);
            skyRight.Effect = SpriteEffects.FlipHorizontally;
            skyRight.DrawOrder = 300;
            var portal = new BackGround(this, "TowerElements/tower_portal", new Vector2(1095, Ground.Position.Y - 104), 1, 1, true);
            portal.DrawOrder = 150;



            var backLvl = new BackLevelDrawer(this, new Vector2(1200, Ground.Position.Y - 320));

            charac = new Character(this, Ground.Position + new Vector2(40, 0), Ground, new Vector2(20, -77))
                         {HitBox = {Width = 42, Height = 72,}};

            soundMgr = new SoundManager(this);

            listDrawable.Add(levelBg);
            listDrawable.Add(towerBg);
            listDrawable.Add(skyLeft);
            listDrawable.Add(skyRight);
            listDrawable.Add(portal);
            listDrawable.Add(charac);
            listDrawable.Add(backLvl);
            listDrawable.Add(Ground);
            listDrawable.Add(Ceiling);
            listDrawable.Add(_timer);

            listComponent.Add(soundMgr);

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

            OneWhitePixel = Content.Load<Texture2D>("onewhitepixel");
            menu = new MenuManager(this, Content);

            soundMgr.playGameMusic();
            levelMgr = new LevelManager(this, Ground, Ceiling, charac);

            
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            soundMgr.stopMusic();
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            timerRetry -= gameTime.ElapsedGameTime.Milliseconds;

            if (_menu && timerRetry < 0)
            {
                foreach (var c in this.Components)
                {
                    if (c is GameComponent)
                    {
                        ((GameComponent)c).Enabled = false;

                        if (c is DrawableGameComponent)
                            ((DrawableGameComponent)c).Visible = false;
                    }

                }
                menu.Enabled = true;
                menu.Visible = true;

                if (Keyboard.GetState().IsKeyDown(Keys.Enter) || Keyboard.GetState().IsKeyDown(Keys.Space)|| GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
                {
                    _menu = false;
                    _load = false;
                }

                return;
            }
            
            
            if (!_load && !_menu)
            {
                foreach (GameComponent c in listComponent)
                    c.Enabled = true;

                foreach (DrawableGameComponent d in listDrawable)
                {
                    d.Enabled = true;
                    d.Visible = true;
                }

                menu.Enabled = false;
                menu.Visible = false;

                levelMgr.LoadContent(this, this.Content);
                levelMgr.NextPallier(this);
                _load = true;
            }

            if (endGame)
            {
                foreach (var c in this.Components)
                {
                    var gameComponent = c as GameComponent;
                    if (gameComponent != null)
                    {
                        (gameComponent).Enabled = false;

                        var drawableGameComponent = c as DrawableGameComponent;
                        if (drawableGameComponent != null)
                            (drawableGameComponent).Visible = false;
                    }
                }
                _timer.Visible = true;
                menu.endGame();
                menu.Enabled = true;
                menu.Visible = true;

                if (Keyboard.GetState().IsKeyDown(Keys.Enter) || Keyboard.GetState().IsKeyDown(Keys.Space) || GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
                {
                    levelMgr.numPallier = -1;
                    /*
                    charac.Enabled = true;
                    charac.Visible = true;
                    Ground.Visible = true;
                    Ground.Enabled = true;
                    soundMgr.Enabled = true;
                    Ceiling.Visible = true;
                    Ceiling.Enabled = true;
                     */
                    /*
                    foreach (GameComponent c in listComponent)
                        c.Enabled = true;

                    foreach (DrawableGameComponent d in listDrawable)
                    {
                        d.Enabled = true;
                        d.Visible = true;
                    }

                    levelMgr.LoadContent(this, this.Content);
                    levelMgr.NextPallier(this);
                    
                    menu.Enabled = false;
                    menu.Visible = false;
                    */
                    _menu = true;
                    menu.menuPrinc();
                    endGame = false;
                    timerRetry = 500;

                }

                return;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                soundMgr.stopMusic();
                this.Exit();
                
            }
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);

            if(levelMgr.SpecialDisplay != null && spriteBatch != null)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                spriteBatch.Draw(levelMgr.SpecialDisplay, new Vector2(-(float)Math.Cos(-GameCore.Angle), -(float)Math.Sin(-GameCore.Angle)) * 3, Color.White);
                spriteBatch.End();

                if (Transitioner.Visible)
                    Transitioner.Draw(gameTime);
            }
        }

        /// <summary>
        /// applies a rotation from center levelOrigin
        /// </summary>
        /// <param name="posXY"></param>
        /// <param name="levelOrigin"></param>
        /// <returns></returns>
        public Vector2 ToDrawPosition(Vector2 posXY, Vector2 levelOrigin)
        {
            var tiltedPos = new Vector2(
                (float)Math.Cos(Angle) * (posXY.X - levelOrigin.X) - (float)Math.Sin(Angle) * (posXY.Y - levelOrigin.Y) + levelOrigin.X,
                (float)Math.Sin(Angle) * (posXY.X - levelOrigin.X) + (float)Math.Cos(Angle) * (posXY.Y - levelOrigin.Y) + levelOrigin.Y);
            return tiltedPos;
        }
        
        /// <summary>
        /// Spritebatch must be initialized (begin) before
        /// </summary>
        /// <param name="r"></param>
        public void DrawHitBox(Rectangle r)
        {
#if DEBUG
            //spriteBatch.Draw(OneWhitePixel, this.ToDrawPosition(new Vector2(r.X, r.Y), Ground.Position), null, new Color(255, 0, 0, 8), (float) Angle, new Vector2(), new Vector2(r.Width, r.Height), SpriteEffects.None, 0);
#endif
        }
         
    }
}
