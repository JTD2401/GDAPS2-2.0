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
        bool wallHitLeft;
        bool wallHitRight;
        bool wallHitUp;
        bool wallHitDown;
        Random rng;
        Vector2 direction;
        Texture2D stoneTex;
        Circle circle;
        bool rapidFire;
        bool inverter;

        public Stone(Texture2D t2, int x, int y, int radius) : base(t2, x, y)
        {
            circle = new Circle(x, y, radius);
            Thread.Sleep(30);
            rng = new Random();
            var dir = rng.Next(1, 9);
            if (dir == 1)
                direction = new Vector2(3, 0);
            if (dir == 2)
                direction = new Vector2(-3, 0);
            if (dir == 3)
                direction = new Vector2(0, 3);
            if (dir == 4)
                direction = new Vector2(0, -3);
            if (dir == 5)
                direction = new Vector2(3, 3);
            if (dir == 6)
                Direction = new Vector2(-3, -3);
            if (dir == 7)
                Direction = new Vector2(3, -3);
            if (dir == 8)
                Direction = new Vector2(-3, 3);
            stoneTex = t2;
            wallHitLeft = false;
            wallHitRight = false;
            wallHitUp = false;
            wallHitDown = false;
        }

        public bool WallHitLeft
        {
            get { return wallHitLeft; }
            set { wallHitLeft = value; }
        }

        public bool WallHitRight
        {
            get { return wallHitRight; }
            set { wallHitRight = value; }
        }

        public bool WallHitUp
        {
            get { return wallHitUp; }
            set { wallHitUp = value; }
        }

        public bool WallHitDown
        {
            get { return wallHitDown; }
            set { wallHitDown = value; }
        }

        public Circle Circle
        {
            get { return circle; }
        }

        public bool RapidFire
        {
            get { return rapidFire; }
            set { rapidFire = value; }
        }

        public bool Inverter
        {
            get { return inverter; }
            set { inverter = value; }
        }

        public Texture2D StoneTex
        {
            get { return stoneTex; }
            set { stoneTex = value; }
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
            set { direction = value; }
        }

    }
}