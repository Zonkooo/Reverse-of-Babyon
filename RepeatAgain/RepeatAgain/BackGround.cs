using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RepeatAgain
{
    class BackGround : DrawableGameComponent
    {
        private GameCore _game;

        private readonly Vector2 _position;
        private Texture2D _texture;
        private readonly int _repeatX;
        private readonly int _repeatY;
        private readonly bool _tilted;
        private readonly string _spritePath;

        public SpriteEffects Effect = SpriteEffects.None;

        public BackGround(GameCore game, string sprite, Vector2 position, int repeatX, int repeatY, bool tilted) : base(game)
        {
            this._game = game;
            this._position = position;
            this._repeatX = repeatX;
            this._repeatY = repeatY;
            this._tilted = tilted;
            this._spritePath = sprite;

            this._game.Components.Add(this);
            this.Visible = true;
            this.Enabled = false; //nothing to update
        }

        protected override void LoadContent()
        {
            _texture = _game.Content.Load<Texture2D>(_spritePath);
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            if(_game.spriteBatch != null)
            {
                _game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                var offset = new Vector2();
                for (int i = 0; i < _repeatX; ++i )
                {
                    for(int j = 0; j < _repeatY; ++j)
                    {
                        _game.spriteBatch.Draw(_texture, _tilted ?_game.ToDrawPosition(_position + offset, _game.Ground.Position): _position + offset, null, Color.White, _tilted?(float)(GameCore.Angle) : 0, new Vector2(), 1.0f, Effect, 0);
                        offset.Y += _texture.Height;
                    }
                    offset.Y = 0;
                    offset.X += _texture.Width;
                }

                    _game.spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
