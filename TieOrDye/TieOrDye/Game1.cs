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
        #region attributes
        GraphicsDeviceManager graphics;
        GameWindow viewport;
        SpriteBatch spriteBatch;
        Player p1; //P1's object
        Player p2; //P2's object
        Rectangle windowsize;
        //Buttons are 392 x 103
        Texture2D inactiveStartButtonTex; //GameBoard Inactive Start Button
        Texture2D activeStartButtonTex; //GameBoard Active Start Button
        bool startActive; //GameBoard Start Button Active boolean
        MouseState currMState; //Current Mouse State
        MouseState prevState;
        Rectangle cursorRect; //Cursor rectangle
        KeyboardState currKbState, prevKbState; //Current and previous keyboard state
        Texture2D pauseScreen, resumeButton, options, mainMenu, exit , start, p1Tex, p2Tex, gameBoard, Level1, blueStone, orangeStone, cursorTex, optionsScreen, noTexture, arrowRight;  // base pause screen
        BinaryReader read;
        double playerSpeed1, playerSpeed2;
        int player1InitialX, player1InitialY, player2InitialX, player2InitialY;
        Random rand; //Random object
        List<Stone> stonesList; //list of stones
        Texture2D stoneTex; //gray stone texture
        SpriteFont font;
        double time;
        int p1Count;
        int p2Count;
        //List of blue orbs
        List<Orb> blueOrbs;
        Texture2D bOrbTex;
        //List of orange orbs
        List<Orb> orangeOrbs;
        Texture2D oOrbTex;
        const int NUMBER_OF_STONES = 25;
        const int WIDTH_OF_STONES = 30;
        const int ORB_WIDTH = 5;
        const int ORB_SPEED = 3;
        bool cameFromMenu = false;
        string[] resolution = new string[] { "1920x1080", "1600x900", "1366x768", "1280x720", "1920x1200", "1680x1050", "1440x900", "1280x800", "1920x1440", "1856x1392", "1600x1200", "1440x1080", "1280x960", "1024x768" };
        int location;
        //List that will contain different direction sprite for players
        List<Texture2D> player1Sprites;
        List<Texture2D> player2Sprites;

        //Animation object to animate the player's movement based on direction key
        Animation player1Animation;
        Animation player2Animation;
        Texture2D tempSprite;

        enum GameStates { Menu, Pause, Options, InGame, GameOver }; //Enum for game states
        GameStates currentGameState; //Attribute for current game state

        #endregion

        #region game1
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Changes resolution - Default resolution is 800x480 -- This code changes it to 1000x800

            //graphics.IsFullScreen = true;
            //this.IsMouseVisible = true;
        }
        #endregion

        #region initialize
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

            try
            {
                location = Array.IndexOf(resolution, "1920x1080");
            }
            catch(Exception)
            {
                location = 0;
            }

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

            //Empty lists for orbs
            blueOrbs = new List<Orb>();
            orangeOrbs = new List<Orb>();

            //stoneColor = Color.White;
            //this.IsMouseVisible = true;
            base.Initialize();
        }
        #endregion

        #region loadContent
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
            arrowRight = Content.Load<Texture2D>("arrowRight");
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

            bOrbTex = Content.Load<Texture2D>("BlueOrb");

            oOrbTex = Content.Load<Texture2D>("OrangeOrb");

            optionsScreen = Content.Load<Texture2D>("OptionsScreen");

            noTexture = Content.Load<Texture2D>("notexture");
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
        #endregion

        #region UnloadContent
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        #endregion

        #region update
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            // TODO: Add your update logic here
            //Current mouse state
            currMState = Mouse.GetState();

            //Keyboard object
            currKbState = Keyboard.GetState();

            // improved finite state machine
            switch (currentGameState)
            {
                #region Menu
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
                        startActive = false;//Make the button inactive

                    if (cursorRect.Intersects(new Rectangle(190, 450, 392, 103)))
                        if (currMState.LeftButton == ButtonState.Pressed)
                        {
                            currentGameState = GameStates.Options;
                            cameFromMenu = true;
                        }


                    if (cursorRect.Intersects(new Rectangle(190, 750, exit.Width, exit.Height)))
                        if (currMState.LeftButton == ButtonState.Pressed)
                            this.Exit();
                    //Sets total game time
                    time = 61;
                    break;
                #endregion
                #region pause
                case GameStates.Pause:  // options menu state

                    // determines if game state is changed to InGame when player clicks on the Resume rectangle
                    if (cursorRect.Intersects(new Rectangle(GraphicsDevice.Viewport.Width / 3, 150, GraphicsDevice.Viewport.Width / 3, 150)))
                        if (currMState.LeftButton == ButtonState.Pressed)
                            currentGameState = GameStates.InGame;
                    // determines if game state is changed to Menu when player clicks on the Main Menu rectangle 
                    if (cursorRect.Intersects(new Rectangle(GraphicsDevice.Viewport.Width / 3, 550, GraphicsDevice.Viewport.Width / 3, 150)))
                        if (currMState.LeftButton == ButtonState.Pressed)
                            currentGameState = GameStates.Menu;
                    if (cursorRect.Intersects(new Rectangle(GraphicsDevice.Viewport.Width / 3, 350, GraphicsDevice.Viewport.Width / 3, 150)))
                        if (currMState.LeftButton == ButtonState.Pressed)
                            currentGameState = GameStates.Options;
                    // determines if game is exited when player clicks on the Exit rectangle
                    if (cursorRect.Intersects(new Rectangle(GraphicsDevice.Viewport.Width / 3, 750, GraphicsDevice.Viewport.Width / 3, 150)))
                        if (currMState.LeftButton == ButtonState.Pressed)
                            this.Exit();
                    break;
                #endregion
                #region ingame
                case GameStates.InGame:  // gameplay state
                    //Uses the animation class to process the input from keyboard as well as updating the rectangle's position according to direction pressed
                    player1Animation.processInput(currKbState, p1, Keys.W, Keys.A, Keys.S, Keys.D);
                    p1.PlayerRect = player1Animation.PlayerPositionRectangle;
                    player2Animation.processInput(currKbState, p2, Keys.Up, Keys.Left, Keys.Down, Keys.Right);
                    p2.PlayerRect = player2Animation.PlayerPositionRectangle;

                    // prevents players from passing beyond the border
                    ScreenBorder(p1);
                    ScreenBorder(p2);
                    MoveStones(stonesList);
                    
                    //Creates orbs - Cooldown is only 1 update loop currently
                    if ((currKbState.IsKeyDown(Keys.Space) && (prevKbState.IsKeyDown(Keys.Space) == false))) //P1 Shoots
                    {
                        Orb o1 = new Orb(bOrbTex, 0, 0, p1, player1Animation, ORB_WIDTH, ORB_SPEED);
                        blueOrbs.Add(o1);
                    }
                    if ((currKbState.IsKeyDown(Keys.RightShift) && (prevKbState.IsKeyDown(Keys.RightShift) == false))) //P2 Shoots
                    {
                        Orb o2 = new Orb(oOrbTex, 0, 0, p2, player2Animation, ORB_WIDTH, ORB_SPEED);
                        blueOrbs.Add(o2);
                    }

                    //Update orb locations
                    for (int i = 0; i < blueOrbs.Count; i++)
                        blueOrbs[i].UpdateOrbs();
                    for (int i = 0; i < orangeOrbs.Count; i++)
                        orangeOrbs[i].UpdateOrbs();

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
                        currentGameState = GameStates.GameOver;

                    if (currKbState.IsKeyDown(Keys.P) || currKbState.IsKeyDown(Keys.Escape)) { currentGameState = GameStates.Pause; }

                    break;
                #endregion
                #region options
                case GameStates.Options:
                    int z = (GraphicsDevice.Viewport.Width / 8);
                    if (cursorRect.Intersects(new Rectangle((z + z + 10 + GraphicsDevice.Viewport.Width / 6) - 80, 175, 80, 100)))
                        if (currMState.LeftButton == ButtonState.Pressed && prevState.LeftButton == ButtonState.Released)
                        {
                            if (location == 13)
                                return;
                            location += 1;
                        }
                    if(cursorRect.Intersects(new Rectangle((z + z + 10), 175, 80, 100)))
                        if(currMState.LeftButton == ButtonState.Pressed && prevState.LeftButton == ButtonState.Released)
                        {
                            if (location == 0)
                                return;
                            location -= 1;
                        }
                    prevState = currMState;
                    break;
                #endregion
                #region gameover
                case GameStates.GameOver:  // end of game state
                    if (currKbState.IsKeyDown(Keys.Space))
                    {
                        currentGameState = GameStates.Menu;
                    }
                    break;
                    #endregion
            }
            //Previous Keyboard state object
            prevKbState = currKbState;

            base.Update(gameTime);
        }
        #endregion

        #region Draw
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
                #region menu
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
                #endregion
                #region ingame
                case GameStates.InGame:
                    spriteBatch.Draw(Level1, windowsize, Color.White);
                    DrawStones(stonesList);
                    player1Animation.drawAnimation(spriteBatch);
                    player2Animation.drawAnimation(spriteBatch);
                    //Draw orbs
                    for (int i = 0; i < blueOrbs.Count; i++)
                    {
                        blueOrbs[i].DrawOrbs(spriteBatch);
                    }
                    for (int i = 0; i < orangeOrbs.Count; i++)
                    {
                        orangeOrbs[i].DrawOrbs(spriteBatch);
                    }

                    spriteBatch.DrawString(font, "TIME: " + (int)time, new Vector2(890, 45), Color.White);
                    spriteBatch.DrawString(font, "P1 Score: " + p1Count, new Vector2(120, 45), Color.White);
                    spriteBatch.DrawString(font, "P2 Score: " + p2Count, new Vector2(1650, 45), Color.White);
                    break;
                #endregion
                #region pause
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

                    spriteBatch.DrawString(font, ""+p1Count, new Vector2(275, 110), Color.White, 0f, Vector2.Zero, 2, SpriteEffects.None, 0);
                    spriteBatch.DrawString(font, "" + p2Count, new Vector2(1650, 110), Color.White, 0f, Vector2.Zero, 2, SpriteEffects.None, 0);
                    spriteBatch.DrawString(font, "TIME: " + (int)time, new Vector2(850, 110), Color.White, 0f, Vector2.Zero, 2, SpriteEffects.None, 0);

                    spriteBatch.Draw(cursorTex, cursorRect, Color.White);
                    break;
                #endregion
                #region options
                case GameStates.Options:
                    int x = (GraphicsDevice.Viewport.Width / 8);
                    spriteBatch.Draw(optionsScreen, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);  // draws pause screen
                    Rectangle rectangle = new Rectangle(x, 50, x, 100);
                    spriteBatch.Draw(noTexture, rectangle, Color.Black);
                    Vector2 size  = font.MeasureString("Options");
                    Vector2 bounds = Center(rectangle);
                    Vector2 origin = size * 0.5f;
                    spriteBatch.DrawString(font, "Options", bounds, Color.White, 0, origin, 1, SpriteEffects.None, 0);
                    rectangle = new Rectangle(x, 175, x, 100);
                    spriteBatch.Draw(noTexture, rectangle, Color.Black);
                    size = font.MeasureString("Resolution");
                    bounds = Center(rectangle);
                    origin = size * 0.5f;
                    spriteBatch.DrawString(font, "Resolution", bounds, Color.White, 0, origin, 1, SpriteEffects.None, 0);
                    rectangle = new Rectangle(x +x + 10, 175, GraphicsDevice.Viewport.Width / 6, 100);
                    size = font.MeasureString("XXXXxXXXX");
                    bounds = Center(rectangle);
                    origin = size * 0.5f;
                    spriteBatch.Draw(noTexture, rectangle, Color.Black);
                    spriteBatch.Draw(arrowRight, new Rectangle((x + x + 10 + GraphicsDevice.Viewport.Width / 6) - 80, 175, 80, 100), Color.White);
                    spriteBatch.Draw(arrowRight, null, new Rectangle((x + x + 10), 175, 80, 100), null, null, 0, null, Color.White, SpriteEffects.FlipHorizontally, 0 );
                    spriteBatch.DrawString(font, resolution[location], bounds, Color.White, 0, origin, 1, SpriteEffects.None, 0);
                    spriteBatch.Draw(cursorTex, cursorRect, Color.White);
                    break;
                #endregion
                #region gameover
                case GameStates.GameOver:
                    spriteBatch.DrawString(font, "Press Space to go to Menu", new Vector2(500, 400), Color.Black);
                    break;
                    #endregion
            }
            //End spritebatch
            spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion

        #region Movestones
        private void MoveStones(List<Stone> stonesList)
        {
            for (int i = 0; i < stonesList.Count; i++)
            {
                stonesList[i].XPos += (int)stonesList[i].Direction.X;
                stonesList[i].YPos += (int)stonesList[i].Direction.Y;
            }
        }
        #endregion

        #region screenborder
        //Screen wrapping method for the player objects
        void ScreenBorder(Player pl)
        {
            //Keep the player on the screen at all times
            //If the player is all the way to the left of the screen, prevent it from moving too far to the left
            if (pl.X < 0)
                pl.X = 0;
            //If the player is all the way to the right of the screen, prevent it from moving too far to the right
            if (pl.X > GraphicsDevice.Viewport.Width - p1.PlayerRect.Width)
                pl.X = GraphicsDevice.Viewport.Width - p1.PlayerRect.Width;
            //If the player is all the way below the screen, prevent it from moving too far down
            if (pl.Y > GraphicsDevice.Viewport.Height - p1.PlayerRect.Height)
                pl.Y = GraphicsDevice.Viewport.Height - p1.PlayerRect.Height;
            //If the player is all the way above the screen, prevent it from moving too far up
            if (pl.PlayerRect.Y < 0)
                pl.Y = 0;
        }
        #endregion

        #region createstones
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
        #endregion

        #region drawstones
        //Method to draw all stones after they have been created using the CreateStones method - Needs to be added to significantly when orbs are introduced
        //Make sure to use this method inside a spritebatch
        void DrawStones(List<Stone> sl)
        {
            for (int i = 0; i < sl.Count; i++)  //For each stone in the stone list
            {
                spriteBatch.Draw(stonesList[i].StoneTex, new Rectangle(sl[i].XPos, sl[i].YPos, WIDTH_OF_STONES, WIDTH_OF_STONES), Color.White); //draw it
            }
        }
        #endregion

        #region center
        private Vector2 Center(Rectangle rect)
        {
            return new Vector2(rect.Left + rect.Width / 2,
                             rect.Top + rect.Height / 2);
        }
        #endregion
    }
}