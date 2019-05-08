using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperMarioBros
{
    public class Bullet : Mob
    {
        public Bullet(Rectangle rectangle, Texture2D texture) : base(rectangle, texture)
        {
            this.texture = ContentManager.Textures["bullet"];
            System.Timers.Timer timer = new System.Timers.Timer(5000);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            timer.AutoReset = false;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.enemydead = true;
        }

        public override void Update()
        {

        }
        
        public void SetVelocity(Vector2 mario, Vector2 plant)
        {
            Vector2 direction = mario - plant;
            direction.Normalize();
            direction *= 100;

            this.velocityX = direction.X;
            this.velocityY = direction.Y;
        }

        public override void Move(float TimeElapsed)
        {
            if (collisionX || collisionYbot || collisionYtop) this.enemydead = true;
            rectangle.X += velocityX * TimeElapsed;
            rectangle.Y += velocityY * TimeElapsed;
        }

        float angle = 0;
        public override void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            angle += 0.3f;
            spriteBatch.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) + offset, scale: new Vector2(rectangle.width / texture.Width, rectangle.height / texture.Height), origin: new Vector2(16, 8), rotation: angle);
        }
    }

    enum PlantState {grow , look , shoot };

    class Plant : Enemy , IDamaged
    {
        public PlantState state;
        float Time;
        bool lethal;
        bool shoot = true;
        public Plant(Rectangle rectangle, Texture2D texture) : base(rectangle, texture)
        {
            state = PlantState.grow;
            Time = 0;
            lethal = false;
        }

        public override void Update(){}

        public override void Move(float TimeElapsed)
        {
            Time += TimeElapsed;
            if(Time >= 10) { Time = 0; }
            if(Time < 2)
            {
                lethal = true;
                shoot = true;
                velocityY = -30;
                state = PlantState.grow;
            }
            else if(Time < 4)
            {
                velocityY = 0;
                state = PlantState.look;
            }
            else if(Time < 5)
            {
                if (shoot)
                {
                    Bullet bullet = new Bullet(new Rectangle(rectangle.X,rectangle.Y,16,16), null);
                    bullet.SetVelocity(new Vector2(scene.player.rectangle.X, scene.player.rectangle.Y), new Vector2(this.rectangle.X, this.rectangle.Y) );
                    bullet.velocityX *= 3;
                    bullet.velocityY *= 3;
                    scene.QueryAddGameObject(bullet);
                    shoot = false;
                }
                state = PlantState.shoot;
            }
            else if (Time < 6)
            {
                state = PlantState.look;
            }
            else if(Time < 8)
            {
                velocityY = 30;
                state = PlantState.grow;
            }
            else
            {
                lethal = false;
                velocityY = 0;
            }
            rectangle.Y += velocityY * TimeElapsed;

            if (lethal && (GetHitSide || GetHitTopLeft || GetHitTopRight)) scene.player.GetHit(this);
            
            //base.Move(TimeElapsed);
        }

        Rectangle IDamaged.GetRect { get { return rectangle; } }
        void IDamaged.GetDamaged()
        {
            this.enemydead = true;
            collidable = false;
            scene.QueryAddGameObject(new CloudSpark(rectangle.X , rectangle.Y - 40));
        }
        

        int i = 0; int j = 0;
        public override void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            i++;
            if (i > 5) { i = 0; j++; if (j == 2) j = 0; }

            switch (state)
            {
                case PlantState.grow:
                    {
                        spriteBatch.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) + offset, scale: new Vector2(1, 1), sourceRectangle: Animations.animations["plant_grow"][j]);
                        break;
                    }
                case PlantState.look:
                    {
                        if (scene.player.rectangle.X < this.rectangle.X)
                        {
                            if (scene.player.rectangle.Y > this.rectangle.Y) spriteBatch.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) + offset, scale: new Vector2(1, 1), sourceRectangle: Animations.animations["plant_shoot"][0]);
                            else spriteBatch.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) + offset, scale: new Vector2(1, 1), sourceRectangle: Animations.animations["plant_shoot"][1]);
                        }
                        else
                        {
                            if (scene.player.rectangle.Y > this.rectangle.Y) spriteBatch.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) + offset, scale: new Vector2(1, 1), sourceRectangle: Animations.animations["plant_shoot"][0], effects: SpriteEffects.FlipHorizontally);
                            else spriteBatch.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) + offset, scale: new Vector2(1, 1), sourceRectangle: Animations.animations["plant_shoot"][1], effects: SpriteEffects.FlipHorizontally);
                        }
                        break;
                    }
                case PlantState.shoot:
                    {

                        if (scene.player.rectangle.X < this.rectangle.X)
                        {
                            if (scene.player.rectangle.Y > this.rectangle.Y) spriteBatch.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) + offset, scale: new Vector2(1, 1), sourceRectangle: Animations.animations["plant_shoot"][2]);
                            else spriteBatch.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) + offset, scale: new Vector2(1, 1), sourceRectangle: Animations.animations["plant_shoot"][3]);
                        }
                        else
                        {
                            if (scene.player.rectangle.Y > this.rectangle.Y) spriteBatch.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) + offset, scale: new Vector2(1, 1), sourceRectangle: Animations.animations["plant_shoot"][2], effects: SpriteEffects.FlipHorizontally);
                            else spriteBatch.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) + offset, scale: new Vector2(1, 1), sourceRectangle: Animations.animations["plant_shoot"][3], effects: SpriteEffects.FlipHorizontally);
                        }
                        break;
                        
                    }

            }
        }

    }

    enum walkerState { alive, dying, damaged}
    
    public class Walker : Enemy, IDamaged
    {
        public static Random rnd = new Random();
        walkerState state;
        float timer;

        public Walker(Rectangle rectangle, Texture2D texture) : base(rectangle, texture)
        {
            this.rectangle = rectangle;
            int d = rnd.Next(0, 2);
            int e = rnd.Next(0, 2);
            this.texture = 0 == e?ContentManager.Textures["walker1"]: ContentManager.Textures["walker2"];
            velocityX = d == 0 ? 140 : -140;
            state = walkerState.alive;
            timer = 0;
        }
        

        public override void Update()
        {
            base.Update();
        }
        
        public override void Move(float TimeElapsed)
        {
            if (state == walkerState.dying)
            {
                timer += TimeElapsed;
                if (timer >= 2) enemydead = true ;
                if (collisionYbot) velocityY = 0;
                rectangle.Y += velocityY * TimeElapsed;
                
            }
            else if (state == walkerState.damaged)
            {
                
                timer += TimeElapsed;
                if (timer >= 2) enemydead = true;
                rectangle.Y += velocityY * TimeElapsed;
            }
            else
            {
                if (GetHitTopLeft || GetHitTopRight)
                {
                    GetHitTopLeft = false;
                    GetHitTopRight = false;
                    state = walkerState.dying;
                    collidable = false;
                    velocityX = 0;
                }
                if (GetHitSide)
                {
                    scene.player.GetHit(this);
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
                    velocityY = 0;
                    if (collisionYtop)
                    {
                        velocityY += 15;
                        rectangle.Y += velocityY * TimeElapsed;
                    }
                }
            }
        }

        int i = 0; int k = 0;

        Rectangle IDamaged.GetRect { get { return rectangle; } }
        void IDamaged.GetDamaged()
        {
            velocityY = -150;
            collidable = false;
            state = walkerState.damaged;
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            i++;
            if (i > 5)
            {
                i = 0;
                k++;
                if (k == 2) k = 0;
            }
            switch (state)
            {
                case walkerState.alive:
                    spriteBatch.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) + offset, scale: new Vector2(1, 1), sourceRectangle: Animations.animations["walker"][k], effects: collidable ? SpriteEffects.None : SpriteEffects.FlipVertically);
                    break;
                case walkerState.damaged:
                    spriteBatch.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) + offset, scale: new Vector2(1, 1), sourceRectangle: Animations.animations["walker"][1], effects: collidable ? SpriteEffects.None : SpriteEffects.FlipVertically);
                    break;
                case walkerState.dying:
                    spriteBatch.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) + offset, scale: new Vector2(1, 1), sourceRectangle: Animations.animations["walker"][2], effects: SpriteEffects.None );
                    break;
                    
            }

        }

        
    }
}
