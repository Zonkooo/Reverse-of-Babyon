using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;


namespace RepeatAgain
{
    public class LevelManager
    {
        public int numPallier;
        public int nbHeight;
        public int nbWidth;
        public float sizeTexture;
        public bool ice;
        public float iceSpeed;

        public Texture2D[] tabAsset;

        private Texture2D pick;
        private Texture2D caisse;
        private Texture2D proj;
        private Texture2D button;
        private Texture2D bigBox;

        private List<Trap> listTrap;
        private Ground _ground;
        private Ground _ceiling;
        private Character _charac;
        private GameCore _game;
        private Random generator;
        private List<Door> listDoor;

        public Texture2D SpecialDisplay;
        

        public LevelManager(GameCore game, Ground ground, Ground ceiling, Character charac)
        {
            numPallier = -1;
            nbHeight = 6;
            nbWidth = 26;
            sizeTexture = 40;

            _ground = ground;
            _ceiling = ceiling;
            _charac = charac;
            _game = game;

            ice = false;
            iceSpeed = 0.05f;

            generator = new Random();

            listTrap = new List<Trap>();
            listDoor = new List<Door>();

        }

        public void LoadContent(GameCore game, ContentManager content)
        {
            //Image Load
            this.pick = content.Load<Texture2D>("Traps/picks_anim");
            this.caisse = content.Load<Texture2D>("Traps/tower_smallblock");
            this.proj = content.Load<Texture2D>("Traps/tower_firemouth");
            this.button = content.Load<Texture2D>("Traps/button_anim");
            this.bigBox = content.Load<Texture2D>("Traps/tower_bigblock");

            tabAsset = new Texture2D[15];
            tabAsset[0] = pick;
            tabAsset[1] = pick;
            tabAsset[2] = pick;
            tabAsset[3] = pick;
            tabAsset[4] = proj;
            tabAsset[5] = proj;
            tabAsset[6] = proj;
            tabAsset[7] = proj;
            tabAsset[8] = proj;
            tabAsset[9] = proj;
            tabAsset[10] = button;
            tabAsset[11] = button;
            tabAsset[12] = button;
            tabAsset[13] = caisse;
            tabAsset[14] = bigBox;

        }

