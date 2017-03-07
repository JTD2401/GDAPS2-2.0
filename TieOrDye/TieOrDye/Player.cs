using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/*
TieOrDye Player Class
*/

namespace TieOrDye
{
    class Player : GameObject
    {
        //Game1 object
        Game1 gm1 = new Game1();

        //Attributes
        int currentStones; //int for stones that are the current player's color
        Rectangle playerRect; //player rectangle       
        bool immune; //Immunity boolean
        int pWidth; //Player width
        int pHeight; //Player height

        //player constructor
        public Player(Texture2D t2, int x, int y, int pw, int ph) :base(t2, x, y)
        {
            pWidth = pw;
            pHeight = ph;
            playerRect = new Rectangle(x, y, pw, ph);
        }

        //Player rectangle property
        public Rectangle PlayerRect
        {
            get { return playerRect; }
            set { playerRect = value; }
        }

        public int X
        {
            get { return playerRect.X; }
            set { playerRect.X = value; }
        }

        public int Y
        {
            get { return playerRect.Y; }
            set { playerRect.Y = value; }
        }

        //Determine what happens when orb hits a player
        public void OnHit()
        {

        }
    }
}
