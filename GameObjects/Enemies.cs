using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public interface IDamager
{
    SuperMarioBros.Rectangle GetRect { get; }
    bool IsDamaging { get; }
}
public interface IDamaged
{
    SuperMarioBros.Rectangle GetRect { get; }
    void GetDamaged();
}

namespace SuperMarioBros
{
    public enum TurtleState { stand, walk, roll}

    public class Enemy : Mob
    {
        public bool GetHitTopLeft = false;
        public bool GetHitTopRight = false;
        public bool GetHitSide = false;
        public void CheckMario() { }

        public override void Update()
        {
            base.Update();
        }

        public Enemy(Rectangle rectangle, Texture2D texture) : base(rectangle, texture)
        {
        }
        
    }

    public class Turtle : Enemy, IDamager, IDamaged
    {
        private static Random rnd = new Random();
        public Turtle(Rectangle rectangle, Texture2D texture) : base(rectangle, texture)
        {
            this.texture = ContentManager.Textures["turtle" + rnd.Next(1, 4)];
            state = TurtleState.walk;
            prevstate = TurtleState.walk;
            velocityX = 100;
            timer = new System.Timers.Timer(200);
            timer.AutoReset = false;
            timer.Elapsed += Timer_Elapsed1;
        }
        public System.Timers.Timer timer;
        private void Timer_Elapsed1(object sender, System.Timers.ElapsedEventArgs e)
        {
            justspawned = false;
        }

        public TurtleState state;
        TurtleState prevstate;

        public override void Update()
        {
            prevstate = state;
            velocityY += 15f;
        }

        public override void Move(float TimeElapsed)
        {
            if (!collidable)
            {
                rectangle.Y += velocityY * TimeElapsed;
                return;
            }

            if (GetHitTopLeft || GetHitTopRight) ContentManager.SoundEffects["Shell"].Play();
            switch (prevstate)
            {
                case TurtleState.walk:
                    {
                        if (GetHitTopLeft || GetHitTopRight)
                        {
                            GetHitTopLeft = false;
                            GetHitTopRight = false;
                            state = TurtleState.stand;
                            this.rectangle = new Rectangle(rectangle.X, rectangle.Y + 32, 32, 32);
                            velocityX = 0;
                        }
                        if (GetHitSide)
                        {
                            scene.player.GetHit(this);
                        }
                        break;
                        Microsoft.Xna.Framework.Rectangle rc;
                    }
                case TurtleState.stand:
                    {
                        if (GetHitSide || GetHitTopLeft || GetHitTopRight)
                        {
                            if(Keyboard.GetState().IsKeyDown(Keys.C) && !scene.player.HasAttachedTurtle)
                            {
                                scene.player.HasAttachedTurtle = true;
                                this.enemydead = true;
                                return;
                            }
                            ContentManager.SoundEffects["Shell"].Play();
                            if (rectangle.height == 64)
                            {
                                this.rectangle = new Rectangle(rectangle.X, rectangle.Y + 32, 32, 32);
                            }
                        }
                        float speed = 300;
                        if (GetHitTopLeft )
                        {
                            GetHitTopLeft = false;
                            state = TurtleState.roll;
                            velocityX = speed;
                        }
                        else if (GetHitTopRight)
                        {
                            GetHitTopRight = false;
                            state = TurtleState.roll;
                            velocityX = -speed;
                        }
                        else if (GetHitSide)
                        {
                            GetHitSide = false;
                            if(scene.player.rectangle.X < this.rectangle.X)
                            {
                                state = TurtleState.roll;
                                velocityX = speed;
                            }
                            else
                            {
                                state = TurtleState.roll;
                                velocityX = -speed;
                            }
                        }
                        break;
                    }
                case TurtleState.roll:
                    {
                        if (GetHitTopLeft)
                        {
                            GetHitTopLeft = false;
                            state = TurtleState.stand;
                            velocityX = 0;
                        }
                        else if (GetHitTopRight)
                        {
                            GetHitTopRight = false;
                            state = TurtleState.stand;
                            velocityX = 0;
                        }
                        else if (GetHitSide)
                        {
                            GetHitSide = false;
                            scene.player.GetHit(this);
                        }
                        break;
                    }
            }

            CheckForRespawn(TimeElapsed);
            Random rnd = new Random();
            /////////////////////////////////
            if (!collisionYbot && !collisionYtop)
            {
                if (state == TurtleState.walk && velocityY<25)
                {
                    if (canturn)
                    {
                        System.Timers.Timer timer = new System.Timers.Timer(140);
                        timer.Elapsed += Timer_Elapsed2;
                        timer.AutoReset = false;
                        canturn = false;
                        timer.Start();
                        //rectangle.X += velocityX > 0 ? -4 : 4;
                        rectangle.Y -= (velocityY * TimeElapsed);
                    }

                    velocityX *= -1; 
                }
                rectangle.Y += velocityY * TimeElapsed;
            }
            else
            {
                velocityY = 0;
                if (collisionYtop)
                {
                    rectangle.Y += velocityY * TimeElapsed;
                }
            }

            if (state == TurtleState.walk)
            {
                if (velocityX == 0)
                {
                    int a = rnd.Next(0, 1);
                    if (a == 0) velocityX = 100;
                    else velocityX = -100;
                }
                else velocityX = Math.Sign(velocityX) * 100;
            }

            if (!collisionX)
            {
                rectangle.X += velocityX * TimeElapsed;
            }
            else
                if (Math.Abs(velocityX) < 450 && state == TurtleState.roll)
                {
                velocityX = -Math.Sign(velocityX) * (Math.Abs(velocityX) + 30);
                }
                else velocityX = -velocityX;
            
            if (collisionX && collisionYbot && justspawned) rectangle.X -= 6;
        }

