using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Utilities.Png;

/*
TieOrDye GameBoard Class
Made by: Zen Lin
*/

namespace TieOrDye
{
    class GameBoard : Game1
    {
        //attributes
        bool started;

        //For getting the device/computer's current resolution, each property returns an int
        int userDisplayWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        int userDisplayHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

        //For getting the game window's resolution NEED HELP FROM THE TA
        int userGameWindowWidth = 0;
        int userGameWindowHeight = 0;
        
        //For getting the content image board's dimension
        

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
