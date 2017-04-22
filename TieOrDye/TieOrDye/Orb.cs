using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
/*
TieOrDye Orb Class
*/

namespace TieOrDye
{
    class Orb : GameObject
    {
        //attributes
        Texture2D orbTex;
        Circle orbCirc;

        int orbSpeed;
        int orbWidth;

        int orbX;
        int orbY;

        bool travelToMaxDistance;
        int MAX_DISTANCE = 550;

        bool orbBounce;

        int initialPlayerXPos;
        int initialPlayerYPos;

        enum orbDir { Up, Down, Left, Right, UpLeft, UpRight, DownLeft, Downright };
        orbDir dir;

        public Orb(Texture2D t2, int x, int y, Player p, Animation anim, int oWidth, int oSpeed) : base(t2, x, y)
        {
            //Set texture
            orbTex = t2;

            //Default orb location if switch statement can't change location
            orbX = x;
            orbY = y;

            //Initial player position
            initialPlayerXPos = p.X;
            initialPlayerYPos = p.Y;

            orbWidth = oWidth;
            orbSpeed = oSpeed;

            orbCirc = new Circle(x, y, oWidth / 2);

            travelToMaxDistance = false;
            orbBounce = false;


            //Place the orb in different locations depending on direction of player when shot
            //Also sets orb direction 
            switch (anim.Look)
            {
                case Animation.PlayerState.Up: //Place orb centered above player (-1 Pixel)
                    orbX = p.X + (p.PlayerRect.Width / 2) - (orbWidth / 2);
                    orbY = p.Y - orbWidth - 1;
                    dir = orbDir.Up;
                    break;
                case Animation.PlayerState.Down: //Place orb centered beneath player (+ 1 Pixel)
                    orbX = p.X + (p.PlayerRect.Width / 2) - (orbWidth / 2);
                    orbY = p.Y + p.PlayerRect.Height + 1;
                    dir = orbDir.Down;
                    break;
                case Animation.PlayerState.Left: //Place orb at center left of player (-1 Pixel)
                    orbX = p.X - orbWidth - 1;
                    orbY = p.Y + (p.PlayerRect.Height / 2) - (orbWidth / 2);
                    dir = orbDir.Left;
                    break;
                case Animation.PlayerState.Right:    //Place orb at center right of player (+1 Pixel)
                    orbX = p.X + p.PlayerRect.Width + 1;
                    orbY = p.Y + (p.PlayerRect.Height / 2) - (orbWidth / 2);
                    dir = orbDir.Right;
                    break;
                case Animation.PlayerState.UpLeft: //Place orb above top left corner of player
                    orbX = p.X - orbWidth - 1;
                    orbY = p.Y - orbWidth - 1;
                    dir = orbDir.UpLeft;
                    break;
                case Animation.PlayerState.UpRight: //Place orb above top right corner of player
                    orbX = p.X + p.PlayerRect.Width + 1;
                    orbY = p.Y - orbWidth - 1;
                    dir = orbDir.UpRight;
                    break;
                case Animation.PlayerState.DownLeft: //Place orb below bottom left corner of player
                    orbX = p.X - orbWidth - 1;
                    orbY = p.Y + p.PlayerRect.Height + 1;
                    dir = orbDir.DownLeft;
                    break;
                case Animation.PlayerState.Downright: //Place orb below bottom right corner of player
                    orbX = p.X + p.PlayerRect.Width + 1;
                    orbY = p.Y + p.PlayerRect.Height + 1;
                    dir = orbDir.Downright;
                    break;
                default:
                    break;
            }

            //Set orb properties
            X = orbX;
            Y = orbY;
        }


        public void UpdateOrbs()
        {
            //Check for collision  and change direction

            //Check for maximum distance traveled
            double theXValue = Math.Pow(X - initialPlayerXPos, 2);
            double theYValue = Math.Pow(Y - initialPlayerYPos, 2);

            if (Math.Sqrt(theXValue + theYValue) >= MAX_DISTANCE)
            {
                travelToMaxDistance = true;
            }

            //Update orb position
            switch (dir)
            {
                case orbDir.Up:
                    Y -= orbSpeed;
                    break;
                case orbDir.Down:
                    Y += orbSpeed;
                    break;
                case orbDir.Left:
                    X -= orbSpeed;
                    break;
                case orbDir.Right:
                    X += orbSpeed;
                    break;
                case orbDir.UpLeft:
                    X -= orbSpeed;
                    Y -= orbSpeed;
                    break;
                case orbDir.UpRight:
                    X += orbSpeed;
                    Y -= orbSpeed;
                    break;
                case orbDir.DownLeft:
                    X -= orbSpeed;
                    Y += orbSpeed;
                    break;
                case orbDir.Downright:
                    X += orbSpeed;
                    Y += orbSpeed;
                    break;
            }
        }

        public void DrawOrbs(SpriteBatch sb)
        {
                sb.Draw(orbTex, new Rectangle((int)X, (int)Y, orbWidth, orbWidth), Color.White);
        }

        public Texture2D OrbTex
        {
            get { return orbTex; }
        }

        public int OrbSpeed
        {
            get { return orbSpeed; }
            set { orbSpeed = value; }
        }



        public Circle OrbCirc
        {
            get { return orbCirc; }
        }

        public bool TravelToMaxDistance
        {
            get
            {  return travelToMaxDistance; }
        }


    }
}
