using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace SuperMarioBros
{
    public interface IHittableFromBot
    {
        void GetHit();
    }
    public interface IHittableFromTop
    {
        void GetHit();
    }
    public interface IHittable
    {
        void GetHit();
    }
    public interface IMovable
    {
        void Update(float TimeElapsed);
    }

    public class DestroyableTile : Tile, IHittableFromBot
    {
        public DestroyableTile(Rectangle rectangle, Texture2D texture) : base(rectangle, texture, true)
        {
        }
        public void Destroy()
        {
            SpawnFlyingTiles();
            ContentManager.SoundEffects["Breaking Blocks"].Play();
            alive = false;
        }

        public void GetHit()
        {
            if (!(scene.player.playerState is SmallMarioState))
            {
                SpawnFlyingTiles();
                ContentManager.SoundEffects["Breaking Blocks"].Play();
                alive = false;
            }
        }
        Random rnd = new Random();
        public void SpawnFlyingTiles()
        {
            for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
            {
                scene.QueryAddGameObject(new FlyingTile(new Rectangle(rectangle.X + i * 8,rectangle.Y + j*8,8,8), texture.ExtractTexture(new Rectangle(i*8,j*8,8,8)), new Vector2(rnd.Next()%80 - 40, rnd.Next()%30 - 140)));
            }
           
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public class FlyingTile : Tile, IMovable
    {
        Vector2 velocity;
        float TTL;
        private void SentenceToDeath()
        {
            TTL = 1;
        }
        public FlyingTile(Rectangle rectangle, Texture2D texture, Vector2 speed) : base(rectangle, texture, false)
        {
            velocity = speed;
            SentenceToDeath();
        }
        public void Update(float TimeElapsed)
        {
            TTL -= TimeElapsed;
            if (TTL < 0) this.alive = false;
            velocity.Y += 15;
            rectangle.X += velocity.X * TimeElapsed;
            rectangle.Y += velocity.Y * TimeElapsed;
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            spriteBatch.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) + offset, scale: new Vector2(rectangle.width / texture.Width, rectangle.height / texture.Height)/*, rotation:velocity.Length()*/);
        }
    }
    public abstract class SpawningTile: Tile, IHittableFromBot
    {
        protected bool gothit = false;
        public SpawningTile(Rectangle rectangle, Texture2D texture) : base(rectangle, texture, true)
        {
        }
        public abstract void SpawnMob();
        public abstract void GetHit();
    }

    public enum WhatIsSpawned { shroom, feather, flower, frog,coin}
    
    public class SpawningMob : SpawningTile
    {
        public static Random rnd = new Random(DateTime.Now.GetHashCode());

        public WhatIsSpawned what;
        public SpawningMob(Rectangle rectangle, Texture2D texture, WhatIsSpawned what) : base(rectangle, texture)
        {
            this.what = what;
            rand = rnd.Next() % 4;
        }
        int rand;
        int r = 0;  int i = 0;
        public override void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            if (!gothit)
            {
                r++; if (r > 5)
                {
                    r = 0; i++; if (i == 4) i = 0;
                }
                texture = ContentManager.tilesTextures[67 + i, rand];
            }
            spriteBatch.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) + offset, scale: new Vector2(rectangle.width / texture.Width, rectangle.height / texture.Height));
        }

        public override void GetHit()
        {
            if (!gothit)
            {
                gothit = true;
                this.texture = ContentManager.tilesTextures[60, 1];
                SpawnMob();

                switch (what)
                {
                    case WhatIsSpawned.shroom:
                        ContentManager.SoundEffects["Level Chest Opened"].Play(0.3f, 0, 0);
                        break;
                    case WhatIsSpawned.feather:
                        break;
                    case WhatIsSpawned.flower:
                        ContentManager.SoundEffects["Pausing"].Play(0.3f, 0, 0);
                        break;
                    case WhatIsSpawned.coin:
                        ContentManager.SoundEffects["coin"].Play(0.3f, 0, 0);
                        break;
                }
            }
        }

        public override void SpawnMob()
        {
            switch(what){
                case WhatIsSpawned.shroom:
                    SpawnShroom();
                    break;
                case WhatIsSpawned.feather:
                    SpawnFeather();
                    break;
                case WhatIsSpawned.coin:
                    SpawnCoin();
                    break;
                case WhatIsSpawned.flower:
                    Console.WriteLine("flower");
                    SpawnFlower();
                    break;
            }
           
        }

        private void SpawnCoin()
        {
            CoinSpark coinSpark = new CoinSpark(rectangle.X, rectangle.Y-32);
            scene.QueryAddGameObject(coinSpark);
        }

        private void SpawnShroom()
        {
            Shroom shroom = new Shroom(new Rectangle(rectangle.X, rectangle.Y - 1, 32, 32), ContentManager.Textures["mushroom"]);
            shroom.spawning = true;
            scene.QueryAddGameObject(shroom);
        }
        private void SpawnFeather()
        {
            Feather feather = new Feather(new Rectangle(rectangle.X - 64, rectangle.Y - 64, 32, 32), ContentManager.Textures["feather"], new Vector2(10, 0));
            scene.QueryAddGameObject(new CloudSpark(rectangle.X -64, rectangle.Y - 64));
            scene.QueryAddGameObject(feather);
        }
        private void SpawnFlower()
        {
            Flower flower = new Flower(new Rectangle(rectangle.X, rectangle.Y - 1, 32, 32));
            flower.spawning = true;
            scene.QueryAddGameObject(flower);
            scene.QueryAddGameObject(new CloudSpark(rectangle.X, rectangle.Y - 32));
        }
    }

    public class Cloud : Tile, IMovable
    {
        Random rnd = new Random(DateTime.Now.GetHashCode());
        public bool combo = false;
        public Cloud(Rectangle rectangle, bool combo = false) 
        {
            this.rectangle = rectangle;
            this.solid = false;

            if (combo)
            {
                this.combo = true;
                this.texture = Texture2DExtender.ProperCloud(10);
                velocity = new Vector2((rnd.Next(2) == 1 ? -1 : 1) * rnd.Next(50, 100), 0);
            }
            else
            {
                switch (rnd.Next(3))
                {
                    case 0:
                        this.texture = Texture2DExtender.GetTextureRectangleFromTable(ContentManager.tilesTextures, 42, 0, 44, 1);
                        break;
                    case 1:
                        this.texture = Texture2DExtender.GetTextureRectangleFromTable(ContentManager.tilesTextures, 44, 2, 46, 3);
                        break;
                    case 2:
                        this.texture = Texture2DExtender.GetTextureRectangleFromTable(ContentManager.tilesTextures, 47, 2, 49, 4);
                        break;
                }
                velocity = new Vector2((rnd.Next(2) == 1 ? -1 : 1) * rnd.Next(30, 60), 0);
            }
            
            
        }

        Vector2 velocity;

        public void Update(float TimeElapsed)
        {
            rectangle.X += velocity.X * TimeElapsed;
            if (rectangle.X < 0 || rectangle.X > 6000) velocity.X *= -1;
            
        }
    }

    public class Feather : Tile , IMovable, IHittable
    {
        Vector2 velocity;
        float TimeToSwing = 0;
        public Feather(Rectangle rectangle, Texture2D texture, Vector2 speed) : base(rectangle, texture, false)
        {
            velocity = speed;
        }
        public void Update(float TimeElapsed)
        {
            //////////// Swinging logic
            TimeToSwing += TimeElapsed;
            if(TimeToSwing > 0.6)
            {
                TimeToSwing = 0;
                velocity.Y = -75;
                if (velocity.X > 0) velocity.X = -0.005f;
                else velocity.X = 0.005f;
            }

            /////////    
            velocity.X +=  velocity.X > 0? 15f : -15f;
            velocity.Y += 7;
            rectangle.X += velocity.X * TimeElapsed;
            rectangle.Y += velocity.Y * TimeElapsed;
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            if (velocity.X<0) spriteBatch.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) + offset, scale: new Vector2(rectangle.width / texture.Width, rectangle.height / texture.Height)/*, rotation:velocity.Length()*/);
            else spriteBatch.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) + offset, scale: new Vector2(rectangle.width / texture.Width, rectangle.height / texture.Height), effects: SpriteEffects.FlipHorizontally/*, rotation:velocity.Length()*/);
        }

        void IHittable.GetHit()
        {
            Console.WriteLine("feather");
            this.alive = false;
            scene.player.GetHit(this);
        }
    }

    public class Platform : Mob
    {
        public Tile[] tiles;
        private float dist = 0;
        bool falling = false;
        float accel = 1.0f;
        public float maxDist = 0.0f;

        public Platform Clone()
        {
            Tile[] copiedTiles = new Tile[3];
            for (int i = 0; i < 3; i++)
            {
                copiedTiles[i] = new Tile((Rectangle)tiles[i].rectangle.Clone(), tiles[i].texture, false);
            }
            return new Platform((Rectangle)rectangle.Clone(), copiedTiles, maxDist);
        }

        public Platform(Rectangle rectangle, Tile[] tiles, float maxDist) : base(rectangle, null)
        {
            this.tiles = tiles;
            this.velocityX = 120f;
            this.maxDist = maxDist;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            foreach (var i in tiles)
            {
                i.Draw(spriteBatch, offset);
            }
            return;
        }

        public void AddToScene(Scene scene)
        {
            scene.AddGameObject(tiles);
            this.scene = scene;
        }

        public override void Move(float TimeElapsed)
        {

            Rectangle temp = this.rectangle + scene.player.rectangle;
            temp.Y -= 4f;
            temp.height -= rectangle.height;
            temp.height -= scene.player.rectangle.height / 2;
            Vector2 MarioCenter = scene.player.rectangle.center();
            temp += new Rectangle(0, 0, -4, 0);
            bool collideY = false;
            if (temp.X < MarioCenter.X && temp.X + temp.width > MarioCenter.X && temp.Y < MarioCenter.Y && temp.Y + temp.height > MarioCenter.Y)
            {
                if (scene.player.velocityY > 0)
                {
                    if (!falling) velocityY = 40f;
                    falling = true;
                    collideY = true;
                    velocityX = 0;
                }
            }
            
            float m = velocityX * TimeElapsed;
            float w = velocityY * TimeElapsed;
            rectangle.X += m;
            rectangle.Y += w;
            if (collideY)
            {
                if (!scene.player.collisionYbot)
                {
                    if (scene.player.velocityY >= -40)
                    {
                        scene.player.collisionYbot = true;
                        scene.player.TextureRect.Y = rectangle.Y - scene.player.TextureRect.height;
                        scene.player.rectangle.Y = rectangle.Y - scene.player.rectangle.height;
                        scene.player.velocityY = velocityY;
                    }
                    else
                    {
                        scene.player.velocityY = velocityY;
                        collisionYbot = false;
                    }
                }

            }
            foreach (var i in tiles)
            {
                i.rectangle.X += m;
                i.rectangle.Y += w;
            }
            dist += m;
            if (dist > maxDist || dist < 0) velocityX *= -1;
        }

        public override void Update()
        {
            if (falling)
            {
                velocityY += 8;
                //accel *= 1f + 2*TimeElapsed;
                //w *= accel;
            }
            return;
        }
    }
}
