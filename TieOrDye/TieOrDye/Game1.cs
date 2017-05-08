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
        //Constants 
        const int NUMBER_OF_STONES = 40;
        const int WIDTH_OF_STONES = 30;
        const int ORB_WIDTH = 10;
        const int ORB_SPEED = 3 * 2;
        const int BLUE_STOPPER = 250;
        const int ORANGE_STOPPER = 250;
        const int PLAYER_STUN_DURATION = 2000;
        const int PLAYER_IMMUNITY_DURATION = 3000;

        //Variables to store the player speed and positions
        double playerSpeed1, playerSpeed2;
        int player1InitialX, player1InitialY, player2InitialX, player2InitialY;

        //Timer of the game
        double time;

        //Time players get stunned
        Stopwatch p1StunWatch;
        Stopwatch p2StunWatch;

        //Attribute and enum for the game state
        enum GameStates { Menu, Pause, Options, ControlOptions, InGame, GameOver, Instructions };
        GameStates currentGameState;

        //Random object
        Random rand;

        //Player Object
        Player p1;
        Player p2;

        //Player count
        int p1Count;
        int p2Count;

        float volumeLevel = 0f;
        double dblVolumeLevel = 1;

        //Game interface and draw variable
        GraphicsDeviceManager graphics;
        GameWindow viewport;
        SpriteBatch spriteBatch;
         
        //Reader for the tool
        BinaryReader read;

        //Rectangle that stores the window resolution of user
        Rectangle windowsize;

        //Font for the game
        SpriteFont font;

        bool cameFromMenu = false;
        string[] resolution = new string[] { "1920x1080", "1600x900", "1920x1200", "1680x1050", "1440x900", "1920x1440", "1856x1392", "1600x1200", "1440x1080", "1280x960"};
        int location;

        Texture2D pauseScreen, resumeButton, options, mainMenu, exit , start, p1Tex, p2Tex, gameBoard, Level1, blueStone, orangeStone, cursorTex, optionsScreen, noTexture, arrowRight;  // base pause screen
        Texture2D instructions, instructionsButton, gameover;

        //list of walls
        List<Wall> walls;
        Texture2D wallTex;

        //List of blue orbs
        List<Orb> blueOrbs;
        Texture2D bOrbTex;

        //List of orange orbs
        List<Orb> orangeOrbs;
        Texture2D oOrbTex;

        //List and texture of the stones
        List<Stone> stonesList; 
        Texture2D stoneTex;

        //list for a grid
        List<Rectangle> grid;

        //List that will contain different direction sprite for players
        List<Texture2D> player1Sprites;
        List<Texture2D> player2Sprites;

        //Animation object to animate the player's movement based on direction key
        Animation player1Animation;
        Animation player2Animation;
        Texture2D tempSprite;

        //Mouse state and rectangle to store cursor position
        MouseState currMState;
        MouseState prevState;
        Rectangle cursorRect;

        //Keyboard state
        KeyboardState currKbState;
        KeyboardState prevKbState;

        //Buttons are 392 x 103
        //Gameboard inactive and active start button, along with boolean if button is active
        Texture2D inactiveStartButtonTex;
        Texture2D activeStartButtonTex;

        //Sound effect and sound
        SoundEffect walkSound;

        //Attribute for player shooting sound effect
        SoundEffect laserSoundEffect;
        SoundEffectInstance laserSoundEffectInstance;

        //Textures for the game item
        Texture2D inverterImage;
        Texture2D speedupImage;

        bool startActive;
        bool orangeShot = false;
        bool blueShot = false;
        bool fromPauseMenu = false;

        Stopwatch blueStopper;
        Stopwatch orangeStopper;
        Stopwatch endGameTimer;

        Song song;
        int playernumber = 1;

        Keys p1Up, p1Down, p1Left, p1Right, p1Shoot, p2Up, p2Down, p2Left, p2Right, p2Shoot;


        int mouseOffsetX;
        int mouseOffsetY;

        Item item1;
        Item item2;
        double effectTime;
        int firstItemCount;
        bool check1;
        bool check2;
        bool check3;
        bool check4;
        bool fromMenu;
        int width;
        int height;
        int gridX;
        int gridY;
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
            graphics.IsFullScreen = false;
            //Window.IsBorderless = true;
            graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            Debug.WriteLine(graphics.PreferredBackBufferFormat.ToString());

            grid = new List<Rectangle>();
            gridX = graphics.PreferredBackBufferWidth / 10;
            gridY = graphics.PreferredBackBufferHeight / 10;
            for (int j = 0; j < graphics.PreferredBackBufferHeight; j += gridY)
            {
                for (int i = 0; i < graphics.PreferredBackBufferWidth; i += gridX)
                {
                    grid.Add(new Rectangle(i, j, gridX, gridY));
                }

            }

            //Window.IsBorderless = true;
            graphics.ApplyChanges();
            ResetMouseOffsets();

            //Keybinding of both players
            p1Up = Keys.W;
            p1Down = Keys.S;
            p1Left = Keys.A;
            p1Right = Keys.D;
            p1Shoot = Keys.Space;

            p2Up = Keys.Up;
            p2Down = Keys.Down;
            p2Left = Keys.Left;
            p2Right = Keys.Right;
            p2Shoot = Keys.RightShift;

            item1 = new Item(inverterImage, -1000, -1000, 100, 1);
            item2 = new Item(speedupImage, -1000, -1000, 100, 3);

            fromMenu = true;
            effectTime = 6;
            firstItemCount = 0;
            check1 = false;
            check2 = false;
            check3 = false;
            check4 = false;
            width = GraphicsDevice.Viewport.Width;
            height = GraphicsDevice.Viewport.Height;

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

            //Walls 
            walls = new List<Wall>();

            walls.Add(new Wall(wallTex, grid[22].X, grid[22].Y, gridX, gridY * 2));
            walls.Add(new Wall(wallTex, grid[27].X, grid[27].Y, gridX, gridY * 2));
            walls.Add(new Wall(wallTex, grid[62].X, grid[62].Y, gridX, gridY * 2));
            walls.Add(new Wall(wallTex, grid[67].X, grid[67].Y, gridX, gridY * 2));
            walls.Add(new Wall(wallTex, grid[44].X, grid[44].Y, gridX * 2, gridY * 2));
            walls.Add(new Wall(wallTex, grid[0].X, grid[0].Y, gridX * 10, gridY)); 
            walls.Add(new Wall(wallTex, grid[0].X, grid[0].Y, gridX, gridY * 10));
            walls.Add(new Wall(wallTex, grid[9].X, grid[9].Y, gridX, gridY * 10));
            walls.Add(new Wall(wallTex, grid[90].X, grid[90].Y, gridX * 10, gridY));

            //Stunwatches
            p1StunWatch = new Stopwatch();
            p2StunWatch = new Stopwatch();

            //Stopwatch for end game
            endGameTimer = new Stopwatch();

            //stoneColor = Color.White;
            //this.IsMouseVisible = true;
            ResetMouseOffsets();
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
            //instructions button texture
            instructionsButton = Content.Load<Texture2D>("InstructionsButton");
            //instructions screen texture
            instructions = Content.Load<Texture2D>("InstructionsScreen");
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

            gameover = Content.Load<Texture2D>("GameOverScreen");

            wallTex = Content.Load<Texture2D>("Transparent");
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

            //Load the sound effect for the player
            walkSound = Content.Load<SoundEffect>("Thump");

            //Sound effect for player shooting
            laserSoundEffect = Content.Load<SoundEffect>("laserSoundEffect");
            laserSoundEffectInstance = laserSoundEffect.CreateInstance();

            //Load the images used for the items in game
            inverterImage = Content.Load<Texture2D>("Item_Inverter");
            speedupImage = Content.Load<Texture2D>("Item_SpeedUp");



            song = Content.Load<Song>("music");
            MediaPlayer.Play(song);
            MediaPlayer.Volume = volumeLevel;
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

            int width4 = (int)(gridX * 1.5);
            int height2 = (int)(gridY * .9);
            int width3 = (int)(gridX * .9);
            // improved finite state machine
            switch (currentGameState)
            {
                #region Menu
                case GameStates.Menu:  // start menu state
                    if (cursorRect.Intersects(new Rectangle(grid[21].X, grid[21].Y, (gridX * 2), gridY)))
                    {
                        //Make the button active
                        startActive = true;
                        //Check for clicking button
                        if (buttonPress())
                        {
                            p1.X = player1InitialX;
                            p1.Y = player1InitialY;

                            p2.X = player2InitialX;
                            p2.Y = player2InitialY;

                            p1Count = 0;
                            p2Count = 0;
                            firstItemCount = 0;

                            blueOrbs.Clear();
                            orangeOrbs.Clear();

                            item1.OrbC = new Circle(0, 0, 0);
                            item2.OrbC = new Circle(0, 0, 0);

                            //Create stone objects
                            CreateStones(NUMBER_OF_STONES, WIDTH_OF_STONES);
                            //Start game
                            currentGameState = GameStates.InGame;
                        }
                    }
                    else  //When the mouse is no longer hovering over the button
                        startActive = false;//Make the button inactive

                    if (cursorRect.Intersects(new Rectangle(grid[41].X, grid[41].Y, gridX * 2, gridY)))
                        if (buttonPress())
                        {
                            currentGameState = GameStates.Options;
                            cameFromMenu = true;
                        }


                    if (cursorRect.Intersects(new Rectangle(grid[61].X, grid[61].Y, gridX * 2, gridY)))
                        if (buttonPress())
                            currentGameState = GameStates.Instructions;

                    if (cursorRect.Intersects(new Rectangle(grid[81].X, grid[81].Y, gridX * 2, gridY)))
                        if (buttonPress())
                            this.Exit();
                    
                    //Sets total game time
                    prevState = currMState;
                    time = 60;
                    fromMenu = true;
                    break;
                #endregion
                #region ingame
                case GameStates.InGame:  // gameplay state
                    //Uses the animation class to process the input from keyboard as well as updating the rectangle's position according to direction pressed
                    player1Animation.processInput(currKbState, p1, p1Up, p1Left, p1Down, p1Right, walkSound, gameTime);
                    p1.PlayerRect = player1Animation.PlayerPositionRectangle;
                    player2Animation.processInput(currKbState, p2, p2Up, p2Left, p2Down, p2Right, walkSound, gameTime);
                    p2.PlayerRect = player2Animation.PlayerPositionRectangle;

                    // prevents players from passing beyond the border
                    ScreenBorder(p1);
                    ScreenBorder(p2);
                    MoveStones(stonesList);
                    DoWallCollision();

                    //Creates orbs - Cooldown can be changed 
                    CreateOrbs();

                    //Update orb locations
                    UpdateOrbs();

                    //Stuns player when hit by opposite orb
                    StunPlayer();

                    //On collsion: changes stone texture, deletes orb and increments backwards in orb list
                    OrbCollide();

                    //calculates stun time
                    StunTimer();

                    time -= gameTime.ElapsedGameTime.TotalSeconds;
                    if (time <= 0)
                    {
                        currentGameState = GameStates.GameOver;
                        endGameTimer.Start();
                    }

                    if (currKbState.IsKeyDown(Keys.P) || currKbState.IsKeyDown(Keys.Escape))
                    {
                        currentGameState = GameStates.Pause;
                        cameFromMenu = false;
                    }

                    break;
                #endregion
                #region pause
                case GameStates.Pause:  // options menu state
                    fromMenu = false;
                    // determines if game state is changed to InGame when player clicks on the Resume rectangle
                    if (cursorRect.Intersects(new Rectangle(grid[13].X + (int)(gridX * .5), grid[13].Y, gridX * 3, gridY)))
                        if (buttonPress())
                            currentGameState = GameStates.InGame;
                    // determines if game state is changed to Menu when player clicks on the Main Menu rectangle 
                    if (cursorRect.Intersects(new Rectangle(grid[33].X + (int)(gridX * .5), grid[33].Y, gridX * 3, gridY)))
                        if (buttonPress())
                            currentGameState = GameStates.Menu;
                    //determines if options is pressed
                    if (cursorRect.Intersects(new Rectangle(grid[53].X + (int)(gridX * .5), grid[53].Y, gridX * 3, gridY)))
                        if (buttonPress())
                            currentGameState = GameStates.Options;
                    // determines if game state is changed to Instructions when player clicks Instructions button
                    if (cursorRect.Intersects(new Rectangle(grid[73].X + (int)(gridX * .5), grid[73].Y, gridX * 3, gridY)))
                        if (buttonPress())
                        {
                            currentGameState = GameStates.Instructions;
                            fromPauseMenu = true;
                        }
                            
                    // determines if game is exited when player clicks on the Exit rectangle
                    if (cursorRect.Intersects(new Rectangle(grid[93].X + (int)(gridX * .5), grid[93].Y, gridX * 3, gridY)))
                        if (buttonPress())
                            this.Exit();
                        prevState = currMState;
                    break;
                #endregion
                #region instructions
                case GameStates.Instructions:
                    if (prevKbState.IsKeyUp(Keys.Escape) && currKbState.IsKeyDown(Keys.Escape))
                    {
                        if(fromPauseMenu == true)
                        {
                            currentGameState = GameStates.Pause;
                            fromPauseMenu = false;
                        }
                        else
                            currentGameState = GameStates.Menu;
                    }
                    break;
                #endregion
                #region options
                case GameStates.Options:

                    if (cursorRect.Intersects(new Rectangle(grid[23].X + width4 - 80, grid[23].Y, 80, height2)))
                        if (buttonPress()) //resolution right
                        {
                            if (location == 9)
                                return;
                            location += 1;
                        }
                    if(cursorRect.Intersects(new Rectangle(grid[23].X, grid[23].Y, 80, height2)))
                        if(buttonPress()) //resolution left
                        {
                            if (location == 0)
                                return;
                            location -= 1;
                        }
                    if(cursorRect.Intersects(new Rectangle(grid[33].X + width4 - 80, grid[33].Y, 80, height2)))
                        if(buttonPress()) //volume right
                        {
                            if(dblVolumeLevel > .9)
                                return;
                            dblVolumeLevel += .1;
                            volumeLevel = (float)dblVolumeLevel;
                        }
                    if(cursorRect.Intersects(new Rectangle(grid[33].X, grid[33].Y, 80, height2)))
                        if(buttonPress()) //volume left
                        {
                            dblVolumeLevel -= .1;
                            volumeLevel = (float)dblVolumeLevel;
                            if (dblVolumeLevel < .1)
                            {
                                dblVolumeLevel = 0;
                                volumeLevel = 0f;
                                return;
                            }
                        }
                    if (cursorRect.Intersects(new Rectangle(grid[41].X, grid[41].Y, width4, height2)))
                        if (buttonPress()) //controls button
                            currentGameState = GameStates.ControlOptions;
                    if (cursorRect.Intersects(new Rectangle(grid[51].X, grid[51].Y, width3, height2)))
                        if (buttonPress()) //the apply button
                        {
                            if (resolution[location] == graphics.PreferredBackBufferWidth.ToString() + "x" + graphics.PreferredBackBufferHeight.ToString())
                                return;
                            if (time < 60)
                                return;
                            string res = resolution[location]; //sets the string to the string of new resolution
                            var tempArr = res.Split('x'); //splits it by the char x
                            //int width = int.Parse(tempArr[0]); //sets width to the first parse
                            //int height = int.Parse(tempArr[1]); //sets height to the second parse
                            this.width = int.Parse(tempArr[0]);
                            this.height = int.Parse(tempArr[1]);

                            if (width > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width || height > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height)
                                return;

                            graphics.PreferredBackBufferWidth = width; //sets width and height accordingly
                            graphics.PreferredBackBufferHeight = height;
                            graphics.ApplyChanges(); //applys the change
                            ResetGrid();
                            ResetMouseOffsets();
                        }
                    if (cursorRect.Intersects(new Rectangle(grid[52].X, grid[52].Y, width3, height2))) //back button
                        if (buttonPress())
                            if (cameFromMenu)
                                currentGameState = GameStates.Menu;
                            else
                                currentGameState = GameStates.Pause;
                    MediaPlayer.Volume = volumeLevel;
                    prevState = currMState;
                    break;
                #endregion
                #region control options
                case GameStates.ControlOptions:
                    if (cursorRect.Intersects(new Rectangle(grid[21].X, grid[21].Y, 80, height2)))
                        if (buttonPress())
                        {
                            if (playernumber == 1)
                                return;
                            playernumber -= 1;
                        }
                    if (cursorRect.Intersects(new Rectangle(grid[21].X + width4 * 2 - 80, grid[21].Y, 80, height2)))
                        if(buttonPress())
                        {
                            if (playernumber == 2)
                                return;
                            playernumber += 1;
                        }
                    if(playernumber == 1)
                    {
                        if (cursorRect.Intersects(new Rectangle(grid[32].X, grid[32].Y, width3, height2)))
                            if (buttonPressForCfg()) //changes up button
                                checkButtonPress(p1Up, "p1Up");
                        if (cursorRect.Intersects(new Rectangle(grid[42].X, grid[42].Y, width3, height2)))
                            if (buttonPressForCfg()) //changes left button
                                checkButtonPress(p1Down, "p1Down");
                        if (cursorRect.Intersects(new Rectangle(grid[52].X, grid[52].Y, width3, height2)))
                            if (buttonPressForCfg()) //changes left button
                                checkButtonPress(p1Left, "p1Left");
                        if (cursorRect.Intersects(new Rectangle(grid[62].X, grid[62].Y, width3, height2)))
                            if (buttonPressForCfg()) //changes right button
                                checkButtonPress(p1Right, "p1Right");
                        if(cursorRect.Intersects(new Rectangle(grid[72].X, grid[72].Y, width3, height2)))
                            if (buttonPressForCfg()) //changes shoot button
                                checkButtonPress(p1Shoot, "p1Shoot");
                    }
                    else //player 2
                    {
                        if (cursorRect.Intersects(new Rectangle(grid[32].X, grid[32].Y, width3, height2)))
                            if (buttonPressForCfg()) //changes up button
                                checkButtonPress(p2Up, "p2Up");
                        if (cursorRect.Intersects(new Rectangle(grid[42].X, grid[42].Y, width3, height2)))
                            if (buttonPressForCfg()) //changes down button
                                checkButtonPress(p2Down, "p2Down");
                        if (cursorRect.Intersects(new Rectangle(grid[52].X, grid[52].Y, width3, height2)))
                            if (buttonPressForCfg()) //changes left button
                                checkButtonPress(p2Left, "p2Left");
                        if (cursorRect.Intersects(new Rectangle(grid[62].X, grid[62].Y, width3, height2)))
                            if (buttonPressForCfg()) //changes right button
                                checkButtonPress(p2Right, "p2Right");
                        if (cursorRect.Intersects(new Rectangle(grid[72].X, grid[72].Y, width3, height2)))
                            if (buttonPressForCfg()) //changes shoot button
                                checkButtonPress(p2Shoot, "p2Shoot");
                    }
                    if (cursorRect.Intersects(new Rectangle(grid[81].X, grid[81].Y, width3, height2)))
                        if (buttonPress())
                            currentGameState = GameStates.Options;
                    prevState = currMState;
                    break;
                #endregion
                #region gameover
                case GameStates.GameOver:  // end of game state
                    //Add wait period to prevent accidentally leaving the game screen
                    int endGameDelay = 3000; //Delay period

                    if(endGameTimer.ElapsedMilliseconds >= endGameDelay)
                    {
                        if (currKbState.IsKeyDown(Keys.Space))
                        {
                            currentGameState = GameStates.Menu;
                            endGameTimer.Reset();
                        }
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

            var mouse = Mouse.GetState();
            var mousePos = GetMousePosition();
            cursorRect = new Rectangle((int)mousePos.X, (int)mousePos.Y, 20, 20);
            int width2 = (int)(gridX * 1.5);
            int height2 = (int)(gridY * .9);
            int width3 = (int)(gridX * .9);

            switch (currentGameState)
            {
                #region menu
                case GameStates.Menu:
                    //Draw gameboard
                    spriteBatch.Draw(gameBoard, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

                    makeBox(new Rectangle(grid[21].X, grid[21].Y, (gridX * 2), gridY), "", start, Color.White, true); //start button

                    makeBox(new Rectangle(grid[41].X, grid[41].Y, gridX * 2, gridY), "", options, Color.White, true); // options button

                    makeBox(new Rectangle(grid[61].X, grid[61].Y, gridX * 2, gridY), "", instructionsButton, Color.Gray, true); //instructions button

                    makeBox(new Rectangle(grid[81].X, grid[81].Y, gridX * 2, gridY), "", exit, Color.White, true); //exit button
                    //Draw cursor
                    spriteBatch.Draw(cursorTex, cursorRect, Color.White);  // draws cursor
                    break;
                #endregion
                #region ingame
                case GameStates.InGame:
                    spriteBatch.Draw(Level1, windowsize, Color.White);
                    spriteBatch.Draw(Level1, new Rectangle(0, 0, this.width, this.height), Color.White);
                    DrawStones(stonesList);

                    player1Animation.drawAnimation(spriteBatch, p1, 1);
                    player2Animation.drawAnimation(spriteBatch, p2, 2);

                    checkItemIntersects(item1, gameTime);
                    checkItemIntersects(item2, gameTime);

                    

                    //Draw orbs
                    for (int i = 0; i < blueOrbs.Count; i++)
                    {
                        blueOrbs[i].DrawOrbs(spriteBatch);
                    }
                    for (int i = 0; i < orangeOrbs.Count; i++)
                    {
                        orangeOrbs[i].DrawOrbs(spriteBatch);
                    }

                    spriteBatch.DrawString(font, "TIME: " + (int)time, new Vector2(grid[4].X + 50, grid[5].Y + 50), Color.White);
                    spriteBatch.DrawString(font, "P1 Score: " + p1Count, new Vector2(grid[1].X, grid[1].Y + 50), Color.White);
                    spriteBatch.DrawString(font, "P2 Score: " + p2Count, new Vector2(grid[8].X, grid[8].Y + 50), Color.White);
                    break;
                #endregion
                #region pause
                case GameStates.Pause:  // draws pause screen

                    spriteBatch.Draw(pauseScreen, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);  // draws pause screen

                    // draws resume button 
                    makeBox(new Rectangle(grid[13].X + (int)(gridX * .5), grid[13].Y, gridX * 3, gridY), "", resumeButton, Color.White, true);

                    // draws main menu button
                    makeBox(new Rectangle(grid[33].X + (int)(gridX * .5), grid[33].Y, gridX * 3, gridY), "", mainMenu, Color.White, true);

                    // draws options button 
                    makeBox(new Rectangle(grid[53].X + (int)(gridX * .5), grid[53].Y, gridX * 3, gridY), "", options, Color.White, true);

                    // draws instruction button
                    makeBox(new Rectangle(grid[73].X + (int)(gridX * .5), grid[73].Y, gridX * 3, gridY), "", instructionsButton, Color.White, true);

                    //draws exit button
                    makeBox(new Rectangle(grid[93].X + (int)(gridX * .5), grid[93].Y, gridX * 3, gridY), "", exit, Color.White, true);

                    // draws player models on either side of screen
                    spriteBatch.Draw(p1Tex, new Rectangle(grid[0].X + 10, grid[10].Y + 10, gridX * 2, gridY * 7), Color.White);
                    spriteBatch.Draw(p2Tex, new Rectangle(grid[7].X + (int)(gridX * .5), grid[17].Y + 10, gridX * 2, gridY * 7), Color.White);

                    spriteBatch.DrawString(font, ""+p1Count, new Vector2(grid[1].X, grid[1].Y + 50), Color.White, 0f, Vector2.Zero, 2, SpriteEffects.None, 0);
                    spriteBatch.DrawString(font, "" + p2Count, new Vector2(grid[8].X, grid[8].Y + 50), Color.White, 0f, Vector2.Zero, 2, SpriteEffects.None, 0);
                    spriteBatch.DrawString(font, "TIME: " + (int)time, new Vector2(grid[4].X + 50, grid[5].Y + 50), Color.White, 0f, Vector2.Zero, 2, SpriteEffects.None, 0);

                    spriteBatch.Draw(cursorTex, cursorRect, Color.White);
                    break;
                #endregion
                #region instructions
                case GameStates.Instructions:
                    spriteBatch.Draw(instructions, new Rectangle(0, 0, this.width, this.height), Color.White);
                    
                    
                    spriteBatch.Draw(cursorTex, cursorRect, Color.White);
                    break;
                #endregion instructions
                #region options
                case GameStates.Options:
                    Rectangle rectangle;
                    spriteBatch.Draw(optionsScreen, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White); //draws the background screen

                    rectangle = new Rectangle(grid[11].X, grid[11].Y, width2, height2); //options box
                    makeBox(rectangle, "Options", noTexture, Color.White, false, false);

                    rectangle = new Rectangle(grid[21].X, grid[21].Y, width2, height2); //resolution box
                    if(time < 60)
                        makeBox(rectangle, "Not While In Game", noTexture, Color.White, false, false);
                    else
                        makeBox(rectangle, "Resolution", noTexture, Color.White, false, false);

                    rectangle = new Rectangle(grid[23].X, grid[23].Y, width2, height2); //current resolution box
                    makeBox(rectangle, resolution[location], noTexture, Color.White, true, false);

                    rectangle = new Rectangle(grid[31].X, grid[31].Y, width2, height2); //Volume Box
                    makeBox(rectangle, "Volume", noTexture, Color.White, false, false);

                    rectangle = new Rectangle(grid[33].X, grid[33].Y, width2, height2); //change Volume Box
                    makeBox(rectangle, volumeLevel.ToString(), noTexture, Color.White, true, false);

                    rectangle = new Rectangle(grid[41].X, grid[41].Y, width2, height2); //Controls Box
                    makeBox(rectangle, "Controls", noTexture, Color.White, false, false);

                    rectangle = new Rectangle(grid[51].X, grid[51].Y, width3 , height2); //Apply Box
                    makeBox(rectangle, "Apply", noTexture, Color.White, false, false);

                    rectangle = new Rectangle(grid[52].X, grid[52].Y, width3, height2); //Back Box
                    makeBox(rectangle, "Back", noTexture, Color.White, false, false);

                    spriteBatch.Draw(cursorTex, cursorRect, Color.White);
                    break;
                #endregion
                #region control options
                case GameStates.ControlOptions:

                    spriteBatch.Draw(optionsScreen, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                    width2 = (int)(gridX * 1.5);
                    height2 = (int)(gridY * .9);
                    width3 = (int)(gridX * .9);

                    rectangle = new Rectangle(grid[11].X, grid[11].Y, width2, height2); //keyboard box
                    makeBox(rectangle, "Keyboard", noTexture, Color.White, false, false);

                    rectangle = new Rectangle(grid[21].X, grid[21].Y, width2 * 2, height2); //player box
                    makeBox(rectangle, "Player" + playernumber.ToString(), noTexture, Color.White, true, false);

                    rectangle = new Rectangle(grid[31].X, grid[31].Y, width3, height2); //up box
                    makeBox(rectangle, "Up", noTexture, Color.White, false, false);

                    rectangle = new Rectangle(grid[41].X, grid[41].Y, width3, height2); //down box
                    makeBox(rectangle, "Down", noTexture, Color.White, false, false);

                    rectangle = new Rectangle(grid[51].X, grid[51].Y, width3, height2); //left box
                    makeBox(rectangle, "Left", noTexture, Color.White, false, false);

                    rectangle = new Rectangle(grid[61].X, grid[61].Y, width3, height2); //right box
                    makeBox(rectangle, "Right", noTexture, Color.White, false, false);

                    rectangle = new Rectangle(grid[71].X, grid[71].Y, width3, height2); //shoot box
                    makeBox(rectangle, "Shoot", noTexture, Color.White, false, false);

                    if (playernumber == 1)
                    {
                        rectangle = new Rectangle(grid[32].X, grid[32].Y, width3, height2); //p1UpBox
                        makeBox(rectangle, p1Up.ToString(), noTexture, Color.White, false, true);

                        rectangle = new Rectangle(grid[42].X, grid[42].Y, width3, height2); //p1DownBox
                        makeBox(rectangle, p1Down.ToString(), noTexture, Color.White, false, true);

                        rectangle = new Rectangle(grid[52].X, grid[52].Y, width3, height2); //p1LeftBox
                        makeBox(rectangle, p1Left.ToString(), noTexture, Color.White, false, true);

                        rectangle = new Rectangle(grid[62].X, grid[62].Y, width3, height2); //p1RightBox
                        makeBox(rectangle, p1Right.ToString(), noTexture, Color.White, false, true);

                        rectangle = new Rectangle(grid[72].X, grid[72].Y, width3, height2); //p1ShootBox
                        makeBox(rectangle, p1Shoot.ToString(), noTexture, Color.White, false, true);
                    }
                    else
                    {
                        rectangle = new Rectangle(grid[32].X, grid[32].Y, width3, height2); //p2UpBox
                        makeBox(rectangle, p2Up.ToString(), noTexture, Color.White, false, true);

                        rectangle = new Rectangle(grid[42].X, grid[42].Y, width3, height2); //p2DownBox
                        makeBox(rectangle, p2Down.ToString(), noTexture, Color.White, false, true);

                        rectangle = new Rectangle(grid[52].X, grid[52].Y, width3, height2); //p2LeftBox
                        makeBox(rectangle, p2Left.ToString(), noTexture, Color.White, false, true);

                        rectangle = new Rectangle(grid[62].X, grid[62].Y, width3, height2); //p2RightBox
                        makeBox(rectangle, p2Right.ToString(), noTexture, Color.White, false, true);

                        rectangle = new Rectangle(grid[72].X, grid[72].Y, width3, height2);//p2ShootBox
                        makeBox(rectangle, p2Shoot.ToString(), noTexture, Color.White, false, true);
                    }
                    rectangle = new Rectangle(grid[81].X, grid[81].Y, width3, height2);
                    makeBox(rectangle, "Back", noTexture, Color.White, false, false);
                    spriteBatch.Draw(cursorTex, cursorRect, Color.White);
                    break;
                #endregion
                #region gameover
                case GameStates.GameOver:
                    spriteBatch.Draw(gameover, new Rectangle(0, 0, this.width, this.height), Color.White);
                    spriteBatch.DrawString(font, "P1 Score: " + p1Count, new Vector2(500, 400), Color.Aquamarine);
                    spriteBatch.DrawString(font, "P2 Score: " + p2Count, new Vector2(500, 500), Color.Aquamarine);
                    if (p1Count > p2Count)
                        spriteBatch.DrawString(font, "P1 Wins!", new Vector2(500, 600), Color.Aquamarine);
                    if (p2Count > p1Count)
                        spriteBatch.DrawString(font, "P2 Wins!", new Vector2(500, 600), Color.Aquamarine);
                    if (p1Count == p2Count)
                        spriteBatch.DrawString(font, "Tie game!", new Vector2(500, 600), Color.Aquamarine);
                    break;
                    #endregion
            }
            //End spritebatch
            spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion

        #region helper methods

        #region Movestones
        private void MoveStones(List<Stone> stonesList)
        {
            for (int i = 0; i < stonesList.Count; i++)
            {
                StoneChange(stonesList[i]);

                DoWallCollision(stonesList[i]);

                stonesList[i].XPos += (int)stonesList[i].Direction.X;
                stonesList[i].YPos += (int)stonesList[i].Direction.Y;
            }
        }
        #endregion

        #region StoneChange
        private void StoneChange(Stone stone)
        {
            Circle c1 = new Circle(stone.XPos + (WIDTH_OF_STONES / 2), stone.YPos + (WIDTH_OF_STONES / 2), (WIDTH_OF_STONES / 2));
            foreach(Stone obj in stonesList)
            {
                Circle c2 = new Circle(obj.XPos + (WIDTH_OF_STONES / 2), obj.YPos + (WIDTH_OF_STONES / 2), (WIDTH_OF_STONES / 2));

                if (stone.Equals(obj))
                    return;
                if (c1.Intersects(c2))
                {
                    int x = stone.XPos - obj.XPos;
                    int y = stone.YPos - obj.YPos;//measurments used for later

                    stone.Direction = new Vector2(-stone.Direction.X, -stone.Direction.Y); //since they intersect change their directions
                    obj.Direction = new Vector2(-obj.Direction.X, -obj.Direction.Y);
                    c1.X += (int)stone.Direction.X; //psuedo code to see if applying the direction of the stones would cause them to collide again
                    c1.Y += (int)stone.Direction.Y;
                    c2.X += (int)obj.Direction.X;
                    c2.Y += (int)obj.Direction.Y;
                    if (c1.Intersects(c2)) //if they do collide again
                    {
                        if (stone.Direction.X == 0) { } //add the x or y value of the distance based of the direction they are going
                        else
                            stone.XPos += x;
                        if(obj.Direction.X == 0) { }
                        else
                            obj.XPos += -x;
                        if (stone.Direction.Y == 0) { }
                        else
                            stone.YPos += y;
                        if (obj.Direction.Y == 0) { }
                        else
                            obj.YPos += -y;
                    }
                    else
                        return; //else return
                }
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

                Stone s = new Stone(stoneTex, stoneX, stoneY, stoneWidth);
                Circle c1 = new Circle(s.XPos + (WIDTH_OF_STONES / 2), s.YPos + (WIDTH_OF_STONES / 2), WIDTH_OF_STONES / 2);
                //Check if a stone is already drawn in that location
                for (int j = 0; j < stonesList.Count; j++)   //For each stone in list
                {
                    if ((stoneX >= stonesList[j].X && stoneX <= stonesList[j].X + stoneWidth) && (stoneY >= stonesList[j].Y && stoneY <= stonesList[j].Y + stoneWidth)) //if potential stone's x  and y collides with the stone's x and y coordinates
                    {
                        canCreate = false; //prevent the stone from being created
                    }
                    
                }

                foreach (Wall obj in walls)
                {
                    if (obj.Bounds.Contains(stoneX + WIDTH_OF_STONES, stoneY + WIDTH_OF_STONES))
                        canCreate = false;
                    if (c1.Intersects(obj.Bounds))
                        canCreate = false;
                }

                if (canCreate)//If the stone can be created
                {
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
                var num = rand.Next(0, stonesList.Count);
                if (sl[i].Inverter)
                    spriteBatch.Draw(inverterImage, new Rectangle(sl[i].XPos, sl[i].YPos, WIDTH_OF_STONES, WIDTH_OF_STONES), Color.White); //draw it
                else if (sl[i].RapidFire)
                    spriteBatch.Draw(speedupImage, new Rectangle(sl[i].XPos, sl[i].YPos, WIDTH_OF_STONES, WIDTH_OF_STONES), Color.White); //draw it
                else
                    spriteBatch.Draw(stonesList[i].StoneTex, new Rectangle(sl[i].XPos, sl[i].YPos, WIDTH_OF_STONES, WIDTH_OF_STONES), Color.White); //draw it
                if (firstItemCount == 0)
                {
                    
                    stonesList[num].RapidFire = true;
                    num = rand.Next(0, stonesList.Count);
                    stonesList[num].Inverter = true;
                    firstItemCount++;
                }
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

        #region getMousePosition
        public Vector2 GetMousePosition()
        {
            var ms = OpenTK.Input.Mouse.GetCursorState();
            return new Vector2(ms.X 
                //- mouseOffsetX
                , ms.Y 
                //- mouseOffsetY
                );
        }
        #endregion

        #region ResetMouseOffsets
        public void ResetMouseOffsets()
        {
            mouseOffsetX = (GraphicsDevice.Adapter.CurrentDisplayMode.Width - graphics.PreferredBackBufferWidth);
            mouseOffsetY = (GraphicsDevice.Adapter.CurrentDisplayMode.Height - graphics.PreferredBackBufferHeight);
        }
        #endregion

        #region MakeBox
        public void makeBox(Rectangle rect, string text, Texture2D texture, Color color, bool makearrow, bool buttonConfig) //makes a box
        {
            Vector2 size = font.MeasureString(text);
            Vector2 bounds = Center(rect); //used to center the text in the box
            Vector2 origin = size * 0.5f;

            spriteBatch.Draw(texture, rect, color);

            if (makearrow) //makes arrows
            {
                if (cursorRect.Intersects(new Rectangle(rect.X + rect.Width - 80, rect.Y, 80, rect.Height))) //changes color if the mouse is in the box
                    spriteBatch.Draw(arrowRight, null, new Rectangle(rect.Width + rect.X - 80, rect.Y, 80, rect.Height), null, null, 0, null, Color.Blue, SpriteEffects.None, 0);
                else
                    spriteBatch.Draw(arrowRight, null, new Rectangle(rect.X + rect.Width - 80, rect.Y, 80, rect.Height), null, null, 0, null, color, SpriteEffects.None, 0);
                if (cursorRect.Intersects(new Rectangle(rect.X, rect.Y, 80, rect.Height)))
                    spriteBatch.Draw(arrowRight, null, new Rectangle(rect.X, rect.Y, 80, rect.Height), null, null, 0, null, Color.Blue, SpriteEffects.FlipHorizontally, 0);
                else
                    spriteBatch.Draw(arrowRight, null, new Rectangle(rect.X, rect.Y, 80, rect.Height), null, null, 0, null, color, SpriteEffects.FlipHorizontally, 0);
            }

            if (buttonConfig)
            {
                if (cursorRect.Intersects(rect))
                    if (buttonPressForCfg())
                        spriteBatch.DrawString(font, "...", bounds, Color.Blue, 0, origin, 1, SpriteEffects.None, 0);
                    else
                        spriteBatch.DrawString(font, text, bounds, Color.Blue, 0, origin, 1, SpriteEffects.None, 0);
                else
                    spriteBatch.DrawString(font, text, bounds, Color.White, 0, origin, 1, SpriteEffects.None, 0);
            }
            else if(!buttonConfig)
            {
                if (cursorRect.Intersects(rect)) //changes the color of the text if the cursor is within the box
                    spriteBatch.DrawString(font, text, bounds, Color.Blue, 0, origin, 1, SpriteEffects.None, 0);
                else
                    spriteBatch.DrawString(font, text, bounds, color, 0, origin, 1, SpriteEffects.None, 0);
            }

        }
        public void makeBox(Rectangle rect, string text, Texture2D texture, Color color, bool menu) //makes a box
        {
            Vector2 size = font.MeasureString(text);
            Vector2 bounds = Center(rect); //used to center the text in the box
            Vector2 origin = size * 0.5f;

            

            if (cursorRect.Intersects(rect)) //changes color
                spriteBatch.Draw(texture, new Rectangle(rect.X + 10, rect.Y, rect.Width, rect.Height), Color.Violet);
            else
                spriteBatch.Draw(texture, rect, color);

        }
        #endregion

        #region checkForButtonPress
        public void checkButtonPress(Keys keyChange, string toChange)
        {
            KeyboardState state = Keyboard.GetState();
            var input = new KeyboardState();
            if (state != input)
            {
                Debug.WriteLine(keyChange.ToString());
                var key = state.GetPressedKeys();
                keyChange = key[0];
                if(toChange == "p1Up") { p1Up = keyChange; }
                if(toChange == "p1Down") { p1Down = keyChange; }
                if(toChange == "p1Left") { p1Left = keyChange; }
                if(toChange == "p1Right") { p1Right = keyChange; }
                if(toChange == "p1Shoot") { p1Shoot = keyChange; }
                if (toChange == "p2Up") { p2Up = keyChange; }
                if (toChange == "p2Down") { p2Down = keyChange; }
                if (toChange == "p2Left") { p2Left = keyChange; }
                if (toChange == "p2Right") { p2Right = keyChange; }
                if (toChange == "p2Shoot") { p2Shoot = keyChange; }
            }
        }
        #endregion

        #region buttonPressweird
        public bool buttonPressForCfg()
        {
            if (currMState.LeftButton == ButtonState.Pressed && prevState.LeftButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }
        #endregion

        #region buttoPress
        public bool buttonPress()
        {
            if (currMState.LeftButton == ButtonState.Released && prevState.LeftButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }
        #endregion

        #region CheckItemIntersects
        void checkItemIntersects(Item item, GameTime gt)
        {
            // checks if item selected is item1
            if (item == item1)
            {
                // checks if item has been picked up
                if (item.ItemCirc.Intersects(p1.PlayerRect))
                    check1 = true;

                else if (item.ItemCirc.Intersects(p2.PlayerRect))
                    check2 = true;
                // draws if both checks are false
                if (check1 == false && check2 == false)
                    item.DrawItem(spriteBatch);

                // goes through if player 1 collected item
                if (check1 == true && check2 == false)
                {
                    // decrements effect time
                    effectTime -= gt.ElapsedGameTime.TotalSeconds;
                    // as long as effect time is greater than 0, activate ability
                    if (effectTime > 0)
                        item.ItemGet(p1, item.Type, blueOrbs, player1Animation, bOrbTex);
                    // if effect time becomes 0, end ability
                    else if ((int)effectTime == 0)
                    {
                        // reverts everything back to its default
                        item.ItemGet(p1, 2, blueOrbs, player1Animation, bOrbTex);
                        check1 = false;
                        check2 = false;
                        item.OrbC = new Circle(-100, -100, 10);
                        item.StoneC = new Circle(-50, -50, 10);
                        item.ItemCirc = new Circle(-200, -200, 10);
                        effectTime = 6;
                    }

                }
                // goes through same as before, but for player two
                else if (check1 == false && check2 == true)
                {
                    effectTime -= gt.ElapsedGameTime.TotalSeconds;
                    if (effectTime > 0)
                        item.ItemGet(p2, item.Type, orangeOrbs, player2Animation, oOrbTex);

                    else if ((int)effectTime == 0)
                    {
                        item.ItemGet(p2, 2, orangeOrbs, player2Animation, oOrbTex);
                        check1 = false;
                        check2 = false;
                        item.OrbC = new Circle(-100, -100, 10);
                        item.StoneC = new Circle(-50, -50, 10);
                        item.ItemCirc = new Circle(-200, -200, 10);
                        effectTime = 6;
                    }
                }
            }
            // checks if its the second item that was picked up
            else if (item == item2)
            {
                // checks which player picked up item
                if (item.ItemCirc.Intersects(p1.PlayerRect))
                    check3 = true;

                else if (item.ItemCirc.Intersects(p2.PlayerRect))
                    check4 = true;

                // draws if nobody picked it up
                if (check3 == false && check4 == false)
                    item.DrawItem(spriteBatch);

                // once again, checks which player picked up the item and then activates ability
                if (check3 == true && check2 == check4)
                {
                    // activate ability as long as effectTime is greater than 0
                    effectTime -= gt.ElapsedGameTime.TotalSeconds;
                    if (effectTime > 0)
                    {
                        item.ItemGet(p1, item.Type, blueOrbs, player1Animation, bOrbTex);
                    }
                    // deactivate ability, reset to default
                    else if ((int)effectTime == 0)
                    {
                        item.ItemGet(p1, 2, blueOrbs, player1Animation, bOrbTex);
                        check3 = false;
                        check4 = false;
                        item.OrbC = new Circle(-100, -100, 10);
                        item.StoneC = new Circle(-50, -50, 10);
                        item.ItemCirc = new Circle(-200, -200, 10);
                        effectTime = 6;
                    }

                }
                // do same as above but with player 2
                else if (check3 == false && check4 == true)
                {
                    effectTime -= gt.ElapsedGameTime.TotalSeconds;
                    if (effectTime > 0)
                    {
                        item.ItemGet(p2, item.Type, orangeOrbs, player2Animation, oOrbTex);
                    }
                    else if ((int)effectTime == 0)
                    {
                        item.ItemGet(p2, 2, orangeOrbs, player2Animation, oOrbTex);
                        check3 = false;
                        check4 = false;
                        item.OrbC = new Circle(-100, -100, 10);
                        item.StoneC = new Circle(-50, -50, 10);
                        item.ItemCirc = new Circle(-200, -200, 10);
                        effectTime = 6;
                    }
                }
            }

        }

        #endregion

        #region Wall Collide
        public void DoWallCollision()
        {
            foreach (Wall obj in walls)
            {
                if (obj.Bounds.Intersects(p1.PlayerRect))
                {
                    if (Math.Abs(obj.Bounds.Top - p1.PlayerRect.Bottom) < p1.PlayerRect.Height /2)
                        p1.Y = (obj.Bounds.Y - p1.PlayerRect.Height);
                    if (Math.Abs(obj.Bounds.Left - p1.PlayerRect.Right) < p1.PlayerRect.Width /2)
                        p1.X = (obj.Bounds.X - p1.PlayerRect.Width);
                    if (Math.Abs(obj.Bounds.Right - p1.PlayerRect.Left) < p1.PlayerRect.Width / 2)
                        p1.X = (obj.Bounds.X + obj.Bounds.Width);
                    if (Math.Abs(obj.Bounds.Bottom - p1.PlayerRect.Top) < p1.PlayerRect.Height / 2)
                        p1.Y = (obj.Bounds.Y + obj.Bounds.Height);
                }
                if (obj.Bounds.Intersects(p2.PlayerRect))
                {
                    if (Math.Abs(obj.Bounds.Top - p2.PlayerRect.Bottom) < p2.PlayerRect.Height /2)
                        p2.Y = (obj.Bounds.Y - p2.PlayerRect.Height);
                    if (Math.Abs(obj.Bounds.Left - p2.PlayerRect.Right) < p2.PlayerRect.Width / 2)
                        p2.X = (obj.Bounds.X - p2.PlayerRect.Width);
                    if (Math.Abs(obj.Bounds.Right - p2.PlayerRect.Left) < p2.PlayerRect.Width / 2)
                        p2.X = (obj.Bounds.X + obj.Bounds.Width);
                    if (Math.Abs(obj.Bounds.Bottom - p2.PlayerRect.Top) < p2.PlayerRect.Height / 2)
                        p2.Y = (obj.Bounds.Y + obj.Bounds.Height);
                }
            }
        }

        void DoWallCollision(Stone stone)
        {
            Circle c1 = new Circle(stone.XPos + (WIDTH_OF_STONES / 2), stone.YPos + (WIDTH_OF_STONES / 2), (WIDTH_OF_STONES / 2));
            foreach(Wall obj in walls)
            {
                if (c1.Intersects(obj.Bounds))
                {
                    if (Math.Abs(obj.Bounds.Top - (stone.YPos + WIDTH_OF_STONES)) < WIDTH_OF_STONES)
                    {
                        int yChange = obj.Bounds.Top - (stone.YPos + WIDTH_OF_STONES);
                        stone.YPos += yChange;
                        stone.Direction = new Vector2(stone.Direction.X, -stone.Direction.Y);

                    }
                    if (Math.Abs(obj.Bounds.Bottom - stone.YPos) < WIDTH_OF_STONES)
                    {
                        int yChange = obj.Bounds.Bottom - stone.YPos;
                        stone.YPos += yChange;
                        stone.Direction = new Vector2(stone.Direction.X, -stone.Direction.Y);
                    }
                    if (Math.Abs(obj.Bounds.Left - (stone.XPos + WIDTH_OF_STONES)) < WIDTH_OF_STONES)
                    {
                        int xChange = obj.Bounds.Left - (stone.XPos + WIDTH_OF_STONES);
                        stone.XPos += xChange;
                        stone.Direction = new Vector2(-stone.Direction.X, stone.Direction.Y);
                    }
                    if (Math.Abs(obj.Bounds.Right - stone.XPos) < WIDTH_OF_STONES)
                    {
                        int xChange = obj.Bounds.Right - stone.XPos;
                        stone.XPos += xChange;
                        stone.Direction = new Vector2(-stone.Direction.X, stone.Direction.Y);
                    }
                }
            }

            if (c1.Intersects(p1.PlayerRect))
            {
                if (Math.Abs(p1.PlayerRect.Top - (stone.YPos + WIDTH_OF_STONES)) < WIDTH_OF_STONES)
                {
                    stone.Direction = new Vector2(stone.Direction.X, -stone.Direction.Y);

                }
                if (Math.Abs(p1.PlayerRect.Bottom - stone.YPos) < WIDTH_OF_STONES)
                    stone.Direction = new Vector2(stone.Direction.X, -stone.Direction.Y);
                if (Math.Abs(p1.PlayerRect.Left - stone.YPos) < WIDTH_OF_STONES)
                    stone.Direction = new Vector2(-stone.Direction.X, stone.Direction.Y);
                if (Math.Abs(p1.PlayerRect.Right - (stone.XPos + WIDTH_OF_STONES)) < WIDTH_OF_STONES)
                    stone.Direction = new Vector2(-stone.Direction.X, stone.Direction.Y);
            }
            if (c1.Intersects(p2.PlayerRect))
            {
                if (Math.Abs(p2.PlayerRect.Top - (stone.YPos + WIDTH_OF_STONES)) < WIDTH_OF_STONES)
                    stone.Direction = new Vector2(stone.Direction.X, -stone.Direction.Y);
                if (Math.Abs(p2.PlayerRect.Bottom - stone.YPos) < WIDTH_OF_STONES)
                    stone.Direction = new Vector2(stone.Direction.X, -stone.Direction.Y);
                if (Math.Abs(p2.PlayerRect.Left - stone.YPos) < WIDTH_OF_STONES)
                    stone.Direction = new Vector2(-stone.Direction.X, stone.Direction.Y);
                if (Math.Abs(p2.PlayerRect.Right - (stone.XPos + WIDTH_OF_STONES)) < WIDTH_OF_STONES)
                    stone.Direction = new Vector2(-stone.Direction.X, stone.Direction.Y);
            }

        }
        #endregion

        #region ResetGrid
        public void ResetGrid()
        {
            grid.Clear();
            gridX = graphics.PreferredBackBufferWidth / 10;
            gridY = graphics.PreferredBackBufferHeight / 10;
            for (int j = 0; j < graphics.PreferredBackBufferHeight; j += gridY)
            {
                for (int i = 0; i < graphics.PreferredBackBufferWidth; i += gridX)
                {
                    grid.Add(new Rectangle(i, j, gridX, gridY));
                }

            }
            walls.Clear();

            walls.Add(new Wall(wallTex, grid[22].X, grid[22].Y, gridX, gridY * 2));
            walls.Add(new Wall(wallTex, grid[27].X, grid[27].Y, gridX, gridY * 2));
            walls.Add(new Wall(wallTex, grid[62].X, grid[62].Y, gridX, gridY * 2));
            walls.Add(new Wall(wallTex, grid[67].X, grid[67].Y, gridX, gridY * 2));
            walls.Add(new Wall(wallTex, grid[44].X, grid[44].Y, gridX * 2, gridY * 2));
            walls.Add(new Wall(wallTex, grid[0].X, grid[0].Y, gridX * 10, gridY));
            walls.Add(new Wall(wallTex, grid[0].X, grid[0].Y, gridX, gridY * 10));
            walls.Add(new Wall(wallTex, grid[9].X, grid[9].Y, gridX, gridY * 10));
            walls.Add(new Wall(wallTex, grid[90].X, grid[90].Y, gridX * 10, gridY));
        }
        #endregion

        #region OrbCollision
        public void OrbCollide()
        {
            for (int x = 0; x < stonesList.Count; x++)
            {
                Circle c1 = new Circle((int)stonesList[x].XPos + (WIDTH_OF_STONES / 2), (int)stonesList[x].YPos + (WIDTH_OF_STONES / 2), (WIDTH_OF_STONES / 2)); //Circle object for each stone to check for collisions

                for (int i = 0; i < blueOrbs.Count; i++)
                {

                    Circle c2 = new Circle((int)blueOrbs[i].X + (ORB_WIDTH / 2), (int)blueOrbs[i].Y + (ORB_WIDTH / 2), (ORB_WIDTH / 2)); //Circle object for orb
                    if (c1.Intersects(c2))
                    {
                        stonesList[x].StoneTex = blueStone;
                        blueOrbs.Remove(blueOrbs[i]);
                        i--;
                        if (stonesList[x].RapidFire == true)
                        {
                            stonesList[x].RapidFire = false;
                            item1 = new Item(speedupImage, stonesList[x].XPos, stonesList[x].YPos, stonesList[x].Circle.Radius, 1);
                            item1.ItemCheckInfo(c2, c1);
                            item1.changeItemLoc(stonesList, 1, stoneTex);
                        }
                        if (stonesList[x].Inverter == true)
                        {
                            stonesList[x].Inverter = false;
                            item2 = new Item(inverterImage, stonesList[x].XPos, stonesList[x].YPos, stonesList[x].Circle.Radius, 3);
                            item2.ItemCheckInfo(c2, c1);
                            item2.changeItemLoc(stonesList, 2, stoneTex);
                        }
                    }
                }
                for (int j = 0; j < orangeOrbs.Count; j++)
                {
                    Circle c3 = new Circle((int)orangeOrbs[j].X + (ORB_WIDTH / 2), (int)orangeOrbs[j].Y + (ORB_WIDTH / 2), (ORB_WIDTH / 2));
                    if (c1.Intersects(c3))
                    {
                        stonesList[x].StoneTex = orangeStone;
                        orangeOrbs.Remove(orangeOrbs[j]);
                        j--;
                        if (stonesList[x].RapidFire == true)
                        {
                            stonesList[x].RapidFire = false;
                            item1 = new Item(speedupImage, stonesList[x].XPos, stonesList[x].YPos, stonesList[x].Circle.Radius, 1);
                            item1.ItemCheckInfo(c3, c1);
                            item1.changeItemLoc(stonesList, 1, stoneTex);
                        }
                        if (stonesList[x].Inverter == true)
                        {
                            stonesList[x].Inverter = false;
                            item2 = new Item(inverterImage, stonesList[x].XPos, stonesList[x].YPos, stonesList[x].Circle.Radius, 3);
                            item2.ItemCheckInfo(c3, c1);
                            item2.changeItemLoc(stonesList, 2, stoneTex);
                        }
                    }
                }

                //Update Score
                p1Count = 0;
                p2Count = 0;
                for (int k = 0; k < stonesList.Count; k++)
                {
                    if (stonesList[k].StoneTex == blueStone)
                    {
                        p1Count++;
                    }
                    if (stonesList[k].StoneTex == orangeStone)
                    {
                        p2Count++;
                    }
                }
            }
        }
        #endregion

        #region createOrbs
        public void CreateOrbs()
        {
            if (blueShot)
            {
                if (blueStopper.ElapsedMilliseconds >= BLUE_STOPPER)
                {
                    blueShot = false;
                    blueStopper.Stop();
                    blueStopper.Reset();
                }
                else { }
            }
            else
            {
                if ((currKbState.IsKeyUp(p1Shoot) && (prevKbState.IsKeyDown(p1Shoot)))) //P1 Shoots
                {
                    if (p1.Stunned == false)
                    {
                        Orb o1 = new Orb(bOrbTex, 0, 0, p1, player1Animation, ORB_WIDTH, ORB_SPEED);
                        blueOrbs.Add(o1);
                        blueShot = true;

                        laserSoundEffectInstance.Play();

                        blueStopper = new Stopwatch();
                        blueStopper.Start();
                    }
                }
                else { }
            }
            if (orangeShot)
            {
                if (orangeStopper.ElapsedMilliseconds >= ORANGE_STOPPER)
                {
                    orangeShot = false;
                    orangeStopper.Stop();
                    orangeStopper.Reset();
                }
            }
            else
            {
                if ((currKbState.IsKeyDown(p2Shoot) && (prevKbState.IsKeyDown(p2Shoot) == false))) //P2 Shoots
                {
                    if (p2.Stunned == false)
                    {
                        Orb o2 = new Orb(oOrbTex, 0, 0, p2, player2Animation, ORB_WIDTH, ORB_SPEED);
                        orangeOrbs.Add(o2);
                        orangeShot = true;

                        laserSoundEffectInstance.Play();

                        orangeStopper = new Stopwatch();
                        orangeStopper.Start();
                    }
                }
            }
        }
        #endregion

        #region UpdateOrbs
        public void UpdateOrbs()
        {
            for (int i = 0; i < blueOrbs.Count; i++)
            {
                blueOrbs[i].UpdateOrbs();

                if (blueOrbs[i].TravelToMaxDistance == true)
                {
                    blueOrbs.RemoveAt(i);
                }
            }
            for (int i = 0; i < orangeOrbs.Count; i++)
            {
                orangeOrbs[i].UpdateOrbs();

                if (orangeOrbs[i].TravelToMaxDistance == true)
                {
                    orangeOrbs.RemoveAt(i);
                }
            }
        }
        #endregion

        #region StunPlayer
        public void StunPlayer()
        {
            for (int h = 0; h < blueOrbs.Count; h++)
            {
                Circle blueOrbCirc = new Circle((int)blueOrbs[h].X + (ORB_WIDTH / 2), (int)blueOrbs[h].Y + (ORB_WIDTH / 2), (ORB_WIDTH / 2)); //Circle object for orb
                if (blueOrbCirc.Intersects(p2.PlayerRect))    //Blue orb hits orange player
                {
                    blueOrbs.Remove(blueOrbs[h]);
                    h--;
                    p2.Stunned = true;
                    p2StunWatch.Start();
                }
            }
            for (int j = 0; j < orangeOrbs.Count; j++)
            {
                Circle orangeOrbCirc = new Circle((int)orangeOrbs[j].X + (ORB_WIDTH / 2), (int)orangeOrbs[j].Y + (ORB_WIDTH / 2), (ORB_WIDTH / 2));
                if (orangeOrbCirc.Intersects(p1.PlayerRect)) //orange orb hits blue player
                {
                    orangeOrbs.Remove(orangeOrbs[j]);
                    j--;
                    p1.Stunned = true;
                    p1StunWatch.Start();
                }
            }
        }
        #endregion

        #region StunTimer
        public void StunTimer()
        {
            if (p1StunWatch.IsRunning)
            {
                //Unstun player 1 and make them immune
                if (p1StunWatch.ElapsedMilliseconds >= PLAYER_STUN_DURATION)
                {
                    p1.Stunned = false;
                    p1.Immune = true;
                    //Unimmune player 1 and reset the stopwatch
                    if (p1StunWatch.ElapsedMilliseconds >= PLAYER_STUN_DURATION + PLAYER_IMMUNITY_DURATION)
                    {
                        p1.Immune = false;
                        p1StunWatch.Reset();
                    }
                }
            }
            if (p2StunWatch.IsRunning)
            {
                if (p2StunWatch.ElapsedMilliseconds >= PLAYER_STUN_DURATION)
                {
                    p2.Stunned = false;
                    p2.Immune = true;
                    if (p2StunWatch.ElapsedMilliseconds >= PLAYER_STUN_DURATION + PLAYER_IMMUNITY_DURATION)
                    {
                        p2.Immune = false;
                        p2StunWatch.Reset();
                    }
                }
            }
        }
        #endregion

        #endregion
    }
}
