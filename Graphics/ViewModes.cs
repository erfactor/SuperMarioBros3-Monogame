using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using static SuperMarioBros.ContentManager;

namespace SuperMarioBros
{
    public enum Mode { level_1, level_2, level_3,level_4, main_screen, textures, songs_and_sfx, map, nothing}

    public interface ViewMode
    {
        void Load();
        void Update(ref float accumulator);
        void Draw();
    }

    public static class MainView
    {
        public static ViewMode CurrentMode;

        public static void SwitchMode(Mode newMode)
        {
            switch (newMode)
            {
                case Mode.level_1:
                    CurrentMode = Game1.level1;
                    break;
                case Mode.level_2:
                    CurrentMode = Game1.level2;
                    break;
                case Mode.level_3:
                    CurrentMode = Game1.level3;
                    break;
                case Mode.level_4:
                    CurrentMode = Game1.level4;
                    break;
                case Mode.textures:
                    CurrentMode = new ShowAvailableTiles(Game1.spriteBatch);
                    break;
                case Mode.songs_and_sfx:
                    CurrentMode = new ShowSongsAndSFX(Game1.spriteBatch);
                    break;
                case Mode.main_screen:
                    CurrentMode = new MainScreen();
                    break;
                case Mode.map:
                    MediaPlayer.Play(ContentManager.Songs["Giants.World"]);
                    CurrentMode = Map.GetMap();
                    break;
                default:
                    return;
            }
            CurrentMode.Load();
        }
        
    }
    public class ShowAvailableTiles : ViewMode
    {

        SpriteBatch spriteBatch;
        Texture2D[,] stage;
        Vector2 offset = new Vector2(0, 0);
        Vector2 center = new Vector2(0, 0);
        Vector2 velocity = new Vector2(0, 0);

        public ShowAvailableTiles(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
        }

        void ViewMode.Draw()
        {
            offset = new Vector2(-center.X + Game1.graphics.GraphicsDevice.Viewport.Width / 2, -center.Y + Game1.graphics.GraphicsDevice.Viewport.Height / 2);
            spriteBatch.Begin();
            
            for (int i = 0; i < stage.GetLength(0); i++)
                for (int j = 0; j < stage.GetLength(1); j++)
                {
                    spriteBatch.Draw(stage[i, j], new Vector2(32 * i, 32 * j) + offset, null);
                }

            for (int i = 0; i < stage.GetLength(0); i++)
            {
                spriteBatch.DrawString(Game1.font, $"{i}", new Vector2(32 * i, -24) + offset, Color.Black, 0, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0);
                spriteBatch.DrawString(Game1.font, $"{i}", new Vector2(32 * i, stage.GetLength(1) * 32) + offset, Color.Black, 0, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0);
            }
            for (int i = 0; i < stage.GetLength(1); i++)
            {
                spriteBatch.DrawString(Game1.font, $"{i,2}", new Vector2(-24, 32 * i) + offset, Color.Black, 0, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0);
                spriteBatch.DrawString(Game1.font, $"{i,2}", new Vector2(stage.GetLength(0) * 32, 32 * i) + offset, Color.Black, 0, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0);
                spriteBatch.DrawString(Game1.font, $"{i,2}", new Vector2(20 * 32, 32 * i) + offset, Color.Red, 0, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0);
                spriteBatch.DrawString(Game1.font, $"{i,2}", new Vector2(40 * 32, 32 * i) + offset, Color.Red, 0, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0);
            }
            spriteBatch.End();
        }

        void ViewMode.Load()
        {
            stage = ContentManager.tilesTextures;
        }

        void ViewMode.Update(ref float accumulator)
        {
            GetInput();
            center += velocity * accumulator;
            accumulator = 0;
            SlowDown(ref velocity);
        }
        void GetInput()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) velocity.X -= 10;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) velocity.X += 10;
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) velocity.Y -= 10;
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) velocity.Y += 10;
        }
        void SlowDown(ref Vector2 vec)
        {
            int slow = 4;
            if (vec.X > 0) vec.X -= slow;
            else if (vec.X < 0) vec.X += slow;
            if (vec.Y > 0) vec.Y -= slow;
            else if (vec.Y < 0) vec.Y += slow;

            if (Math.Abs(vec.X) < slow + 1) vec.X = 0;
            if (Math.Abs(vec.Y) < slow + 1) vec.Y = 0;
        }
    }

    public class ShowSongsAndSFX : ViewMode
    {
        List <string> Songs;
        List <string> SFX;

        SpriteBatch spriteBatch;
        float acc;
        int offset = 1;
        int lastoffset = 0;
        // Vector2 velocity = new Vector2(0, 0);

        public ShowSongsAndSFX(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
        }

        void ViewMode.Load()
        {
            Songs = ContentManager.Songs.Select(x => x.Key).ToList();
            SFX = ContentManager.SoundEffects.Select(x => x.Key).ToList();
        }

        void ViewMode.Update(ref float accumulator)
        {
            if (accumulator > 0.1)
            {
            lastoffset = offset;
            GetInput();
            accumulator = 0;
            }
        }

        public void GetInput()
        {
            MouseState mouse;
            mouse = Mouse.GetState();
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) offset++;
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) offset--;
            if(mouse.LeftButton == ButtonState.Pressed) {
                int ind = (int)mouse.Position.Y / 32;
                if (mouse.Position.X < 350)
                {
                    MediaPlayer.Play(ContentManager.Songs.ElementAt(-offset + ind).Value);
                }
                else
                {
                    MediaPlayer.Pause();
                    ContentManager.SoundEffects.ElementAt(-offset + ind).Value.Play();
                }
                
            }
            
        }
        void ViewMode.Draw()
        {
            offset = Mouse.GetState().ScrollWheelValue / 120;
            LimitOffset();
            spriteBatch.Begin();
            spriteBatch.DrawString(Game1.font, "SONGS" , new Vector2(0, -32 + 32 * offset), Color.SeaGreen, 0, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0);
            spriteBatch.DrawString(Game1.font, "SOUND EFFECTS", new Vector2(350, -32 + 32 * offset), Color.SeaGreen, 0, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0);

            for (int i = 0; i < Songs.Count; i++)
            {
                spriteBatch.DrawString(Game1.font, Songs[i], new Vector2(0, 32 * i + 32 * offset )  , Color.Black, 0, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0);
            }
            for (int i = 0; i < SFX.Count; i++)
            {
                spriteBatch.DrawString(Game1.font, $"{SFX[i],-40}", new Vector2(350, 32 * i + 32 * offset), Color.Black, 0, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0);
            }
            spriteBatch.End();
        }

       void LimitOffset()
        {
            if (offset > 1) offset = 1;
            if (offset < -(Math.Max(Songs.Count, SFX.Count) - spriteBatch.GraphicsDevice.Viewport.Height / 32)) offset = -(Math.Max(Songs.Count, SFX.Count) - spriteBatch.GraphicsDevice.Viewport.Height / 32);
        }
    }

}
