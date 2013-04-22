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
            if (number == 15)
            {
                numPositions = 4;
                collectiblePositions = new Vector2[numPositions];

                collectiblePositions[0].X = 1512;
                collectiblePositions[0].Y = 540;
                collectiblePositions[1].X = 324;
                collectiblePositions[1].Y = 370;
                collectiblePositions[2].X = 812;
                collectiblePositions[2].Y = 676;
                collectiblePositions[3].X = 258;
                collectiblePositions[3].Y = 266;
            }
            else if (number == 9)
            {
                numPositions = 3;
                collectiblePositions = new Vector2[numPositions];

                collectiblePositions[0].X = 1556;
                collectiblePositions[0].Y = 1000;
                collectiblePositions[1].X = 1864;
                collectiblePositions[1].Y = 276;
                collectiblePositions[2].X = 1744;
                collectiblePositions[2].Y = 1008;
            }
            else if (number == 9)
            {
                numPositions = 3;
                collectiblePositions = new Vector2[numPositions];

                collectiblePositions[0].X = 1556;
                collectiblePositions[0].Y = 1000;
                collectiblePositions[1].X = 1864;
                collectiblePositions[1].Y = 276;
                collectiblePositions[2].X = 1744;
                collectiblePositions[2].Y = 1008;
            }
            else if (number == 10)
            {
                numPositions = 2;
                collectiblePositions = new Vector2[numPositions];

                collectiblePositions[0].X = 1744;
                collectiblePositions[0].Y = 742;
                collectiblePositions[1].X = 1828;
                collectiblePositions[1].Y = 558;
            }
            else if (number == 12)
            {
                numPositions = 2;
                collectiblePositions = new Vector2[numPositions];

                collectiblePositions[0].X = 1682;
                collectiblePositions[0].Y = 740;
                collectiblePositions[1].X = 1640;
                collectiblePositions[1].Y = 560;
            }
            else if (number == 16)
            {
                numPositions = 1;
                collectiblePositions = new Vector2[numPositions];

                collectiblePositions[0].X = 188;
                collectiblePositions[0].Y = 438;
            }
            else if (number == 19)
            {
                numPositions = 2;
                collectiblePositions = new Vector2[numPositions];

                collectiblePositions[0].X = 803;
                collectiblePositions[0].Y = 439;
                collectiblePositions[1].X = 503;
                collectiblePositions[1].Y = 381;
            }
            else if (number == 20)
            {
                numPositions = 1;
                collectiblePositions = new Vector2[numPositions];

                collectiblePositions[0].X = 1119;
                collectiblePositions[0].Y = 631;
            }
            else//Default
            {
                numPositions = 1;
                collectiblePositions = new Vector2[1];
                collectiblePositions[0].X = -100;
                collectiblePositions[0].Y = -100;
            }
        }
    }
}
