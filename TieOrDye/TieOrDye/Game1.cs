using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

/*
TieOrDye Game Class
*/

namespace TieOrDye
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch; 
        Texture2D p1Tex; //P1's Sprite
        Player p1; //P1's object
        Texture2D p2Tex; //P2's sprite
        Player p2; //P2's object
        Texture2D gameBoard; //GameBoard Image
        //Buttons are 392 x 103
        Texture2D inactiveStartButtonTex; //GameBoard Inactive Start Button
        Texture2D activeStartButtonTex; //GameBoard Active Start Button
        bool startActive; //GameBoard Start Button Active boolean
        MouseState currMState; //Current Mouse State
        Texture2D cursorTex; //Mouse cursor
        Rectangle cursorRect; //Cursor rectangle
        KeyboardState currKbState, prevKbState; //Current and previous keyboard state

        enum GameStates { Menu, Options, InGame, GameOver}; //Enum for game states
        GameStates currentGameState; //Attribute for current game state

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Changes resolution - Default resolution is 800x480 -- This code changes it to 1000x800
            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 800;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            //Set initial state to menu
            currentGameState = GameStates.Menu;

            //Buttons are inactive by default
            startActive = false;

            //P1 Object initialized at 0,0
            p1 = new Player(p1Tex, 0, 0, 80, 120);
             
            //P2 Object initialized at 900,0
            p2 = new Player(p2Tex, 900, 0, 80, 120);


            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Load temporary p1 image
            p1Tex = Content.Load<Texture2D>("Player1");
            //Load temporary p2 image
            p2Tex = Content.Load<Texture2D>("Player2");
            //Load mouse cursor
            cursorTex = Content.Load<Texture2D>("Cursor");
            //Load gameboard image
            gameBoard = Content.Load<Texture2D>("StartScreenNoButton");
            //Load gameboard buttons
            //Inactive Start button
            inactiveStartButtonTex = Content.Load<Texture2D>("InactiveStartButton");
            //Active Start button
            activeStartButtonTex = Content.Load<Texture2D>("ActiveStartButton");
            

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            //Current mouse state
            currMState = Mouse.GetState();
            //Previous Keyboard state object
            prevKbState = currKbState;
            //Keyboard object
            currKbState = Keyboard.GetState();

            //TEMPORARY GAME STATE SET UNTIL MENU IS WORKING
            //currentGameState = GameStates.InGame;

            //Menu State
            if(currentGameState == GameStates.Menu)
            {
                //Place cursor at mouse location

                //If mouse hovers over button of size 392x103(Default button size) at 40,40
                if(cursorRect.Intersects(new Rectangle(40, 40, activeStartButtonTex.Width, activeStartButtonTex.Height)))
                {
                    //Make the button active
                    startActive = true;
                    //Check for clicking button
                    if(currMState.LeftButton == ButtonState.Pressed)
                    {
                        //Start game
                        currentGameState = GameStates.InGame;
                    }
                }
                else  //When the mouse is no longer hovering over the button
                {
                    //Make the button inactive
                    startActive = false;
                }
            }
            //Options State
            if (currentGameState == GameStates.Options)
            {

            }
            //In Game State
            if (currentGameState == GameStates.InGame)
            {
                //WASD - P1 Movement
                //Currently changes float by 1, can replace the 1 with a movement increment variable
                //P1 Up
                if (currKbState.IsKeyDown(Keys.W))
                {
                    p1.Y--; //simplified some of this code which you don't even need to do this vector has a set x and y property which you could use to change the Vector
                }
                //P1 Left
                if (currKbState.IsKeyDown(Keys.A))
                {
                    p1.X--;
                }
                //P1 Down
                if (currKbState.IsKeyDown(Keys.S))
                {
                    p1.Y++;
                }
                //P1 Right
                if (currKbState.IsKeyDown(Keys.D))
                {
                    p1.X++;
                }
                //Arrow Keys - P2 Movement
                //P2 Up
                if (currKbState.IsKeyDown(Keys.Up))
                {
                    p2.Y--;
                }
                //P2 Left
                if (currKbState.IsKeyDown(Keys.Left))
                {
                    p2.X--;
                }
                //P2 Down
                if (currKbState.IsKeyDown(Keys.Down))
                {
                    p2.Y++;
                }
                //P2 Right
                if (currKbState.IsKeyDown(Keys.Right))
                {
                    p2.X++;
                }
                //Prevent player objects from exiting screen
                ScreenBorder(p1);
                ScreenBorder(p2);
                
            }
            //Game Over State
            if (currentGameState == GameStates.GameOver)
            {

            }

            

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            //Start spritebatch
            spriteBatch.Begin();

            //Menu State
            if (currentGameState == GameStates.Menu)
            {
                //Draw gameboard
                spriteBatch.Draw(gameBoard, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                //Set cursor rectangle
                cursorRect = new Rectangle(currMState.Position.X, currMState.Position.Y, 5, 5);
                if (startActive == true)
                {
                    //Draw active start button at (40, 40) of size 392x103(Sprite's Default Size)
                    spriteBatch.Draw(activeStartButtonTex, new Rectangle(40, 40, activeStartButtonTex.Width, activeStartButtonTex.Height), Color.White);
                }
                else if (startActive == false)
                {
                    //Draw inactive start button at (40, 40) of size 392x103(Sprite's Default Size)
                    spriteBatch.Draw(inactiveStartButtonTex, new Rectangle(40, 40, activeStartButtonTex.Width, activeStartButtonTex.Height), Color.White);
                }
                //Draw cursor
                spriteBatch.Draw(cursorTex, cursorRect, Color.White);
            }
            //Options State
            if (currentGameState == GameStates.Options)
            {

            }
            //InGame State
            if (currentGameState == GameStates.InGame)
            {
                //Draw p1 at their current position
                spriteBatch.Draw(p1Tex, p1.PlayerRect, Color.White);
                spriteBatch.Draw(p2Tex, p2.PlayerRect, Color.White);
            }
            //Game Over State
            if (currentGameState == GameStates.GameOver)
            {

            }
            
            //End spritebatch
            spriteBatch.End();

            base.Draw(gameTime);
        }
        
        //Screen wrapping method for the player objects
        void ScreenBorder(Player pl)
        {
            //Keep the player on the screen at all times
            //If the player is all the way to the left of the screen, prevent it from moving too far to the left
            if (pl.X < 0)
            {
                pl.X = 0;
            }
            //If the player is all the way to the right of the screen, prevent it from moving too far to the right
            if (pl.X > GraphicsDevice.Viewport.Width - p1.PlayerRect.Width)
            {
                pl.X = GraphicsDevice.Viewport.Width - p1.PlayerRect.Width;
            }
            //If the player is all the way below the screen, prevent it from moving too far down
            if (pl.Y > GraphicsDevice.Viewport.Height - p1.PlayerRect.Height)
            {
                pl.X = GraphicsDevice.Viewport.Height - p1.PlayerRect.Height;
            }
            //If the player is all the way above the screen, prevent it from moving too far up
            if (pl.PlayerRect.Y < 0)
            {
                pl.Y = 0;
            }
        }

    }
}
