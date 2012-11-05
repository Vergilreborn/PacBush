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
    class Enemy
    {

        Texture2D enemySheet;
        Vector2 startResetPos;
        
        public Rectangle destRect, sourceRectangle;

        int unicornNum;
        
        int width, height;
        float timer = 0;
        float maxTimer = 100f;
        int frames = 3;
        int current = 0;
        int increment = 1;
        int speed = 2;
        int currentDirection = 1;
        int currentMove = 0;
        
        public Enemy(Texture2D enemySheet, Vector2 startPos, int unicornNum)
        {
            this.enemySheet = enemySheet;
            startResetPos = startPos;
            width = height = 33;
            destRect = new Rectangle((int)startPos.X, (int) startPos.Y, 40,40);
            sourceRectangle = new Rectangle((int)unicornNum * 99, (int)0, width, height);
            this.unicornNum = unicornNum;
        }
        public void reset()
        {
            destRect.X = (int)startResetPos.X;
            destRect.Y = (int)startResetPos.Y;
            currentMove = 0;

        }

        public void UpdateAI(GameTime gameTime, Random random, int time, CollisionBoxes box)
        {
            //0 = down
            //1 = left
            //2 = right
            //3 = up
            bool[] collision = getOuterWalls(box.walls);


            if (currentMove > 1)
            {
                    currentMove -= speed;
                    switch (currentDirection)
                    {
                        case 0: destRect.Y += speed; break;
                        case 1: destRect.X -= speed; break;
                        case 2: destRect.X += speed; break;
                        case 3: destRect.Y -= speed; break;
                    }
                
            }else if( currentMove == 1 && !collision[currentDirection]){
                 currentMove -= 1;
                switch (currentDirection)
                {
                    case 0: destRect.Y += 1; break;
                    case 1: destRect.X -= 1; break;
                    case 2: destRect.X += 1; break;
                    case 3: destRect.Y -= 1; break;
                }
            }
            else
            {
                if (currentMove == 1)
                {
                    switch (currentDirection)
                    {
                        case 0: destRect.Y += 1; break;
                        case 1: destRect.X -= 1; break;
                        case 2: destRect.X += 1; break;
                        case 3: destRect.Y -= 1; break;
                    }
                    collision = getOuterWalls(box.walls);
                }
                int newDir= (time + gameTime.ElapsedGameTime.Milliseconds +  random.Next()) % 4;
                for (int i = 0; i < 3; i++)
                {
                    newDir = (newDir+1) % 4;
                    if (!collision[newDir])
                        break;
                }

           
                currentDirection = newDir;
                currentMove = 45;
            }
            animate(gameTime, currentDirection);

            time += random.Next();


            


        }

        public bool [] getOuterWalls(List<Rectangle> boxes)
        {

            bool[] dir = new bool[4];
            Rectangle leftBox;
            Rectangle rightBox;
            Rectangle upbox;
            Rectangle downBox;

            leftBox = new Rectangle(destRect.X - 5,destRect.Y, 45,45);
            rightBox = new Rectangle(destRect.X + 5, destRect.Y, 45, 45);
            upbox = new Rectangle(destRect.X, destRect.Y - 5, 45, 45);
            downBox = new Rectangle(destRect.X, destRect.Y + 5, 45, 45);

            for (int i = 0; i < boxes.Count; i++)
            {
               if(leftBox.Intersects(boxes[i]))
                    dir[1] = true;
               if(rightBox.Intersects(boxes[i]))
                    dir[2] = true;
               if(upbox.Intersects(boxes[i]))
                    dir[3] = true;
               if(downBox.Intersects(boxes[i]))
                    dir[0] = true;
            }
            return dir;

        }

        public void animate(GameTime gameTime, int direction)
        {

            timer += gameTime.ElapsedGameTime.Milliseconds;

            if (timer > maxTimer)
            {
                timer = 0f;
                current += increment;
                if (current >= frames || current < 0)
                {
                    increment *= -1;
                    current += increment;

                }
               
                sourceRectangle.X = (unicornNum * 99) + (current * width);
                sourceRectangle.Y = height * direction;
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {

            spriteBatch.Draw(enemySheet, destRect, sourceRectangle, Color.White);
        }
    }
}
