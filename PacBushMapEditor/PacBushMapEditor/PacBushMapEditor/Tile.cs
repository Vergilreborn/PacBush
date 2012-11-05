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

namespace PacBushMapEditor
{
    public class Tile
    {

        public Rectangle dest;
        public int x;
        public int y;
        public char type;

        public Tile(int x, int y, char type)
        {

            dest = new Rectangle(x*45, y*45, 45, 45);
            this.x = x;
            this.y = y;
            this.type = type;
        }
    }
}
