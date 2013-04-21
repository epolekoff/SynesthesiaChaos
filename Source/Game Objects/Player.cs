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
        public Animation flyupAnimation;
        public Animation rollAnimation;
        public Animation wallslideAnimation;
        bool directionRight= true;
        public bool movingRight = true;
        float oldX;
        int animSpeed = 40;
        public Rectangle rectangle;

        //Poofs
        public LinkedList<JumpPoof> poofs;
        Texture2D jump_poof;

        //Physics Stuff
        public Vector2 position;
        double speedX;
        public double speedY;
        double accel = 0.45;
        int direction = 1;//This is a positive/negative modifier for acceleration.
        double gravity;
        double regularGravity = 0.4;
        int maxSpeed;
        int regularMaxSpeed = 7;
        int jumpHeight = 10;
        int shortHopHeight = 6;
        int terminalVelocity;
        int regularTerminalVelocity = 10;
        bool inAir = true;
        public bool flyUp = false;
        public bool invincible = false;
        bool rolling = false;
        public Color[] collisionColor;

        //Burst Mode
        int burstSpeed;
        int maxBurstSpeedLevel = 5;
        public int burstTimer;
        public int burstTimerMax = 120;
        public bool burstMode;
        public int burstDistance = 0;//The amount of time(distance) spent bursting.

        //Spinning
        public bool spinning = false;
        int spinTimer = 0;
        int spinTimerMax = 25;
        int hopHeight = 4;

        //Wall Slide
        bool wallSliding = false;
        double wallGravity = 0.05;
        int wallTerminalVelocity = 8;
        double wallSpeedX = 12;
        int wallSlideTimer = 0;
        int wallSlideTimerMax = 15;

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
            Texture2D player_flyup = Content.Load<Texture2D>("playerflyup");
            Texture2D player_roll = Content.Load<Texture2D>("playerroll");
            Texture2D player_wallslide = Content.Load<Texture2D>("playerwallslide");
            jump_poof = Content.Load<Texture2D>("jumppoof");

            //Jump Poofs
            poofs = new LinkedList<JumpPoof>();

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
            flyupAnimation = new Animation();
            rollAnimation = new Animation();
            wallslideAnimation = new Animation();
            animation = new Animation();
            
            idleAnimation.Initialize(player_idle, position, spriteWidth, spriteHeight, 0, 5, animSpeed*2, Color.White, 1, true);
            runAnimation.Initialize(player_run, position, spriteWidth, spriteHeight, 6, 13, animSpeed, Color.White, 1, true);
            sprintAnimation.Initialize(player_sprint, position, spriteWidth, spriteHeight, 0, 7, animSpeed, Color.White, 1, true);
            jumpAnimation.Initialize(player_jump, position, spriteWidth, spriteHeight, 0, 3, 3*animSpeed, Color.White, 1, false);
            turnAnimation.Initialize(player_turn, position, spriteWidth, spriteHeight, 0, 0, animSpeed, Color.White, 1, false);
            fallAnimation.Initialize(player_fall, position, spriteWidth, spriteHeight, 0, 1, 3*animSpeed, Color.White, 1, false);
            spinAnimation.Initialize(player_spin, position, spriteWidth, spriteHeight, 0, 6, animSpeed/2, Color.White, 1, true);
            flyupAnimation.Initialize(player_flyup, position, spriteWidth, spriteHeight, 0, 1, animSpeed, Color.White, 1, true);
            rollAnimation.Initialize(player_roll, position, spriteWidth, spriteHeight, 0, 11, 2*animSpeed/3, Color.White, 1, false);
            wallslideAnimation.Initialize(player_wallslide, position, spriteWidth, spriteHeight, 0, 2, animSpeed, Color.White, 1, true);
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
                if (!Keyboard.GetState().IsKeyDown(Keys.Up) && !Keyboard.GetState().IsKeyDown(Keys.Space) && numTaps == 0 && !flyUp)
                {
                    if (speedY > shortHopHeight)
                        speedY = shortHopHeight;//If you are not holding the jump button, you jump less.
                }
            }

            //Set the burst speed.
            if (level <= maxBurstSpeedLevel)
                burstSpeed = regularMaxSpeed + level;

            //Spin timer
            if (spinTimer > 0 && spinning)
                spinTimer--;
            else
                spinning = false;

            //Wall Slides
            if (wallSlideTimer > 0)
                wallSlideTimer--;
            if (wallSliding && speedY <= 0)
            {
                terminalVelocity = wallTerminalVelocity;
                gravity = wallGravity;
            }
            else
            {
                terminalVelocity = regularTerminalVelocity;
                gravity = regularGravity;
            }

            //Jump Poofs
            for(int i = 0; i < poofs.Count(); i ++)
            {
                if (poofs.ElementAt(i).anim.currentFrame == poofs.ElementAt(i).anim.endFrame)
                {
                    poofs.Remove(poofs.ElementAt(i));
                }
                else
                {
                    poofs.ElementAt(i).Update(gameTime);
                }
            }

            //Move the player
            horizontal_movement(level, gesture, numTaps, tapPositionX);
            vertical_movement(stages, gesture, numTaps);

            //Animate
            //Which animation do you play?
            choose_animation(level);

            animation.Update(gameTime, new Vector2(position.X + spriteWidth, position.Y + spriteHeight));

            //Actually do the movement
            if (!hit_ground(new Vector2((float)(position.X + spriteWidth / 2 + Math.Sign(speedX) * spriteWidth / 2), position.Y + spriteHeight / 2), stages))//If their next move is not hitting a wall.
            {
                if (wallSlideTimer <= 0)//Allow them to press the opposite direction of the wall and jump instead of falling off.
                {
                    position.X += (float)speedX;//Allow the player to move horizontally.
                    wallSliding = false;
                }
            }
            else
            {
                speedX = 0;//If you hit a wall, you are no longer moving.

                //Wall Slide
                if (inAir)
                {
                    wallSliding = true;
                    wallSlideTimer = wallSlideTimerMax;//Give a little but of time on the wall before falling off.
                }
                //You can maintain burst mode by wall sliding.
                if (!wallSliding)
                {
                    burstMode = false;//And burst mode is killed
                    burstTimer = 0;
                }
            }
            position.Y -= (float)speedY;

            //Update the rectangle based on the new position.
            rectangle = new Rectangle((int)position.X, (int)position.Y, spriteWidth, spriteHeight);

            //Update the keyboard with the new state
            oldstate = Keyboard.GetState();
            oldX = position.X;

        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            if(animation == wallslideAnimation)
                animation.Draw(spriteBatch, movingRight);
            else
                animation.Draw(spriteBatch, directionRight);

            //Draw jump poofs
            for (int i = 0; i < poofs.Count(); i++)
            {
                poofs.ElementAt(i).Draw(spriteBatch);
            }
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
                if (speedX > maxSpeed)
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
                    maxSpeed = burstSpeed;
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
            }
            //Calculate th direction moving
            if (oldX < position.X)
                movingRight = false;
            else if (oldX > position.X)
                movingRight = true;

            //Cancel burst speed
            if (speedX < accel && speedX > -accel && !wallSliding)
            {
                burstMode = false;
                burstTimer = 0;//Stopping cancels the burst speed.
                maxSpeed = regularMaxSpeed;
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
                //Set the rolling variable to roll for when you hit the ground.
                if (burstMode)
                {
                    rolling = true;
                    rollAnimation.currentFrame = 0;
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
                //Spinning
                spinning = false;//Stop spinning when you hit the ground.
                spinTimer = spinTimerMax;

                //Stop flying upward after falling in a pit.
                if (flyUp == true)
                {
                    flyUp = false;
                    invincible = false;
                }

                //Stop rolling at the end of the animation.
                if(rolling && animation.currentFrame == animation.endFrame && animation == rollAnimation)
                {
                    rolling = false;
                }

                //Wall Sliding
                wallSliding = false;//If you are standing on the ground, you are not wall sliding.
            }

            //Jump
            if (Keyboard.GetState().IsKeyDown(Keys.Space) || Keyboard.GetState().IsKeyDown(Keys.Up) || gesture.GestureType == GestureType.VerticalDrag || numTaps >= 2)
            {
                if (!inAir || wallSliding)//They can only jump when not in the air.
                {
                    //Only jump if you press the jump button for wall jumps. Holding it is not allowed for wall jumps.
                    if(!wallSliding || (oldstate.IsKeyUp(Keys.Space) && oldstate.IsKeyUp(Keys.Up)))
                        speedY = jumpHeight;
                    if (wallSliding && (oldstate.IsKeyUp(Keys.Space) && oldstate.IsKeyUp(Keys.Up)))
                    {
                        speedX -= direction*wallSpeedX;
                    }
                    wallSlideTimer = 0;

                    //Jump Poof
                    if (!inAir)
                    {
                        poofs.AddFirst(new JumpPoof(new Vector2(position.X + spriteWidth, position.Y + spriteHeight), jump_poof));
                    }
                }
                //Spin
                if (inAir && !spinning && !wallSliding && spinTimer == spinTimerMax && ((Keyboard.GetState().IsKeyDown(Keys.Space) && oldstate.IsKeyUp(Keys.Space)) || (Keyboard.GetState().IsKeyDown(Keys.Up) && oldstate.IsKeyUp(Keys.Up))))
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
                        if (rolling)
                            animation = rollAnimation;
                        else
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
                    if (flyUp)
                    {
                        animation = flyupAnimation;
                    }
                    else
                    {
                        animation = jumpAnimation;
                    }
                }
                else
                {
                    animation = fallAnimation;
                }
                if (spinning)
                {
                    animation = spinAnimation;
                }
                if (wallSliding)
                {
                    animation = wallslideAnimation;
                }
            }
        }

    }
}
