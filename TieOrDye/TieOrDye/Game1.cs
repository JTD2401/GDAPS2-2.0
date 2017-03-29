using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using System.Diagnostics;
using System;
using System.Collections.Generic;

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
        GameWindow viewport;
        SpriteBatch spriteBatch;
        Texture2D p1Tex; //P1's Front-Facing Sprite
        Player p1; //P1's object
        Texture2D p2Tex; //P2's Front-Facing Sprite
        Player p2; //P2's object
        Texture2D gameBoard; //GameBoard Image
        Texture2D Level1;
        Rectangle windowsize;
        //Buttons are 392 x 103
        Texture2D inactiveStartButtonTex; //GameBoard Inactive Start Button
        Texture2D activeStartButtonTex; //GameBoard Active Start Button
        bool startActive; //GameBoard Start Button Active boolean
        MouseState currMState; //Current Mouse State
        Texture2D cursorTex; //Mouse cursor
        Rectangle cursorRect; //Cursor rectangle
        KeyboardState currKbState, prevKbState; //Current and previous keyboard state
        Texture2D pauseScreen;  // base pause screen
        Texture2D resumeButton;  // base resume button
        Texture2D options;  // base options button
        Texture2D mainMenu; // Main Menu button
        Texture2D exit;  // exit button
        Texture2D start;  // start button
        BinaryReader read;
        double playerSpeed1;
        double playerSpeed2;
        int player1InitialX;
        int player2InitialX;
        int player1InitialY;
        int player2InitialY;
        Random rand; //Random object
        List<Stone> stonesList; //list of stones
        Texture2D stoneTex; //gray stone texture
        SpriteFont font;
        double time;
        //Color stoneColor;
        Texture2D blueStone;
        Texture2D orangeStone;
        int p1Count;
        int p2Count;

        const int NUMBER_OF_STONES = 25;
        const int WIDTH_OF_STONES = 30;

        //List that will contain different direction sprite for players
        List<Texture2D> player1Sprites;
        List<Texture2D> player2Sprites;

        //Animation object to animate the player's movement based on direction key
        Animation player1Animation;
        Animation player2Animation;
        Texture2D tempSprite;

        enum GameStates { Menu, Pause, Options, InGame, GameOver }; //Enum for game states
        GameStates currentGameState; //Attribute for current game state

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Changes resolution - Default resolution is 800x480 -- This code changes it to 1000x800

            //graphics.IsFullScreen = true;
            //this.IsMouseVisible = true;
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
            graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            windowsize = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            p1Count = 0;
            p2Count = 0;

            //Set initial state to menu
            currentGameState = GameStates.Menu;

            Mouse.WindowHandle = this.Window.Handle;
            //Buttons are inactive by default
            startActive = false;

            string filepath = @"" + Directory.GetCurrentDirectory();

            filepath += @"\Content\player.dat";

            read = new BinaryReader(File.OpenRead(filepath));

            playerSpeed1 = read.ReadDouble();

            player1InitialX = read.ReadInt32();
            player1InitialY = read.ReadInt32();

            //P1 Object initialized at 0,0
            p1 = new Player(p1Tex, player1InitialX, player1InitialY, read.ReadInt32(), read.ReadInt32());


            playerSpeed2 = read.ReadDouble();

            player2InitialX = read.ReadInt32();
            player2InitialY = read.ReadInt32();

            //P2 Object initialized at 900,0
            p2 = new Player(p2Tex, player2InitialX, player2InitialY, read.ReadInt32(), read.ReadInt32());


            //List of Texture2D that contains different directional sprites for players
            player1Sprites = new List<Texture2D>();
            player2Sprites = new List<Texture2D>();

            //Animation object initialization
            player1Animation = new Animation(player1Sprites, playerSpeed1);
            player2Animation = new Animation(player2Sprites, playerSpeed2);

            //stoneColor = Color.White;
            //this.IsMouseVisible = true;
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

            //Load player one front image
            p1Tex = Content.Load<Texture2D>("GolemFrontBlue");
            //Load player two front image
            p2Tex = Content.Load<Texture2D>("GolemFrontOrange");
            //Load mouse cursor
            cursorTex = Content.Load<Texture2D>("Cursor");
            //Load gameboard image
            gameBoard = Content.Load<Texture2D>("StartScreenNoButtons");
            //Load gameboard buttons
            //Inactive Start button
            inactiveStartButtonTex = Content.Load<Texture2D>("InactiveStartButton");
            //Active Start button
            activeStartButtonTex = Content.Load<Texture2D>("ActiveStartButton");
            // loads texture into pauseScreen attribute
            pauseScreen = Content.Load<Texture2D>("Pause Screen No Buttons");
            // loads resume button texture
            resumeButton = Content.Load<Texture2D>("Resume Button");
            // loads options button texture
            options = Content.Load<Texture2D>("Options Button");
            // loads Main Menu button texture
            mainMenu = Content.Load<Texture2D>("Main Menu Button");
            // loads exit button texture
            exit = Content.Load<Texture2D>("Exit Button");
            // loads start button texture
            start = Content.Load<Texture2D>("Start Button");
            //loads level1 image
            Level1 = Content.Load<Texture2D>("GameBoard2");
            //Stone image
            stoneTex = Content.Load<Texture2D>("Stone");

            font = Content.Load<SpriteFont>("NewSpriteFont");

            blueStone = Content.Load<Texture2D>("BlueStone");

            orangeStone = Content.Load<Texture2D>("OrangeStone");
            // TODO: use this.Content to load your game content here

            //Following will be loading the sprite for golems in
            //Blue golem will be player1, Orange golem will be player2


            //Order of the direction for sprites will be (Up, Down, Left, Right, UpLeft, UpRight, DownLeft, Downright)

            //This portion is for Player1 (Blue Golem)
            tempSprite = Content.Load<Texture2D>("GolemBackBlue");
            player1Sprites.Add(tempSprite);
            tempSprite = Content.Load<Texture2D>("GolemFrontBlue");
            player1Sprites.Add(tempSprite);
            tempSprite = Content.Load<Texture2D>("GolemSideLeftBlue");
            player1Sprites.Add(tempSprite);
            tempSprite = Content.Load<Texture2D>("GolemSideRightBlue");
            player1Sprites.Add(tempSprite);
            tempSprite = Content.Load<Texture2D>("GolemAngleUpLeftBlue");
            player1Sprites.Add(tempSprite);
            tempSprite = Content.Load<Texture2D>("GolemAngleUpRightBlue");
            player1Sprites.Add(tempSprite);
            tempSprite = Content.Load<Texture2D>("GolemAngleDownBlue");
            player1Sprites.Add(tempSprite);
            tempSprite = Content.Load<Texture2D>("GolemAngleDownRightBlue");
            player1Sprites.Add(tempSprite);

            //This portion is for Player2 (Orange Golem)
            tempSprite = Content.Load<Texture2D>("GolemBackOrange");
            player2Sprites.Add(tempSprite);
            tempSprite = Content.Load<Texture2D>("GolemFrontOrange");
            player2Sprites.Add(tempSprite);
            tempSprite = Content.Load<Texture2D>("GolemSideLeftOrange");
            player2Sprites.Add(tempSprite);
            tempSprite = Content.Load<Texture2D>("GolemSideRightOrange");
            player2Sprites.Add(tempSprite);
            tempSprite = Content.Load<Texture2D>("GolemAngleUpOrange");
            player2Sprites.Add(tempSprite);
            tempSprite = Content.Load<Texture2D>("GolemAngleUpRightOrange");
            player2Sprites.Add(tempSprite);
            tempSprite = Content.Load<Texture2D>("GolemAngleDownOrange");
            player2Sprites.Add(tempSprite);
            tempSprite = Content.Load<Texture2D>("GolemAngleDownRightOrange");
            player2Sprites.Add(tempSprite);
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

            //Keyboard object
            currKbState = Keyboard.GetState();






            // improved finite state machine
            switch (currentGameState)
            {
                case GameStates.Menu:  // start menu state
                    if (cursorRect.Intersects(new Rectangle(190, 150, activeStartButtonTex.Width, activeStartButtonTex.Height)))
                    {
                        //Make the button active
                        startActive = true;
                        //Check for clicking button
                        if (currMState.LeftButton == ButtonState.Pressed)
                        {
                            p1.X = player1InitialX;
                            p1.Y = player1InitialY;

                            p2.X = player2InitialX;
                            p2.Y = player2InitialY;

                            p1Count = 0;
                            p2Count = 0;

                            //Create stone objects
                            CreateStones(NUMBER_OF_STONES, WIDTH_OF_STONES);
                            //Start game
                            currentGameState = GameStates.InGame;
                        }
                    }
                    else  //When the mouse is no longer hovering over the button
                    {
                        //Make the button inactive
                        startActive = false;
                    }

                    if (cursorRect.Intersects(new Rectangle(190, 750, exit.Width, exit.Height)))
                    {
                        if (currMState.LeftButton == ButtonState.Pressed)
                        {
                            this.Exit();
                        }
                    }

                    time = 11;
                    

                    break;
                case GameStates.Pause:  // options menu state

                    // determines if game state is changed to InGame when player clicks on the Resume rectangle
                    if (cursorRect.Intersects(new Rectangle(GraphicsDevice.Viewport.Width / 3, 150, GraphicsDevice.Viewport.Width / 3, 150)))
                    {
                        if (currMState.LeftButton == ButtonState.Pressed)
                        {
                            currentGameState = GameStates.InGame;
                        }
                    }

                    // determines if game state is changed to Menu when player clicks on the Main Menu rectangle 
                    if (cursorRect.Intersects(new Rectangle(GraphicsDevice.Viewport.Width / 3, 550, GraphicsDevice.Viewport.Width / 3, 150)))
                    {
                        if (currMState.LeftButton == ButtonState.Pressed)
                        {

                            currentGameState = GameStates.Menu;
                        }
                    }

                    // determines if game is exited when player clicks on the Exit rectangle
                    if (cursorRect.Intersects(new Rectangle(GraphicsDevice.Viewport.Width / 3, 750, GraphicsDevice.Viewport.Width / 3, 150)))
                    {
                        if (currMState.LeftButton == ButtonState.Pressed)
                        {
                            this.Exit();
                        }
                    }
                    break;
                case GameStates.InGame:  // gameplay state
                    //Uses the animation class to process the input from keyboard as well as updating the rectangle's position according to direction pressed
                    player1Animation.processInput(currKbState, p1, Keys.W, Keys.A, Keys.S, Keys.D);
                    p1.PlayerRect = player1Animation.PlayerPositionRectangle;
                    player2Animation.processInput(currKbState, p2, Keys.Up, Keys.Left, Keys.Down, Keys.Right);
                    p2.PlayerRect = player2Animation.PlayerPositionRectangle;

                    // prevents players from passing beyond the boarder
                    ScreenBorder(p1);
                    ScreenBorder(p2);
                    //MoveStones(stonesList);

                    for (int x = 0; x < stonesList.Count; x++)
                    {
                        

                        bool p1Inter = stonesList[x].Circle.Intersects(p1.PlayerRect);
                        bool p2Inter = stonesList[x].Circle.Intersects(p2.PlayerRect);

                        if(p1Inter && stonesList[x].StoneTex == stoneTex)
                        {
                            
                            stonesList[x].StoneTex = blueStone;
                            p1Count++;
                        }

                        if(p2Inter && stonesList[x].StoneTex == stoneTex)
                        {
                            
                            stonesList[x].StoneTex = orangeStone;
                            p2Count++;
                        }
                    }

                    
                    time -= gameTime.ElapsedGameTime.TotalSeconds;
                    if (time <= 0)
                    {
                        currentGameState = GameStates.GameOver;
                    }

                    if (currKbState.IsKeyDown(Keys.P) || currKbState.IsKeyDown(Keys.Escape)) { currentGameState = GameStates.Pause; }
                    break;
                case GameStates.Options:
                    break;
                case GameStates.GameOver:  // end of game state
                    if (currKbState.IsKeyDown(Keys.Space))
                    {
                        currentGameState = GameStates.Menu;
                    }
                    break;



            }


            //Previous Keyboard state object
            prevKbState = currKbState;



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

            var mouse = OpenTK.Input.Mouse.GetCursorState();


            cursorRect = new Rectangle(mouse.X, mouse.Y, 20, 20);

            switch (currentGameState)
            {
                case GameStates.Menu:
                    //Draw gameboard
                    spriteBatch.Draw(gameBoard, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                    Debug.WriteLine(cursorRect);


                    if (cursorRect.Intersects(new Rectangle(190, 150, 392, 103)))
                    {
                        spriteBatch.Draw(start, new Rectangle(200, 150, 392, 103), Color.Violet);
                    }
                    else
                    {
                        spriteBatch.Draw(start, new Rectangle(190, 150, 392, 103), Color.White);
                    }

                    if (cursorRect.Intersects(new Rectangle(190, 450, 392, 103)))
                    {
                        spriteBatch.Draw(options, new Rectangle(200, 450, 392, 103), Color.Violet);
                    }
                    else
                    {
                        spriteBatch.Draw(options, new Rectangle(190, 450, 392, 103), Color.White);
                    }

                    if (cursorRect.Intersects(new Rectangle(190, 750, 392, 103)))
                    {
                        spriteBatch.Draw(exit, new Rectangle(200, 750, 392, 103), Color.Violet);
                    }
                    else
                    {
                        spriteBatch.Draw(exit, new Rectangle(190, 750, 392, 103), Color.White);
                    }
                    //Draw cursor
                    spriteBatch.Draw(cursorTex, cursorRect, Color.White);  // draws cursor
                    break;
                case GameStates.InGame:
                    spriteBatch.Draw(Level1, windowsize, Color.White);
                    DrawStones(stonesList);
                    player1Animation.drawAnimation(spriteBatch);
                    player2Animation.drawAnimation(spriteBatch);

                    spriteBatch.DrawString(font, "TIME: " + (int)time, new Vector2(890, 45), Color.White);
                    spriteBatch.DrawString(font, "P1 Score: " + p1Count, new Vector2(120, 45), Color.White);
                    spriteBatch.DrawString(font, "P2 Score: " + p2Count, new Vector2(1650, 45), Color.White);
                    break;
                case GameStates.Pause:  // draws pause screen

                    spriteBatch.Draw(pauseScreen, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);  // draws pause screen

                    // draws resume button and checks if the mouse is over it
                    spriteBatch.Draw(resumeButton, new Rectangle(GraphicsDevice.Viewport.Width / 3, 150, GraphicsDevice.Viewport.Width / 3, 150), Color.White);
                    if (cursorRect.Intersects(new Rectangle(GraphicsDevice.Viewport.Width / 3, 150, GraphicsDevice.Viewport.Width / 3, 150)))
                    {
                        spriteBatch.Draw(resumeButton, new Rectangle(GraphicsDevice.Viewport.Width / 3, 150, GraphicsDevice.Viewport.Width / 3, 150), Color.Violet);  // if mouse is over button, changes color
                    }

                    // draws options button and checks if the mouse is over it
                    spriteBatch.Draw(options, new Rectangle(GraphicsDevice.Viewport.Width / 3, 350, GraphicsDevice.Viewport.Width / 3, 150), Color.White);
                    if (cursorRect.Intersects(new Rectangle(GraphicsDevice.Viewport.Width / 3, 350, GraphicsDevice.Viewport.Width / 3, 150)))
                    {
                        spriteBatch.Draw(options, new Rectangle(GraphicsDevice.Viewport.Width / 3, 350, GraphicsDevice.Viewport.Width / 3, 150), Color.Violet);  // if mouse is over button, changes color
                    }

                    // draws Main Menu button and checks if the mouse is over it
                    spriteBatch.Draw(mainMenu, new Rectangle(GraphicsDevice.Viewport.Width / 3, 550, GraphicsDevice.Viewport.Width / 3, 150), Color.White);
                    if (cursorRect.Intersects(new Rectangle(GraphicsDevice.Viewport.Width / 3, 550, GraphicsDevice.Viewport.Width / 3, 150)))
                    {
                        spriteBatch.Draw(mainMenu, new Rectangle(GraphicsDevice.Viewport.Width / 3, 550, GraphicsDevice.Viewport.Width / 3, 150), Color.Violet);  // if mouse is over button, changes color
                    }

                    // draws Exit button and checks if the mouse is over it
                    spriteBatch.Draw(exit, new Rectangle(GraphicsDevice.Viewport.Width / 3, 750, GraphicsDevice.Viewport.Width / 3, 150), Color.White);
                    if (cursorRect.Intersects(new Rectangle(GraphicsDevice.Viewport.Width / 3, 750, GraphicsDevice.Viewport.Width / 3, 150)))
                    {
                        spriteBatch.Draw(exit, new Rectangle(GraphicsDevice.Viewport.Width / 3, 750, GraphicsDevice.Viewport.Width / 3, 150), Color.Violet);  // if mouse is over button, changes color
                    }

                    // draws player models on either side of screen
                    spriteBatch.Draw(p1Tex, new Rectangle(0, 300, 450, 600), Color.White);
                    spriteBatch.Draw(p2Tex, new Rectangle(1400, 300, 500, 600), Color.White);



                    spriteBatch.Draw(cursorTex, cursorRect, Color.White);
                    break;
                case GameStates.Options:
                    break;
                case GameStates.GameOver:
                    spriteBatch.DrawString(font, "Press Space to go to Menu", new Vector2(500, 400), Color.Black);
                    break;
            }




            //End spritebatch
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void MoveStones(List<Stone> stonesList)
        {
            for (int i = 0; i < stonesList.Count; i++)
            {
                stonesList[i].XPos += (int)stonesList[i].Direction.X;
                stonesList[i].YPos += (int)stonesList[i].Direction.Y;
            }
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
                pl.Y = GraphicsDevice.Viewport.Height - p1.PlayerRect.Height;
            }
            //If the player is all the way above the screen, prevent it from moving too far up
            if (pl.PlayerRect.Y < 0)
            {
                pl.Y = 0;
            }
        }

        //Method to create all stones in the game
        void CreateStones(int numOfStones, int stoneWidth)
        {
            //Random object
            rand = new Random();
            //Create an empty list to hold stone objects
            stonesList = new List<Stone>();
            for (int i = 0; i < numOfStones; i++)
            {
                //Can create stone by default
                bool canCreate = true;
                //Generate random int for stone x location
                int stoneX = rand.Next(0, GraphicsDevice.Viewport.Width - stoneWidth + 1);
                //Generate random int for stone y location
                int stoneY = rand.Next(0, GraphicsDevice.Viewport.Height - stoneWidth + 1);
                //Check if a stone is already drawn in that location
                for (int j = 0; j < stonesList.Count; j++)   //For each stone in list
                {
                    if ((stoneX >= stonesList[j].X && stoneX <= stonesList[j].X + stoneWidth) && (stoneY >= stonesList[j].Y && stoneY <= stonesList[j].Y + stoneWidth)) //if potential stone's x  and y collides with the stone's x and y coordinates
                    {
                        canCreate = false; //prevent the stone from being created
                    }
                }
                if (canCreate)//If the stone can be created
                {
                    //Create stone object
                    Stone s = new Stone(stoneTex, stoneX, stoneY, stoneWidth);
                    //Add stone object to stone list
                    stonesList.Add(s);
                }
                else //If current stone can't be created, try again
                {
                    //Send list of stones back one
                    i--;
                }
            }
        }

        //Method to draw all stones after they have been created using the CreateStones method - Needs to be added to significantly when orbs are introduced
        //Make sure to use this method inside a spritebatch
        void DrawStones(List<Stone> sl)
        {
            for (int i = 0; i < sl.Count; i++)  //For each stone in the stone list
            {
                spriteBatch.Draw(stonesList[i].StoneTex, new Rectangle(sl[i].XPos, sl[i].YPos, WIDTH_OF_STONES, WIDTH_OF_STONES), Color.White); //draw it
            }
        }
    }
}