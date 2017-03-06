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
        Vector2 playerPos; //player position
        Rectangle playerRect; //player rectangle       
        bool immune; //Immunity boolean
        int pWidth; //Player width
        int pHeight; //Player height

        //player constructor
        public Player(Texture2D t2D, Vector2 v2, int pw, int ph) :base(t2D, v2)
        {
            pWidth = pw;
            pHeight = ph;
            playerRect = new Rectangle((int) playerPos.X, (int) playerPos.Y, pWidth, pHeight);
        }

        

        //Player property for position vector
        public Rectangle PlayerRect
        {
            get { return new Rectangle(); }
        }

        //Get player's current position
        /*
        public Vector2 GetPos()
        {
            return new Vector2(PosX, PosY);
        }
        */

        //Determine what happens when orb hits a player
        public void OnHit()
        {

        }

        //Check for player input
        public void CheckInput()
        {
            //Keyboard object
            KeyboardState kbState = Keyboard.GetState();

            //WASD - P1 Movement
            //Currently changes float by 1, can replace the 1 with a movement increment variable
            if(playerIsOne == true)
            {
                //P1 Up
                if (kbState.IsKeyDown(Keys.W))
                {
                    PosY--; //simplified some of this code which you don't even need to do this vector has a set x and y property which you could use to change the Vector
                }
                //P1 Left
                if (kbState.IsKeyDown(Keys.A))
                {
                    PosX--;
                }
                //P1 Down
                if (kbState.IsKeyDown(Keys.S))
                {
                    PosY++;
                }
                //P1 Right
                if (kbState.IsKeyDown(Keys.D))
                {
                    PosX++;
                }
            }

            //Arrow Keys - P2 Movement
            if(playerIsOne == false)
            {
                //P2 Up
                if (kbState.IsKeyDown(Keys.Up))
                {
                    PosY--;
                }
                //P2 Left
                if (kbState.IsKeyDown(Keys.Left))
                {
                    PosX--;
                }
                //P2 Down
                if (kbState.IsKeyDown(Keys.Down))
                {
                    PosY++;
                }
                //P2 Right
                if (kbState.IsKeyDown(Keys.Right))
                {
                    PosX++;
                }
            }
            

        }
    }
}
