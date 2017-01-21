using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace RepeatAgain
{
    enum CharacterState
    {
        Still = 2,
        Walk = 0,
        Jump = 1,
        Die = 3,
    };

    /// <summary>
    /// 
    /// </summary>
    public class Character : DrawableGameComponent
    {
        public Rectangle HitBox;
        private Vector2 _hitBoxOffset;

        private readonly GameCore _game;

        private readonly Ground _ground;

        public bool Controlable = true;
        //walking
        private Vector2 _position;
        private const float HorizontalSpeed = 0.28f;
        //jumping
        private bool _isJumping = false;
        private float _verticalSpeed = 0.0f;
        private const float VerticalKick = 0.5f;
        private const float Gravity = 0.022f;

        private Dictionary<CharacterState, int> _nbFramesAnim = new Dictionary<CharacterState, int>()
                                                                    {
                                                                        {CharacterState.Still, 16},
                                                                        {CharacterState.Walk, 10},
                                                                        {CharacterState.Jump, 8},
                                                                        {CharacterState.Die, 5},
                                                                    };

        private CharacterState _state;
        private CharacterState _previousState;
        private Texture2D[] _tabAnims;
        private Texture2D[] _tabAnimsBack;
        private int _indexAnim;
        private int _timerAnim;
        private bool _behind;
        private bool _dead;
        private int _timerDead;
        private int _timerBehind;
        private int _sens;
        public bool right;
        private float thumbOffset;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        /// <param name="position"></param>
        /// <param name="ground"></param>
        public Character(GameCore game, Vector2 position, Ground ground, Vector2 hitBoxOffset) : base(game)
        {
            this._game = game;
            this._position = position;
            this._ground = ground;

            this._hitBoxOffset = hitBoxOffset;
            HitBox.X = (int)(position.X + hitBoxOffset.X);
            HitBox.Y = (int)(position.Y + hitBoxOffset.Y);

            _sens = 1;
            right = true;

            Game.Components.Add(this);
            this.Enabled = true;
            this.Visible = true;
            this.DrawOrder = 200;

            thumbOffset = 0.1f;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void LoadContent()
        {
            _tabAnims = new Texture2D[Enum.GetValues(typeof(CharacterState)).Length];
            _tabAnims[(int)CharacterState.Walk] = _game.Content.Load<Texture2D>("AnimPerso/prince_run");
            _tabAnims[(int)CharacterState.Jump] = _game.Content.Load<Texture2D>("AnimPerso/prince_jump");
            _tabAnims[(int)CharacterState.Still] = _game.Content.Load<Texture2D>("AnimPerso/prince_stand");
            _tabAnims[(int)CharacterState.Die] = _game.Content.Load<Texture2D>("AnimPerso/blood");

            _tabAnimsBack = new Texture2D[Enum.GetValues(typeof(CharacterState)).Length];
            _tabAnimsBack[(int)CharacterState.Walk] = _game.Content.Load<Texture2D>("AnimPerso/shadow_run");
            _tabAnimsBack[(int)CharacterState.Jump] = _game.Content.Load<Texture2D>("AnimPerso/shadow_jump");
            _tabAnimsBack[(int)CharacterState.Still] = _game.Content.Load<Texture2D>("AnimPerso/shadow_stand");
            _tabAnimsBack[(int)CharacterState.Die] = _tabAnims[(int)CharacterState.Die]; //TODO : have a shadow blood

            _state = CharacterState.Die;
            _indexAnim = 0;
            _timerAnim = 0;
            
            base.LoadContent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            _previousState = _state;
            Vector2 previousPos = _position;
            if (_dead)
            {
                _timerDead -= gameTime.ElapsedGameTime.Milliseconds;

                if (_timerDead < 0)
                    Respawn(false);
                return;
            }

            //Change Behind
            if (_timerBehind > 0)
            {
                _timerBehind -= gameTime.ElapsedGameTime.Milliseconds;

                if (_timerBehind < 0)
                {
                    Controlable = true;
                }
            }

            Vector2 savePos = _position;

            if (Controlable)
            {
                Vector2 leftStick = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left;
                

                if (Keyboard.GetState().IsKeyDown(Keys.Right) || (leftStick.Length() > thumbOffset && leftStick.X > 0))
                {
                    this._position.X += HorizontalSpeed * _sens * gameTime.ElapsedGameTime.Milliseconds;
                    if (_state != CharacterState.Jump)
                        _state = CharacterState.Walk;
                    right = true;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Left) || (leftStick.Length() > thumbOffset && leftStick.X < 0))
                {
                    this._position.X -= HorizontalSpeed * _sens * gameTime.ElapsedGameTime.Milliseconds;
                    if (_state != CharacterState.Jump)
                        _state = CharacterState.Walk;
                    right = false;
                }
                else
                {
                    if (_state != CharacterState.Jump)
                        _state = CharacterState.Still;
                }
            }
            else if(!_isJumping)
            {
                if (_state != CharacterState.Jump)
                    _state = CharacterState.Still;
            }

            //---------- JUMP -----------
            if (Controlable)
            {
                if ((Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.Space) || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed) && !_isJumping)
                {
                    this._verticalSpeed = VerticalKick;
                    this._state = CharacterState.Jump;
                    this._isJumping = true;
                }
            }

            //---------- GRAVITY -----------
            _verticalSpeed -= Gravity;
            //MathHelper.Clamp(_verticalSpeed, -VerticalKick, VerticalKick);
            this._position.Y -= _verticalSpeed * gameTime.ElapsedGameTime.Milliseconds;


            //-------ICE--------
            if (_game.levelMgr.ice && _position.Y > _ground.Position.Y - 5)
                _position.X -= (float)(_game.levelMgr.iceSpeed * gameTime.ElapsedGameTime.Milliseconds);
            
            //---------- COLLISIONS -----------
            //trying to move on X axis
            HitBox.X = (int)(_position.X + _hitBoxOffset.X);
            //HitBox.Y = (int)_position.Y - HitBox.Height;
            if (_game.levelMgr.CollidesWith(this.HitBox))
            {
                _position.X = previousPos.X;
            }
            //trying to move on Y axis
            HitBox.X = (int)(_position.X + _hitBoxOffset.X);
            HitBox.Y = (int)(_position.Y + _hitBoxOffset.Y);
            if (_game.levelMgr.CollidesWith(this.HitBox))
            {
                _position.Y = previousPos.Y;
                if(_verticalSpeed < 0)
                {
                    this._isJumping = false;
                    if (_state == CharacterState.Jump)
                        _state = CharacterState.Still;
                }
                _verticalSpeed = 0.0f;
            }
            //reset HitBox values
            HitBox.X = (int)(_position.X + _hitBoxOffset.X);
            HitBox.Y = (int)(_position.Y  + _hitBoxOffset.Y);
            
            //---------- FALL -----------
            if (_position.Y > _ground.Position.Y + 100)
                dead();

            //---------- RELOAD -----------
            if ((Keyboard.GetState().IsKeyDown(Keys.R) || GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Pressed))
            {
                Respawn(false);
            }
            
            //---------- SWITCH SIDE -----------
#if DEBUG
            if (Controlable && Keyboard.GetState().IsKeyDown(Keys.Tab))
            {
                _game.Transitioner.ChangeLevel();
                Controlable = false;
                Respawn(true);
            }
#endif
            if (_position.X > 1100)
            {
                if (!_behind)
                {
                    _behind = true;
                    _position = _game.Ground.Position + new Vector2(40, 0);
                    _sens = -1;
                    Controlable = false;
                    _timerBehind = 500;
                    right = false;
                }
                else
                {
                    _game.Transitioner.ChangeLevel();
                    Controlable = false;
                    Respawn(true);
                }
            }
            
            base.Update(gameTime);
        }

        /// <summary>
        /// 
        /// </summary>
        public void dead()
        {
            _dead = true;
            _isJumping = false;
            _timerDead = GetTimerAnims(CharacterState.Die).Sum();
            _state = CharacterState.Die;
            Controlable = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Respawn(bool wait)
        {
            _position = _game.Ground.Position + new Vector2(40, 0);
            _verticalSpeed = 0.0f;
            _dead = false;
            _behind = false;
            _sens = 1;
            if (wait)
                _timerBehind = 500;
            else
                Controlable = true;
            right = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            var state = _state;
            int[] tabTimerAnims = GetTimerAnims(state);

            if(_previousState != state) //state changed = reset animation counters
            {
                _timerAnim = tabTimerAnims[0];
                _indexAnim = 0;
            }

            int nbAnims = _nbFramesAnim[state];

            //Anim
            _timerAnim -= gameTime.ElapsedGameTime.Milliseconds;
            if (nbAnims > 1 && _timerAnim < 0)
            {
                _indexAnim = (_indexAnim + 1) % nbAnims;

                _timerAnim = tabTimerAnims[_indexAnim];
            }

            var sprite = _tabAnims[(int) state];

            if (_game.spriteBatch != null)
            {
                _game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                SpriteEffects sensCharac = !right ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                if(!_behind)
                    _game.spriteBatch.Draw(_tabAnims[(int)state], _game.ToDrawPosition(_position, _ground.Position), new Rectangle(_indexAnim * (sprite.Width / nbAnims), 0, (sprite.Width / nbAnims), sprite.Height), Color.White, (float)(GameCore.Angle), new Vector2(0, sprite.Height), 1.0f, sensCharac, 0);
                
                if (_behind)
                {
                    Vector2 temp = _game.ToDrawPosition(_position, _ground.Position);
                    var posBehind = new Vector2(GraphicsDevice.Viewport.Width - temp.X + 15 - (sprite.Width / nbAnims), temp.Y - 369);
                    _game.spriteBatch.Draw(_tabAnimsBack[(int)state], posBehind, new Rectangle(_indexAnim * (sprite.Width / nbAnims), 0, (sprite.Width / nbAnims), sprite.Height), Color.Black, -(float)(GameCore.Angle), new Vector2(0, sprite.Height), 1.0f, sensCharac, 0);

                }

                _game.DrawHitBox(HitBox);

                _game.spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        private int[] GetTimerAnims(CharacterState a)
        {
            switch(a)
            {
                case CharacterState.Walk:
                    {
                        int[] tab = new int[_nbFramesAnim[a]];

                        for (int i = 0; i < tab.Length; i++)
                        {
                            tab[i] = 50;

                        }

                        return tab;
                    }

                case CharacterState.Jump :
                    {
                        int[] tab = new int[_nbFramesAnim[a]];
                        for (int i = 0; i < tab.Length; i++)
                        {
                            tab[i] = 33;

                        }
                        return tab;
                    }

                case CharacterState.Still :
                    {
                        int[] tab = new int[_nbFramesAnim[a]];
                        for (int i = 0; i < tab.Length; i++)
                        {
                            tab[i] = 33;

                        }
                        return tab;
                    }

                case CharacterState.Die :
                    {
                        int[] tab = new int[_nbFramesAnim[a]];
                        for (int i = 0; i < tab.Length; i++)
                        {
                            tab[i] = 50;

                        }
                        tab[tab.Length - 1] = 600;
                        return tab;
                    }

                default:
                    {
                        Debug.Assert(false);
                        return null;
                    }
            }
        }

    }
}
