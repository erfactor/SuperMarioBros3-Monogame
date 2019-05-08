using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace SuperMarioBros
{
    public class FireBall : Enemy, IDamager
    {
        public FireBall(Rectangle rectangle, Texture2D texture,bool right) : base(rectangle, texture)
        {
            //this.velocityX = right ? 260 : -260;
            deathTimer = new System.Timers.Timer(3000);
            deathTimer.Elapsed += DeathTimer_Elapsed;
            deathTimer.Start();
        }
        bool justSpawned = true;

        private void DeathTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.enemydead = true;
        }

        Rectangle IDamager.GetRect { get { return rectangle; } }

        bool IDamager.IsDamaging { get { return true; } }
        
        System.Timers.Timer deathTimer;
        public override void Move(float TimeElapsed)
        {
            if (justSpawned)
            {
                if (collisionX) rectangle.X += velocityX > 0 ? -16 : +16;
                justSpawned = false;
            }
            if (!collisionX)
            {
                rectangle.X += velocityX * TimeElapsed;
            }
            else velocityX = -velocityX;

            if (!collisionYbot && !collisionYtop)
            {
                rectangle.Y += velocityY * TimeElapsed;
            }
            else
            {
                velocityY = -Math.Sign(velocityY) * 200;
                //if (collisionYtop) rectangle.Y += velocityY * TimeElapsed;
                
            }
            
        }

        float angle = 0;
        public override void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            angle += 0.4f;
            spriteBatch.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) + offset, scale: new Vector2(rectangle.width / texture.Width, rectangle.height / texture.Height), origin: new Vector2(16, 16), rotation: angle);
        }

    }

    public class Coin : Tile , IHittable
    {
        public Coin(Rectangle rectangle, Texture2D texture) : base(rectangle,texture,false)
        {
            texture = ContentManager.Textures["coin"];
        }
        
        int i = 0, k = 0;
        public override void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            i++; if (i > 5)
            {
                i = 0;
                k++; if (k == 5) k = 0;
            }
            spriteBatch.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) + offset, scale: new Vector2(rectangle.width / 32, rectangle.height / 32), sourceRectangle: Animations.animations["coin"][k]);
        }

        void IHittable.GetHit()
        {
            alive = false;
            scene.player.coins++;
            SpawnSparkles();
            ContentManager.SoundEffects["coin"].Play(0.3f, 0, 0);
        }

        public static Random rnd = new Random();
        private void SpawnSparkles()
        {
            scene.QueryAddGameObject(new Sparkle(rectangle.X-12 +rnd.Next()%11, rectangle.Y-12 +rnd.Next()%11));
            scene.QueryAddGameObject(new Sparkle(rectangle.X+12 +rnd.Next()%11, rectangle.Y-12 +rnd.Next()%11));
            scene.QueryAddGameObject(new Sparkle(rectangle.X-12 +rnd.Next()%11, rectangle.Y+12 +rnd.Next()%11));
            scene.QueryAddGameObject(new Sparkle(rectangle.X+12 +rnd.Next()%11, rectangle.Y+12 +rnd.Next()%11));
        }
    }

    public class Star : Tile, IHittable
    {
        public Star(Rectangle rectangle) 
        {
            this.rectangle = rectangle;
            texture = ContentManager.Textures["star1"];
        }
        Texture2D texture_b = ContentManager.Textures["star2"];

        int i = 0, k = 0;
        public override void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            i++; if (i > 5)
            {
                i = 0;
                k++; if (k == 2) k = 0;
            }
            if (k == 0)
                spriteBatch.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) + offset);
            else
                spriteBatch.Draw(texture_b, position: new Vector2(rectangle.X, rectangle.Y) + offset);
            
        }

        void IHittable.GetHit()
        {
            alive = false;
            scene.player.finished = true;
            MediaPlayer.Pause();
        }
        
    }

    public class Sparkle : Tile, IMovable
    {
        public static Random rnd = new Random();
        private float TTL;
        private float start;
        Texture2D sparkle = ContentManager.Textures["sparkle"];
        public Sparkle(float x, float y)
        {
            this.rectangle = new Rectangle(x,y,14,14);
            this.texture = sparkle.ExtractTexture(new Rectangle(0*16,0,16,16));
            TTL = (float) (0.5 + rnd.NextDouble() * 0.3);
            start = TTL;
            texture1 = sparkle.ExtractTexture(new Rectangle(4 * 16, 0, 16, 16)); 
            texture2 = sparkle.ExtractTexture(new Rectangle(3 * 16, 0, 16, 16)); 
            texture3 = sparkle.ExtractTexture(new Rectangle(2 * 16, 0, 16, 16)); 
            texture4 = sparkle.ExtractTexture(new Rectangle(1 * 16, 0, 16, 16)); 
        }
        Texture2D texture1, texture2, texture3, texture4;

        void IMovable.Update(float TimeElapsed)
        {
            TTL -= TimeElapsed;
            switch (TTL/start)
            {
                case float x when (x <= 0):
                    this.alive = false;
                    break;
                case float x when (x <= 0.2):
                    texture = texture1;
                    break;
                case float x when (x <= 0.4):
                    texture = texture2;
                    break;
                case float x when (x <= 0.6):
                    texture = texture3;
                    break;
                case float x when (x <= 0.8):
                    texture = texture4;
                    break;
            }
        }
    }

    public class CloudSpark : Tile, IMovable
    {
        public static Random rnd = new Random();
        private float TTL;
        private float start;
        Texture2D sparkle = ContentManager.Textures["cloud_anim"];
        public CloudSpark(float x, float y)
        {
            this.solid = false;
            this.rectangle = new Rectangle(x, y, 40, 40);
            this.texture = sparkle.ExtractTexture(new Rectangle(0 * 16, 0, 16, 16));
            TTL = (float)(0.6 + rnd.NextDouble() * 0.3);
            start = TTL;
        }

        void IMovable.Update(float TimeElapsed)
        {
            TTL -= TimeElapsed;
            switch (TTL / start)
            {
                case float x when (x <= 0):
                    this.alive = false;
                    break;
                case float x when (x <= 0.25):
                    texture = sparkle.ExtractTexture(new Rectangle(3 * 16, 0, 16, 16));
                    break;
                case float x when (x <= 0.5):
                    texture = sparkle.ExtractTexture(new Rectangle(2 * 16, 0, 16, 16));
                    break;
                case float x when (x <= 0.75):
                    texture = sparkle.ExtractTexture(new Rectangle(1 * 16, 0, 16, 16));
                    break;
            }
        }
    }

    public class CoinSpark : Tile, IMovable
    {
        private float TTL;
        public CoinSpark(float x, float y)
        {
            this.rectangle = new Rectangle(x, y, 32, 32);
            this.texture = ContentManager.tilesTextures[60, 7];
            this.solid = false;
            TTL = 0.2f;
        }
        float VelY;

        void IMovable.Update(float TimeElapsed)
        {
            rectangle.Y -= 15;
            TTL -= TimeElapsed;
            if (TTL < 0)
            {
                this.alive = false;
            }
        }
    }
}
