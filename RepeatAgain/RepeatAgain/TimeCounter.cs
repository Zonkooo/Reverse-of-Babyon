using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RepeatAgain
{
    class TimeCounter : DrawableGameComponent
    {
        private readonly GameCore _game;
        private SpriteFont _font;
        private TimeSpan _time;

        public TimeCounter(GameCore game)
            : base(game)
        {
            this._game = game;
            this.Enabled = false;
            _game.Components.Add(this);
            this.DrawOrder = 5000;
        }

        protected override void LoadContent()
        {
            this._font = _game.Content.Load<SpriteFont>("SpriteFont1");
            base.LoadContent();
        }

        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            if(this.Enabled)
                _time = new TimeSpan();
            base.OnEnabledChanged(sender, args);
        }

        public override void Update(GameTime gameTime)
        {
            _time += gameTime.ElapsedGameTime;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (_game.spriteBatch != null)
            {
                _game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                _game.spriteBatch.DrawString(_font, "time : " + _time.TotalSeconds.ToString("0.0"), new Vector2(5,GraphicsDevice.Viewport.Height - (this.Enabled ? 5 : 17)), this.Enabled ? Color.White : new Color(114, 115, 141), this.Enabled ? -MathHelper.PiOver2 : 0, new Vector2(), 1, SpriteEffects.None, 0);

                _game.spriteBatch.End();
            }
            base.Draw(gameTime);
        }
    }
}
