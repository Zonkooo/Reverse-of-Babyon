using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RepeatAgain
{

    public class Projectile : DrawableGameComponent
    {

        public Rectangle HitBox;

        private GameCore _game;
        public Vector2 Position;
        private Texture2D _sprite;
        private float _speed;
        private int _nbAnim;
        private int _initTimerAnim;
        private int _timerAnim;
        private int _indexAnim;
        private Vector2 sizeTexture;
        private Vector2 _levelOrigin;
        private int _sens;
        private bool _boom;
        private int _initTimerBoom;
        private int _timerBoom;
        private int _indexBoom;
        private int _nbAnimBoom;
        private Texture2D _spriteBoom;
        
        
        public Projectile(GameCore game, Vector2 levelOrigin, int sens, Texture2D texture)
            : base(game)
        {
            this._game = game;
            Game.Components.Add(this);
            this.Visible = false;
            this.Enabled = false;
            this.DrawOrder = 95;

            _sprite = texture;
            _spriteBoom = _game.Content.Load<Texture2D>("Traps/boom_mc");
            _game = game;
            _levelOrigin = levelOrigin;
            _sens = sens;

            _speed = 0.5f;
            _boom = false;

            _nbAnim = 8;
            _initTimerAnim = 67;
            _timerAnim = _initTimerAnim ;
            sizeTexture = new Vector2(40, 40);
            HitBox.Width = (int)sizeTexture.X/2;
            HitBox.Height = (int)sizeTexture.Y/2;

            _initTimerBoom = 33;
            _nbAnimBoom = 8;
        }

        public override void Update(GameTime gameTime)
        {
            if (_boom)
                return;

            Position.X += _speed * _sens * gameTime.ElapsedGameTime.Milliseconds;

            HitBox.X = (int)Position.X + (_sens < 0 ? 5 : 45);
            HitBox.Y = (int)(Position.Y) + 8;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {

            //Anim
            if (!_boom)
            {
                _timerAnim -= gameTime.ElapsedGameTime.Milliseconds;
                if (_nbAnim != 1 && _timerAnim < 0)
                {
                    _indexAnim++;

                    if (_indexAnim > _nbAnim - 1)
                        _indexAnim = 0;
                    _timerAnim = _initTimerAnim;
                }

            }

            else
            {
                _timerBoom -= gameTime.ElapsedGameTime.Milliseconds;
                if (_nbAnimBoom != 1 && _timerBoom < 0)
                {
                    _indexBoom++;

                    if (_indexBoom > _nbAnimBoom - 1)
                    {
                        this.Visible = false;
                        this.Enabled = false;
                    }
                        

                    _timerBoom = _initTimerBoom;
                }

            }
            


            if (_game.spriteBatch != null)
            {
                _game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                SpriteEffects sensProj;

                if (_sens == 1)
                    sensProj = SpriteEffects.FlipHorizontally;
                else
                    sensProj = SpriteEffects.None;

                if(!_boom)
                    _game.spriteBatch.Draw(_sprite, _game.ToDrawPosition(Position, _levelOrigin), new Rectangle(_indexAnim * (_sprite.Width / _nbAnim), 0, (_sprite.Width / _nbAnim), _sprite.Height), Color.White, (float)(GameCore.Angle), new Vector2(), 1.0f, sensProj, 0);

                else
                    _game.spriteBatch.Draw(_spriteBoom, _game.ToDrawPosition(Position, _levelOrigin), new Rectangle(_indexBoom * (_spriteBoom.Width / _nbAnimBoom), 0, (_spriteBoom.Width / _nbAnimBoom), _sprite.Height), Color.White, (float)(GameCore.Angle), new Vector2(), 1.0f, sensProj, 0);

                _game.DrawHitBox(HitBox);

                _game.spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        public void Kill()
        {
            _boom = true;
            _indexBoom = 0;
            _timerBoom = _initTimerBoom;

            if(Position.X > 0 && Position.X < GraphicsDevice.Viewport.Width)
                _game.soundMgr.PlaySound(_game.soundMgr.fireEnd);
            
        }

        public void Activate(Vector2 position)
        {
            this.Position = position;
            this.Visible = true;
            this.Enabled = true;
            _boom = false;
        }
    }
}
