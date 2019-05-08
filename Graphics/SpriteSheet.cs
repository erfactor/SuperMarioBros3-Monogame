using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SuperMarioBros
{
    public static class Animations
    {
        public static Dictionary<string, Microsoft.Xna.Framework.Rectangle[]> animations;

        public static void Initalize()
        {

            animations = new Dictionary<string, Microsoft.Xna.Framework.Rectangle[]>();
            animations.Add("mario_jump", new Microsoft.Xna.Framework.Rectangle[] { new Microsoft.Xna.Framework.Rectangle(0, 64, 32, 32) });
            animations.Add("mario_walk", new Microsoft.Xna.Framework.Rectangle[] { new Microsoft.Xna.Framework.Rectangle(0, 32, 32, 32), new Microsoft.Xna.Framework.Rectangle(32, 32, 32, 32), new Microsoft.Xna.Framework.Rectangle(64, 32, 32, 32) });
            animations.Add("mario_stand", new Microsoft.Xna.Framework.Rectangle[] { new Microsoft.Xna.Framework.Rectangle(0, 0, 32, 32) });

            animations.Add("turtle_walk", new Microsoft.Xna.Framework.Rectangle[] { new Microsoft.Xna.Framework.Rectangle(0, 0, 16 * 2, 32 * 2), new Microsoft.Xna.Framework.Rectangle(16 * 2, 0, 16 * 2, 32 * 2) });
            animations.Add("turtle_roll", new Microsoft.Xna.Framework.Rectangle[] { new Microsoft.Xna.Framework.Rectangle(32 * 2, 0, 16 * 2, 16 * 2), new Microsoft.Xna.Framework.Rectangle(48 * 2, 0, 16 * 2, 16 * 2), new Microsoft.Xna.Framework.Rectangle(64 * 2, 0, 16 * 2, 16 * 2), new Microsoft.Xna.Framework.Rectangle(80 * 2, 0, 16 * 2, 16 * 2) });
            animations.Add("turtle_stand", new Microsoft.Xna.Framework.Rectangle[] { new Microsoft.Xna.Framework.Rectangle(32 * 2, 16 * 2, 16 * 2, 16 * 2) });

            animations.Add("plant_grow", new Microsoft.Xna.Framework.Rectangle[] { new Microsoft.Xna.Framework.Rectangle(0, 0, 32, 64), new Microsoft.Xna.Framework.Rectangle(32, 0, 32, 64) });
            animations.Add("plant_shoot", new Microsoft.Xna.Framework.Rectangle[] { new Microsoft.Xna.Framework.Rectangle(3 * 32, 0, 32, 64), new Microsoft.Xna.Framework.Rectangle(5 * 32, 0, 32, 64) , new Microsoft.Xna.Framework.Rectangle(2 * 32, 0, 32, 64), new Microsoft.Xna.Framework.Rectangle(4 * 32, 0, 32, 64) });

            animations.Add("walker", new Microsoft.Xna.Framework.Rectangle[] { new Microsoft.Xna.Framework.Rectangle(0, 0, 32, 32), new Microsoft.Xna.Framework.Rectangle(32, 0, 32, 32), new Microsoft.Xna.Framework.Rectangle(64, 0, 32, 32) });

           
            animations.Add("small_mario", MakeTab(11, 32, 32, 32));
            
            animations.Add("big mario",  MakeTab(13,64,64,64,32) );
            animations.Add("fire mario",  MakeTab(13,64,64,64,224) );
            
            animations.Add("raccoon",  MakeTab(15,64,64,64,96) );
            animations.Add("raccoonfly",  MakeTab(5,64,64,64,96 + 64) );
            animations.Add("coin", MakeTab(5, 34, 32, 32));
            animations.Add("raccoon1", MakeTab(10, 30, 24, 32));
        }
            
        public static Microsoft.Xna.Framework.Rectangle[] MakeTab(int count, int offset , int width, int height,int offset_y = 0)
        {
            Microsoft.Xna.Framework.Rectangle[] tab = new Microsoft.Xna.Framework.Rectangle[count];

            for (int i = 0; i < count; i++)
            {
                tab[i] = new Microsoft.Xna.Framework.Rectangle(i * offset, offset_y, width, height);
            }
            return tab;
        }

        public static int RED(this uint u)
        {
            return (int)u>>16 & 0xff;
        }
        public static int GREEN(this uint u)
        {
            return (int)u >> 8 & 0xff;
        }
        public static int BLUE(this uint u)
        {
            return (int)u & 0xff;
        }
    }

    //public class Animation
    //{
    //    SpriteBatch spriteBatch;
    //    List<Texture2D> frames;

    //    public Animation()
    //    {
    //        frames = new List<Texture2D>();
    //    }
    //    public void AddFrame(Texture2D texture)
    //    {
    //        frames.Add(texture);
    //    }

    //    public void PlayAnimation()
    //    {

    //    }
    //}
}
