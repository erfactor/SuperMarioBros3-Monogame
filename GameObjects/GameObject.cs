using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace SuperMarioBros
{
    public class GameObject
    {
        public Rectangle rectangle;
        public Texture2D texture;

        public GameObject(){}

        public GameObject(Rectangle rectangle, Texture2D texture)
        {
            this.rectangle = rectangle;
            this.texture = texture;
        }

        public virtual void Draw(SpriteBatch spriteBatch,Vector2 offset)
        {
            spriteBatch.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) + offset, scale: new Vector2(rectangle.width / texture.Width, rectangle.height / texture.Height));
        }

    }

    public class Mob : GameObject
    {
        public Scene scene;
        public bool collisionX = false;
        public bool collisionYtop = false;
        public bool collisionYbot = false;
        public float velocityX = 0;
        public float velocityY = 0;
        public bool alive = true;
        public bool enemydead = false;
        public bool collidable = true;
        public Mob(Rectangle rectangle, Texture2D texture) : base(rectangle, texture) { }

        public Mob() { }

        public void SetScene(Scene scene)
        {
            this.scene = scene;
        }

        public virtual void Update()
        {
            velocityY += 15f;
        }

        public virtual void Move(float TimeElapsed)
        {
            if (!collisionX)
                {
                    rectangle.X += velocityX * TimeElapsed;
                }
            else velocityX = 0;

            if (!collisionYbot && !collisionYtop)
            {
                rectangle.Y += velocityY * TimeElapsed;
            }
            else
            {
                velocityY = 0;
                if (collisionYtop)
                {
                    //velocityY += 15;
                    rectangle.Y += velocityY * TimeElapsed;
                }
            }

        }
        public void ChangeVelocity(float x, float y)
        {
            velocityX += x;
            velocityY += y;
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            if(velocityX>=0 )spriteBatch.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) + offset, scale: new Vector2(rectangle.width / texture.Width, rectangle.height / texture.Height));
            else spriteBatch.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) + offset, scale: new Vector2(rectangle.width / texture.Width, rectangle.height / texture.Height), effects: SpriteEffects.FlipHorizontally);
        }
    }

    public class Shroom : Mob
    {
        Random rand;
        public bool spawning = false;
        float TimeSpawning = 0;
        public Shroom(Rectangle rectangle, Texture2D texture) : base(rectangle, texture)
        {
            int seed = DateTime.Now.Second;
            rand = new Random(seed);
            this.velocityX = ((float)rand.Next() % 2 - 0.5f) * 200;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Move(float TimeElapsed)
        {
            
            if (spawning)
            {
                TimeSpawning += TimeElapsed;
                if (TimeSpawning > 1) spawning = false;
                rectangle.Y -= 32 * TimeElapsed;
                return;
            }

            ///////////////
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
                velocityY = 0;
                if (collisionYtop)
                {
                    velocityY += 15;
                    rectangle.Y += velocityY * TimeElapsed;
                }
            }

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
    }

    
    public class Tile : GameObject
    {
        public Scene scene;
        public bool solid;
        public bool alive = true;
        public Tile(Rectangle rectangle, Texture2D texture, bool solid) : base(rectangle, texture)
        {
            this.solid = solid;
        }
        
        public Tile(Tile t) : this(t.rectangle, t.texture, t.solid) { }

        public Tile(){}
    }

    
    public class Block : Tile
    {
        public static Random rnd = new Random();
        public Block(Rectangle rectangle, Texture2D texture) : base(rectangle, texture, false)
        {
        }
        public Block(Point point,int width, int height)
        {
            
            int a = rnd.Next(3);
            if (a == 0) texture = ContentManager.Textures["block1"];
            if (a == 1) texture = ContentManager.Textures["block2"];
            if (a == 2) texture = ContentManager.Textures["block3"];
            texture = Texture2DExtender.ProperBlockTexture(texture , width, height);
            (var x,var y) = point;
            rectangle = new Rectangle(x,y,width*32,height*32 );
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            spriteBatch.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) + offset);
            spriteBatch.Draw(ContentManager.tilesTextures[20, 5], new Rectangle(rectangle.X + rectangle.width, rectangle.Y+8,8,rectangle.height-8) + offset, Color.Black);
        }
    }


}
