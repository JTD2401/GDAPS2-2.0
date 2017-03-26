using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/*
TieOrDye Stone Class
*/

namespace TieOrDye
{
    class Stone : GameObject
    {
        //attributes
        Random rng;
        Vector2 direction;
        Texture2D stoneTex;
        Circle circle;

        public Stone(Texture2D t2, int x, int y, int radius) : base(t2, x, y)
        {
            circle = new Circle(x, y, radius);
            Thread.Sleep(30);
            rng = new Random();
            var dir = rng.Next(1,5);
            if(dir == 1)
                direction = new Vector2(2, 0);
            if (dir == 2)
                direction = new Vector2(-2, 0);
            if (dir == 3)
                direction = new Vector2(0, 2);
            if (dir == 4)
                direction = new Vector2(0, -2);
            stoneTex = t2;
        }

        public int XPos
        {
            get { return circle.X; }
            set { circle.X = value; }
        }

        public int YPos
        {
            get { return circle.Y; }
            set { circle.Y = value; }
        }

        public Vector2 Direction
        {
            get { return direction; }
        }
        
    }
}
