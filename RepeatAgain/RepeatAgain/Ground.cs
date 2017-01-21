using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RepeatAgain
{
    public class Ground : DrawableGameComponent
    {
        public Rectangle HitBox;
        private Vector2 _hitBoxOffset;

        public Texture2D _sprite;
        public Texture2D _sprite_ice;
        private int _nbRepeat;
        private readonly GameCore _game;

        public Vector2 Position;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        /// <param name="position"></param>
        /// <param name="hitBoxOffset"></param>
        /// <param name="nbContentCells"></param>
        public Ground(GameCore game, Vector2 position, Vector2 hitBoxOffset, int nbContentCells)
            : base(game)
        {
            this._game = game;
            this.Position = position;
            _nbRepeat = nbContentCells;
            this.HitBox = new Rectangle((int)(position.X + hitBoxOffset.X), (int)(position.Y + hitBoxOffset.Y), 0, 0);
            this._hitBoxOffset = hitBoxOffset;

            Game.Components.Add(this);
            this.Enabled = true;
            this.Visible = true;
            this.DrawOrder = 100;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void LoadContent()
        {
            this._sprite = _game.Content.Load<Texture2D>("TowerElements/tower_ground");
            this._sprite_ice = _game.Content.Load<Texture2D>("TowerElements/tower_ground_skin");
            base.LoadContent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            if (_game.spriteBatch != null)
            {
                _game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                var xOffset = new Vector2();
                for (int i = 0; i < _nbRepeat; ++i )
                {
                    _game.spriteBatch.Draw(_game.levelMgr.ice && this == _game.Ground ? _sprite_ice : _sprite, 
                        _game.ToDrawPosition(Position + xOffset, _game.Ground.Position),
                        null, 
                        Color.White, 
                        (float)(GameCore.Angle),
                        new Vector2(), 
                        1.0f,
                        SpriteEffects.None, 
                        0);
                    xOffset.X += _sprite.Width;
                }

                _game.spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
