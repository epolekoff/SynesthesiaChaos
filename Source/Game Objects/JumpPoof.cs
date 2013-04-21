using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    class JumpPoof
    {
        public Vector2 position;
        Texture2D texture;
        public Animation anim;
        public int spriteWidth = 50;
        public int spriteHeight = 50;

        public JumpPoof(Vector2 position, Texture2D texture)
        {
            this.texture = texture;
            this.position = position;

            anim = new Animation();
            anim.Initialize(texture, position, spriteWidth, spriteHeight, 0, 5, 60, Color.White, 1, false);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            anim.Draw(spriteBatch, true);
        }

        public void Update(GameTime gameTime)
        {
            anim.Update(gameTime, position);
        }
    }
}
