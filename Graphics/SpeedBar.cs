using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace SuperMarioBros
{
    public class SpeedBar
    {
        public SpeedBar()
        {
        }
        private const float maxspeed = 315;

        Texture2D triangle_unactive = ContentManager.Textures["speedbar"].ExtractTexture(new Rectangle(0, 0, 32, 32));
        Texture2D triangle_active = ContentManager.Textures["speedbar"].ExtractTexture(new Rectangle(32, 0, 32, 32));
        Texture2D p_unactive = ContentManager.Textures["speedbar"].ExtractTexture(new Rectangle(64, 0, 32, 32));
        Texture2D p_active = ContentManager.Textures["speedbar"].ExtractTexture(new Rectangle(96, 0, 32, 32));

        public void UpdateAndDraw(SpriteBatch sprite,float speed,bool unactive,bool superactive)
        {
            int height = 450;
            int width = 20;

            if (unactive)
            {
                for (int i = 0; i < 5; i++)
                {
                    sprite.Draw(triangle_unactive, position: new Vector2(width + i * 32, height), scale: new Vector2(0.8f, 0.8f));
                }
                    sprite.Draw(p_unactive, position: new Vector2(width + 5 * 32, height - 6), scale: new Vector2(1f, 1f));
                return;
            }
            else if (superactive)
            {
                for (int i = 0; i < 5; i++)
                {
                    sprite.Draw(triangle_active, position: new Vector2(width + i * 32, height), scale: new Vector2(0.8f, 0.8f));
                }
                sprite.Draw(p_active, position: new Vector2(width + 5 * 32, height - 6), scale: new Vector2(1f, 1f));
                return;
            }

            int level=0;
            for(int i = 0; i < 7; i++)
            {
                if (speed / maxspeed <= (i+1) / 7f)
                { level = i;break; }
            }
            int max = 5;
            if (level < max) max = level;
            for(int i = 0; i < max; i++)
            {
                sprite.Draw(triangle_active, position: new Vector2(width + i * 32, height), scale:new Vector2(0.8f,0.8f));
            }
            for(int i = max; i < 5; i++)
            {
                sprite.Draw(triangle_unactive, position: new Vector2(width + i * 32, height), scale: new Vector2(0.8f, 0.8f));
            }
            if(level == 6)
            {
                sprite.Draw(p_active, position: new Vector2(width + 5 * 32, height-6), scale: new Vector2(1f, 1f));
            }
            else
            {
                sprite.Draw(p_unactive, position: new Vector2(width + 5 * 32, height-6), scale: new Vector2(1f, 1f));
            }

        }
    }
}