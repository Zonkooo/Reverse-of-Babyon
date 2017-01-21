using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RepeatAgain
{
    class BackLevelDrawer : DrawableGameComponent
    {
        private readonly GameCore _game;

        private readonly Vector2 _position;
        private Texture2D _portal;
        private Texture2D _bg;

        public BackLevelDrawer(GameCore game, Vector2 position)
            : base(game)
        {
            this._game = game;
            _game.Components.Add(this);
            this.DrawOrder = 5;
            this.Visible = true;

            this._position = position;
        }

        protected override void LoadContent()
        {
            _portal = _game.Content.Load<Texture2D>("TowerElements/tower_portal_shadow");
            _bg = _game.Content.Load<Texture2D>("TowerElements/tunnel_shadow");
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            if (_game.spriteBatch != null)
            {
                _game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                
                Vector2 offsetDirection = new Vector2(-(float)Math.Cos(-GameCore.Angle), -(float)Math.Sin(-GameCore.Angle));
                var offset = new Vector2();
                for (int i = 0; i < 28; ++i)
                {
                    var origin = new Vector2(40, 320);
                    _game.spriteBatch.Draw(_bg, _position + offset, null, Color.White, -(float) GameCore.Angle, origin, 1, SpriteEffects.None, 0);
                    offset += offsetDirection*40;
                }
                _game.spriteBatch.Draw(_portal, new Vector2(100, 234), null, Color.White, -(float)GameCore.Angle, new Vector2(0, 104), 1, SpriteEffects.FlipHorizontally, 0);

                _game.spriteBatch.End();
            }
            base.Draw(gameTime);
        }
    }
}
