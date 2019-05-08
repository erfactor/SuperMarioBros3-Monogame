using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperMarioBros
{
    public class MainScreen : ViewMode
    {
        List<ScreenTile> ScreenTiles = new List<ScreenTile>();
        SpriteBatch sprite;
        void ViewMode.Load()
        {
            this.sprite = Game1.spriteBatch;
            ScreenTiles.Add(new ScreenTile(ContentManager.Textures["mapTile"], new Rectangle(0, 0, 480, 480), Mode.map));

            ScreenTiles.Add(new ScreenTile(ContentManager.tilesTextures[66, 3], new Rectangle(550, 20, 200, 200), Mode.textures));
            ScreenTiles.Add(new ScreenTile(ContentManager.tilesTextures[61, 7], new Rectangle(550, 260, 200, 200), Mode.songs_and_sfx));
        }

        void ViewMode.Update(ref float accumulator)
        {

            MouseState mouse = Mouse.GetState();
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                Console.WriteLine(mouse.Position);
                CheckScreenTiles(mouse.Position);
            }
        }
        void CheckScreenTiles(Point mouse)
        {
            foreach(ScreenTile screenTile in ScreenTiles)
            {
                if(screenTile.Rect.IsPointInside(mouse))
                {
                    MainView.SwitchMode(screenTile.mode);
                }
            }
        }

        void ViewMode.Draw()
        {
            sprite.Begin();
            Game1.graphics.GraphicsDevice.Clear(Color.Aquamarine);
            foreach (ScreenTile screenTile in ScreenTiles)
            {
                screenTile.Draw(sprite);
            }

            sprite.End();
        }
    }

    public class ScreenTile
    {
        public Texture2D texture;
        Rectangle rectangle;
        public Mode mode;
        string str;
        
        public Rectangle Rect { get { return rectangle; } }

        public ScreenTile(Texture2D texture, Rectangle rectangle, Mode mode)
        {
            this.texture = texture;
            this.rectangle = rectangle;
            this.mode = mode;
        }

        public void Draw(SpriteBatch sprite)
        {
            sprite.Draw(texture, position: new Vector2(rectangle.X, rectangle.Y) , scale: new Vector2(rectangle.width / texture.Width, rectangle.height / texture.Height));
        }
    }
}
