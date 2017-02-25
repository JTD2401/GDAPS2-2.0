using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
TieOrDye GameBoard Class
*/

namespace TieOrDye
{
    class GameBoard : Game1
    {
        //attributes
        bool started;

        //Gameboard constructor
        public GameBoard()
        {
            //Game is not started by default
            started = false;
        }

        //Run to start the game
        public void PlayGame()
        {
            //Starts game
            started = true;
        }
    }
}
