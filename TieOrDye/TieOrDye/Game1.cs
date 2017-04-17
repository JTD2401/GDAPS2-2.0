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
        const int NUMBER_OF_STONES = 25;
        const int WIDTH_OF_STONES = 30;
        const int ORB_WIDTH = 10;
        const int ORB_SPEED = 3;
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
        enum GameStates { Menu, Pause, Options, ControlOptions, InGame, GameOver };
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

        bool startActive;
        bool orangeShot = false;
        bool blueShot = false;

        Stopwatch blueStopper;
        Stopwatch orangeStopper;

        Song song;
        int playernumber = 1;

        Keys p1Up, p1Down, p1Left, p1Right, p1Shoot, p2Up, p2Down, p2Left, p2Right, p2Shoot;


        int mouseOffsetX;
        int mouseOffsetY;

        Item item;
        bool timesUp;
        int num;
        int num2;
        double effectTime;
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
            Window.IsBorderless = true;
            graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            Debug.WriteLine(graphics.PreferredBackBufferFormat.ToString());
            //Window.IsBorderless = true;
            graphics.ApplyChanges();
            ResetMouseOffsets();

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

            item = new Item(stoneTex, -1000, -1000, 100, 1);
            timesUp = false;
            num = 0;
            num2 = 0;
            effectTime = 6;

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

            walls.Add(new Wall(wallTex, 329, 185, 151, 200));
            walls.Add(new Wall(wallTex, 1446, 193, 151, 200));
            walls.Add(new Wall(wallTex, 337, 734, 151, 200));
            walls.Add(new Wall(wallTex, 1446, 730, 151, 200));
            walls.Add(new Wall(wallTex, 782, 508, 360, 153));
            walls.Add(new Wall(wallTex, 813, 492, 300, 17));
            walls.Add(new Wall(wallTex, 812, 660, 300, 17));
            walls.Add(new Wall(wallTex, 0, 0, 1920, 84));
            walls.Add(new Wall(wallTex, 0, 0, 150, 1080));
            walls.Add(new Wall(wallTex, 1776, 5, 150, 1080));
            walls.Add(new Wall(wallTex, 10, 1002, 1920, 85));

            //Stunwatches
            p1StunWatch = new Stopwatch();
            p2StunWatch = new Stopwatch();

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
                        if (buttonPress())
                        {
                            p1.X = player1InitialX;
                            p1.Y = player1InitialY;

                            p2.X = player2InitialX;
                            p2.Y = player2InitialY;

                            p1Count = 0;
                            p2Count = 0;

                            blueOrbs.Clear();
                            orangeOrbs.Clear();

                            item.OrbC = new Circle(0, 0, 0);

                            //Create stone objects
                            CreateStones(NUMBER_OF_STONES, WIDTH_OF_STONES);
                            //Start game
                            currentGameState = GameStates.InGame;
                        }
                    }
                    else  //When the mouse is no longer hovering over the button
                        startActive = false;//Make the button inactive

                    if (cursorRect.Intersects(new Rectangle(190, 450, 392, 103)))
                        if (buttonPress())
                        {
                            Debug.WriteLine("Click");
                            currentGameState = GameStates.Options;
                            cameFromMenu = true;
                        }


                    if (cursorRect.Intersects(new Rectangle(190, 750, exit.Width, exit.Height)))
                        if (buttonPress())
                            this.Exit();
                    //Sets total game time
                    prevState = currMState;
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
                        if (currMState.LeftButton == ButtonState.Pressed && prevState.LeftButton == ButtonState.Pressed)
                            this.Exit();

                    prevState = currMState;
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

                    //Creates orbs - Cooldown is only 1 update loop currently
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
                            if(p1.Stunned == false)
                            {
                                Orb o1 = new Orb(bOrbTex, 0, 0, p1, player1Animation, ORB_WIDTH, ORB_SPEED);
                                blueOrbs.Add(o1);
                                blueShot = true;
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
                            if(p2.Stunned == false)
                            {
                                Orb o2 = new Orb(oOrbTex, 0, 0, p2, player2Animation, ORB_WIDTH, ORB_SPEED);
                                orangeOrbs.Add(o2);
                                orangeShot = true;
                                orangeStopper = new Stopwatch();
                                orangeStopper.Start();
                            }
                        }
                    }

                    //Update orb locations
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
                    }

                    //Stuns player when hit by opposite orb
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

                    ///
                    /// Orb colors  - Needs many changes
                    /// For each stone: loops through all orbs, checks for collisions
                    /// On collsion: changes stone texture, deletes orb and increments backwards in orb list
                    ///
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
                                if (stonesList[x].ItemSpawn == true)
                                {
                                    item = new Item(stoneTex, stonesList[x].XPos, stonesList[x].YPos, stonesList[x].Circle.Radius, 1);
                                    item.ItemCheckInfo(c2, c1);
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
                                if (stonesList[x].ItemSpawn == true)
                                {
                                    item = new Item(stoneTex, stonesList[x].XPos, stonesList[x].YPos, stonesList[x].Circle.Radius, 1);
                                    item.ItemCheckInfo(c3, c1);
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
                    if(p2StunWatch.IsRunning)
                    {
                        if (p2StunWatch.ElapsedMilliseconds >= PLAYER_STUN_DURATION)
                        {
                            p2.Stunned = false;
                            p2.Immune = true;
                            if (p2StunWatch.ElapsedMilliseconds >= PLAYER_STUN_DURATION + PLAYER_IMMUNITY_DURATION)
                            {
                                p2.Immune = true;
                                p2StunWatch.Reset();
                            }
                        }
                    }

                    time -= gameTime.ElapsedGameTime.TotalSeconds;
                    if (time <= 0)
                        currentGameState = GameStates.GameOver;

                    if (currKbState.IsKeyDown(Keys.P) || currKbState.IsKeyDown(Keys.Escape)) { currentGameState = GameStates.Pause; cameFromMenu = false; }

                    break;
                #endregion
                #region options
                case GameStates.Options:
                    int z = (GraphicsDevice.Viewport.Width / 8);
                    if (cursorRect.Intersects(new Rectangle((z + z + 10 + GraphicsDevice.Viewport.Width / 6) - 80, 175, 80, 100)))
                        if (buttonPress()) //resolution right
                        {
                            if (location == 9)
                                return;
                            location += 1;
                        }
                    if(cursorRect.Intersects(new Rectangle((z + z + 10), 175, 80, 100)))
                        if(buttonPress()) //resolution left
                        {
                            if (location == 0)
                                return;
                            location -= 1;
                        }
                    if(cursorRect.Intersects(new Rectangle((z + z + 10 + GraphicsDevice.Viewport.Width / 6) - 80, 295, 80, 100)))
                        if(buttonPress()) //volume right
                        {
                            if(dblVolumeLevel > .9)
                                return;
                            dblVolumeLevel += .1;
                            volumeLevel = (float)dblVolumeLevel;
                        }
                    if(cursorRect.Intersects(new Rectangle((z + z + 10), 295, 80, 100)))
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
                    if (cursorRect.Intersects(new Rectangle(z, 415, z, 100)))
                        if (buttonPress()) //controls button
                            currentGameState = GameStates.ControlOptions;
                    if (cursorRect.Intersects(new Rectangle(z, 535, z, 100)))
                        if (buttonPress()) //the apply button
                        {
                            if (resolution[location] == graphics.PreferredBackBufferWidth.ToString() + "x" + graphics.PreferredBackBufferHeight.ToString())
                                return;
                            string res = resolution[location]; //sets the string to the string of new resolution
                            var tempArr = res.Split('x'); //splits it by the char x
                            int width = int.Parse(tempArr[0]); //sets width to the first parse
                            int height = int.Parse(tempArr[1]); //sets height to the second parse

                            if (width > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width || height > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height)
                                return;

                            graphics.PreferredBackBufferWidth = width; //sets width and height accordingly
                            graphics.PreferredBackBufferHeight = height;
                            graphics.ApplyChanges(); //applys the change
                            ResetMouseOffsets();
                        }
                    if (cursorRect.Intersects(new Rectangle(z + z + 10, 535, z, 100))) //back button
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
                    z = GraphicsDevice.Viewport.Width / 18;
                    int width2 = GraphicsDevice.Viewport.Width / 8;
                    if (cursorRect.Intersects(new Rectangle(z, 175, 80, 100)))
                        if (buttonPress())
                        {
                            if (playernumber == 1)
                                return;
                            playernumber -= 1;
                        }
                    if (cursorRect.Intersects(new Rectangle((z + width2 + GraphicsDevice.Viewport.Width / 8) - 80, 175, 80, 100)))
                        if(buttonPress())
                        {
                            if (playernumber == 2)
                                return;
                            playernumber += 1;
                        }
                    if(playernumber == 1)
                    {
                        if (cursorRect.Intersects(new Rectangle(z + width2 + 10, 295, width2, 100)))
                            if (buttonPressForCfg()) //changes up button
                                checkButtonPress(p1Up, "p1Up");
                        if (cursorRect.Intersects(new Rectangle(z + width2 + 10, 415, width2, 100)))
                            if (buttonPressForCfg()) //changes left button
                                checkButtonPress(p1Down, "p1Down");
                        if (cursorRect.Intersects(new Rectangle(z + width2 + 10, 535, width2, 100)))
                            if (buttonPressForCfg()) //changes left button
                                checkButtonPress(p1Left, "p1Left");
                        if (cursorRect.Intersects(new Rectangle(z + width2 + 10, 655, width2, 100)))
                            if (buttonPressForCfg()) //changes right button
                                checkButtonPress(p1Right, "p1Right");
                        if(cursorRect.Intersects(new Rectangle(z + width2 + 10, 775, width2, 100)))
                            if(buttonPressForCfg()) //changes shoot button
                                checkButtonPress(p1Shoot, "p1Shoot");
                    }
                    else //player 2
                    {
                        if (cursorRect.Intersects(new Rectangle(z + width2 + 10, 295, width2, 100)))
                            if (buttonPressForCfg()) //changes up button
                                checkButtonPress(p2Up, "p2Up");
                        if (cursorRect.Intersects(new Rectangle(z + width2 + 10, 415, width2, 100)))
                            if (buttonPressForCfg()) //changes down button
                                checkButtonPress(p2Down, "p2Down");
                        if (cursorRect.Intersects(new Rectangle(z + width2 + 10, 535, width2, 100)))
                            if (buttonPressForCfg()) //changes left button
                                checkButtonPress(p2Left, "p2Left");
                        if (cursorRect.Intersects(new Rectangle(z + width2 + 10, 655, width2, 100)))
                            if (buttonPressForCfg()) //changes right button
                                checkButtonPress(p2Right, "p2Right");
                        if (cursorRect.Intersects(new Rectangle(z + width2 + 10, 775, width2, 100)))
                            if (buttonPressForCfg()) //changes shoot button
                                checkButtonPress(p2Shoot, "p2Shoot");
                    }
                    if (cursorRect.Intersects(new Rectangle(z, 850, width2, 100)))
                        if (buttonPress())
                            currentGameState = GameStates.Options;
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

            var mouse = Mouse.GetState();
            var mousePos = GetMousePosition();
            cursorRect = new Rectangle((int)mousePos.X, (int)mousePos.Y, 20, 20);

            switch (currentGameState)
            {
                #region menu
                case GameStates.Menu:
                    //Draw gameboard
                    spriteBatch.Draw(gameBoard, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                    if (cursorRect.Intersects(new Rectangle(190, 150, 392, 103)))
                        spriteBatch.Draw(start, new Rectangle(200, 150, 392, 103), Color.Violet);
                    else
                        spriteBatch.Draw(start, new Rectangle(190, 150, 392, 103), Color.White);
                    if (cursorRect.Intersects(new Rectangle(190, 450, 392, 103)))
                        spriteBatch.Draw(options, new Rectangle(200, 450, 392, 103), Color.Violet);
                    else
                        spriteBatch.Draw(options, new Rectangle(190, 450, 392, 103), Color.White);
                    if (cursorRect.Intersects(new Rectangle(190, 750, 392, 103)))
                        spriteBatch.Draw(exit, new Rectangle(200, 750, 392, 103), Color.Violet);
                    else
                        spriteBatch.Draw(exit, new Rectangle(190, 750, 392, 103), Color.White);
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

                    checkItemIntersects(p1, blueOrbs, gameTime, num, player1Animation, bOrbTex);
                    checkItemIntersects(p2, orangeOrbs, gameTime, num2, player2Animation, oOrbTex);

                    

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
                    spriteBatch.Draw(p1Tex, new Rectangle(GraphicsDevice.Viewport.Width / 50, 300, GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 2), Color.White);
                    spriteBatch.Draw(p2Tex, new Rectangle(((GraphicsDevice.Viewport.Width / 4) + (GraphicsDevice.Viewport.Width / 2)) - 35, 300, GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 2), Color.White);

                    spriteBatch.DrawString(font, ""+p1Count, new Vector2(275, 110), Color.White, 0f, Vector2.Zero, 2, SpriteEffects.None, 0);
                    spriteBatch.DrawString(font, "" + p2Count, new Vector2(1650, 110), Color.White, 0f, Vector2.Zero, 2, SpriteEffects.None, 0);
                    spriteBatch.DrawString(font, "TIME: " + (int)time, new Vector2(850, 110), Color.White, 0f, Vector2.Zero, 2, SpriteEffects.None, 0);

                    spriteBatch.Draw(cursorTex, cursorRect, Color.White);
                    break;
                #endregion
                #region options
                case GameStates.Options:
                    Rectangle rectangle;
                    int x = (GraphicsDevice.Viewport.Width / 8); //I use this a lot so it makes the code shorter
                    spriteBatch.Draw(optionsScreen, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White); //draws the background screen

                    rectangle = new Rectangle(x, 50, x, 100); //options box
                    makeBox(rectangle, "Options", noTexture, Color.White, false, false);

                    rectangle = new Rectangle(x, 175, x, 100); //resolution box
                    makeBox(rectangle, "Resolution", noTexture, Color.White, false, false);

                    rectangle = new Rectangle(x + x + 10, 175, GraphicsDevice.Viewport.Width / 6, 100); //current resolution box
                    makeBox(rectangle, resolution[location], noTexture, Color.White, true, false);

                    rectangle = new Rectangle(x, 295, x, 100); //Volume Box
                    makeBox(rectangle, "Volume", noTexture, Color.White, false, false);

                    rectangle = new Rectangle(x + x + 10, 295, GraphicsDevice.Viewport.Width / 6, 100); //change Volume Box
                    makeBox(rectangle, volumeLevel.ToString(), noTexture, Color.White, true, false);

                    rectangle = new Rectangle(x, 415, x, 100); //Controls Box
                    makeBox(rectangle, "Controls", noTexture, Color.White, false, false);

                    rectangle = new Rectangle(x, 535, x, 100); //Apply Box
                    makeBox(rectangle, "Apply", noTexture, Color.White, false, false);

                    rectangle = new Rectangle(x + x + 10, 535, x, 100); //Back Box
                    makeBox(rectangle, "Back", noTexture, Color.White, false, false);

                    spriteBatch.Draw(cursorTex, cursorRect, Color.White);
                    break;
                #endregion
                #region control options
                case GameStates.ControlOptions:

                    spriteBatch.Draw(optionsScreen, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                    x = GraphicsDevice.Viewport.Width / 18;
                    int width = GraphicsDevice.Viewport.Width / 8;

                    rectangle = new Rectangle(x, 50, width, 100); //keyboard box
                    makeBox(rectangle, "Keyboard", noTexture, Color.White, false, false);

                    rectangle = new Rectangle(x, 175, width * 2, 100); //player box
                    makeBox(rectangle, "Player" + playernumber.ToString(), noTexture, Color.White, true, false);

                    rectangle = new Rectangle(x, 295, width, 100); //up box
                    makeBox(rectangle, "Up", noTexture, Color.White, false, false);

                    rectangle = new Rectangle(x, 415, width, 100); //down box
                    makeBox(rectangle, "Down", noTexture, Color.White, false, false);

                    rectangle = new Rectangle(x, 535, width, 100); //left box
                    makeBox(rectangle, "Left", noTexture, Color.White, false, false);

                    rectangle = new Rectangle(x, 655, width, 100); //right box
                    makeBox(rectangle, "Right", noTexture, Color.White, false, false);

                    rectangle = new Rectangle(x, 775, width, 100); //shoot box
                    makeBox(rectangle, "Shoot", noTexture, Color.White, false, false);

                    if (playernumber == 1)
                    {
                        rectangle = new Rectangle(x + width + 10, 295, width, 100); //p1UpBox
                        makeBox(rectangle, p1Up.ToString(), noTexture, Color.White, false, true);

                        rectangle = new Rectangle(x + width + 10, 415, width, 100); //p1DownBox
                        makeBox(rectangle, p1Down.ToString(), noTexture, Color.White, false, true);

                        rectangle = new Rectangle(x + width + 10, 535, width, 100); //p1LeftBox
                        makeBox(rectangle, p1Left.ToString(), noTexture, Color.White, false, true);

                        rectangle = new Rectangle(x + width + 10, 655, width, 100); //p1RightBox
                        makeBox(rectangle, p1Right.ToString(), noTexture, Color.White, false, true);

                        rectangle = new Rectangle(x + width + 10, 775, width, 100); //p1ShootBox
                        makeBox(rectangle, p1Shoot.ToString(), noTexture, Color.White, false, true);
                    }
                    else
                    {
                        rectangle = new Rectangle(x + width + 10, 295, width, 100); //p2UpBox
                        makeBox(rectangle, p2Up.ToString(), noTexture, Color.White, false, true);

                        rectangle = new Rectangle(x + width + 10, 415, width, 100); //p2DownBox
                        makeBox(rectangle, p2Down.ToString(), noTexture, Color.White, false, true);

                        rectangle = new Rectangle(x + width + 10, 535, width, 100); //p2LeftBox
                        makeBox(rectangle, p2Left.ToString(), noTexture, Color.White, false, true);

                        rectangle = new Rectangle(x + width + 10, 655, width, 100); //p2RightBox
                        makeBox(rectangle, p2Right.ToString(), noTexture, Color.White, false, true);

                        rectangle = new Rectangle(x + width + 10, 775, width, 100);
                        makeBox(rectangle, p2Shoot.ToString(), noTexture, Color.White, false, true);
                    }
                    rectangle = new Rectangle(x, 895, width, 100);
                    makeBox(rectangle, "Back", noTexture, Color.White, false, false);
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
                Circle c2 = new Circle((int)obj.XPos + (WIDTH_OF_STONES / 2), (int)obj.YPos + (WIDTH_OF_STONES / 2), (WIDTH_OF_STONES / 2));

                if (stone.Equals(obj))
                    return;
                if (c1.Intersects(c2))
                {
                    stone.Direction = new Vector2(-stone.Direction.X, -stone.Direction.Y);
                    obj.Direction = new Vector2(-obj.Direction.X, -obj.Direction.Y);
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
                stonesList[i].ItemSpawn = true;
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
        void checkItemIntersects(Player player, List<Orb> orbList, GameTime gt, int counter, Animation anim, Texture2D orbTex)
        {
            if (item.ItemCirc.Intersects(player.PlayerRect))
            {
                if (player == p1) { num++; counter = num; }
                if (player == p2) { num2++; counter = num2; }
            }

            if (num == 0 && num2 == 0)
            {
                item.DrawItem(spriteBatch);
            }

            if (num == 0 || num2 == 0)
            {
                if (counter > 0)
                {
                    effectTime -= gt.ElapsedGameTime.TotalSeconds;
                    if (effectTime > 0)
                    {
                        item.ItemGet(player, 1, orbList, anim, orbTex);
                    }
                    else if ((int)effectTime == 0)
                    {
                        item.ItemGet(player, 2, orbList, anim, orbTex);
                        num = 0;
                        num2 = 0;
                        item.OrbC = new Circle(-100, -100, 10);
                        item.StoneC = new Circle(-50, -50, 10);
                        item = new Item(stoneTex, -100, -100, 10, 1);
                        effectTime = 5;
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
                        stone.Direction = new Vector2(stone.Direction.X, -stone.Direction.Y);
                    if (Math.Abs(obj.Bounds.Bottom - stone.YPos) < WIDTH_OF_STONES)
                        stone.Direction = new Vector2(stone.Direction.X, -stone.Direction.Y);
                    if (Math.Abs(obj.Bounds.Left - stone.XPos) < WIDTH_OF_STONES)
                        stone.Direction = new Vector2(-stone.Direction.X, stone.Direction.Y);
                    if (Math.Abs(obj.Bounds.Right - (stone.XPos + WIDTH_OF_STONES)) < WIDTH_OF_STONES)
                        stone.Direction = new Vector2(-stone.Direction.X, stone.Direction.Y);
                }
            }

            if (c1.Intersects(p1.PlayerRect))
            {
                if (Math.Abs(p1.PlayerRect.Top - (stone.YPos + WIDTH_OF_STONES)) < WIDTH_OF_STONES)
                    stone.Direction = new Vector2(stone.Direction.X, -stone.Direction.Y);
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

        #endregion
    }
}
