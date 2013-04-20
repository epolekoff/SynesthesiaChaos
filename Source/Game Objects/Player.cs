using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace SynesthesiaChaos
{
    class Player : Movable
    {
        //Sprite Stuff
        public int spriteHeight;
        public int spriteWidth;
        public Animation animation;
        public Animation idleAnimation;
        public Animation runAnimation;
        public Animation sprintAnimation;
        public Animation jumpAnimation;
        public Animation turnAnimation;
        public Animation fallAnimation;
        public Animation spinAnimation;
        bool directionRight= true;
        int animSpeed = 40;
        public Rectangle rectangle;

        //Physics Stuff
        public Vector2 position;
        double speedX;
        public double speedY;
        double accel = 0.45;
        int direction = 1;//This is a positive/negative modifier for acceleration.
        double gravity = 0.4;
        int maxSpeed = 7;
        int jumpHeight = 10;
        int shortHopHeight = 6;
        int terminalVelocity = 10;
        bool inAir = true;
        public Color[] collisionColor;

        //Burst Mode
        int burstSpeed;
        int maxBurstSpeedLevel = 5;
        public int burstTimer;
        public int burstTimerMax = 120;
        public bool burstMode;

        //Spinning
        public bool spinning = false;
        int spinTimer = 0;
        int spinTimerMax = 25;
        int hopHeight = 4;

        //Touch stuff
        int numTaps = 0;
        float tapPositionX;

        KeyboardState oldstate;


        public Player()
        {

        }

        public void Initialize(int sprite_width, int sprite_height, ContentManager Content, int initial_x, int initial_y)
        {
            //Load Testures
            Texture2D player_run = Content.Load<Texture2D>("playerrun");
            Texture2D player_sprint = Content.Load<Texture2D>("playersprint");
            Texture2D player_idle = Content.Load<Texture2D>("playeridle");
            Texture2D player_jump = Content.Load<Texture2D>("playerjump");
            Texture2D player_turn = Content.Load<Texture2D>("playerturn");
            Texture2D player_fall = Content.Load<Texture2D>("playerfall");
            Texture2D player_spin = Content.Load<Texture2D>("playerspin");

            position.X = initial_x;
            position.Y = initial_y;

            spriteHeight = sprite_height;
            spriteWidth = sprite_width;
            collisionColor = new Color[1];

            rectangle = new Rectangle((int)position.X, (int)position.Y, spriteWidth, spriteHeight);

            //Set up the animation
            idleAnimation = new Animation();
            runAnimation = new Animation();
            sprintAnimation = new Animation();
            jumpAnimation = new Animation();
            turnAnimation = new Animation();
            fallAnimation = new Animation();
            spinAnimation = new Animation();
            animation = new Animation();
            
            idleAnimation.Initialize(player_idle, position, spriteWidth, spriteHeight, 0, 5, animSpeed*2, Color.White, 1, true);
            runAnimation.Initialize(player_run, position, spriteWidth, spriteHeight, 6, 13, animSpeed, Color.White, 1, true);
            sprintAnimation.Initialize(player_sprint, position, spriteWidth, spriteHeight, 0, 7, animSpeed, Color.White, 1, true);
            jumpAnimation.Initialize(player_jump, position, spriteWidth, spriteHeight, 0, 3, 3*animSpeed, Color.White, 1, false);
            turnAnimation.Initialize(player_turn, position, spriteWidth, spriteHeight, 0, 0, animSpeed, Color.White, 1, false);
            fallAnimation.Initialize(player_fall, position, spriteWidth, spriteHeight, 0, 1, 3*animSpeed, Color.White, 1, false);
            spinAnimation.Initialize(player_spin, position, spriteWidth, spriteHeight, 0, 6, animSpeed/2, Color.White, 1, true);
            animation = idleAnimation;//By default

            oldstate = new KeyboardState();
        }

        public void Update(LinkedList<Stage> stages, GameTime gameTime,int level, GestureSample gesture)
        {
            TouchPanelCapabilities tc = TouchPanel.GetCapabilities();//Determine if there is a touch panel connected or not.
               
            numTaps = 0;//The number of fingers on the screen.
            TouchCollection touchCollection = TouchPanel.GetState();
            foreach (TouchLocation tl in touchCollection)
            {
                if ((tl.State == TouchLocationState.Pressed)
                        || (tl.State == TouchLocationState.Moved))
                {
                    numTaps++;//All of this to get the number of fingers on the screen and each one's position.
                    tapPositionX = tl.Position.X;
                }
            }

            //Ground Collision
            if (hit_ground(new Vector2(position.X + spriteWidth / 2, position.Y + spriteHeight), stages))
            {
                inAir = false;
            }
            else
            {
                inAir = true;
                //If you are not already in burst mode, falling removes burst mode.
                if (!burstMode)
                {
                    burstMode = false;
                    burstTimer = 0;
                }

                //Short hop
                if (!Keyboard.GetState().IsKeyDown(Keys.Up) && !Keyboard.GetState().IsKeyDown(Keys.Space) && numTaps == 0)
                {
                    if (speedY > shortHopHeight)
                        speedY = shortHopHeight;//If you are not holding the jump button, you jump less.
                }
            }

            //Set the burst speed.
            if (level <= maxBurstSpeedLevel)
                burstSpeed = maxSpeed + level;

            //Spin timer
            if (spinTimer > 0 && spinning)
                spinTimer--;
            else
                spinning = false;
            

            //Move the player
            horizontal_movement(level, gesture, numTaps, tapPositionX);
            vertical_movement(stages, gesture, numTaps);

            //Animate
            //Which animation do you play?
            choose_animation(level);

            animation.Update(gameTime, new Vector2(position.X + spriteWidth, position.Y + spriteHeight));

            //Actually do the movement
            if (!hit_ground(new Vector2((float)(position.X + spriteWidth / 2 + Math.Sign(speedX) * spriteWidth / 2), position.Y + spriteHeight / 2), stages))
            {
                position.X += (float)speedX;//Only move in the X if there is not a wall in your path.
            }
            else
            {
                speedX = 0;//If you hit a wall, you are no longer moving.
                burstMode = false;//And burst mode is killed
                burstTimer = 0;
            }
            position.Y -= (float)speedY;

            //Update the rectangle based on the new position.
            rectangle = new Rectangle((int)position.X, (int)position.Y, spriteWidth, spriteHeight);

            //Update the keyboard with the new state
            oldstate = Keyboard.GetState();
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(sprite, position, Color.White);
            animation.Draw(spriteBatch, directionRight);
        }



        //Custom Functions
        void horizontal_movement(int level, GestureSample gesture, int numTaps, float tapPositionX)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Right) || (tapPositionX > EntireGame.screenWidth / 2 && numTaps >= 1))
            {
                //Accelerate
                if (speedX < maxSpeed)
                {
                    direction = 1;
                    speedX += accel;
                }
                //Account for overshoot
                if (speedX > maxSpeed && !burstMode)
                {
                    speedX = maxSpeed;//Make them equal if it is over.
                }
                directionRight = true;

                //Burst Mode
                if (speedX == maxSpeed && burstTimer < burstTimerMax)
                {
                    burstTimer++;
                    burstMode = false;
                }
                else if (burstTimer >= burstTimerMax)
                {
                    burstMode = true;
                    speedX = burstSpeed;
                }

            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Left) || (tapPositionX < EntireGame.screenWidth / 2 && numTaps >= 1))
            {
                //Accelerate
                if (speedX > -maxSpeed)
                {
                    direction = -1;
                    speedX += accel*direction;
                }
                //Account for overshoot
                if (speedX < -maxSpeed)
                {
                    speedX = -maxSpeed;//Make them equal if it is over.
                }
                directionRight = false;

                //Cancel burst speed
                burstMode = false;
                burstTimer = 0;//Jumping cancels the burst speed.
            }
            //If no key is held.
            else if (numTaps == 0 && Keyboard.GetState().IsKeyUp(Keys.Right) && Keyboard.GetState().IsKeyUp(Keys.Left))
            {
                if (speedX < accel && speedX > -accel)
                {
                    speedX = 0;
                    runAnimation.currentFrame = 0;//Reset the animation
                }
                else if (speedX >= accel)
                {
                    speedX -= accel;
                }
                else if (speedX <= -accel)
                {
                    speedX += accel;
                }

                //Cancel burst speed
                burstMode = false;
                burstTimer = 0;//Jumping cancels the burst speed.
            }
        }

        void vertical_movement(LinkedList<Stage> stages, GestureSample gesture, int numTaps)
        {
            //Fall
            if (inAir)//In the air, fall
            {
                speedY -= gravity;
                if (speedY < -terminalVelocity){    speedY = -terminalVelocity;  }//Don't fall too fast

                //If you hit your head, stop.
                if (hit_ground(new Vector2(position.X + spriteWidth / 2, position.Y), stages) && speedY > 0)//Going up and hit ground, stop going up.
                {
                    speedY = 0;
                }
            }
            else//If they are on the ground, stop moving.
            {
                speedY = 0;
                for (int i = 0; i < spriteHeight/2; i++)
                {
                    if (!hit_ground(new Vector2(position.X + spriteWidth/2, position.Y + spriteHeight- i), stages))
                    {
                        position.Y -= (i-1);
                        break;
                    }
                }
                spinning = false;//Stop spinning when you hit the ground.
                spinTimer = spinTimerMax;
            }

            //Jump
            if (Keyboard.GetState().IsKeyDown(Keys.Space) || Keyboard.GetState().IsKeyDown(Keys.Up) || gesture.GestureType == GestureType.VerticalDrag || numTaps >= 2)
            {
                if (!inAir)//They can only jump when not in the air.
                {
                    speedY = jumpHeight;
                }
                //Spin
                if (inAir && !spinning && spinTimer == spinTimerMax && ((Keyboard.GetState().IsKeyDown(Keys.Space) && oldstate.IsKeyUp(Keys.Space)) || (Keyboard.GetState().IsKeyDown(Keys.Up) && oldstate.IsKeyUp(Keys.Up))))
                {
                    spinning = true;
                    spinAnimation.currentFrame = 0;
                    speedY += hopHeight;
                }
            }
        }

        void choose_animation(int level)
        {
            if (!inAir)
            {
                if (speedX != 0)
                {
                    if (speedX * direction < 0)//They are decellerating
                    {
                        animation = turnAnimation;
                    }
                    else if (burstMode && level != 0)//Don't start burst mode if you are at level 0.
                    {
                        animation = sprintAnimation;
                    }
                    else
                    {
                        animation = runAnimation;
                    }

                }
                else
                {
                    animation = idleAnimation;
                }
            }
            else
            {
                if (speedY > 0)
                {
                    animation = jumpAnimation;
                }
                else if(!burstMode)
                {
                    animation = fallAnimation;
                }
                if (spinning)
                {
                    animation = spinAnimation;
                }
            }
        }

    }
}