        private List<string> LoadLevel(out bool end)
        {
            var lines = new List<string>();
            string file = string.Format("levels/level_{0:D2}.txt", numPallier);
            end = false;
            if (!File.Exists(file))
            {
                end = true;
                numPallier = 0;
                //return LoadLevel();
                return lines;
            }
            using (var reader = new StreamReader(file)) //if it crashes here, you have reached the end of the game :p
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    lines.Add(line);
                    line = reader.ReadLine();
                }
            }
            return lines;
        }

        private void Clean()
        {
            foreach (var trap in listTrap)
            {
                if (trap.TypeTrap == TrapType.PJ1 || trap.TypeTrap == TrapType.PJ2 || trap.TypeTrap == TrapType.PJ3 || trap.TypeTrap == TrapType.PJI1 || trap.TypeTrap == TrapType.PJI2 || trap.TypeTrap == TrapType.PJI3)
                    trap.cleanProjectile();
                trap.Dispose();
            }
            listTrap.Clear();
            foreach (var door in listDoor)
            {
                door.Dispose();
            }
            listDoor.Clear();
        }

        public void NextPallier(GameCore game)
        {
            var basePosition = _ground.Position;

            numPallier++;
            Clean();
            bool end = false;
            List<string> level = LoadLevel(out end);

            if (end)
            {
                _game.endGame = true;
                return;
            }

            if (numPallier == 0)
                SpecialDisplay = game.Content.Load<Texture2D>("guide01");
            else if (numPallier == 1)
                SpecialDisplay = game.Content.Load<Texture2D>("guide02");
            else
                SpecialDisplay = null;

            ice = false;

            //init actual Pallier
            for (int i = 0; i < nbHeight; i++)
            {
                string line = level[level.Count - i - 1];
                string[] elements = line.Split(new[] {' ', '\t', '\n'});

                int ind = 0;

                listTrap.Add(new Trap(game, game.OneWhitePixel, new Vector2(game.Ground.Position.X - 40, game.Ground.Position.Y - (i*40)), TrapType.BOX, _ground.Position, false, this));
                listTrap.Last().HitBox.Width = 40;
                listTrap.Last().HitBox.Height = 40;

                foreach (var trapString in elements)
                {
                    //ICE
                    if (trapString == "ICE")
                        ice = true;

                    //Caisse
                    else if (trapString == "C")
                    {
                        Vector2 pos = basePosition + new Vector2(ind * sizeTexture, -i * sizeTexture);

                        Texture2D blockText;

                        double randf = generator.NextDouble();

                        if (randf < 0.5)
                            blockText = this.tabAsset[(int)TrapType.BOX - 1];
                        else
                            blockText = game.Content.Load<Texture2D>("Traps/tower_smallblock2");

                        Trap t = new Trap(game, blockText, pos, TrapType.BOX, _ground.Position, false, this);

                        listTrap.Add(t);
                    }

                    else if (trapString == "BBOX")
                    {
                        Vector2 pos = basePosition + new Vector2(ind * sizeTexture, -i * sizeTexture);

                        Trap t = new Trap(_game, this.tabAsset[(int)TrapType.BBOX - 1], pos, TrapType.BBOX, _ground.Position, false, this);

                        listTrap.Add(t);
                    }

                    //Pick Fixe
                    else if (trapString == "PF" || trapString == "PFC")
                    {
                        Vector2 pos = basePosition + new Vector2(ind * sizeTexture, -i * sizeTexture);

                        bool ceiling = trapString == "PFC";

                        Trap t = new Trap(game, this.tabAsset[(int)TrapType.PICKFIXE - 1], pos, TrapType.PICKFIXE,
                                          _ground.Position, ceiling, this);

                        listTrap.Add(t);
                    }

                        //Pick AR1
                    else if (trapString == "PAR1" || trapString == "PAR1C")
                    {
                        bool ceiling = trapString == "PAR1C";

                        Vector2 pos = basePosition + new Vector2(ind * sizeTexture, -i * sizeTexture);
                        Trap t = new Trap(game, this.tabAsset[(int)TrapType.PICKAR1 - 1], pos, TrapType.PICKAR1,
                                          _ground.Position, ceiling, this);

                        listTrap.Add(t);
                    }

                        //Pick AR2
                    else if (trapString == "PAR2" || trapString == "PAR2C")
                    {
                        bool ceiling = trapString == "PAR2C";

                        Vector2 pos = basePosition + new Vector2(ind * sizeTexture, -i * sizeTexture);
                        Trap t = new Trap(game, this.tabAsset[(int)TrapType.PICKAR2 - 1], pos, TrapType.PICKAR2,
                                          _ground.Position, ceiling, this);

                        listTrap.Add(t);
                    }

                        //Pick AR1
                    else if (trapString == "PAR3" || trapString == "PAR3C")
                    {
                        bool ceiling = trapString == "PAR3C";

                        Vector2 pos = basePosition + new Vector2(ind * sizeTexture, -i * sizeTexture);
                        Trap t = new Trap(game, this.tabAsset[(int)TrapType.PICKAR3 - 1], pos, TrapType.PICKAR3,
                                          _ground.Position, ceiling, this);

                        listTrap.Add(t);
                    }

                        //Projectile 1
                    else if (trapString == "PJ1" || trapString == "PJ2" || trapString == "PJ3" || trapString == "PJI1" ||
                             trapString == "PJI2" || trapString == "PJI3")
                    {
                        Vector2 pos = basePosition + new Vector2(ind * sizeTexture, -i * sizeTexture);
                        TrapType typeTrap = TrapType.NONE;

                        if (trapString == "PJ1")
                            typeTrap = TrapType.PJ1;
                        else if (trapString == "PJ2")
                            typeTrap = TrapType.PJ2;
                        else if (trapString == "PJ3")
                            typeTrap = TrapType.PJ3;
                        else if (trapString == "PJI1")
                            typeTrap = TrapType.PJI1;
                        else if (trapString == "PJI2")
                            typeTrap = TrapType.PJI2;
                        else if (trapString == "PJI3")
                            typeTrap = TrapType.PJI3;

                        Trap t = new Trap(game, this.tabAsset[(int)typeTrap - 1], pos, typeTrap, _ground.Position,
                                          false, game.levelMgr);
                        listTrap.Add(t);
                    }
                    //-------INTERRUPTEUR DOOR
                    else if (trapString == "ID1" || trapString == "ID2" || trapString == "ID3")
                    {
                        Vector2 pos = basePosition + new Vector2(ind * sizeTexture, -i * sizeTexture);
                        TrapType typeTrap = TrapType.NONE;

                        if (trapString == "ID1")
                            typeTrap = TrapType.ID1;
                        else if (trapString == "ID2")
                            typeTrap = TrapType.ID2;
                        else if (trapString == "ID3")
                            typeTrap = TrapType.ID3;


                        Trap t = new Trap(game, this.tabAsset[(int)typeTrap - 1], pos, typeTrap, _ground.Position,
                                          false, game.levelMgr);
                        t.HitBox.Width /= 2;
                        listTrap.Add(t);
                    }

                        //-------DOOR-----
                    else if (trapString == "D1" || trapString == "D2" || trapString == "D3")
                    {
                        Vector2 pos = basePosition + new Vector2(ind * sizeTexture, -i * sizeTexture);
                        DoorType typeDoor = DoorType.NONE;

                        if (trapString == "D1")
                            typeDoor = DoorType.D1;
                        else if (trapString == "D2")
                            typeDoor = DoorType.D2;
                        else if (trapString == "D3")
                            typeDoor = DoorType.D3;

                        Door d = new Door(game, typeDoor, pos);
                        listDoor.Add(d);
                    }

                    ind++;
                }
            }
        }


        public bool CollidesWith(Rectangle perso)
        {
            return CollidesWith(perso, true);
        }
        public bool CollidesWith(Rectangle perso, bool withBullets)
        {
            if (perso.Intersects(_ground.HitBox))
                return true;
            if (perso.Intersects(_ceiling.HitBox))
                return true;


            //DOOR
            foreach (Door d in listDoor)
            {
                if (d.state == DoorState.CLOSE && perso.Intersects(d.hitBox))
                {
                    return true;
                }
            }


            foreach (Trap trap in listTrap)
            {
                if (trap.TypeTrap > TrapType.NONE 
                    && perso.Intersects(trap.HitBox))
                {
                    //-------BOX----------
                    if (trap.TypeTrap == TrapType.BOX)
                        return true;

                    else if (trap.TypeTrap == TrapType.BBOX)
                        return true;

                    //-----PICK-------
                    else if (trap.TypeTrap == TrapType.PICKFIXE)
                    {
                        _charac.dead();
                        _game.soundMgr.PlaySound(_game.soundMgr.pickDeath);
                        return false;
                    }

                    else if ((trap.TypeTrap == TrapType.PICKAR1 || trap.TypeTrap == TrapType.PICKAR2 || trap.TypeTrap == TrapType.PICKAR3) && trap.IsActive())
                    {
                        _charac.dead();
                        _game.soundMgr.PlaySound(_game.soundMgr.pickDeath);
                        return false;
                    }


                }

            
                //-----bullets-------
                else if (withBullets && (trap.TypeTrap == TrapType.PJ1 || trap.TypeTrap == TrapType.PJ2 ||trap.TypeTrap == TrapType.PJ3 || trap.TypeTrap == TrapType.PJI1 || trap.TypeTrap == TrapType.PJI2 || trap.TypeTrap == TrapType.PJI3))
                {
                    foreach (var bullet in trap.ListProj)
                    {
                        if (perso.Intersects(bullet.HitBox))
                        {
                            _game.soundMgr.PlaySound(_game.soundMgr.fireDeath);
                            _charac.dead();
                            trap.DeleteProjectile(bullet);
                            return false;
                        }
                    }
                }
            }
            return false;
        }


        public void openDoor(DoorType typeDoor)
        {
            foreach (Door d in listDoor)
            {
                if (d.typeDoor == typeDoor)
                    d.openDoor();
            }
        }

        public void closeDoor(DoorType typeDoor)
        {
            foreach (Door d in listDoor)
            {
                if (d.typeDoor == typeDoor)
                    d.closeDoor();
            }
        }


        
    }
}
