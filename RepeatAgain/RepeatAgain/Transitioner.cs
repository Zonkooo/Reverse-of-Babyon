using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RepeatAgain
{
    public class Transitioner : DrawableGameComponent
    {
        private readonly GameCore _game;

        private Texture2D _texture;
        private Vector2 _position;

        private bool _updated;

        public Transitioner(GameCore game)
            : base(game)
        {
            this._game = game;
            this._game.Components.Add(this);
            this.Enabled = false;
            this.Visible = false;
            this.DrawOrder = 10000;
        }

        protected override void LoadContent()
        {
            _texture = _game.Content.Load<Texture2D>("clouds");
            base.LoadContent();
        }

        public void ChangeLevel()
        {
            this.Enabled = true;
            this.Visible = true;
            this._updated = false;
            this._position = new Vector2(0, -_texture.Height);
            _game.charac.Visible = false; //hide character while effect
            _game.charac.Enabled = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (!_updated && _position.Y > -(_texture.Height - GraphicsDevice.Viewport.Height) / 2)
            {
                _game.levelMgr.NextPallier(_game);
                _game.charac.Visible = true;
                _game.charac.Enabled = true;
                _updated = true;
            }
            _position.Y += gameTime.ElapsedGameTime.Milliseconds*(_texture.Height + GraphicsDevice.Viewport.Height)/2000f;
            if (_position.Y > GraphicsDevice.Viewport.Height)
            {
                this.Enabled = false;
                this.Visible = false;
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (_game.spriteBatch != null)
            {
                _game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                _game.spriteBatch.Draw(_texture, _position, Color.White);

                _game.spriteBatch.End();
            }
            base.Draw(gameTime);
        }
    }
}
