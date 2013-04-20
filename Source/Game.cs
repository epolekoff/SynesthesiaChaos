using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.Storage;
using Windows.Storage.Streams;
using SharpDX.XAudio2;

namespace SynesthesiaChaos
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class EntireGame : Microsoft.Xna.Framework.Game
    {
        //Constants
        const int TITLE = 0;
        const int GAME = 1;
        const int GAMEOVER = 2;
        const int PAUSED = 3;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState oldstate;

        Camera cameraView;

        //Game Stuff
        Player player;
        int lives;
        int level;
        int tempScore;
        int score;
        int displayedScore;
        //String playerName;
        int playerHurtTimer;
        int playerHurtTimerMax = 20;//Frames
        Random random;

        //Screen data
        public static int screenWidth = 1920;//GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        public static int screenHeight = 1080;//GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        int screenCenter = screenWidth/2;
        int heart_x = 15;
        int heart_y = screenHeight - 65;

        //Create the Game States
        public int gameState = TITLE;


        //Textures
        //Sprites
        Texture2D heart_sprite;
        int playerWidth = 50;
        int playerHeight = 50;

        //Backgrounds
        Texture2D[] bg;
        Texture2D bgx;
        Texture2D blocker_bg;
        Texture2D[] parallaxImage;
        Texture2D title;
        Texture2D gameover;
        Texture2D paused;

        //Collisions
        Texture2D[] cm;
        Texture2D cmx;
        Texture2D blocker_cm;
        LinkedList<Color[]> collisionColorArray;

        //Rectangles
        Rectangle screenRectangle;

        //Fonts
        SpriteFont scoreFont;

        //Stages
        int levelLength = 10;
        int numStages = 19;//Number of bgs and cms
        public LinkedList<Stage> stages;
        bool inSafehouse = false;
        int numParallax = 7;
        Vector2[,] parallaxPosition;
        float[] parallaxFactor = {30f, 7f, 6f, 5f, 4f, 3f, 2f, 0};

        //Sound
        WaveManager waveManager = new WaveManager();
        SoundEffect hit;
        SoundEffect ground;
        SoundEffectInstance ground_instance;
        double bps;
        double fpsTimer = 0;
        int clapCount;

        //Fragments
        LinkedList<Firework> fireworks;
        LinkedList<Fragment> fragments;

        //Score
        bool dataLoaded = false;
        [DataContract]
        public class SaveGameData
        {
            [DataMember]
            public int[] score;
            [DataMember]
            public String[] name;

            public SaveGameData(int size)
            {
                score = new int[size];
                name = new String[size];
            }
        }
        SaveGameData highscores;



        public EntireGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Set the screen size
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.IsFullScreen = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Set/Reset variables
            lives = 5;
            level = 0;
            bps = 2;
            tempScore = 0;
            score = 0;
            displayedScore = 0;
            playerHurtTimer = 0;

            random = new Random();

            //Initialize the player
            player = new Player();

            //Camera
            cameraView = new Camera(GraphicsDevice.Viewport);

            //Background stuff
            screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
            parallaxPosition = new Vector2[2, numParallax];
            parallaxImage = new Texture2D[numParallax];

            //Stages
            stages = new LinkedList<Stage>();
            bg = new Texture2D[numStages];
            cm = new Texture2D[numStages];
            collisionColorArray = new LinkedList<Color[]>();

            //Sounds

            //Fragments
            fragments = new LinkedList<Fragment>();
            fireworks = new LinkedList<Firework>();

            //Windows 8 stuff
            Windows8.Initialize();

            //Touch stuff
            TouchPanel.EnabledGestures = GestureType.FreeDrag |
                                         GestureType.Flick |
                                         GestureType.Tap |
                                         GestureType.DoubleTap |
                                         GestureType.VerticalDrag |
                                         GestureType.Hold |
                                         GestureType.PinchComplete |
                                         GestureType.None;

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

            //Keyboard
            oldstate = new KeyboardState();

            //Initialize the player
            player.Initialize(playerWidth, playerHeight, Content, (screenCenter - playerWidth / 2), 2 * screenHeight / 3);

            //Load Heart sprite
            heart_sprite = Content.Load<Texture2D>("heart");


            //Load Backgrounds
            bg[0] = Content.Load<Texture2D>("background/bg0");
            bg[1] = Content.Load<Texture2D>("background/bg1");
            bg[2] = Content.Load<Texture2D>("background/bg2");
            bg[3] = Content.Load<Texture2D>("background/bg3");
            bg[4] = Content.Load<Texture2D>("background/bg4");
            bg[5] = Content.Load<Texture2D>("background/bg5");
            bg[6] = Content.Load<Texture2D>("background/bg6");
            bg[7] = Content.Load<Texture2D>("background/bg7");
            bg[8] = Content.Load<Texture2D>("background/bg8");
            bg[9] = Content.Load<Texture2D>("background/bg9");
            bg[10] = Content.Load<Texture2D>("background/bg10");
            bg[11] = Content.Load<Texture2D>("background/bg11");
            bg[12] = Content.Load<Texture2D>("background/bg12");
            bg[13] = Content.Load<Texture2D>("background/bg13");
            bg[14] = Content.Load<Texture2D>("background/bg14");
            bg[15] = Content.Load<Texture2D>("background/bg15");
            bg[16] = Content.Load<Texture2D>("background/bg16");
            bg[17] = Content.Load<Texture2D>("background/bg17");
            bg[18] = Content.Load<Texture2D>("background/bg18");
            bgx = Content.Load<Texture2D>("background/bgx");
            blocker_bg = Content.Load<Texture2D>("background/blocker_bg");
            parallaxImage[0] = Content.Load<Texture2D>("background/parallax");
            parallaxImage[1] = Content.Load<Texture2D>("background/parallax1");
            parallaxImage[2] = Content.Load<Texture2D>("background/parallax2");
            //parallaxImage[3] = Content.Load<Texture2D>("background/parallax3");
            parallaxImage[3] = Content.Load<Texture2D>("background/parallax4");
            parallaxImage[4] = Content.Load<Texture2D>("background/parallax5");
            parallaxImage[5] = Content.Load<Texture2D>("background/parallax6");
            parallaxImage[6] = Content.Load<Texture2D>("background/parallax7");
            title = Content.Load<Texture2D>("title");
            gameover = Content.Load<Texture2D>("gameover");
            paused = Content.Load<Texture2D>("paused");

            for (int i = 0; i < numParallax; i++)
            {
                parallaxPosition[0, i] = new Vector2(-parallaxImage[i].Width, 0);
                parallaxPosition[1, i] = new Vector2(0, 0);
            }


            //Load Collisions
            cm[0] = Content.Load<Texture2D>("cm/cm0");
            cm[1] = Content.Load<Texture2D>("cm/cm1");
            cm[2] = Content.Load<Texture2D>("cm/cm2");
            cm[3] = Content.Load<Texture2D>("cm/cm3");
            cm[4] = Content.Load<Texture2D>("cm/cm4");
            cm[5] = Content.Load<Texture2D>("cm/cm5");
            cm[6] = Content.Load<Texture2D>("cm/cm6");
            cm[7] = Content.Load<Texture2D>("cm/cm7");
            cm[8] = Content.Load<Texture2D>("cm/cm8");
            cm[9] = Content.Load<Texture2D>("cm/cm9");
            cm[10] = Content.Load<Texture2D>("cm/cm10");
            cm[11] = Content.Load<Texture2D>("cm/cm11");
            cm[12] = Content.Load<Texture2D>("cm/cm12");
            cm[13] = Content.Load<Texture2D>("cm/cm13");
            cm[14] = Content.Load<Texture2D>("cm/cm14");
            cm[15] = Content.Load<Texture2D>("cm/cm15");
            cm[16] = Content.Load<Texture2D>("cm/cm16");
            cm[17] = Content.Load<Texture2D>("cm/cm17");
            cm[18] = Content.Load<Texture2D>("cm/cm18");
            cmx = Content.Load<Texture2D>("cm/cmx");
            blocker_cm = Content.Load<Texture2D>("cm/blocker_cm");
            for (int i = 0; i < numStages; i++)
            {
                //Make an array out of each cm and add it to the list.
                collisionColorArray.AddLast(make_collision_color(cm[i]));
            }
            //Add the last 2 cms
            collisionColorArray.AddLast(make_collision_color(cmx));
            collisionColorArray.AddLast(make_collision_color(blocker_cm));

            //Fonts
            scoreFont = Content.Load<SpriteFont>("fonts/ScoreFont");

            //Stages
            stages.AddLast(new Stage(bg[0], cm[0], collisionColorArray.ElementAt(0), -bg[0].Width / 2, 0, graphics.GraphicsDevice));
            stages.AddLast(new Stage(bg[1], cm[1], collisionColorArray.ElementAt(1), stages.ElementAt(0).position.X + stages.ElementAt(0).width, 0, graphics.GraphicsDevice));

            //Score
            if (!dataLoaded)
            {
                highscores = new SaveGameData(10);
                load_data();
                dataLoaded = true;
            }

            //Sound
            waveManager.LoadWave("Content/music.wav", "bgm");
            hit = Content.Load<SoundEffect>("hit");
            ground = Content.Load<SoundEffect>("ground");

            ground_instance = ground.CreateInstance();
            ground_instance.Volume = 0.5f;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {


        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) &&
                    oldstate.IsKeyUp(Keys.Escape))
            {
                if (gameState == PAUSED || gameState == GAMEOVER)
                {
                    gameState = TITLE;
                    waveManager.StopSong();
                }
            }

            //Touch input
            TouchPanelCapabilities tc = TouchPanel.GetCapabilities();//Determine if there is a touch panel connected or not.
            GestureSample gesture;
            if (tc.IsConnected && TouchPanel.IsGestureAvailable)
                gesture = TouchPanel.ReadGesture();
            else
                gesture = new GestureSample();//Reset it so it's not stuck on one touch method.
            //Get the number of fingers on the screen.
            int numTaps = 0;
            TouchCollection touchCollection = TouchPanel.GetState();
            foreach (TouchLocation tl in touchCollection)
            {
                if ((tl.State == TouchLocationState.Pressed)
                        || (tl.State == TouchLocationState.Moved))
                {
                    numTaps++;//All of this to get the number of fingers on the screen and each one's position.
                }
            }



            //Pausing
            if ((Keyboard.GetState().IsKeyDown(Keys.P) || Keyboard.GetState().IsKeyDown(Keys.Enter)) &&
                (oldstate.IsKeyUp(Keys.P) && oldstate.IsKeyUp(Keys.Enter)))
            {
                if (gameState == GAME)
                    gameState = PAUSED;
                else if (gameState == PAUSED)
                    gameState = GAME;
            }
            else if (numTaps >= 3 || gesture.GestureType == GestureType.Flick)
            {
                if (gameState == GAME && numTaps >= 3)
                    gameState = PAUSED;
                else if (gameState == PAUSED && gesture.GestureType == GestureType.Flick)
                    gameState = GAME;
            }

            //Delete Scores
            if (Keyboard.GetState().IsKeyUp(Keys.X) && oldstate.IsKeyDown(Keys.X))
            {
                clear_scores(10);
                //save_data();
            }

            //Title
            if (gameState == TITLE)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter) || gesture.GestureType == GestureType.Tap)
                {
                    gameState = GAME;

                    //Reset the game
                    Initialize();
                    waveManager.PlayWave("bgm");
                }
            }
            //Game
            else if (gameState == GAME)
            {
                //Do all of the player stuff.
                player.Update(stages, gameTime, level, gesture);

                //Update the camera.
                camera(player);

                //Add new stage if necessary
                add_new_stage(stages);

                //Play sound on beat
                beat_timer(gameTime);

                //Fireworks
                for (int i = 0; i < fireworks.Count(); i++)
                {
                    fireworks.ElementAt(i).Update();
                }

                //Fragments
                for (int i = 0; i < fragments.Count(); i++)
                {
                    fragments.ElementAt(i).Update();
                    paint_sky(fragments.ElementAt(i));

                    bool hit_ground = fragments.ElementAt(i).hit_ground(fragments.ElementAt(i).position, stages);
                    bool hit_player = player.rectangle.Intersects(fragments.ElementAt(i).rectangle);

                    //Hit the player
                    if (hit_player)
                    {
                        if (!player.spinning)
                        {
                            if (playerHurtTimer <= 0)//Don't let the player get hurt too much.
                            {
                                lives -= 1;
                                hit.Play();
                                playerHurtTimer = playerHurtTimerMax;
                                paint_splat(fragments.ElementAt(i).color);
                            }
                            fragments.Remove(fragments.ElementAt(i));
                        }
                        //If the player is spinning
                        else
                        {
                            fragments.ElementAt(i).speedY = -5;
                            score += 30;
                        }
                    }
                    //Remove frags that hit the ground or go offscreen.
                    else if (hit_ground || fragments.ElementAt(i).position.Y > screenHeight)
                    {
                        fragments.Remove(fragments.ElementAt(i));
                        ground_instance.Play();//Use instance so only one can play at a time.
                    }
                }

                //Check if the player is in the safehouse
                for (int i = 0; i < stages.Count(); i++)
                {
                    //Reach the safehouse
                    if (player.rectangle.Intersects(stages.ElementAt(i).rectangle) && stages.ElementAt(i).safehouse)
                    {
                        inSafehouse = true;
                    }
                    else if (player.rectangle.Intersects(stages.ElementAt(i).rectangle) && !stages.ElementAt(i).safehouse)
                    {
                        inSafehouse = false;
                    }
                }

                //Paint the ground
                paint_the_ground();

                //Manage Lives
                if (lives <= 0)
                {
                    gameState = GAMEOVER;
                    //Show them their score
                    add_score(displayedScore);
                }
                //Subtract lives if the player falls off the stage.
                if (player.position.Y >= screenHeight)
                {
                    lives -= 1;
                    hit.Play();
                    player.position.Y = screenHeight / 3;
                    player.speedY = 0;//They don't have momentum built up.
                    player.spinning = false;
                    player.burstMode = false;
                }

                //Pause the game if the screen is snapped.
                if (Windows8._windowState == WindowState.Snap1Quarter)
                {
                    gameState = PAUSED;
                }

                //Update the camera
                //cameraView.Update(new Vector2(player.position.X - (screenWidth / 2), player.position.Y - (screenHeight / 2)));

                //Decrement timers
                if (playerHurtTimer > 0)
                    playerHurtTimer--;
            }
            else if (gameState == GAMEOVER)
            {
                if ((Keyboard.GetState().IsKeyUp(Keys.Enter) && oldstate.IsKeyDown(Keys.Enter)) || gesture.GestureType == GestureType.Tap)
                {
                    //Reset the game
                    SaveGameData temp = highscores;
                    Initialize();
                    highscores = temp;
                    gameState = GAME;
                    waveManager.RestartSong();
                }
            }
            else if (gameState == PAUSED)
            {

            }
            //Update the old keyboard state each frame.
            oldstate = Keyboard.GetState();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, cameraView.GetViewMatrix());

            //Call Title
            if (gameState == TITLE)
            {
                spriteBatch.Draw(title, new Vector2(0, 0), Color.White);
            }

            //Call Game
            else if (gameState == GAME || gameState == PAUSED)
            {
                //Draw the parallaxing background
                for (int j = 0; j < numParallax; j++)
                {
                    for (int i = 0; i < 2; i++)//Canonly have 2 backgrounds. They need to be > 1920 in width.
                        spriteBatch.Draw(parallaxImage[j], new Vector2(parallaxPosition[i, j].X, parallaxPosition[i, j].Y), Color.White);//Parallaxing bg
                }

                //Draw the stages over the player
                for (int i = 0; i < stages.Count(); i++)
                {
                    if (stages.ElementAt(i).position.X > -stages.ElementAt(i).width && stages.ElementAt(i).position.X < screenWidth)
                    {
                        spriteBatch.Draw(stages.ElementAt(i).bg, stages.ElementAt(i).position, Color.White);
                        spriteBatch.Draw(stages.ElementAt(i).color_map, stages.ElementAt(i).position, Color.White);
                    }

                    //Set up the rectangle for checking collisions and painting the sky.
                    stages.ElementAt(i).rectangle = new Rectangle((int)stages.ElementAt(i).position.X, (int)stages.ElementAt(i).position.Y,
                                                                    stages.ElementAt(i).width, stages.ElementAt(i).height);
                }
                //Draw the firworks
                for (int i = 0; i < fireworks.Count(); i++)
                {
                    fireworks.ElementAt(i).Draw(spriteBatch);
                }

                //Draw the fragments
                for (int i = 0; i < fragments.Count(); i++)
                {
                    fragments.ElementAt(i).Draw(spriteBatch);
                }

                //Draw the player
                player.Draw(spriteBatch);

                spriteBatch.End();

                //Start drawing stuff that doesn't scroll with the camera view.
                spriteBatch.Begin();

                //Draw the hearts
                for (int i = 0; i < lives; i++)
                {
                    spriteBatch.Draw(heart_sprite, new Vector2(heart_x + i * heart_sprite.Width, heart_y), Color.White);
                }

                //Draw the Score
                spriteBatch.DrawString(scoreFont, "Best: " + ((displayedScore > highscores.score[0]) ? displayedScore : highscores.score[0]) + "\nCurrent: " + displayedScore,
                                        new Vector2(20, 20), Color.Black);

                //Paused
                if (gameState == PAUSED)
                {
                    if (Windows8._windowState == WindowState.Snap1Quarter)
                    {
                        spriteBatch.Draw(paused, new Vector2(-45, (screenHeight / 2) - paused.Height / 2), Color.White);
                    }
                    else//This is the normal spot to draw the pause image.
                        spriteBatch.Draw(paused, new Vector2((screenWidth / 2) - paused.Width / 2, (screenHeight / 2) - paused.Height / 2), Color.White);
                }
            }
            //Game Over
            else if (gameState == GAMEOVER)
            {
                spriteBatch.Draw(gameover, new Vector2(0, 0), Color.White);
                /*spriteBatch.DrawString(scoreFont, "Score: " + displayedScore + "\nBest: " + highScore,
                                        new Vector2(screenWidth/2 - 100, screenHeight/2 - 20), Color.Black);*/
                spriteBatch.DrawString(scoreFont, "Score: " + displayedScore, new Vector2(screenWidth / 2 - 100, screenHeight / 3 - 20), Color.Black);
                for (int i = 0; i < 10; i++)
                {
                    spriteBatch.DrawString(scoreFont, (i + 1) + ": " + highscores.score[i], new Vector2(screenWidth / 2 - 100, screenHeight / 3 + +20 + 20 * i), Color.Black);
                }
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        //Camera function for moving the bg
        void camera(Player player)
        {
            float shift;

            //Set shift amount to the distance from the center.
            shift = player.position.X - (screenCenter - player.spriteWidth / 2);
            //Move the player to the center of the screen.
            player.position.X = (screenCenter - player.spriteWidth / 2);

            //Move the CMs and BGs
            for (int i = 0; i < stages.Count; i++)
            {
                stages.ElementAt(i).position.X -= shift;
            }

            //Move the fragments
            for (int i = 0; i < fragments.Count; i++)
            {
                fragments.ElementAt(i).position.X -= shift;
            }

            //Increase score based on shift
            tempScore += (int)shift;

            if (tempScore > 35)
            {
                score += (int)(2 * bps);
                tempScore = 0;
            }
            if (score > displayedScore)
                displayedScore = score;

            //Parallaxing Background
            for (int j = 0; j < numParallax; j++)
            {
                for (int i = 0; i < 2; i++)//There should only be 2 parallax backgrounds on each layer. Each one needs to be > 1920 in width.
                {
                    int other = (i == 0) ? 1 : 0;
                    //Allow backgrounds to not move.
                    if (parallaxFactor[j] != 0)
                    {
                        parallaxPosition[i, j].X -= shift / parallaxFactor[j];
                        if (parallaxPosition[i, j].X + parallaxImage[j].Width <= 0)
                        {
                            parallaxPosition[i, j].X = parallaxPosition[other, j].X + parallaxImage[j].Width - shift;//Move it to the end of the other one.
                        }
                        else if (parallaxPosition[i, j].X >= parallaxImage[j].Width)
                        {
                            parallaxPosition[i, j].X = parallaxPosition[other, j].X - parallaxImage[j].Width - shift;
                        }
                    }
                }
            }

        }

        void add_new_stage(LinkedList<Stage> stages)
        {
            if (stages.Last().position.X + stages.Last().width <= screenWidth)
            {
                //Any regular stage.
                if (stages.Count() < levelLength - 1)
                {
                    stages.AddLast(random_stage(new Vector2(stages.Last().position.X + stages.Last().width - 20, 0)));
                }
                //The one before the final stage is always bg1
                else if (stages.Count() == levelLength - 1)
                {
                    stages.AddLast(new Stage(bg[1], cm[1], collisionColorArray.ElementAt(1), stages.Last().position.X + stages.Last().width - 20, 0, graphics.GraphicsDevice));
                }
                //The safehouse
                else if (stages.Count() == levelLength)
                {
                    stages.AddLast(new Stage(bgx, cmx, collisionColorArray.ElementAt(collisionColorArray.Count() - 2), stages.Last().position.X + stages.Last().width - 20, 0, graphics.GraphicsDevice));
                    stages.Last().safehouse = true;
                    stages.AddLast(new Stage(bg[1], cm[1], collisionColorArray.ElementAt(1), stages.Last().position.X + stages.Last().width - 20, 0, graphics.GraphicsDevice));//The one after a safehouse is flat.
                }
                else if (stages.Count() >= levelLength && stages.ElementAt(stages.Count() - 2).position.X < (screenWidth / 2 - stages.ElementAt(stages.Count() - 2).width / 4))
                {
                    //Back up the last 3, clear out the rest of the list, then add the new 0 and 1.
                    Stage temp0 = stages.ElementAt(levelLength - 1);
                    Stage tempSafehouse = stages.ElementAt(levelLength);
                    Stage temp1 = stages.ElementAt(levelLength + 1);
                    stages.Clear();

                    //Add the 2 new ones
                    stages.AddLast(temp0);
                    stages.AddLast(new Stage(blocker_bg, blocker_cm, collisionColorArray.Last(), tempSafehouse.position.X, 0, graphics.GraphicsDevice));
                    //Let it keep the same color to look cool.
                    stages.Last().color_map = tempSafehouse.color_map;
                    stages.Last().safehouse = true;

                    //Finally, add a new stage, since this one reached the end.
                    stages.AddLast(temp1);

                    //Now you can increase the bps and lives. 
                    bps += 0.1;
                    //lives++;//You no longer increase your lives when you beat a level.
                    level++;
                    waveManager.ChangeSpeed(bps - 1);
                }

            }
        }

        public Stage random_stage(Vector2 position)
        {
            int r;
            r = random.Next(1, numStages);

            return new Stage(bg[r], cm[r], collisionColorArray.ElementAt(r), position.X, position.Y, graphics.GraphicsDevice);
        }

        public void beat_timer(GameTime gameTime)
        {

            fpsTimer += bps;

            //Set the voulme
            if (inSafehouse)
            {
                SoundEffect.MasterVolume = 0.1f;
            }
            else
            {
                SoundEffect.MasterVolume = 1f;
            }


            //Do stuff based on the FPS timer
            if ((fpsTimer >= 15 && fpsTimer <= 15 + bps) ||//Try to get it in the range.
                (fpsTimer >= 45 && fpsTimer <= 45 + bps))
            {
                // if (level >= 3)
                //hihat.Play();
            }
            if (fpsTimer >= 30 && fpsTimer <= 30 + bps)//Try to get it in the range.
            {
                //if (level >= 1)
                // offbeat.Play();
                //if (level >= 3)
                // hihat.Play();
            }
            else if (fpsTimer >= 60)//On beat. Launch the firework and play the beat.
            {

                fpsTimer = 0;//Reset the timer.

                //Play the beat
                //beat.Play();

                if (level >= 2)
                {
                    clapCount++;
                    if (clapCount > 1)
                    {
                        //   clap.Play();
                        clapCount = 0;
                    }
                }

                //Create a new firework every beat.
                fireworks.AddLast(new Firework(graphics.GraphicsDevice, random, bps));

                //Create a temporary list of fragments for this firework to add to the overall list.
                LinkedList<Fragment> temp = new LinkedList<Fragment>();
                temp = fireworks.ElementAt(0).explode(fireworks.ElementAt(0).position, level,
                                                        graphics.GraphicsDevice, random);
                for (int i = 0; i < temp.Count(); i++)
                {
                    fragments.AddLast(temp.ElementAt(i));//Add each element in the temp list to the overall list.
                }
                //Remove the dead firework.
                if (fireworks.Count() > 1)
                    fireworks.RemoveFirst();
            }
        }

        public void paint_sky(Fragment fragment)
        {
            int trailWidth = 7;
            int trailHeight = 7;

            //Set the color and size of the trail.
            Color[] colorArray = new Color[trailWidth * trailHeight];
            for (int i = 0; i < colorArray.Length; i++)
            {
                colorArray[i] = new Color(fragment.color.R, fragment.color.G, fragment.color.B, 255);
            }

            //Check if it collides with anything and paint if it does.
            for (int i = 0; i < stages.Count(); i++)
            {
                if (fragment.rectangle.Intersects(stages.ElementAt(i).rectangle))
                {
                    int newWidth = trailWidth;
                    int newHeight = trailWidth;
                    int frag_posx;
                    int frag_posy;

                    while (newWidth > 0 && newHeight > 0)
                    {
                        //Get the x and y of the fragment with respect to the background.
                        frag_posx = (int)(fragment.position.X + fragment.width / 2 - stages.ElementAt(i).position.X);
                        frag_posy = (int)(fragment.position.Y + fragment.height / 2);

                        //If the fragment fits on the screen entirely, print it.
                        if (frag_posx > 0 && frag_posy > 0 &&
                            frag_posx < stages.ElementAt(i).width - trailWidth && frag_posy < stages.ElementAt(i).height - trailHeight)
                        {
                            Rectangle trailRect = new Rectangle(frag_posx, frag_posy, trailWidth, trailHeight);
                            stages.ElementAt(i).color_map.SetData<Color>(0, trailRect, colorArray, 0, colorArray.Length);
                            break;//The rectangle was drawn, good job.
                        }
                        //Decrease the size to try to fit on the screen and try again.
                        newWidth--;
                        newHeight--;
                    }
                }
            }
        }

        public void paint_the_ground()
        {
            int trailWidth = 10;
            int trailHeight = 10;
            int randomColor = random.Next();
            Color trailBlockColor = Movable.get_random_color(random); ;
            //Only paint if he is in burstMode and running.
            if (player.burstMode && level > 0)
            {
                //Set the color
                Color[] colorArray = new Color[trailWidth * trailHeight];
                for (int i = 0; i < colorArray.Length; i++)
                {
                    colorArray[i] = trailBlockColor;
                }

                //Place it behind him
                for (int i = 0; i < stages.Count(); i++)
                {
                    if (player.rectangle.Intersects(stages.ElementAt(i).rectangle))
                    {
                        Rectangle trailRect = new Rectangle((int)(player.position.X - stages.ElementAt(i).position.X), (int)(player.position.Y + player.spriteHeight), trailWidth, trailHeight);
                        stages.ElementAt(i).color_map.SetData<Color>(0, trailRect, colorArray, 0, colorArray.Length);
                    }
                }
            }
        }

        public void paint_splat(Color color)
        {
            //Set the color
            Color[] colorArray = new Color[player.spriteHeight * player.spriteWidth];
            Color[] playerSpriteArray = new Color[player.spriteHeight * player.spriteWidth];
            //player.sprite.GetData<Color>(playerSpriteArray);

            for (int i = 0; i < colorArray.Length; i++)
            {
                if (playerSpriteArray[i].A /*!*/== 0)
                {
                    colorArray[i] = color;
                }
            }

            for (int i = 0; i < stages.Count(); i++)
            {
                if (player.rectangle.Intersects(stages.ElementAt(i).rectangle))
                {
                    Rectangle splatRect = new Rectangle((int)(player.position.X - stages.ElementAt(i).position.X), (int)(player.position.Y), player.spriteWidth, player.spriteHeight);
                    stages.ElementAt(i).color_map.SetData<Color>(0, splatRect, colorArray, 0, colorArray.Length);
                }
            }
        }

        Color[] make_collision_color(Texture2D cm)
        {
            //Establish a collision_color array
            Color[] collisionColor = new Color[cm.Width * cm.Height];
            cm.GetData<Color>(collisionColor);

            return collisionColor;
        }

        void add_score(int newScore)
        {
            int i, j;
            for (i = 0; i < 10; i++)
            {
                if (newScore > highscores.score[i])
                {
                    for (j = 9; j > i; j--)
                    {
                        highscores.score[j] = highscores.score[j - 1];
                    }
                    highscores.score[i] = newScore;
                    break;
                }
            }

            //Save the new highscores.
            save_data();
        }

        private async void save_data()
        {
            await SaveAsync(highscores);
        }

        async Task SaveAsync(SaveGameData saveData)
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("SynesthesiaSave", CreationCollisionOption.ReplaceExisting);
            IRandomAccessStream raStream = await file.OpenAsync(FileAccessMode.ReadWrite);
            using (IOutputStream outStream = raStream.GetOutputStreamAt(0))
            {
                // Serialize the Session State.
                DataContractSerializer serializer = new DataContractSerializer(typeof(SaveGameData));
                serializer.WriteObject(outStream.AsStreamForWrite(), saveData);
                await outStream.FlushAsync();
            }

        }

        protected async void load_data()
        {
            await RestoreAsync();
        }

        async Task RestoreAsync()
        {
            try
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync("SynesthesiaSave");
                IRandomAccessStream inStream = await file.OpenReadAsync();
                // Deserialize the Session State.
                DataContractSerializer serializer = new DataContractSerializer(typeof(SaveGameData));
                highscores = (SaveGameData)serializer.ReadObject(inStream.AsStreamForRead());

                inStream.Dispose();
            }
            catch
            {
                return;
            }
        }

        void clear_scores(int size)
        {
            for (int i = 0; i < size; i++)
                highscores.score[i] = 0;
        }
    }
}
