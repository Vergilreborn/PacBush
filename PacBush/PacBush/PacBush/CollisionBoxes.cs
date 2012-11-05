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
    class CollisionBoxes
    {

        Texture2D wallsSheet,moneySheet;

        public List<Rectangle> walls = new List<Rectangle>();
        public List<Rectangle> money = new List<Rectangle>();
        
        public CollisionBoxes(Texture2D wallsSheet, Texture2D money)
        {
            this.wallsSheet = wallsSheet;
            this.moneySheet = money;
        }

        public void addWall(Vector2 position)
        {
            walls.Add(new Rectangle((int)position.X, (int)position.Y, wallsSheet.Width, wallsSheet.Height));
        }

        public void addMoney(Vector2 position)
        {
            money.Add(new Rectangle((int)position.X, (int)position.Y, moneySheet.Width, moneySheet.Height));
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Rectangle wall in walls)
            {
                spriteBatch.Draw(wallsSheet, wall, Color.White);
            }
            foreach (Rectangle dollar in money)
            {
                spriteBatch.Draw(moneySheet, dollar, Color.White);
            }
        }

    }
}
