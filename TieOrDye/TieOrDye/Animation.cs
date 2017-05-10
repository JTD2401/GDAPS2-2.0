using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using System.Diagnostics;

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

        Stopwatch soundEffectBuffer;
        int amountTimeBuffer = 250;

        double playerSpeed;
        bool ableToPlay;

        public enum PlayerState { Up, Down, Left, Right, UpLeft, UpRight, DownLeft, Downright };
        PlayerState look;

        //property for playerState enum
        public PlayerState Look
        {
            get { return look; }
        }

        public Animation(List<Texture2D> inputListOfSprite, double inputPlayerSpeed)
        {
            playerSprites = inputListOfSprite;
            playerSpeed = inputPlayerSpeed;
            soundEffectBuffer = new Stopwatch();
        }

        public void processInput(KeyboardState inputKeyboardState, Player inputPlayerObject, Keys inputUp, Keys inputLeft, Keys inputDown, Keys inputRight, SoundEffect inputWalkSound, GameTime inputGameTime)
        {
            playerPositionRectangle = inputPlayerObject.PlayerRect;

            //Stopwatch
            soundEffectBuffer.Start();

            if(soundEffectBuffer.ElapsedMilliseconds >= amountTimeBuffer)
            {
                soundEffectBuffer.Stop();
                soundEffectBuffer.Reset();

                ableToPlay = true;
            }
            else
            {
                ableToPlay = false;
            }


            // TODO: Add your update logic here
            string dir = "";

            // see if WASD keys are pressed
            previousKState = kState;
            kState = inputKeyboardState;

            //Decrease volume of walking soundeffect
            SoundEffectInstance temp = inputWalkSound.CreateInstance();
            temp.Volume = 0.3f;
            

            //Check each key's pressed down or not and change the truth value
            wasd[0] = kState.IsKeyDown(inputUp);
            wasd[1] = kState.IsKeyDown(inputLeft);
            wasd[2] = kState.IsKeyDown(inputDown);
            wasd[3] = kState.IsKeyDown(inputRight);

            //previous keyboard state to store the previous keyboard state
            previousOfwasd[0] = previousKState.IsKeyDown(inputUp);
            previousOfwasd[1] = previousKState.IsKeyDown(inputLeft);
            previousOfwasd[2] = previousKState.IsKeyDown(inputDown);
            previousOfwasd[3] = previousKState.IsKeyDown(inputRight);

            //Only process player input when player is not stunned
            if(inputPlayerObject.Stunned == false)
            {
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
                        playerPositionRectangle.Y -= (int)playerSpeed;
                        if (ableToPlay)
                        {
                            temp.Play();
                        }
                        break;
                    case "A":
                        look = PlayerState.Left;
                        playerPositionRectangle.X -= (int)playerSpeed;
                        if (ableToPlay)
                        {
                            temp.Play();
                        }
                        break;
                    case "S":
                        look = PlayerState.Down;
                        playerPositionRectangle.Y += (int)playerSpeed;
                        if (ableToPlay)
                        {
                            temp.Play();
                        }
                        break;
                    case "D":
                        look = PlayerState.Right;
                        playerPositionRectangle.X += (int)playerSpeed;
                        if (ableToPlay)
                        {
                            temp.Play();
                        }
                        break;
                    case "WA":
                        look = PlayerState.UpLeft;
                        playerPositionRectangle.Y -= (int)playerSpeed;
                        playerPositionRectangle.X -= (int)playerSpeed;
                        if (ableToPlay)
                        {
                            temp.Play();
                        }
                        break;
                    case "WD":
                        look = PlayerState.UpRight;
                        playerPositionRectangle.Y -= (int)playerSpeed;
                        playerPositionRectangle.X += (int)playerSpeed;
                        if (ableToPlay)
                        {
                            temp.Play();
                        }
                        break;
                    case "SA":
                        look = PlayerState.DownLeft;
                        playerPositionRectangle.Y += (int)playerSpeed;
                        playerPositionRectangle.X -= (int)playerSpeed;
                        if (ableToPlay)
                        {
                            temp.Play();
                        }
                        break;
                    case "SD":
                        look = PlayerState.Downright;
                        playerPositionRectangle.Y += (int)playerSpeed;
                        playerPositionRectangle.X += (int)playerSpeed;
                        if (ableToPlay)
                        {
                            temp.Play();
                        }
                        break;
                    default:
                        return;
                }
            }
        }

        public void drawAnimation(SpriteBatch spriteBatch, Player pl, int playerNum)
        {
            Color stunTint = new Color();
            Color immuneTint = new Color();

            if(playerNum == 1)
            {
                stunTint = Color.Gray;
                immuneTint = Color.Yellow;
            }
            if(playerNum == 2)
            {
                stunTint = Color.Gray;
                immuneTint = Color.ForestGreen;
            }
            //According to the enum we draw different sprite
            switch (look)
            {
                case PlayerState.Up:
                    if (pl.Stunned == true)
                        spriteBatch.Draw(playerSprites[0], playerPositionRectangle, stunTint);
                    else if(pl.Immune == true)
                        spriteBatch.Draw(playerSprites[0], playerPositionRectangle, immuneTint);
                    else
                        spriteBatch.Draw(playerSprites[0], playerPositionRectangle, Color.White);
                    break;
                case PlayerState.Down:
                    if(pl.Stunned == true)
                        spriteBatch.Draw(playerSprites[1], playerPositionRectangle, stunTint);
                    else if (pl.Immune == true)
                        spriteBatch.Draw(playerSprites[1], playerPositionRectangle, immuneTint);
                    else
                        spriteBatch.Draw(playerSprites[1], playerPositionRectangle, Color.White);
                    break;
                case PlayerState.Left:
                    if (pl.Stunned == true)
                        spriteBatch.Draw(playerSprites[2], playerPositionRectangle, stunTint);
                    else if (pl.Immune == true)
                        spriteBatch.Draw(playerSprites[2], playerPositionRectangle, immuneTint);
                    else
                        spriteBatch.Draw(playerSprites[2], playerPositionRectangle, Color.White);
                    break;
                case PlayerState.Right:
                    if (pl.Stunned == true)
                        spriteBatch.Draw(playerSprites[3], playerPositionRectangle, stunTint);
                    else if (pl.Immune == true)
                        spriteBatch.Draw(playerSprites[3], playerPositionRectangle, immuneTint);
                    else
                        spriteBatch.Draw(playerSprites[3], playerPositionRectangle, Color.White);
                    break;
                case PlayerState.UpLeft:
                    if (pl.Stunned == true)
                        spriteBatch.Draw(playerSprites[4], playerPositionRectangle, stunTint);
                    else if (pl.Immune == true)
                        spriteBatch.Draw(playerSprites[4], playerPositionRectangle, immuneTint);
                    else
                        spriteBatch.Draw(playerSprites[4], playerPositionRectangle, Color.White);
                    break;
                case PlayerState.UpRight:
                    if (pl.Stunned == true)
                        spriteBatch.Draw(playerSprites[5], playerPositionRectangle, stunTint);
                    else if (pl.Immune == true)
                        spriteBatch.Draw(playerSprites[5], playerPositionRectangle, immuneTint);
                    else
                        spriteBatch.Draw(playerSprites[5], playerPositionRectangle, Color.White);
                    break;
                case PlayerState.DownLeft:
                    if (pl.Stunned == true)
                        spriteBatch.Draw(playerSprites[6], playerPositionRectangle, stunTint);
                    else if (pl.Immune == true)
                        spriteBatch.Draw(playerSprites[6], playerPositionRectangle, immuneTint);
                    else
                        spriteBatch.Draw(playerSprites[6], playerPositionRectangle, Color.White);
                    break;
                case PlayerState.Downright:
                    if (pl.Stunned == true)
                        spriteBatch.Draw(playerSprites[7], playerPositionRectangle, stunTint);
                    else if (pl.Immune == true)
                        spriteBatch.Draw(playerSprites[7], playerPositionRectangle, immuneTint);
                    else
                        spriteBatch.Draw(playerSprites[7], playerPositionRectangle, Color.White);
                    break;
            }
        }

        public Rectangle PlayerPositionRectangle
        {
            get
            {
                return playerPositionRectangle;
            }
        }

    }
}
