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
    public class Movable
    {
        protected int screenWidth = EntireGame.screenWidth;
        protected int screenHeight = EntireGame.screenHeight;

        public bool hit_ground(Vector2 position, LinkedList<Stage> stages)
        {
            //Color[] collisionColor = new Color[screenHeight * screenWidth];
            for (int i = 0; i < stages.Count(); i++)
            {
                int shiftedX = (int)position.X - (int)stages.ElementAt(i).position.X;
                if (shiftedX > 0 && shiftedX < stages.ElementAt(i).width &&
                    position.Y < stages.ElementAt(i).height && position.Y > 0)
                {
                    //stages.ElementAt(i).cm.GetData<Color>(0, new Rectangle(shiftedX, (int)position.Y, 2, 2), collisionColor, 0, 4);
                    //stages.ElementAt(i).cm.GetData<Color>(collisionColor, shiftedX + ((int)position.Y * screenWidth), 1);
                    //stages.ElementAt(i).cm.GetData<Color>(collisionColor);

                    if (stages.ElementAt(i).collisionColor[shiftedX + ((int)position.Y * stages.ElementAt(i).width)] == Color.Black)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static Color get_random_color(Random randomNumber)
        {
            int r = randomNumber.Next(0, 14);

            switch (r)
            {
                case 0:
                    return Color.Red;
                case 1:
                    return Color.OrangeRed;
                case 2:
                    return Color.Yellow;
                case 3:
                    return Color.Green;
                case 4:
                    return Color.Lime;
                case 5:
                    return Color.Teal;
                case 6:
                    return Color.Aqua;
                case 7:
                    return Color.Navy;
                case 8:
                    return Color.Blue;
                case 9:
                    return Color.Purple;
                case 10:
                    return Color.DeepPink;
                case 11:
                    return Color.DarkOrchid;
                case 12:
                    return Color.Maroon;
                default:
                    return Color.Chartreuse;

            }
            //return new Color(randomNumber.Next(0, 255), randomNumber.Next(0, 255), randomNumber.Next(0, 255));
        }

    }
}
