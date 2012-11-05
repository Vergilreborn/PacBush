using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace PacBush
{
    class PacMan
    {


        //Const variables non seceptible to change
        Vector2 movement;
        const int speed = 2; 
        
        //get width and height
        int width;
        int height;
        float rotate;
        bool flip;
        //Collision box, spriteSheet, 
        public Rectangle dest;
        Rectangle source;
        
        //Timer
        float timer = 0f;
        float maxTimer = 100f;
        int frames = 4;
        int current = 0;

        //reset
        int resetX;
        int resetY;

        //sprite sheet
        Texture2D spriteSheet;

        public PacMan(Texture2D spriteSheet, Vector2 pos)
        {
            this.spriteSheet = spriteSheet;
            //width and heigh of sprite
            height = spriteSheet.Height;
            width = spriteSheet.Width / 4;
            //setting up the boxes
            source = new Rectangle(0, 0, width, height);
            dest = new Rectangle((int)pos.X, (int)pos.Y, width, height);
        }
        public void Update(GameTime gameTime, KeyboardState prev, KeyboardState curr, CollisionBoxes coll)
        {

            timer += gameTime.ElapsedGameTime.Milliseconds;

            if (timer > maxTimer)
            {
                timer = 0f;
                current++;
                if (current >= frames)
                {
                    current = 0;
                }
                source.X = current * width;
            }


            //movement 
            if (prev.IsKeyUp(Keys.Space) && curr.IsKeyDown(Keys.Space))
                movement = Vector2.Zero;
            else if(prev.IsKeyUp(Keys.Up) && curr.IsKeyDown(Keys.Up)){
                movement.Y = -speed;
                movement.X = 0;
                flip = true;
                rotate = 90.0f;
            }
            else if (prev.IsKeyUp(Keys.Down) && curr.IsKeyDown(Keys.Down))
            {
                movement.Y = speed;
                movement.X = 0;
                flip = false;
                rotate = 90.0f;
            }
            else if (prev.IsKeyUp(Keys.Right) && curr.IsKeyDown(Keys.Right))
            {
                movement.X = speed;
                movement.Y = 0;
                rotate = 0.0f;
                flip = false;
            }
            else if (prev.IsKeyUp(Keys.Left) && curr.IsKeyDown(Keys.Left))
            {
                movement.X = -speed;
                movement.Y = 0;
                rotate = 0.0f;
                flip = true;
                
            }
            
            //collision variables
            bool collision = false;
            Rectangle possibleMovement = new Rectangle(dest.X + (int)movement.X, dest.Y + (int)movement.Y, dest.Width,dest.Height);
            //check to see if collision stuff is happening
            foreach (Rectangle wall in coll.walls)
            {
                if (wall.Intersects(possibleMovement))
                {
                    collision = true;
                    break;
                }
            }

            //if there is not collision don't move
            if (!collision)
                dest = possibleMovement;
        }

        public void setReset(int x, int y)
        {
            resetX = x;
            resetY = y;
        }

        public void reset()
        {
            dest.X = resetX;
            dest.Y = resetY;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Draws the "pacman"
            Rectangle center = new Rectangle((int)(dest.X + width / 2),(int)( dest.Y + height / 2),width,height);
            if (flip)
            {
             //   spriteBatch.Draw(spriteSheet, dest, source, Color.White);
                spriteBatch.Draw(spriteSheet, center, source, Color.White, rotate, new Vector2(width/2,height/2), SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.Draw(spriteSheet, center, source, Color.White, rotate, new Vector2(width/2, height/2), SpriteEffects.FlipHorizontally, 0);
        
            }
        }

        public void DrawLives(SpriteBatch spriteBatch, int lives)
        {
            
            Rectangle head = new Rectangle(0,0,width,height);
            for (int i = 0; i < lives; i++)
            {
                Rectangle location = new Rectangle(900+ (40*i),500,width,height);
                spriteBatch.Draw(spriteSheet, location, head, Color.White);
            }
        }

    }
}
