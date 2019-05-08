using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SuperMarioBros
{
    public class Camera
    {
        public static float PLAYER_BOX_WIDTH = 250;
        public static float PLAYER_BOX_HEIGHT = 150;
        public Player player;
        public Rectangle playerBox;
        public static List<Rectangle> boundaries = new List<Rectangle>();
        public static int currentBounds=0;
        public Vector2 offset;
        
        public Camera(Player player)
        {
            float xoffset = PLAYER_BOX_WIDTH + player.rectangle.width;
            float yoffset = PLAYER_BOX_HEIGHT + player.rectangle.height;
            this.player = player;
            playerBox = new Rectangle(player.rectangle.center().X - PLAYER_BOX_WIDTH / 2, player.rectangle.center().Y - PLAYER_BOX_HEIGHT / 2, PLAYER_BOX_WIDTH, PLAYER_BOX_HEIGHT);
            calculateOffset();
        }

        public static Vector2 MarioOnScreenScaled;
        public void update()
        {
            updatePlayerBox();
            calculateOffset();
            MarioOnScreenScaled = (player.rectangle.center() + offset) / new Vector2(Game1.graphics.GraphicsDevice.Viewport.Width,Game1.graphics.GraphicsDevice.Viewport.Height);
        }

        public void updatePlayerBox()
        {
            if (player.rectangle.center().Y > boundaries[currentBounds].Bot) playerBox.Y = boundaries[currentBounds].Bot - PLAYER_BOX_HEIGHT;
            else if (player.rectangle.center().Y < boundaries[currentBounds].Y) playerBox.Y = boundaries[currentBounds].Y;
            else
            {
                if (player.rectangle.center().Y < playerBox.Y)
                {
                    playerBox.Y = player.rectangle.center().Y;
                }
                if (player.rectangle.center().Y > playerBox.Bot)
                {
                    playerBox.Y = player.rectangle.center().Y - PLAYER_BOX_HEIGHT;
                }
            }

            if (player.rectangle.center().X > boundaries[currentBounds].Right) playerBox.X = boundaries[currentBounds].Right - PLAYER_BOX_WIDTH;
            else if (player.rectangle.center().X < boundaries[currentBounds].X) playerBox.X = boundaries[currentBounds].X;
            else
            {
                if (player.rectangle.center().X < playerBox.X)
                {
                    playerBox.X = player.rectangle.center().X;
                }
                if (player.rectangle.center().X > playerBox.Right)
                {
                    playerBox.X = player.rectangle.center().X - PLAYER_BOX_WIDTH;
                }
            }


        }

        public void calculateOffset()
        {
            offset = new Vector2(-playerBox.center().X + Game1.graphics.GraphicsDevice.Viewport.Width / 2, -playerBox.center().Y + Game1.graphics.GraphicsDevice.Viewport.Height / 2);
        }
    }
}
