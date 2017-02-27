using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

/*
TieOrDye Player Class
*/

namespace TieOrDye
{
    class Player : Game1
    {
        //Game1 object
        Game1 gm1 = new Game1();

        //Attributes
        float posX;
        float posY;
        //True if player is p1, false if p2
        bool playerIsOne;
        //Immunity boolean
        bool immune;

        //player constructor
        public Player(bool pl)
        {
            playerIsOne = pl;
            if (playerIsOne == true)
            {
                PosX = 0;
                PosY = 0;
            }
            else if (playerIsOne == false)
            {
                PosX = 900;
                PosY = 0;
            }
        }

        //Player property for x position
        public float PosX
        {
            get { return posX; }
            set { posX = value; }
        }

        //Player property for y position
        public float PosY
        {
            get { return posY; }
            set { posY = value; }
        }

        //Player property for position vector
        public Vector2 GetPos
        {
            get { return new Vector2(PosX, PosY); }
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
