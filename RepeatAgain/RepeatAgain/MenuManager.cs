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

    public class MenuManager : DrawableGameComponent
    {

        public Texture2D background;
        private GameCore _game;

        public MenuManager(GameCore game, ContentManager content)
            : base(game)
        {
            _game = game;
            background = content.Load<Texture2D>("intro_screen");

            Game.Components.Add(this);
            this.Enabled = true;
            this.Visible = true;
        }

        public void endGame()
        {
            background = _game.Content.Load<Texture2D>("end_screen");
        }

        public void menuPrinc()
        {
            background = _game.Content.Load<Texture2D>("intro_screen");
        }


        public override void Draw(GameTime gameTime)
        {
            if (_game.spriteBatch != null)
            {
                _game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                _game.spriteBatch.Draw(background, new Vector2(), null, Color.White, 0, new Vector2(), 1.0f, SpriteEffects.None, 0);

                _game.spriteBatch.End();
            }


            base.Draw(gameTime);
        }

    }
    
}
