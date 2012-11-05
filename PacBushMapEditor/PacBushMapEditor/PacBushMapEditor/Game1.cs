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

namespace PacBushMapEditor
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Tile[] tiles;
        int numOfHead;
        int unicorn;
        int level;
        KeyboardState prev, curr;
        Texture2D money, head, wall, unicornSheet;
        MouseState mouse;
        String data;
        SpriteFont font;
        public Game1()
        {
            
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 675;
            graphics.PreferredBackBufferWidth = 900;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            numOfHead = 0;
            unicorn = 0;
            level = 0;
            tiles = new Tile[15 * 20];

            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    tiles[i * 15 + j] = new Tile(i, j, 'n');
                }
            }

            data = "";

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            money = Content.Load<Texture2D>("Money");
            head = Content.Load<Texture2D>("bushHeadSpriteSheet");
            wall = Content.Load<Texture2D>("wall");
            font = Content.Load<SpriteFont>("SpriteFont1");
            unicornSheet = Content.Load<Texture2D>("unicorn");
            // TODO: use this.Content to load your game content here
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
            curr = Keyboard.GetState();
            mouse = Mouse.GetState();

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            Vector2 position = new Vector2(mouse.X, mouse.Y);
            data = mouse.X + " , " + mouse.Y;
            
            int ti= getTile(position);
            if(ti != -1)
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    tiles[ti].type = 'w';
                }
                else if (mouse.RightButton == ButtonState.Pressed)
                {
                    if (tiles[ti].type == 'h')
                        numOfHead = 0;
                    if (tiles[ti].type == 'u')
                        unicorn--;
                    tiles[ti].type = 'n';

                }else if(curr.IsKeyDown(Keys.A)){
                    if (numOfHead == 0)
                    {
                        tiles[ti].type = 'h';
                        numOfHead = 1;
                    }
                }
                else if (curr.IsKeyDown(Keys.D))
                {
                    if(tiles[ti].type != 'u')
                        if (unicorn < 4)
                        {
                            unicorn++;
                            tiles[ti].type = 'u';
                        }
                    

                }
                else if (curr.IsKeyDown(Keys.S))
                {
                    tiles[ti].type = 'm';
                }
                else if (curr.IsKeyDown(Keys.Q) && prev.IsKeyUp(Keys.Q))
                {
                    if(File.Exists("level" + level + ".pac"))   
                        loadMap(level);
                }
                else if (curr.IsKeyDown(Keys.W) && prev.IsKeyUp(Keys.W))
                {
                         saveMap(level);

                }
                if (curr.IsKeyDown(Keys.Z) && prev.IsKeyUp(Keys.Z))
                    level++;

                else if (curr.IsKeyDown(Keys.X) && prev.IsKeyUp(Keys.X))
                    level--;
                // TODO: Add your update logic here
                prev = curr;
            base.Update(gameTime);
        }

        public void loadMap(int level)
        {
            for (int i = 0; i < tiles.Length; i++)
                  tiles[i].type = 'n';
            StreamReader reader = new StreamReader("level" + level + ".pac");
            String line = "";
            String[] data;
            int t = 0;
            while (reader.Peek() != -1)
            {
                line = reader.ReadLine();
                data = line.Split(',');
                for (int i = 0; i < data.Length; i++)
                {
                    tiles[t].type = data[0].ToCharArray()[0];
                }
                t++;
            }
            reader.Close();


        }

        public void saveMap(int level)
        {
            StreamWriter write = new StreamWriter("level" + level + ".pac");
            for (int i = 0; i < tiles.Length; i++)
            {
                write.WriteLine(tiles[i].type + "," + tiles[i].dest.X + "," + tiles[i].dest.Y);

            }
            write.Flush();
            write.Close();
        }

        public int getTile(Vector2 position)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                if (inRect(position,i))
                    return i;
            }
            return -1;

        }

        public bool inRect(Vector2 position, int i)
        {
            Rectangle dest = tiles[i].dest;
            return (dest.X < position.X && position.X < (dest.X + dest.Width)) &&
               (dest.Y < position.Y && position.Y < (dest.Y + dest.Height));
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            foreach (Tile t in tiles)
            {
                if (t.type == 'w')
                    spriteBatch.Draw(wall, t.dest, Color.White);
                else if (t.type == 'h')
                    spriteBatch.Draw(head, t.dest, new Rectangle(0, 0, 40, 40), Color.White);
                else if (t.type == 'm')
                    spriteBatch.Draw(money, t.dest, Color.White);
                else if (t.type == 'u')
                    spriteBatch.Draw(unicornSheet, t.dest, Color.White);
            }

            spriteBatch.DrawString(font, data + "\nFile:level" + level + ".pac" , new Vector2(0, 0), Color.Red);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
