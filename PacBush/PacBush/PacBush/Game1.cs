using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace PacBush
{

    enum GameState
    {
        Intro,
        Gameplay,
        Dead,
        Paused,
        Win

    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        
        //Drawing the sprites
        SpriteBatch spriteBatch;

        //pacman 
        PacMan pacMan;
        
        //Texture2D unicorns
        Texture2D unicornSheet;
        Texture2D introScreen,thankyou;
        String created, pressEnter;

        //keyboards states
        KeyboardState curr, prev;
        int currentLevel;

        //collision boxes of the walls
        CollisionBoxes wallBox;
        //debugging
        SpriteFont font;
        
        GameState state;
        bool dead;
       
        //lives
        int lives = 3;
        int maxLevels = 3;

        //Sound effect
        SoundEffect coinSound;
       
        //int Score
        int score = 0;

        //Random 
        Random r;
        int timeSeconds;
        List<Enemy> unicorns = new List<Enemy>();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 675;
            graphics.PreferredBackBufferWidth = 1100;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            prev = curr = Keyboard.GetState();
            currentLevel = 0;
            r = new Random();
            dead = false;
            state = GameState.Intro;

            created = "Programmed By Alexander Martin";
            pressEnter = "Press Enter Button to Start";

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(Content.Load<Song>("EpicsaxGuy"));
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Font");
            pacMan = new PacMan(Content.Load<Texture2D>("Characters/bushHead"), new Vector2(300,300));
            unicornSheet = Content.Load<Texture2D>("Characters/unicornSprites");
            introScreen = Content.Load<Texture2D>("IntroScreen");
            thankyou = Content.Load<Texture2D>("Thankyou");
            wallBox = new CollisionBoxes(Content.Load<Texture2D>("wall"), Content.Load<Texture2D>("money"));
            coinSound = Content.Load<SoundEffect>("chomp");
            readFile(0);

            // TODO: use this.Content to load your game content here
        }

        public void readFile(int level)
        {

            unicorns.Clear();
            wallBox.walls.Clear();
            wallBox.money.Clear();

            if (!File.Exists("Content/level" + level + ".pac"))
                return;
            StreamReader reader = new StreamReader("Content/level" + level + ".pac");
            String data = "";
            String[] array;
            int unicornNum = 0;
            while (reader.Peek() != -1)
            {
                data = reader.ReadLine();
                array = data.Split(',');
                int onePos =  int.Parse(array[1]);
                int twoPos = int.Parse(array[2]);
                if (array[0].Equals("h"))
                {
                    pacMan.dest.X = onePos;
                    pacMan.dest.Y = twoPos;
                    pacMan.setReset(onePos, twoPos);
                    continue;
                }
                else if (array[0].Equals("w"))
                    wallBox.addWall(new Vector2(onePos, twoPos));
                else if (array[0].Equals("m"))
                    wallBox.addMoney(new Vector2(onePos, twoPos));
                else if (array[0].Equals("u")){
                    unicorns.Add(new Enemy(unicornSheet,new Vector2(onePos,twoPos),unicornNum));
                    unicornNum++;
                }
            }
            reader.Close();

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            timeSeconds = DateTime.Now.Millisecond + gameTime.ElapsedGameTime.Milliseconds;

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            curr = Keyboard.GetState();

            switch(state){
                    //Intro
                case GameState.Intro:
                    if (curr.IsKeyDown(Keys.Enter) && prev.IsKeyUp(Keys.Enter))
                    {
                        readFile(0);
                        state = GameState.Gameplay;

                    }
                    break;

                    //GAMEPLAy
                case GameState.Gameplay:

                    if (curr.IsKeyDown(Keys.P) && prev.IsKeyUp(Keys.P))
                        state = GameState.Paused;
                    
                    foreach (Enemy uni in unicorns)
                    {   
                        uni.UpdateAI(gameTime, r, timeSeconds,wallBox);
                        if (uni.destRect.Intersects(pacMan.dest))
                        {
                            lives--;
                            pacMan.reset();
                            foreach (Enemy un in unicorns)
                                un.reset();
                            if (lives == 0)
                            {
                                dead = true;
                                state = GameState.Dead;
                            }
                        }
                        
                    }
        
                    for (int i = 0; i < wallBox.money.Count; i++ )
                    {
                        Rectangle moneyBox = wallBox.money[i];
                        if (pacMan.dest.Intersects(moneyBox))
                        {
                            wallBox.money.RemoveAt(i);
                            
                            coinSound.Play();
                            i--;
                            score += 100;
                        }
                    }

                    if (wallBox.money.Count == 0)
                    {
                        currentLevel++;
                        readFile(currentLevel);
                    }

                    if (currentLevel == maxLevels)
                        state = GameState.Win;
                        

                    if(!dead)
                        pacMan.Update(gameTime, prev, curr, wallBox);
                    break;

                case GameState.Paused:
                    if (curr.IsKeyDown(Keys.P) && prev.IsKeyUp(Keys.P))
                        state = GameState.Gameplay;
                    
                    break;
                    //END GAMEPLAY

                case GameState.Dead:
                    if (curr.IsKeyDown(Keys.Enter) && prev.IsKeyUp(Keys.Enter))
                    {
                        state = GameState.Intro;
                        lives = 3;
                    }
                    
                    break;
                    
                case GameState.Win:
                    if (curr.IsKeyDown(Keys.Enter) && prev.IsKeyUp(Keys.Enter))
                    {
                        state = GameState.Intro;
                        lives = 3;
                    }
                  

                    break;
                }
            
            prev = curr;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
         
            switch(state){
                case GameState.Intro:
                    spriteBatch.Draw(introScreen, new Vector2(250, 100), Color.White);
                    spriteBatch.DrawString(font, pressEnter, new Vector2(385, 350), Color.Red);
                    spriteBatch.DrawString(font, created, new Vector2(380, 600), Color.Yellow);

                    break;

                case GameState.Paused:

                    wallBox.Draw(spriteBatch);


                foreach (Enemy uni in unicorns)
                {
                 uni.Draw(spriteBatch,font);
                
                }

                if(!dead)
                 pacMan.Draw(gameTime, spriteBatch);
         
                spriteBatch.DrawString(font, "SCORE\n" + score, new Vector2(900,0), Color.White);
                    pacMan.DrawLives(spriteBatch, lives);
                    spriteBatch.DrawString(font,"Level:" + (currentLevel+1), new Vector2(900,100),Color.White);
                    spriteBatch.DrawString(font, "THE GAME HAS BEEN PAUSED", new Vector2(300, 400), Color.Red);
                
                    break;
                case GameState.Gameplay:
                wallBox.Draw(spriteBatch);


                foreach (Enemy uni in unicorns)
                {
                 uni.Draw(spriteBatch,font);
                
                }

                if(!dead)
                 pacMan.Draw(gameTime, spriteBatch);
                spriteBatch.DrawString(font, "Level:" + (currentLevel + 1), new Vector2(900, 100), Color.White);
                pacMan.DrawLives(spriteBatch, lives);
                spriteBatch.DrawString(font, "SCORE\n" + score, new Vector2(900,0), Color.White);
                break;

                case GameState.Dead:
                    spriteBatch.Draw(introScreen, new Vector2(250, 100), Color.White);
                    spriteBatch.DrawString(font, pressEnter, new Vector2(385, 350), Color.Red);
                    spriteBatch.DrawString(font, "Your Score: " + score, new Vector2(385, 450), Color.LightBlue);
                    spriteBatch.DrawString(font, "Thank you for test driving!", new Vector2(385, 550), Color.LightBlue);

                    break;
                case GameState.Win:
                    spriteBatch.Draw(introScreen, new Vector2(250, 100), Color.White);
                    spriteBatch.DrawString(font, "Press Enter to Reset", new Vector2(385, 350), Color.LightBlue);
                   
                    spriteBatch.Draw(thankyou, new Vector2(250, 400), Color.White);
              
                    break;
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
