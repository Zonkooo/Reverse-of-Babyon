using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RepeatAgain
{
    class LevelDisplay : DrawableGameComponent
    {
        private GameCore _game;
        private SpriteFont _font;

        public LevelDisplay(GameCore game) 
            : base(game)
        {
            this._game = game;
            _game.Components.Add(this);
            this.DrawOrder = 5000;
        }

        protected override void LoadContent()
        {
            this._font = _game.Content.Load<SpriteFont>("SpriteFont1");
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            if(_game.spriteBatch != null)
            {
                _game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                _game.spriteBatch.DrawString(_font, "level", new Vector2((90 - _font.MeasureString("level").X) /2, 5), Color.White);
                var numstr = (_game.levelMgr.numPallier - 2).ToString();
                _game.spriteBatch.DrawString(_font, numstr, new Vector2((90 - _font.MeasureString(numstr).X) / 2, _font.MeasureString("level").Y + 10 ), Color.White);

                _game.spriteBatch.End();
            }
            base.Draw(gameTime);
        }
    }
}