        private bool canturn = true;
        private void Timer_Elapsed2(object sender, System.Timers.ElapsedEventArgs e)
        {
            canturn = true;
        }

        int i = 0; int j = 0; int k = 0; int xd = 0; bool up = false; int ii = 0;
        public override void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            i++; ii++;
            xd++; if (xd > 4) xd = 0;
            if (i > 5) {
                i = 0;
                k++; if (k == 2) k = 0;
            }

            if( ii > (300*5)/ Math.Abs(velocityX))
            {
                ii = 0;
                j++; if (j == 4) j = 0;
            }

            switch (state)
            {
                case TurtleState.roll:
                    {
                        spriteBatch.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) + offset, scale: new Vector2(1, 1), sourceRectangle: Animations.animations["turtle_roll"][j], effects: collidable ? SpriteEffects.None : SpriteEffects.FlipVertically);
                        break;
                    }
                case TurtleState.walk:
                    {
                        //this.rectangle = new Rectangle(rectangle. )
                        if (velocityX < 0) spriteBatch.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) + offset, scale: new Vector2(1, 1), sourceRectangle: Animations.animations["turtle_walk"][k], effects: collidable ? SpriteEffects.None : SpriteEffects.FlipVertically);
                        else spriteBatch.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) + offset, scale: new Vector2(1, 1), sourceRectangle: Animations.animations["turtle_walk"][k], effects: SpriteEffects.FlipHorizontally | (collidable ? SpriteEffects.None : SpriteEffects.FlipVertically));
                        break;
                    }
                case TurtleState.stand:
                    {
                        if (BeginRespawn)
                        {
                            if (xd == 1)
                            {
                                if (this.rectangle.height == 32)
                                {
                                    this.rectangle = new Rectangle(rectangle.X, rectangle.Y - 32, 32, 64);
                                    up = true;
                                }
                                else
                                {
                                    this.rectangle = new Rectangle(rectangle.X, rectangle.Y + 32, 32, 32);
                                    up = false;
                                    
                                }
                            }
                            if(! up)
                            {
                                spriteBatch.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) + offset, scale: new Vector2(1, 1), sourceRectangle: Animations.animations["turtle_stand"][0], effects: collidable ? SpriteEffects.None : SpriteEffects.FlipVertically);
                            }
                            else
                            {
                                spriteBatch.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) + offset, scale: new Vector2(1, 1), sourceRectangle: Animations.animations["turtle_walk"][k], effects: collidable ? SpriteEffects.None : SpriteEffects.FlipVertically);
                            }
                        }
                        else
                            spriteBatch.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) + offset, scale: new Vector2(1, 1), sourceRectangle: Animations.animations["turtle_stand"][0], effects: collidable ? SpriteEffects.None : SpriteEffects.FlipVertically);
                        break;
                    }

            }

         
        }

        public float timeToRespawn = 0;
        private bool BeginRespawn = false;
        
        public void CheckForRespawn(float TimeElapsed)
        {
            if (prevstate == TurtleState.stand && state == TurtleState.stand && collisionYbot)
            {
                timeToRespawn += TimeElapsed;
                if(timeToRespawn > 2 && timeToRespawn <= 4)
                {
                    BeginRespawn = true;   
                }
                else if (timeToRespawn > 4)
                {
                    state = TurtleState.walk;
                    if(rectangle.height == 32) this.rectangle = new Rectangle(rectangle.X, rectangle.Y - 32, 32, 64);
                    timeToRespawn = 0;
                    BeginRespawn = false;
                }

            }
            else
            {
                timeToRespawn = 0;
                BeginRespawn = false;
            }
                
        }

        SuperMarioBros.Rectangle IDamager.GetRect { get { return rectangle; } }

        bool IDamager.IsDamaging { get { return !collidable?false:state == TurtleState.roll; } }

        Rectangle IDamaged.GetRect { get { return rectangle; } }

        void IDamaged.GetDamaged()
        {
            System.Timers.Timer timer = new System.Timers.Timer(2000);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            this.collidable = false;
            velocityY = -150;
            turtle_is_dying = true;    
        }
        bool turtle_is_dying = false;
        internal bool justspawned = false;

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.enemydead = true;
        }
    }

}
