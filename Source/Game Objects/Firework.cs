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
    public class Firework : Movable
    {
        //Movement
        public Vector2 position;
        float angle;
        double speed;
        double speedY = 0;
        double gravity = 0.28;
        double bps;

        //Color
        Color color;
        public Texture2D rect;
        int width = 7;
        int height = 7;


        public Firework(GraphicsDevice graphicsDevice, Random randomNumber, double bps)
        {

            this.bps = bps;
            angle = randomNumber.Next(318, 325);
            speed = randomNumber.Next((int)(20*bps), (int)(25*bps));
            position = new Vector2(0, 2*screenHeight/3);//Spawn offscreen

            //Creating the rectangle and using color.
            color = get_random_color(randomNumber);
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
            speedY += gravity*bps;

            //Actually move the fragment
            position.X += (float)(speed * Math.Cos(MathHelper.ToRadians(angle)));
            position.Y += (float)(speed * Math.Sin(MathHelper.ToRadians(angle)) + speedY);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(rect, position, Color.White);
        }


        public LinkedList<Fragment> explode(Vector2 position, int level, GraphicsDevice graphicsDevice, Random random)
        {
            int numFrags = random.Next(1, 2 + (level / 2));
            LinkedList<Fragment> fragments = new LinkedList<Fragment>();

            for (int i = 0; i < numFrags; i++)
            {
                fragments.AddLast(new Fragment(position, graphicsDevice, random));
            }
            return fragments;
        }

    }
}
