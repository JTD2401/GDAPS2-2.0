using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TieOrDye
{
    class Wall : GameObject
    {
        private Texture2D t2;
        private Rectangle wallRect;

        public Wall(Texture2D t2, int x, int y, int width, int height) : base(t2, x, y)
        {
            this.t2 = t2;
            wallRect = new Rectangle(x, y, width, height);
        }

        public Texture2D Texture
        {
            get { return t2; }
        }

        public Rectangle Bounds
        {
            get { return wallRect; }
            set { wallRect = value; }
        }
    }
}
