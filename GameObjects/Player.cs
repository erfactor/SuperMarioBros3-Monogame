using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static SuperMarioBros.Animations;

namespace SuperMarioBros
{
    public enum MarioState { small, big, raccoon , crouching, fire}

    public class Player : Mob
    {
        
        int health;
        public int coins = 0;
        public MarioState prevstate;
        public Rectangle TextureRect;
        public PlayerState playerState;
        private SmallMarioState SmallMS;
        private BigMarioState BigMS;
        private RaccoonMarioState RaccoonMS;
        private FireMarioState FireMS;
        public bool HasAttachedTurtle = false;

        public Player(Rectangle rectangle, Texture2D texture) : base(rectangle, texture)
        {
            TextureRect = rectangle.Clone();
            this.rectangle.X += 4;
            this.rectangle.Y += 8;
            this.rectangle.width -= 8;
            this.rectangle.height -= 8;

            SmallMS = new SmallMarioState(this);
            BigMS = new BigMarioState(this);
            RaccoonMS = new RaccoonMarioState(this);
            FireMS = new FireMarioState(this);
            playerState = SmallMS;
            
        }
        bool canshoot = true;
        private void ShootTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            canshoot = true;
            shootTimer.Dispose();
        }

        public void ChangePlace(Vector2 newpos)
        {
            this.TextureRect.X = newpos.X; 
            if (playerState is SmallMarioState) { this.TextureRect.X += 16; }
            if(teleportType == TeleportType.Bot_Bot || teleportType == TeleportType.Top_Bot)
            {
                this.TextureRect.Bot = newpos.Y;
            }
            else
            {
                this.TextureRect.Y = newpos.Y;
            }
            
            this.rectangle = TextureRect.Clone();
            if (playerState is SmallMarioState) { rectangle.X += 4; rectangle.Y += 8; rectangle.width -= 8; rectangle.height -= 8; }
            else { rectangle.X += 20; rectangle.Y += 8; rectangle.width -= 40; rectangle.height -= 8; }
        }

        public void SetState(MarioState marioState)
        {
            switch (marioState)
            {
                case MarioState.small:
                    playerState = SmallMS;
                    break;
                case MarioState.big:
                    playerState = BigMS;
                    break;
                case MarioState.raccoon:
                    playerState = RaccoonMS;
                    break;
                case MarioState.fire:
                    playerState = FireMS;
                    break;
            }
        }

        public void BiggerCollisionBox()
        {
            rectangle.Y -= 32;
            rectangle.height += 32;
        }
        public void SmallerCollisionBox()
        {
            rectangle.Y += 32;
            rectangle.height -= 32;
        }
        public void BiggerTexture()
        {
            TextureRect = new Rectangle(TextureRect.X - 16, TextureRect.Y - 32, 64, 64);
        }
        public void SmallerTexture()
        {
            TextureRect = new Rectangle(TextureRect.X + 16, TextureRect.Y + 32, 32, 32);
        }
        
        public override void Update()
        {
            if(collidable && TextureRect.Bot != rectangle.Bot)
            {
                TextureRect.Bot = rectangle.Bot;
            }
            HittingMob = null;
            velocityY += 20f;
            if (velocityX >= 0) { if (velocityX > 10) velocityX -= 10; else velocityX = 0; }
            else if (velocityX <= 0) { if (velocityX < -10) velocityX += 10; else velocityX = 0; }

            if (this.rectangle.Y > Camera.boundaries[0].height + 1000) this.alive = false;
        }
        
        public void JumpedOnSth(Enemy enemy)
        {
            switch (enemy)
            {
                case Turtle t:
                    if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    {
                        velocityY = -500;
                    }
                    else goto default;
                    break;
                default:
                    velocityY = -200;
                    break;
            }

        }

        private GameObject HittingMob;
        public void GetHit(GameObject mob)
        {
            HittingMob = mob;
        }

        private bool immune;
        private float immunetime;
        public void MakeImmune()
        {
            immune = true;
            immunetime = 1;
        }

        private void UpdateImmune(float deltaTime)
        {
            if (immune)
            {
                immunetime -= deltaTime;
                if (immunetime < 0)
                    immune = false;
            }
        }

        private void CheckHit()
        {
            if (!(HittingMob is null) && !immune)
            {
                switch (HittingMob)
                {
                    case Shroom shroom:
                        playerState.GetHitByShroom(shroom);
                        break;
                    case Feather feather:
                        playerState.GetHitByFeather(feather);
                        break;
                    case Flower flower:
                        playerState.GetHitByFlower(flower);
                        break;
                    case Bullet bullet:
                        bullet.enemydead = true;
                        goto et;
                    case Enemy enemy:
                    et:
                        playerState.GetDamaged();
                        break;
                }
            }
        }

