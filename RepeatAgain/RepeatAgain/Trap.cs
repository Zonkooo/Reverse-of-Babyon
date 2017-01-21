using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RepeatAgain
{
    public enum TrapType
    {
        NONE, 
        PICKFIXE, 
        PICKAR1, 
        PICKAR2, 
        PICKAR3, 
        PJ1, 
        PJ2, 
        PJ3,
        PJI1,
        PJI2,
        PJI3, 
        ID1, 
        ID2, 
        ID3, 
        BOX ,
        BBOX
    };

    public class Trap : DrawableGameComponent
    {
        public TrapType TypeTrap;
        public Rectangle HitBox;
        public Character Charac;
        

        private readonly GameCore _game;
        private readonly LevelManager _manager;
        private Vector2 _position;
        private Texture2D _sprite;
        private Vector2 _levelOrigin;
        private int nbAnim;
        private int indexAnim;
        private int[] timerAnim;
        private int timer;
        private int initTimerProjectile;
        private int timerProjectile;
        private int _sens;
        private bool _load;
        private bool _ceiling;
        private int _initTimerDoor;
        private int _timerDoor;
        

        private readonly Stack<Projectile> _freeProjectiles = new Stack<Projectile>(10);
        public List<Projectile> ListProj = new List<Projectile>(10);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        /// <param name="sprite"></param>
        /// <param name="pos"></param>
        /// <param name="typeT"></param>
        /// <param name="levelOrigin"></param>
        /// <param name="ceiling"></param>
        /// <param name="manager"> </param>
        public Trap(GameCore game, Texture2D sprite, Vector2 pos, TrapType typeT, Vector2 levelOrigin, bool ceiling, LevelManager manager)
            : base(game)
        {
            this._game = game;
            Game.Components.Add(this);

            this._manager = manager;

            this.Enabled = true;
            this.Visible = true;
            this.DrawOrder = 90;

            _game = game;
            _position = pos;

            Charac = _game.charac;
            TypeTrap = typeT;
            indexAnim = 0;
            _levelOrigin = levelOrigin;
            _ceiling = ceiling;

            nbAnim = getNbAnim(TypeTrap);
            GetTimerAnim(TypeTrap);

            if(nbAnim != 0)
                timer = timerAnim[0];

            _sprite = sprite;

            HitBox = new Rectangle((int)_position.X, (int)_position.Y - sprite.Height, sprite.Width / nbAnim, sprite.Height);

            //---------------PICK-----------------
            if (TypeTrap == TrapType.PICKFIXE || TypeTrap == TrapType.PICKAR1 || TypeTrap == TrapType.PICKAR2 || TypeTrap == TrapType.PICKAR3)
            {
                HitBox = !_ceiling ? 
                    new Rectangle((int)_position.X + 5, (int)_position.Y - sprite.Height / 2, (sprite.Width / nbAnim) - 10, sprite.Height / 2) : 
                    new Rectangle((int)_position.X + 5, (int)_position.Y - sprite.Height, (sprite.Width / nbAnim) - 10, sprite.Height / 2);
            }

            if (TypeTrap == TrapType.PICKAR2)
            {
                timer = timerAnim[0] / 2;
            }

            if (TypeTrap == TrapType.PICKAR3)
            {
                timer = 0;
            }

            initTimerProjectile = 2000;

            //-----PROJECTILE
            if (TypeTrap == TrapType.PJ1 || TypeTrap == TrapType.PJ2 || TypeTrap == TrapType.PJ3 || TypeTrap == TrapType.PJI1 || TypeTrap == TrapType.PJI2 || TypeTrap == TrapType.PJI3)
            {
                

                if (TypeTrap == TrapType.PJ1 || TypeTrap == TrapType.PJ2 || TypeTrap == TrapType.PJ3)
                    _sens = -1;
                else
                    _sens = 1;

                if (TypeTrap == TrapType.PJ1 || TypeTrap == TrapType.PJI1)
                    timerProjectile = initTimerProjectile;
                else if (TypeTrap == TrapType.PJ2 || TypeTrap == TrapType.PJI2)
                    timerProjectile = 1000;
                else if (TypeTrap == TrapType.PJ3 || TypeTrap == TrapType.PJI3)
                    timerProjectile = 500;


                if (TypeTrap == TrapType.PJ1 || TypeTrap == TrapType.PJ2 || TypeTrap == TrapType.PJ3 || TypeTrap == TrapType.PJI1 || TypeTrap == TrapType.PJI2 || TypeTrap == TrapType.PJI3)
                {
                    var projectileTexture = _game.Content.Load<Texture2D>("Traps/fireball");

                    for (int i = 0; i < 10; i++)
                    {
                        _freeProjectiles.Push(new Projectile(_game, _levelOrigin, _sens, projectileTexture));
                    }
                }
            }

            //--INTERRUPTEUR
            if (TypeTrap == TrapType.ID1 || TypeTrap == TrapType.ID2 || TypeTrap == TrapType.ID3)
            {
                nbAnim = 2;
                indexAnim = 0;
                _initTimerDoor = 4250;
            }
            

            _load = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (TypeTrap != TrapType.PJ1 && TypeTrap != TrapType.PJ2 && TypeTrap != TrapType.PJ3 && TypeTrap != TrapType.PJI1 && TypeTrap != TrapType.PJI2 && TypeTrap != TrapType.PJI3 && TypeTrap != TrapType.ID1 && TypeTrap != TrapType.ID2 && TypeTrap != TrapType.ID3)
                return;

            if ((TypeTrap == TrapType.ID1 || TypeTrap == TrapType.ID2 || TypeTrap == TrapType.ID3))
            {

                if (Charac.HitBox.Intersects(this.HitBox))
                {
                    indexAnim = 1;

                    DoorType typeDoor = DoorType.NONE;

                    if (TypeTrap == TrapType.ID1)
                        typeDoor = DoorType.D1;
                    else if (TypeTrap == TrapType.ID2)
                        typeDoor = DoorType.D2;
                    else if (TypeTrap == TrapType.ID3)
                        typeDoor = DoorType.D3;

                    _timerDoor = _initTimerDoor;
                    _game.levelMgr.openDoor(typeDoor);
                }
                

                else if(_timerDoor < 0)
                {
                    indexAnim = 0;

                    DoorType typeDoor = DoorType.NONE;

                    if (TypeTrap == TrapType.ID1)
                        typeDoor = DoorType.D1;
                    else if (TypeTrap == TrapType.ID2)
                        typeDoor = DoorType.D2;
                    else if (TypeTrap == TrapType.ID3)
                        typeDoor = DoorType.D3;

                    _game.levelMgr.closeDoor(typeDoor);
                }

                if(indexAnim == 1)
                    _timerDoor -= gameTime.ElapsedGameTime.Milliseconds;

                return;
            }

            else if (TypeTrap != TrapType.PJ1 && TypeTrap != TrapType.PJ2 && TypeTrap != TrapType.PJ3 && TypeTrap != TrapType.PJI1 && TypeTrap != TrapType.PJI2 && TypeTrap != TrapType.PJI3)
                return;

            foreach (Projectile p in ListProj)
            {
                if(p.Position.X < 0 || p.Position.X > GraphicsDevice.Viewport.Width)
                {
                    this.DeleteProjectile(p);
                    break; //assumes 2 projectiles will never collide at the same time
                }
                if (_manager.CollidesWith(p.HitBox, false))
                {
                    this.DeleteProjectile(p);
                    break; //assumes 2 projectiles will never collide at the same time
                }
            }

            timerProjectile -= gameTime.ElapsedGameTime.Milliseconds;

            if (timerProjectile < 0)
            {
                Projectile p = _freeProjectiles.Pop();
                p.Activate(new Vector2(_position.X, _position.Y - _sprite.Height));
                ListProj.Add(p);
                timerProjectile = initTimerProjectile;
                _game.soundMgr.PlaySound(_game.soundMgr.fireOut);
            }

            base.Update(gameTime);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        public void DeleteProjectile(Projectile p)
        {
            
            ListProj.Remove(p);
            _freeProjectiles.Push(p);
            p.Kill();
        }

        public void cleanProjectile()
        {
            if (ListProj.Count == 0)
                return;

            Projectile[] tab = ListProj.ToArray();
            foreach (Projectile t in tab)
                DeleteProjectile(t);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (!_load)
                return;

            timer -= gameTime.ElapsedGameTime.Milliseconds;
            if (nbAnim != 1 && (TypeTrap != TrapType.PICKFIXE && TypeTrap != TrapType.ID1 && TypeTrap != TrapType.ID2 && TypeTrap != TrapType.ID3) && timer < 0)
            {
                indexAnim++;

                //Sound Pick 
                if (TypeTrap == TrapType.PICKAR1 || TypeTrap == TrapType.PICKAR2 || TypeTrap == TrapType.PICKAR3)
                {
                    _game.soundMgr.PlaySound(indexAnim == 1 ? _game.soundMgr.pickIn : _game.soundMgr.pickOut);
                }

                if (indexAnim > nbAnim -1)
                    indexAnim = 0;

                timer = timerAnim[indexAnim];
            }


            if (_game.spriteBatch != null)
            {
                _game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                SpriteEffects decalageCeiling = _ceiling ? SpriteEffects.FlipVertically : SpriteEffects.None; 

                _game.spriteBatch.Draw(_sprite, _game.ToDrawPosition(_position, _levelOrigin), new Rectangle(indexAnim * (_sprite.Width / (nbAnim)), 0, (_sprite.Width / (nbAnim)), _sprite.Height), Color.White, (float)(GameCore.Angle), new Vector2(0, _sprite.Height), 1.0f, decalageCeiling, 0);                        
                _game.DrawHitBox(HitBox);
                
                _game.spriteBatch.End();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            if (TypeTrap == TrapType.PICKFIXE)// && indexAnim == 0)
                return true;
            else if ((TypeTrap == TrapType.PICKAR1 || TypeTrap == TrapType.PICKAR2 || TypeTrap == TrapType.PICKAR3) && indexAnim == 0)
                return true;
            
                
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private int getNbAnim(TrapType t)
        {
            if (t == TrapType.PICKFIXE)
                return 2;

            if (t == TrapType.PICKAR1)
                return 2;

            if (t == TrapType.PICKAR2)
                return 2;

            if (t == TrapType.PICKAR3)
                return 2;

            return 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        private void GetTimerAnim(TrapType t)
        {
            if (t == TrapType.PICKAR1)
            {
                timerAnim = new int[nbAnim];
                timerAnim[0] = 1000;
                timerAnim[1] = 1000;
            }

            else if (t == TrapType.PICKAR2)
            {
                timerAnim = new int[nbAnim];
                timerAnim[0] = 1000;
                timerAnim[1] = 1000;
            }

            else if (t == TrapType.PICKAR3)
            {
                timerAnim = new int[nbAnim];
                timerAnim[0] = 1000;
                timerAnim[1] = 1000;
            }

            else
            {
                timerAnim = new int[nbAnim];
                timerAnim[0] = 5000;
            }
        }

    }
}
