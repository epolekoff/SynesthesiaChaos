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
        public Vector2 position;
        public Texture2D bg;
        public Texture2D cm;
        public Color[] collisionColor;
        public Texture2D color_map;
        public Rectangle rectangle;
        public bool safehouse = false;
        public int width;
        public int height;

        public Stage(Texture2D background, Texture2D collision, Color[] collisionColor, float x, float y, GraphicsDevice graphicsDevice)
        {
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
        }
    }
}
