using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/* GameBoard Class
 * made by Zen Lin
 * This class will be a class that stores the texture and a rectangle that contains
 * it's location when it's being used in the Game1 class.
 * 
 * The class DO NOT inherit from the Game1 class because that will create
 * infinite loop and thus not work when game itself is running
*/

namespace TieOrDye
{
    class GameBoard
    {
        //attributes
        Texture2D gameBoardImage;
        Rectangle gameBoardPosition;

        //Gameboard constructor
        //The constructor will get input of X and Y position of desired
        //and the image that we want the map to be. I chose to use rectangle
        //for the position and the width and height is that image will be made to fit
        //inside the rectangle so we can also distort the image if needed later on
        public GameBoard(int inputX, int inputY, Texture2D inputImage, GraphicsDevice inputGraphicDevice)
        {
            gameBoardImage = inputImage;
            gameBoardPosition = new Rectangle(inputX, inputY, inputGraphicDevice.Viewport.Width, inputGraphicDevice.Viewport.Height);
        }

        //Method to draw the gameboard when needed, it takes in the Spritebatch which
        //we will get from the Game1 class
        public void drawItself(SpriteBatch inputSpriteBatch)
        {
            inputSpriteBatch.Draw(gameBoardImage, gameBoardPosition, Color.White);
        }

        //Property method to access or assign the Texture image for the gameboard
        public Texture2D GameBoardImage
        {
            get
            {
                return gameBoardImage;
            }
            set
            {
                gameBoardImage = value;
            }
        }

        //Property method to access or assign the Rectangle with position and distorted size rectangle
        public Rectangle GameBoardPosition
        {
            get
            {
                return gameBoardPosition;
            }
            set
            {
                gameBoardPosition = value;
            }
        }
    }
}