        public override void Move(float TimeElapsed)
        {
            CheckHit();
            UpdateImmune(TimeElapsed);

            if (teleportType != TeleportType.None)
            {
                CheckForPipes(TimeElapsed);
                return;
            }

            if (!collisionX)
            {
                rectangle.X += velocityX * TimeElapsed;
                TextureRect.X += velocityX * TimeElapsed;
            }
            else velocityX = 0;

            if (!collisionYbot && !collisionYtop)
            {
                rectangle.Y += velocityY * TimeElapsed;
                TextureRect.Y += velocityY * TimeElapsed;
            }
            else velocityY = 0;

            if (collisionX && collisionYbot && justgotbigger)
            {
                Rectangle futureRectY = new Rectangle(rectangle.X, rectangle.Y+32, rectangle.width, rectangle.height);
                foreach (var t in scene.tiles.Where(t=> t.solid))
                {
                    if (CollisionDetector.isIntersect(futureRectY, t.rectangle))
                    {
                        if (onleft)
                        {
                            rectangle.X += 10;
                            TextureRect.X += 10;
                        }
                        else
                        {
                            rectangle.X -= 10;
                            TextureRect.X -= 10;
                        }
                        return;
                    }
                }
                rectangle.Y += 32;
                TextureRect.Y += 32;

            }
        }

        public bool onleft;
        System.Timers.Timer shootTimer;
        private ButtonState D_prevstate;
        private ButtonState D_state;
        private bool oriented_right = true;
        SimpleDamage damage = null;
        bool tailwhip = false;
        bool groundwhip = true;
        public bool canflystronger=false;
        public bool activebuff=false;
        bool crouching = false;
        public bool onpipe = false;
        public void input()
        {
            if (playerState is RaccoonMarioState)
            {
                if (Math.Abs(velocityX) > 300 && !activebuff)
                {
                    ContentManager.SoundEffects["Running"].Play();
                    System.Timers.Timer timer = new System.Timers.Timer(4500);
                    timer.Elapsed += Timer_Elapsed2;
                    timer.AutoReset = false;
                    timer.Start();
                    canflystronger = true;
                    activebuff = true;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) oriented_right = true;
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) oriented_right = false;

            if (!crouching)
            { 
                if (Keyboard.GetState().IsKeyDown(Keys.Left) && !collisionX) {
                    if (velocityX > -300 && velocityX < -80) velocityX -= 13;
                    else if (velocityX > -300) velocityX = -100;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Right) && !collisionX) {
                    if (velocityX < 300 && velocityX > 80) velocityX += 13;
                    else if (velocityX < 300) velocityX = 100;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up) && Math.Abs(velocityY) < 0.01f && collisionYbot) { velocityY = -600; ContentManager.SoundEffects["Jumping"].Play(0.4f, 0, 0);  }
            //if (Keyboard.GetState().IsKeyDown(Keys.X)) { velocityY = -500; ContentManager.SoundEffects["Jumping"].Play(0.4f,0,0); }
            
            if (Keyboard.GetState().IsKeyDown(Keys.F) && canshoot && playerState is FireMarioState)
            {
                var fireball = new FireBall(new Rectangle(this.TextureRect.center().X, this.TextureRect.center().Y, 16, 16), ContentManager.Textures["fireball"], oriented_right);
                fireball.velocityY = this.velocityY*1.3f;
                fireball.velocityX = Math.Max(300, 200 + this.velocityX);
                if (!oriented_right) fireball.velocityX *= -1;
                scene.QueryAddGameObject(fireball);
                shootTimer = new System.Timers.Timer(1000);
                shootTimer.Start();
                canshoot = false;
                shootTimer.Elapsed += ShootTimer_Elapsed;
                ContentManager.SoundEffects["Flame Ball"].Play(0.4f, 0, 0);
            }
                // Special Interactions
            D_prevstate = D_state;
            D_state = Keyboard.GetState().IsKeyDown(Keys.D) ? ButtonState.Pressed : ButtonState.Released;

