using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperMarioBros
{
    public class Map : ViewMode
    {
        List<Vertex> Vertices = new List<Vertex>();
        Texture2D texture;
        public Vertex CurrentVertex;
        public Vertex LastVertex;
        bool moving = false;
        Vector2 playerPosition;
        Vector2 last_playerPosition;
        Vector2 velocity;
        // Shadery
        public static GraphicsDeviceManager graphics;
        public CircularList<RenderTarget2D> targets;
        IEnumerator<RenderTarget2D> iter;

        public Map()
        {
            last_playerPosition = new Vector2(42, 74);
            texture = ContentManager.Textures["map1"];
            Vertex v1 = new Vertex(new Vector2(42, 74));
            Vertex v2 = new Vertex(new Vector2(106, 74));
            Vertex v3 = new Vertex(new Vector2(106, 10), Mode.level_1);
            Vertex v4 = new Vertex(new Vector2(170, 10));
            Vertex v5 = new Vertex(new Vector2(233, 10), Mode.level_2);
            Vertex v6 = new Vertex(new Vector2(297, 10), Mode.level_3);
            Vertex v7 = new Vertex(new Vector2(361, 10));
            Vertex v8 = new Vertex(new Vector2(361, 74), Mode.textures);
            Vertex v9 = new Vertex(new Vector2(297, 74), Mode.level_4);
            Vertex v10 = new Vertex(new Vector2(233, 74));
            Vertex v11 = new Vertex(new Vector2(233, 138), Mode.songs_and_sfx);
            Vertex v12 = new Vertex(new Vector2(170, 138), Mode.main_screen);
            Vertex v13 = new Vertex(new Vector2(106, 138));
            AddVertex(v1); AddVertex(v2); AddVertex(v3); AddVertex(v4); AddVertex(v5);
            AddVertex(v6); AddVertex(v7); AddVertex(v8); AddVertex(v9); AddVertex(v10);
            AddVertex(v11); AddVertex(v12); AddVertex(v13);
            AddLink(v1, v2); AddLink(v2, v3); AddLink(v3, v4); AddLink(v4, v5);
            AddLink(v5, v6); AddLink(v6, v7); AddLink(v7, v8); AddLink(v8, v9);
            AddLink(v9, v10); AddLink(v10, v5); AddLink(v10, v11); AddLink(v11, v12);
            AddLink(v12, v13); AddLink(v13, v2);

            targets = new CircularList<RenderTarget2D>();
            for (int i = 0; i < 2; i++)
            {
                targets.Add(new RenderTarget2D(graphics.GraphicsDevice, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height));
            }
            iter = targets.GetEnumerator();
            iter.MoveNext();
        }

        private static Map UniqueMap = null;
        public static Map GetMap()
        {
            if (UniqueMap == null)
            {
                UniqueMap = new Map();
            }
            return UniqueMap;
        }

        public void AddVertex(Vertex vertex)
        {
            Vertices.Add(vertex);
        }
        public void AddLink(Vertex v1, Vertex v2)
        {
            if (v1.position.X == v2.position.X)
            {
                if (v1.position.Y < v2.position.Y)
                {
                    v1.Bot = v2;
                    v2.Top = v1;
                }
                else
                {
                    v1.Top = v2;
                    v2.Bot = v1;
                }
            }
            else if (v1.position.Y == v2.position.Y)
            {
                if (v1.position.X < v2.position.X)
                {
                    v1.Right = v2;
                    v2.Left = v1;
                }
                else
                {
                    v1.Left = v2;
                    v2.Right = v1;
                }
            }
        }

        void ViewMode.Load()
        {
            Console.WriteLine("xd");
            Console.WriteLine(playerPosition);
            Console.WriteLine(last_playerPosition);
            playerPosition = last_playerPosition;
            CurrentVertex = Vertices.Single(v => v.position == playerPosition);
        }


        void ViewMode.Update(ref float accumulator)
        {
            GetInput();
            if (moving)
            {
                velocity = CurrentVertex.position - LastVertex.position;
                if ((playerPosition - CurrentVertex.position).Length() <= 3f)
                {
                    playerPosition = CurrentVertex.position;
                    moving = false;
                }
                else
                {
                    playerPosition += velocity * accumulator;
                }
            }
            accumulator = 0;

            if (linear_finished)
            {
                linear_finished = false;
                linearactive = false;
                MainView.SwitchMode(CurrentVertex.action);
                Renderer.circleSize = 0;
            }
        }

        public void GetInput()
        {
            if (linearactive) return;
            if (!moving)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    if (CurrentVertex.action == Mode.nothing) return;
                    last_playerPosition = playerPosition;

                    if (!linearactive)
                    {
                        ResetLinear();
                        linearactive = true;
                    }
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Left) && CurrentVertex.Left != null)
                {
                    moving = true;
                    LastVertex = CurrentVertex;
                    CurrentVertex = CurrentVertex.Left;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Right) && CurrentVertex.Right != null)
                {
                    moving = true;
                    LastVertex = CurrentVertex;
                    CurrentVertex = CurrentVertex.Right;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Up) && CurrentVertex.Top != null)
                {
                    moving = true;
                    LastVertex = CurrentVertex;
                    CurrentVertex = CurrentVertex.Top;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Down) && CurrentVertex.Bot != null)
                {
                    moving = true;
                    LastVertex = CurrentVertex;
                    CurrentVertex = CurrentVertex.Bot;
                }
            }
        }

        SpriteBatch sprite = Game1.spriteBatch;
        float leftfoot;
        Vector2 scl = new Vector2(1.5f, 1.5f);


        void ViewMode.Draw()
        {
            graphics.GraphicsDevice.SetRenderTarget(iter.Current);
            graphics.GraphicsDevice.Clear(Color.Azure);
            sprite.Begin();
            sprite.Draw(texture, new Vector2(), scale: scl);
            if (leftfoot++ % 20 < 10)
                sprite.Draw(texture: ContentManager.Textures["mapmario"], position: playerPosition * scl, scale: scl);
            else sprite.Draw(texture: ContentManager.Textures["mapmario"], position: playerPosition * scl, scale: scl, effects: SpriteEffects.FlipHorizontally);

            sprite.End();
            iter.MoveNext();
            ApplyPostEffects();
            RenderOnScreen();
        }

        public void ApplyPostEffects()
        {
            if (linearactive) ApplyLinear();
            ApplyFade();
        }
        public static float fade = 1.0f;
        public static void ResetFade()
        {
            fade = 1f;
        }
        public void ApplyFade()
        {
            graphics.GraphicsDevice.SetRenderTarget(iter.Current);
            iter.MoveNext();
            sprite.Begin(effect: ContentManager.Shaders["fade"]);
            ContentManager.Shaders["fade"].Parameters["fade"].SetValue(fade);
            fade = Math.Max(fade, 0.1f);
            fade -= 0.03f;
            sprite.Draw(iter.Current, iter.Current.Bounds, Color.White);
            sprite.End();
        }
        float setx = 1;
        float sety = 0.9f;
        bool right = true;
        bool linearactive = false;
        bool linear_finished = false;
        void ResetLinear()
        {
            setx = 1;
            sety = 0.9f;
            right = true;
        }

        public void ApplyLinear()
        {
            if (sety < 0) linear_finished = true;
            graphics.GraphicsDevice.SetRenderTarget(iter.Current);
            iter.MoveNext();
            sprite.Begin(effect: ContentManager.Shaders["linear"]);
            ContentManager.Shaders["linear"].Parameters["setx"].SetValue(setx);
            ContentManager.Shaders["linear"].Parameters["sety"].SetValue(sety);
            ContentManager.Shaders["linear"].Parameters["right"].SetValue(!right);
            if (right)
            {
                setx += 0.1f;
                if (setx >= 1) { right = false; sety -= 0.1f; }
            }
            else
            {
                setx -= 0.1f;
                if (setx <= 0) { right = true; sety -= 0.1f; }
            }
            sprite.Draw(iter.Current, iter.Current.Bounds, Color.White);
            sprite.End();
        }

        public void RenderOnScreen()
        {
            iter.MoveNext();
            graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D)null);//przywróć rysowanie na ekran
            sprite.Begin();
            sprite.Draw(iter.Current, iter.Current.Bounds, Color.White);
            sprite.End();

        }
    }
}
