using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/*
TieOrDye Animation Class
*/

namespace TieOrDye
{
    class Animation
    {
        KeyboardState kState;
        KeyboardState previousKState;
        Boolean[] wasd = { false, false, false, false };
        Boolean[] previousOfwasd = { false, false, false, false };
        List<Texture2D> playerSprites;
        Rectangle playerPositionRectangle;

        enum PlayerState { Up, Down, Left, Right, UpLeft, UpRight, DownLeft, Downright };
        PlayerState look;

        public Animation(List<Texture2D> inputListOfSprite)
        {
            playerSprites = inputListOfSprite;
        }

        public void processInput(KeyboardState inputKeyboardState, Player inputPlayerObject)
        {
            playerPositionRectangle = inputPlayerObject.PlayerRect;

            // TODO: Add your update logic here
            string dir = "";

            // see if WASD keys are pressed
            previousKState = kState;
            kState = inputKeyboardState;

            //Check each key's pressed down or not and change the truth value
            wasd[0] = kState.IsKeyDown(Keys.W);
            wasd[1] = kState.IsKeyDown(Keys.A);
            wasd[2] = kState.IsKeyDown(Keys.S);
            wasd[3] = kState.IsKeyDown(Keys.D);

            //previous keyboard state to store the previous keyboard state
            previousOfwasd[0] = previousKState.IsKeyDown(Keys.W);
            previousOfwasd[1] = previousKState.IsKeyDown(Keys.A);
            previousOfwasd[2] = previousKState.IsKeyDown(Keys.S);
            previousOfwasd[3] = previousKState.IsKeyDown(Keys.D);

            // check which keys are pressed and change the position according to key
            if (previousOfwasd[0] == true && wasd[1] == true)
            {
                dir = "WA";
            }
            else if (previousOfwasd[0] == true && wasd[3] == true)
            {
                dir = "WD";
            }
            else if (previousOfwasd[2] == true && wasd[1] == true)
            {
                dir = "SA";
            }
            else if (previousOfwasd[2] == true && wasd[3] == true)
            {
                dir = "SD";
            }
            else if (wasd[0] == true)
            {
                dir = "W";
            }
            else if (wasd[1] == true)
            {
                dir = "A";
            }
            else if (wasd[2] == true)
            {
                dir = "S";
            }
            else if (wasd[3] == true)
            {
                dir = "D";
            }

            // assign a value to look
            switch (dir)
            {
                case "W":
                    look = PlayerState.Up;
                    break;
                case "A":
                    look = PlayerState.Left;
                    break;
                case "S":
                    look = PlayerState.Down;
                    break;
                case "D":
                    look = PlayerState.Right;
                    break;
                case "WA":
                    look = PlayerState.UpLeft;
                    break;
                case "WD":
                    look = PlayerState.UpRight;
                    break;
                case "SA":
                    look = PlayerState.DownLeft;
                    break;
                case "SD":
                    look = PlayerState.Downright;
                    break;
                default:
                    return;
            }


        }

        public void drawAnimation(SpriteBatch spriteBatch)
        {
            //According to the enum we draw different sprite
            switch (look)
            {
                case PlayerState.Up:
                    spriteBatch.Draw(playerSprites[0], playerPositionRectangle, Color.White);
                    break;
                case PlayerState.Down:
                    spriteBatch.Draw(playerSprites[1], playerPositionRectangle, Color.White);
                    break;
                case PlayerState.Left:
                    spriteBatch.Draw(playerSprites[2], playerPositionRectangle, Color.White);
                    break;
                case PlayerState.Right:
                    spriteBatch.Draw(playerSprites[3], playerPositionRectangle, Color.White);
                    break;
                case PlayerState.UpLeft:
                    spriteBatch.Draw(playerSprites[4], playerPositionRectangle, Color.White);
                    break;
                case PlayerState.UpRight:
                    spriteBatch.Draw(playerSprites[5], playerPositionRectangle, Color.White);
                    break;
                case PlayerState.DownLeft:
                    spriteBatch.Draw(playerSprites[6], playerPositionRectangle, Color.White);
                    break;
                case PlayerState.Downright:
                    spriteBatch.Draw(playerSprites[7], playerPositionRectangle, Color.White);
                    break;
            }
        }

    }
}