            if (D_prevstate == ButtonState.Released && D_state == ButtonState.Pressed  && playerState is RaccoonMarioState && !tailwhip)
            {
                if (canflystronger)
                {
                    velocityY = -270;
                }
                ContentManager.SoundEffects["Raccoon Tail Whip"].Play(0.3f, 0, 0);
                if (collisionYbot)
                {
                    if (oriented_right)
                        damage = new SimpleDamage(new Rectangle(this.rectangle.Right, this.rectangle.Bot - 32, 20, 32), null);
                    else
                        damage = new SimpleDamage(new Rectangle(this.rectangle.X -32, this.rectangle.Bot - 32, 20, 32), null);
                    scene.QueryAddGameObject(damage);
                    System.Timers.Timer timer = new System.Timers.Timer(100);
                    timer.Elapsed += Timer_Elapsed1;
                    timer.AutoReset = false;
                    timer.Start();
                    groundwhip = true;
                }
                else
                {
                    damage = new SimpleDamage(new Rectangle(this.rectangle.Right, this.rectangle.Bot - 32, 1, 1), null);
                    scene.QueryAddGameObject(damage);
                    System.Timers.Timer timer = new System.Timers.Timer(100);
                    timer.Elapsed += Timer_Elapsed1;
                    timer.AutoReset = false;
                    timer.Start();

                    groundwhip = false;
                    
                    if (velocityY > 0 && !canflystronger) velocityY -= 150;
                    
                }
                tailwhip = true;

            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down) && !(playerState is SmallMarioState) && !crouching && teleportType == TeleportType.None && !onpipe)
            {
                crouching = true;
                SmallerCollisionBox();
                //SmallerTexture();
            }
            if (!Keyboard.GetState().IsKeyDown(Keys.Down) && crouching )
            {
                crouching = false;
                BiggerCollisionBox();
                GotBigger();
                //BiggerTexture();
            }
            if (!Keyboard.GetState().IsKeyDown(Keys.C) && HasAttachedTurtle)
            {
                HasAttachedTurtle = false;
                Turtle turtle = null;
                if (oriented_right)
                {
                turtle = new Turtle(new Rectangle(rectangle.Right + 10, rectangle.Bot - 36, 32, 32), null);
                }
                else
                {
                turtle = new Turtle(new Rectangle(rectangle.X - 42, rectangle.Bot - 36, 32, 32), null);
                }
                turtle.state = TurtleState.roll;
                turtle.velocityY = this.velocityY;
                turtle.velocityX = Math.Max(this.velocityX+300,300);
                if (!oriented_right) turtle.velocityX *= -1;
                turtle.justspawned = true;
                turtle.timer.Start();
                scene.QueryAddGameObject(turtle);
                ContentManager.SoundEffects["Flame Ball"].Play();
            }
        }

        private void Timer_Elapsed2(object sender, System.Timers.ElapsedEventArgs e)
        {
            canflystronger = false;
            System.Timers.Timer timer = new System.Timers.Timer(6000);
            timer.Elapsed += Timer_Elapsed3;
            timer.AutoReset = false;
            timer.Start();
        }

        private void Timer_Elapsed3(object sender, System.Timers.ElapsedEventArgs e)
        {
            activebuff = false;
        }

        private void Timer_Elapsed1(object sender, System.Timers.ElapsedEventArgs e)
        {
            damage.enemydead = true;
            tailwhip = false;
        }

        int i = 0; int j = 0; int k = 0; int kk;
        public Random rnd = new Random();

        public override void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            i++;
            if (i > 3)
            {
                i = 0;
                j++; if (j == 2) j = 0;
                k++; if (k == 4) k = 0; kk = k; if (k == 3) kk = 1;
            }

            if (immune)
            {
                if (immunetime * 100 % 20 < 10) return;
            }
            
            switch (playerState)
            {
                case SmallMarioState sms:
                    if (Math.Abs(velocityY) <= 10 && Math.Abs(velocityX) <= 10)
                    {
                        if(oriented_right)
                             spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["small_mario"][0], effects: SpriteEffects.FlipHorizontally);
                        else
                             spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["small_mario"][0]);
                    }
                    else if (Math.Abs(velocityY) >= 10 || !collisionYbot)
                    {
                        if (velocityX >= 0)
                            spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["small_mario"][4], effects: SpriteEffects.FlipHorizontally);
                        else
                            spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["small_mario"][4]);
                    }
                    else if (velocityX >= 0) spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, effects: SpriteEffects.FlipHorizontally, sourceRectangle: animations["small_mario"][j]);
                    else spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["small_mario"][j]);
                    break;
                case BigMarioState bms:
                    if (crouching)
                    {
                        if (oriented_right) spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["big mario"][12], effects: SpriteEffects.FlipHorizontally);
                        else spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["big mario"][12]);
                        break;
                    }
                    if (Math.Abs(velocityY) <= 10 && Math.Abs(velocityX) <= 10)
                    { if (oriented_right) spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["big mario"][0], effects: SpriteEffects.FlipHorizontally);
                        else              spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["big mario"][0]);
                    }
                    else if (Math.Abs(velocityY) >= 10 || !collisionYbot)
                    {
                        if (velocityX >= 0)
                            spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["big mario"][6], effects: SpriteEffects.FlipHorizontally);
                        else
                            spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["big mario"][6]);
                    }
                    else if (velocityX >= 0) spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["big mario"][kk], effects: SpriteEffects.FlipHorizontally);
                    else spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["big mario"][kk]);
                    break;
                case RaccoonMarioState rms:
                    if (crouching)
                    {
                        if (oriented_right) spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["raccoon"][14], effects: SpriteEffects.FlipHorizontally);
                        else spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["raccoon"][14]);
                        break;
                    }
                    if (tailwhip && groundwhip)
                    {
                        if (oriented_right) spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["raccoon"][0]);
                        else spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["raccoon"][0], effects: SpriteEffects.FlipHorizontally);
                        break;
                    }
                    if (Math.Abs(velocityY) <= 10 && Math.Abs(velocityX) <= 10 && collisionYbot)
                    {
                        if (oriented_right) spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["raccoon"][0], effects: SpriteEffects.FlipHorizontally);
                        else spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["raccoon"][0]);
                    }
                    else if (Math.Abs(velocityY) >= 10 || !collisionYbot)
                    {
                        if(velocityY <= 0)
                        {
                            if (!oriented_right)spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["raccoon"][6]);
                            else spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["raccoon"][6], effects: SpriteEffects.FlipHorizontally);
                        }
                        else
                        {
                            if (tailwhip)
                            {
                                if (!oriented_right) spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["raccoonfly"][3]);
                                else spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["raccoonfly"][3], effects: SpriteEffects.FlipHorizontally);
                            }
                            else
                            {
                                if (!oriented_right) spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["raccoonfly"][4]);
                                else spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["raccoonfly"][4], effects: SpriteEffects.FlipHorizontally);
                            }
                        }
                    }
                    else
                    {
                        if (!oriented_right) spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["raccoon"][kk]);
                        else spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["raccoon"][kk], effects: SpriteEffects.FlipHorizontally);
                    }
                    break;
                case FireMarioState fms:
                    if (crouching)
                    {
                        if (oriented_right) spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["fire mario"][12], effects: SpriteEffects.FlipHorizontally);
                        else spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["fire mario"][12]);
                        break;
                    }
                    if (Math.Abs(velocityY) <= 10 && Math.Abs(velocityX) <= 10)
                    {
                        if (oriented_right) spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["fire mario"][0], effects: SpriteEffects.FlipHorizontally);
                        else                spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["fire mario"][0]);
                    }
                    else if (Math.Abs(velocityY) >= 10 || !collisionYbot)
                    {
                        if (velocityX >= 0)
                            spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["fire mario"][6], effects: SpriteEffects.FlipHorizontally);
                        else
                            spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["fire mario"][6]);
                    }
                    else if (velocityX >= 0) spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["fire mario"][kk], effects: SpriteEffects.FlipHorizontally);
                    else spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["fire mario"][kk]);
                    break;
                    //case MarioState.crouching:
                    //  spriteBatch.Draw(texture, position: new Vector2(TextureRect.X, TextureRect.Y) + offset, sourceRectangle: animations["raccoon"][14]);
                    //  break;
            }

            if (HasAttachedTurtle)
            {
                if(oriented_right)
                spriteBatch.Draw(ContentManager.Textures["turtle" + 1], position: new Vector2(rectangle.Right - 12, rectangle.Bot - 28) + offset, scale: new Vector2(1, 1), sourceRectangle: Animations.animations["turtle_roll"][0]);
                else
                    spriteBatch.Draw(ContentManager.Textures["turtle" + 1], position: new Vector2(rectangle.X - 24, rectangle.Bot - 28) + offset, scale: new Vector2(1, 1), sourceRectangle: Animations.animations["turtle_roll"][0]);
            }

        }

        public bool justgotbigger = false;
        public void GotBigger()
        {
            System.Timers.Timer timer = new System.Timers.Timer(200);
            timer.AutoReset = false;
            timer.Elapsed += Timer_Elapsed;
            justgotbigger = true;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            justgotbigger = false;
        }

        Timer Timer_First = null;
        internal void Start_First_Pipe_Movement()
        {
            ContentManager.SoundEffects["Pipe Maze"].Play();
            Timer_First = new Timer(1);
        }

        Timer Timer_Second = null;
        internal void Start_Second_Pipe_Movement()
        {
            ContentManager.SoundEffects["Pipe Maze"].Play();
            Timer_Second = new Timer(1);
        }

        Vector2 destination;
        TeleportType teleportType = TeleportType.None;
        internal bool finished = false;

        internal void StartPipeMovement(Vector2 destination, TeleportType tptype)
        {
            this.collidable = false;
            this.destination = destination;
            this.teleportType = tptype;
            Start_First_Pipe_Movement();
        }

        internal void BotBotPipeMove(Vector2 destination)
        {
            this.collidable = false;
            this.destination = destination;
            Start_First_Pipe_Movement();
            teleportType = TeleportType.Bot_Bot;
        }

        private void CheckForPipes(float TimeElapsed)
        {
            switch (teleportType)
            {
                case TeleportType.Top_Top:
                    if (Timer_First != null)
                    {
                        if (Timer_First.AddTimeCheckIfEnded(TimeElapsed)) MarioMidPipe();
                        else MarioGoDownPipeFirst(TimeElapsed);
                    }
                    if (Timer_Second != null)
                    {
                        if (Timer_Second.AddTimeCheckIfEnded(TimeElapsed)) MarioEndPipe();
                        else MarioGoUpPipeSecond(TimeElapsed);
                    }
                    break;
                case TeleportType.Top_Bot:
                    if (Timer_First != null)
                    {
                        if (Timer_First.AddTimeCheckIfEnded(TimeElapsed)) MarioMidPipe();
                        else MarioGoDownPipeFirst(TimeElapsed);
                    }
                    if (Timer_Second != null)
                    {
                        if (Timer_Second.AddTimeCheckIfEnded(TimeElapsed)) MarioEndPipe();
                        else MarioGoDownPipeSecond(TimeElapsed);
                    }
                    break;
                case TeleportType.Bot_Bot:
                    if (Timer_First != null)
                    {
                        if (Timer_First.AddTimeCheckIfEnded(TimeElapsed)) MarioMidPipe();
                        else MarioGoUpPipeFirst(TimeElapsed);
                    }
                    if (Timer_Second != null)
                    {
                        if (Timer_Second.AddTimeCheckIfEnded(TimeElapsed)) MarioEndPipe();
                        else MarioGoDownPipeSecond(TimeElapsed);
                    }
                    break;
                case TeleportType.Bot_Top:
                    if (Timer_First != null)
                    {
                        if (Timer_First.AddTimeCheckIfEnded(TimeElapsed)) MarioMidPipe();
                        else MarioGoUpPipeFirst(TimeElapsed);
                    }
                    if (Timer_Second != null)
                    {
                        if (Timer_Second.AddTimeCheckIfEnded(TimeElapsed)) MarioEndPipe();
                        else MarioGoUpPipeSecond(TimeElapsed);
                    }
                    break;
                case TeleportType.None:
                    break;
            }

        }

        private void MarioGoUpPipeFirst(float TimeElapsed)
        {
            float upY = (TextureRect.height * TimeElapsed / Timer_First.MaxTime);
            // upY += 0.1f;
            rectangle.Y -= upY;
            TextureRect.Y -= upY;
        }
        private void MarioGoUpPipeSecond(float TimeElapsed)
        {
            float upY = (TextureRect.height * TimeElapsed / Timer_Second.MaxTime);
            //upY += 0.1f;
            rectangle.Y -= upY;
            TextureRect.Y -= upY;
        }

        private void MarioGoDownPipeFirst(float TimeElapsed)
        {
            float downY = TextureRect.height * TimeElapsed / Timer_First.MaxTime;
            rectangle.Y += downY;
            TextureRect.Y += downY;
        }
        private void MarioGoDownPipeSecond(float TimeElapsed)
        {
            float downY = TextureRect.height * TimeElapsed / Timer_Second.MaxTime;
            rectangle.Y += downY;
            TextureRect.Y += downY;
        }

        private void MarioMidPipe()
        {
            Timer_First = null;
            ChangePlace(destination);
            Renderer.circleSize = 0;
            Camera.currentBounds = Camera.currentBounds == 0 ? 1 : 0;
            Start_Second_Pipe_Movement();
        }

        private void MarioEndPipe()
        {
            Timer_Second = null;
            this.collidable = true;
            velocityY = 0;
            teleportType = TeleportType.None;
        }
    }
}
