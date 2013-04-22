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
    class Collectible
    {
        public Vector2 position;
        public int type;
        public Texture2D texture;
        public Animation animation;
        public int spriteHeight = 35;
        public int spriteWidth = 35;
        public Rectangle rectangle;

        public Collectible(Vector2 position, int type, Texture2D texture)
        {
            this.position = position;
            this.type = type;
            this.texture = texture;
            animation = new Animation();
            animation.Initialize(texture, position, spriteWidth, spriteHeight, 0, 3, 80, Color.White, 1, true);

            rectangle = new Rectangle((int)position.X, (int)position.Y, spriteWidth, spriteHeight);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            animation.Draw(spriteBatch, true);
        }

        public void Update(GameTime gameTime)
        {
            animation.Update(gameTime, position);
            rectangle = new Rectangle((int)position.X - spriteWidth, (int)position.Y - spriteHeight, spriteWidth, spriteHeight);
        }

    }
}
