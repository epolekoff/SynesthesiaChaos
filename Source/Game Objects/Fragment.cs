using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SynesthesiaChaos
{
    public class Fragment : Movable
    {
        //Movement
        public Vector2 position;
        float angle;
        double speed;
        public double speedY = 0;
        double gravity = 0.05;

        //Color
        public Color color;
        public Texture2D rect;
        public int width = 12;
        public int height = 12;

        //Collision
        public Rectangle rectangle = new Rectangle();

        //Deflection
        public bool deflected = false;
        int deflectedTimer = 0;
        int deflectedTimerMax = 10;

        public Fragment(Vector2 newPosition, GraphicsDevice graphicsDevice, Random random)
        {

            angle = random.Next(205, 405);
            speed = random.Next(1, 8);
            position = newPosition;

            //Creating the rectangle and using color.
            color = Movable.get_random_color(random);
            Color[] colorArray = new Color[width * height];
            for (int i = 0; i < colorArray.Length; i++)
            {
                colorArray[i] = color;
            }
            rect = new Texture2D(graphicsDevice, width, height);
            rect.SetData<Color>(colorArray);
        }

        public void Update()
        {
            speedY += gravity;

            //Actually move the fragment
            position.X += (float)(speed * Math.Cos(MathHelper.ToRadians(angle)));
            position.Y += (float)(speed * Math.Sin(MathHelper.ToRadians(angle)) + speedY);

            //Make a rectangle for collisions
            rectangle = new Rectangle((int)position.X, (int)position.Y, width, height);

            //Count down on deflected timer
            if (deflectedTimer > 0)
            {
                deflectedTimer--;
            }
            else
            {
                deflected = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(rect, position, Color.White);

        }
        public void deflect()
        {
            deflected = true;
            deflectedTimer = deflectedTimerMax;
            speedY = -5;
        }
    }
}
