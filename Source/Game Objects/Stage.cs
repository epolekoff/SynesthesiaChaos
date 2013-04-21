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
    public class Stage
    {
        int number;
        public Vector2 position;
        public Texture2D bg;
        public Texture2D cm;
        public Color[] collisionColor;
        public Texture2D color_map;
        public Rectangle rectangle;
        public bool safehouse = false;
        public int width;
        public int height;

        //Collectibles
        public Vector2[] collectiblePositions;
        public int numPositions;

        public Stage(int number, Texture2D background, Texture2D collision, Color[] collisionColor, float x, float y, GraphicsDevice graphicsDevice)
        {
            this.number = number;

            bg = background;
            cm = collision;

            position.X = x;
            position.Y = y;

            width = bg.Width;
            height = bg.Height;

            //Create a blank texture.
            color_map = new Texture2D(graphicsDevice, width, height);

            //Take the passed in value.
            this.collisionColor = collisionColor;

            //Set the collectible positions based on what stage this is.
            collectiblePositions = new Vector2[2];
            numPositions = 2;
            if (number % 2 == 0)
            {
                collectiblePositions[0].X = 1000;
                collectiblePositions[0].Y = 500;
                collectiblePositions[1].X = 700;
                collectiblePositions[1].Y = 400;
            }
            else if (number % 2 == 1)
            {
                collectiblePositions[0].X = 10;
                collectiblePositions[0].Y = 10;
                collectiblePositions[1].X = 800;
                collectiblePositions[1].Y = 800;
            }
        }
    }
}
