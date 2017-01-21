using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Text;

namespace RepeatAgain
{
    public class SoundManager : GameComponent
    {
        private GameCore _game;

        
        private Song musicGame;
        public SoundEffect pickIn;
        public SoundEffect pickOut;
        public SoundEffect pickDeath;
        public SoundEffect fireDeath;
        public SoundEffect fireEnd;
        public SoundEffect fireOut;
        public SoundEffect door;

        public Dictionary<SoundEffect, bool> isPlaying;
        private Dictionary<SoundEffect, int> timers;
        

        public SoundManager(GameCore game)
            :base(game)
        {
            _game = game;
            _game.Components.Add(this);
            MediaPlayer.IsRepeating = true;

            musicGame = _game.Content.Load<Song>("Sound/RA-MainGame");
            pickIn = _game.Content.Load<SoundEffect>("Sound/pic in");
            pickOut = _game.Content.Load<SoundEffect>("Sound/pic out");
            pickDeath = _game.Content.Load<SoundEffect>("Sound/pic_death");
            door = _game.Content.Load<SoundEffect>("Sound/Door");
            fireDeath = _game.Content.Load<SoundEffect>("Sound/Fire_Death");
            fireEnd = _game.Content.Load<SoundEffect>("Sound/Fire_end");
            fireOut = _game.Content.Load<SoundEffect>("Sound/Fire_out");
            //musicGame = _game.Content.Load<SoundEffect>("Sound/Fire_Death");
            //musicGameInstance = musicGame.CreateInstance();

            isPlaying = new Dictionary<SoundEffect, bool>()
                            {
                                {pickIn, false},
                                {pickOut, false},
                                {pickDeath, false},
                                {fireDeath, false},
                                {fireEnd, false},
                                {fireOut, false},
                                {door, false},
                            };
            timers = new Dictionary<SoundEffect, int>()
                            {
                                {pickIn, 0},
                                {pickOut, 0},
                                {pickDeath, 0},
                                {fireDeath, 0},
                                {fireEnd, 0},
                                {fireOut, 0},
                                {door, 0},
                            };
        }

        public override void Update(GameTime gameTime)
        {
            for(int i = 0; i < timers.Count; ++i)
            {
                //caca
                KeyValuePair<SoundEffect, int> timer = timers.ElementAt(i);
                if (timer.Value > 0)
                {
                    timers[timer.Key] -= gameTime.ElapsedGameTime.Milliseconds;
                }
                else
                {
                    isPlaying[timer.Key] = false;
                }
            }
            base.Update(gameTime);
        }
        
        public void playMenuMusic()
        {
            //MediaPlayer.Play(musicMenu);

        }

        public void playGameMusic()
        {
            
            MediaPlayer.Play(musicGame);
            MediaPlayer.Volume = 0.5f;
            //MediaPlayer.Play(musicWind); 
        }

        public void stopMusic()
        {
            MediaPlayer.Stop();
        }

        public void PlaySound(SoundEffect sfx)
        {
            if (isPlaying[sfx])
                return;

            isPlaying[sfx] = true;
            timers[sfx] = 50;
            sfx.Play();
        }
        
    }
}
