using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarioBros
{
    public class Flower : Tile, IHittable, IMovable
    {
        public bool spawning = false;
        float TimeSpawning = 0;
        public Flower(Rectangle rectangle) 
        {
            this.rectangle = rectangle;
            this.texture = ContentManager.Textures["flower"];
        }
        
        public override void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            if (spawning && (TimeSpawning * 100 % 20 < 10))
            {
                return;
            }
            else
            {
                base.Draw(spriteBatch, offset);
            }
        }

        void IHittable.GetHit()
        {
            scene.player.GetHit(this);
        }

        public void Update(float TimeElapsed)
        {
            if (spawning)
            {
                TimeSpawning += TimeElapsed;
                if (TimeSpawning > 1) spawning = false;
                rectangle.Y -= 32 * TimeElapsed;
            }
        }
    }

    public class SimpleDamage : Enemy, IDamager
    {
        public SimpleDamage(Rectangle rectangle, Texture2D texture) : base(rectangle, texture)
        {
            this.collidable = false;
        }

        Rectangle IDamager.GetRect { get { return this.rectangle; } }

        bool IDamager.IsDamaging  { get { return true; } }

        public override void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            
        }

        public override void Move(float TimeElapsed)
        {
            
        }

        public override void Update()
        {
            
        }
    }


}
