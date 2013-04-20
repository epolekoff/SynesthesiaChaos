// Animation.cs
//Using declarations
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SynesthesiaChaos
{
    class Animation
    {
        // The image representing the collection of images used for animation
        Texture2D spriteStrip;

        // The scale used to display the sprite strip
        float scale;

        // The time since we last updated the frame
        int elapsedTime;

        // The time we display a frame until the next one
        public int frameTime;

        //Frames
        public int currentFrame;// The index of the current frame we are displaying
        public int startFrame;// The first frame in the looping part of the animation.
        public int endFrame;// The number of frames that the animation contains

        // The color of the frame we will be displaying
        public Color color;

        // The area of the image strip we want to display
        Rectangle sourceRect = new Rectangle();

        // The area where we want to display the image strip in the game
        Rectangle destinationRect = new Rectangle();

        // Width of a given frame
        public int FrameWidth;

        // Height of a given frame
        public int FrameHeight;

        // The state of the Animation
        public bool Moving;

        // Determines if the animation will keep playing or deactivate after one run
        public bool Looping;

        // Width of a given frame
        public Vector2 Position;

        public void Initialize(Texture2D texture, Vector2 position,
                                int frameWidth, int frameHeight, int startFrame, int endFrame,
                                int frametime, Color color, float scale, bool looping)
        {
            // Keep a local copy of the values passed in
            this.color = color;
            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;
            this.startFrame = startFrame;
            this.endFrame = endFrame;

            this.frameTime = frametime;
            this.scale = scale;

            Looping = looping;
            Position = position;
            spriteStrip = texture;

            // Set the time to zero
            elapsedTime = 0;
            currentFrame = 0;

            // Set the Animation to active by default
            Moving = true;
        }

        public void Update(GameTime gameTime, Vector2 newPosition)
        {
            //Set the new position
            Position = newPosition;

            // Update the elapsed time
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            // If the elapsed time is larger than the frame time
            // we need to switch frames
            if (elapsedTime > frameTime)
            {
                // Move to the next frame
                currentFrame++;

                // If the currentFrame is past the last frame reset currentFrame to the start
                if (currentFrame == endFrame + 1)
                {
                    if (Looping)
                        currentFrame = startFrame;
                    else
                        currentFrame = endFrame;

                }

                // Reset the elapsed time to zero
                elapsedTime = 0;
            }

            // Grab the correct frame in the image strip by multiplying the currentFrame index by the frame width
            sourceRect = new Rectangle(currentFrame * FrameWidth, 0, FrameWidth, FrameHeight);

            // Grab the correct frame in the image strip by multiplying the currentFrame index by the frame width
            destinationRect = new Rectangle((int)Position.X - (int)(FrameWidth * scale) / 2,
                                            (int)Position.Y - (int)(FrameHeight * scale) / 2,
                                            (int)(FrameWidth * scale),
                                            (int)(FrameHeight * scale));
        }


        // Draw the Animation Strip
        public void Draw(SpriteBatch spriteBatch, bool direction)
        {
            /*
             public void Draw ( Texture2D texture,
                                Rectangle destinationRectangle,
                                Nullable<Rectangle> sourceRectangle,
                                Color color,
                                float rotation,
                                Vector2 origin,
                                SpriteEffects effects,
                                float layerDepth)
            */
            Vector2 origin = new Vector2(FrameWidth / 2, FrameHeight / 2);

            spriteBatch.Draw(spriteStrip, destinationRect, sourceRect, color,
                0, origin, direction ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
        }
    }
}
