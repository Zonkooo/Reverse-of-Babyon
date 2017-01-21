using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RepeatAgain
{
    public enum DoorType { NONE, D1, D2, D3 };
    public enum DoorState { OPEN, CLOSE };
    
    class Door : DrawableGameComponent
    {
        public Texture2D _sprite;
        public DoorState state;
        public DoorType typeDoor;
        public Rectangle hitBox;

        private int _nbAnim;
        private int _indexAnim;
        private GameCore _game;
        private Vector2 _position;
        private LevelManager lvlMgr;


        public Door(GameCore game, DoorType t, Vector2 pos)
            :base(game)
        {
            _game = game;

            _sprite = _game.Content.Load<Texture2D>("Traps/door_anim");
            _nbAnim = 2;
            _indexAnim = 0;
            state = DoorState.CLOSE;
            typeDoor = t;
            _position = pos;
            lvlMgr = _game.levelMgr;

            hitBox = new Rectangle((int)_position.X, (int)_position.Y - _sprite.Height, (int)(_sprite.Width / _nbAnim), (int)_sprite.Height);

            Game.Components.Add(this);
            this.Enabled = true;
            this.Visible = true;
            this.DrawOrder = 90;
        }

        public void openDoor()
        {
            if (state != DoorState.OPEN)
            {
                state = DoorState.OPEN;
                _indexAnim = 1;

                _game.soundMgr.PlaySound(_game.soundMgr.door);
            }
        }

        public void closeDoor()
        {
            if (state != DoorState.CLOSE)
            {
                state = DoorState.CLOSE;
                _indexAnim = 0;
                _game.soundMgr.PlaySound(_game.soundMgr.door);

                if (hitBox.Intersects(_game.charac.HitBox))
                    _game.charac.dead();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (_game.spriteBatch != null)
            {
                _game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                _game.spriteBatch.Draw(_sprite, _game.ToDrawPosition(_position, _game.Ground.Position), new Rectangle(_indexAnim * (_sprite.Width / _nbAnim), 0, (_sprite.Width / _nbAnim), _sprite.Height), Color.White, (float)(GameCore.Angle), new Vector2(0, _sprite.Height), 1.0f, SpriteEffects.None, 0);

                _game.spriteBatch.End();
            }
            
            base.Draw(gameTime);
        }



    }
}
