using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SuperMarioBros
{
    public class Renderer
    {
        public Scene scene;
        public SpriteBatch spriteBatch;
        public Player mario;
        public GraphicsDeviceManager graphics;
        public Camera camera;
        public CircularList<RenderTarget2D> targets;//teraz tu będziemy rysować
        IEnumerator<RenderTarget2D> iter;
        public bool hypnoactive = false;

        public Renderer(Scene scene, SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {
            this.scene = scene;
            this.spriteBatch = spriteBatch;
            this.graphics = graphics;
            targets = new CircularList<RenderTarget2D>();
            for (int i = 0; i < 2; i++)
            {
                targets.Add(new RenderTarget2D(graphics.GraphicsDevice, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height));
            }
            iter = targets.GetEnumerator();
            iter.MoveNext();
        }

        public SpeedBar speedBar;

        public void SetMario(Player mario)
        {
            this.mario = mario;
            camera = new Camera(mario);
        }

        public void Render(Texture2D background)
        {
            graphics.GraphicsDevice.SetRenderTarget(iter.Current);//ustaw żeby wszystko rysowało się do render targetu, a nie na ekran
            graphics.GraphicsDevice.Clear(Color.Azure);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            graphics.GraphicsDevice.Textures[0] = null;
            camera.update();
            Vector2 offset = camera.offset;
            double procenty = (double)((-offset.Y) / (Camera.boundaries[0].height - Camera.PLAYER_BOX_HEIGHT));
            double procentx = (double)((-offset.X) / (Camera.boundaries[0].width - Camera.PLAYER_BOX_WIDTH));
            Texture2D back = background;
            ///height
            int height_displayed = 240;
            int back_span_y = back.Height - height_displayed;
            int added_height = (int)((Camera.boundaries[0].height - height_displayed) / 4f);
            if (added_height < 0) added_height = 0; else { added_height = Math.Min(added_height, back_span_y); }
            //width
            int width_displayed = 450;
            int back_span_x = back.Width - width_displayed;
            int added_width = (int)((Camera.boundaries[0].width - width_displayed) / 4f);
            if (added_width < 0) added_width = 0; else { added_width = Math.Min(added_width, back_span_x); }
            
            //Vector2 offset = new Vector2(-mario.rectangle.X + graphics.GraphicsDevice.Viewport.Width / 2, -mario.rectangle.Y + graphics.GraphicsDevice.Viewport.Height / 2);
            if(!(MainView.CurrentMode is Level l && l == Game1.level1 && Camera.currentBounds==1)) spriteBatch.Draw(back, destinationRectangle: new Microsoft.Xna.Framework.Rectangle(0, 0, 800, 480), sourceRectangle: new Microsoft.Xna.Framework.Rectangle(back_span_x - added_width + (int)(procentx * (added_width)), back_span_y - added_height + (int)(procenty * (added_height)), width_displayed, height_displayed));

            foreach (var item in scene.tilesToUpdate.Where(t => !t.solid))
            {
                item.Draw(spriteBatch, offset);
            }

            foreach (var item in from b in scene.tilesToUpdate.OfType<Block>()
                                 orderby b.rectangle.Y, b.rectangle.Y
                                 select b
                )
            {
                item.Draw(spriteBatch, offset);
            }

            foreach (var item in scene.mobsToUpdate.Where(m => !(m is Player)))
            {
                item.Draw(spriteBatch, offset);
            }

            foreach (var item in scene.enemiesToUpdate.OfType<Plant>())
            {
                item.Draw(spriteBatch, offset);
            }

            foreach (var item in scene.enemiesToUpdate.Except(scene.enemiesToUpdate.OfType<Plant>()))
            {
                item.Draw(spriteBatch, offset);
            }

            scene.player.Draw(spriteBatch, offset);

            foreach (var item in from tile in scene.tilesToUpdate
                                 where tile.GetType() != typeof(Block) && tile.solid
                                 select tile
                                 )
            {
                item.Draw(spriteBatch, offset);
            }

            string prefix = "";
            if (scene.player.coins < 10) prefix = "0";
            spriteBatch.DrawString(spriteFont: Game1.font, text: $"{prefix + scene.player.coins,2}", position: new Vector2(0, 0), color: Color.Black, rotation: 0f, origin: new Vector2(0, 0), scale: new Vector2(1, 1), effects: SpriteEffects.None, layerDepth: 0);
            spriteBatch.Draw(ContentManager.Textures["coin"], position: new Vector2(36, 8), sourceRectangle: new Microsoft.Xna.Framework.Rectangle(0, 0, 32, 32), scale: new Vector2(0.95f, 0.95f));

            speedBar.UpdateAndDraw(spriteBatch, Math.Abs(scene.player.velocityX), scene.player.activebuff & !scene.player.canflystronger, scene.player.activebuff & scene.player.canflystronger);
            //scene.shader.Draw(spriteBatch);
            spriteBatch.End();
            iter.MoveNext();
            ApplyPostEffects();//narysuj z post processami używając wyrenderowanego obrazu z render targetu
            RenderOnScreen();
        }
        float fade = 1.0f;
        public void ApplyPostEffects()
        {
            //ApplyWater();
            ApplyCircle();
            if (hypnoactive) ApplyHypno();
            if (scene.fuzzy) ApplyBarrel();
            else { strenght = 0; cycles = 0; }
        }
        public void ResetHypnoShader()
        {
            iter_hypno = 0;
            bound_top = 0;
            bound_right = 1;
            bound_bot = 1;
            bound_left = 0;
            current_bound = 1;
            fill = 0;
        }
        int iter_hypno = 0;
        float bound_top = 0;
        float bound_right = 1;
        float bound_bot = 1;
        float bound_left = 0;
        int current_bound = 1;
        float fill = 0;
        
        public void ApplyHypno()
        {
            CalculateHypno();
            if (bound_top > 0.5)
            {
                hypnoactive = false;
            }
            graphics.GraphicsDevice.SetRenderTarget(iter.Current);
            iter.MoveNext();
            spriteBatch.Begin(effect: ContentManager.Shaders["hypno"]);
            ////////////////
            ContentManager.Shaders["hypno"].Parameters["a"].SetValue(bound_top);
            ContentManager.Shaders["hypno"].Parameters["b"].SetValue(bound_right);
            ContentManager.Shaders["hypno"].Parameters["c"].SetValue(bound_bot);
            ContentManager.Shaders["hypno"].Parameters["d"].SetValue(bound_left);
            ContentManager.Shaders["hypno"].Parameters["x"].SetValue(current_bound);
            ContentManager.Shaders["hypno"].Parameters["fill"].SetValue(fill);
            spriteBatch.Draw(iter.Current, iter.Current.Bounds, Color.White);
            spriteBatch.End();
        }

        public void ApplyFade()
        {
            graphics.GraphicsDevice.SetRenderTarget(iter.Current);
            iter.MoveNext();
            spriteBatch.Begin(effect: ContentManager.Shaders["fade"]);
            ContentManager.Shaders["fade"].Parameters["fade"].SetValue(fade);
            fade -= 0.03f;
            if (fade < 0.2f) fade = 0.2f;
            spriteBatch.Draw(iter.Current, iter.Current.Bounds, Color.White);
            spriteBatch.End();
        }

        float strenght = 0;
        float cycles = 0;
        public void ApplyBarrel()
        {
            
            graphics.GraphicsDevice.SetRenderTarget(iter.Current);
            iter.MoveNext();
            spriteBatch.Begin(effect: ContentManager.Shaders["barrel"]);
            ContentManager.Shaders["barrel"].Parameters["power"].SetValue(1.0f + (strenght < 3.14f?1:0.4f)*(float)Math.Sin(strenght));
            spriteBatch.Draw(iter.Current, iter.Current.Bounds, Color.White);
            spriteBatch.End();
            strenght += 0.01f;
            if (strenght > 6.28f) { strenght = 0; cycles++; }
            if (cycles == 1) { cycles = 0; scene.fuzzy = false; }
        }

        public float time = 0;
        public void ApplyWater()
        {
            graphics.GraphicsDevice.SetRenderTarget(iter.Current);
            iter.MoveNext();
            spriteBatch.Begin(effect: ContentManager.Shaders["water"]);
            ContentManager.Shaders["water"].Parameters["waterLevel"].SetValue(CalculateWaterLevel());
            ContentManager.Shaders["water"].Parameters["time"].SetValue(time);
            time += 0.01f;
            spriteBatch.Draw(iter.Current, iter.Current.Bounds, Color.White);
            spriteBatch.End();
        }

        float setx = 0;
        float sety = 0.1f;
        bool right = true;

        public void ApplyLinear()
        {
            graphics.GraphicsDevice.SetRenderTarget(iter.Current);
            iter.MoveNext();
            spriteBatch.Begin(effect: ContentManager.Shaders["linear"]);
            ContentManager.Shaders["linear"].Parameters["setx"].SetValue(setx);
            ContentManager.Shaders["linear"].Parameters["sety"].SetValue(sety);
            ContentManager.Shaders["linear"].Parameters["right"].SetValue(right);
            if (right)
            {
                setx += 0.1f;
                if (setx >= 1) { right = false; sety += 0.1f; }
            }
            else
            {
                setx -= 0.1f;
                if (setx <= 0) { right = true; sety += 0.1f; }
            }
            spriteBatch.Draw(iter.Current, iter.Current.Bounds, Color.White);
            spriteBatch.End();
        }

        public static float circleSize = 0;
        public static Vector2 circlePosition = new Vector2(0.1f, 0.5f);
        public void ApplyCircle()
        {
            graphics.GraphicsDevice.SetRenderTarget(iter.Current);
            iter.MoveNext();
            spriteBatch.Begin(effect: ContentManager.Shaders["circle"]);
            circlePosition = Camera.MarioOnScreenScaled;
            ContentManager.Shaders["circle"].Parameters["circleSize"].SetValue(circleSize);
            ContentManager.Shaders["circle"].Parameters["circlePosition"].SetValue(circlePosition);
            circleSize += 0.015f;
            spriteBatch.Draw(iter.Current, iter.Current.Bounds, Color.White);
            spriteBatch.End();
        }

        public void RenderOnScreen()
        {
            iter.MoveNext();
            graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D)null);//przywróć rysowanie na ekran
            spriteBatch.Begin();
            spriteBatch.Draw(iter.Current, iter.Current.Bounds, Color.White);
            spriteBatch.End();

        }

        public float CalculateWaterLevel()
        {
            float level = scene.waterLevel;
            float ytop = camera.playerBox.center().Y + graphics.GraphicsDevice.Viewport.Height / 2;
            float ybot = camera.playerBox.center().Y - graphics.GraphicsDevice.Viewport.Height / 2;
            float result = (ybot - level) / (ytop - ybot);
            //Console.WriteLine(result);
            return 0.07f;
        }

        float added_black = 0.07f;
        void CalculateHypno()
        {
            switch (current_bound)
            {
                case 1:
                    if (fill < bound_right - 0.1)
                    {
                        fill += added_black;
                    }
                    else
                    {
                        bound_top += 0.1f;
                        fill = bound_top;
                        current_bound = 2;
                    }
                    break;
                case 2:
                    if (fill < bound_bot - 0.1)
                    {
                        fill += added_black;
                    }
                    else
                    {
                        bound_right -= 0.1f;
                        fill = bound_right;
                        current_bound = 3;
                    }
                    break;
                case 3:
                    if (fill > bound_left + 0.1)
                    {
                        fill -= added_black;
                    }
                    else
                    {
                        bound_bot -= 0.1f;
                        fill = bound_bot;
                        current_bound = 4;
                    }
                    break;
                case 4:
                    if (fill > bound_top + 0.1)
                    {
                        fill -= added_black;
                    }
                    else
                    {
                        bound_left += 0.1f;
                        fill = bound_left;
                        current_bound = 1;
                    }
                    break;
            }
        }
    }
}
