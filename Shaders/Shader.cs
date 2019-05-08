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
    public abstract class Shader
    {
        public static int WIDTH_OF_SHADER = 800;
        public static int HEIGHT_OF_SHADER = 480;
        public abstract void Update(float update);
        public abstract void Draw(SpriteBatch sprite);
    }

    public class VerticalShader : Shader
    {
        Color[] colorTab = new Color[WIDTH_OF_SHADER * HEIGHT_OF_SHADER];
        Texture2D container;
        Vector2 PlayerLocation;

        public void GenerateShader(Vector2 playerLocation)
        {
            container = new Texture2D(Game1.graphics.GraphicsDevice, WIDTH_OF_SHADER, HEIGHT_OF_SHADER, false, SurfaceFormat.Color);
            for (int i = 0; i < WIDTH_OF_SHADER * HEIGHT_OF_SHADER; i++)
                {
                    colorTab[i] = Color.Black;
                }
            container.SetData(colorTab);
            radius = 0;
            this.PlayerLocation = playerLocation;
            //Console.WriteLine(playerLocation);
        }

        private float radius;

        public VerticalShader()
        {
            
        }

        public override void Update(float update)
        {
            double i, j;
            double distance, distx, disty;
            radius += 15f;
            for (int k = 0; k < WIDTH_OF_SHADER * HEIGHT_OF_SHADER; k++)
            {
                i = k % WIDTH_OF_SHADER;
                j = k / WIDTH_OF_SHADER;
                distx = PlayerLocation.X - i;
                disty = PlayerLocation.Y - j;
                distance = Math.Sqrt(distx*distx + disty * disty);
                if (distance < radius) colorTab[k] = Color.Transparent;

            }
        }

        public override void Draw(SpriteBatch sprite)
        {
            container.SetData(colorTab);
            sprite.Draw(container, Vector2.Zero);
        }
    }


    public class LinearShader : Shader
    {
        
        List<Texture2D> Containers = new List<Texture2D>();
        int numOfContainers;

        public LinearShader(int numOfContainers)
        {
            this.numOfContainers = numOfContainers;
        }

        public static Random rnd = new Random(DateTime.Now.Millisecond);

        public void GenerateShader()
        {
            List<Point>[] PixelsTabList = new List<Point>[numOfContainers];
            for (int i = 0; i < PixelsTabList.Length; i++) PixelsTabList[i] = new List<Point>();

            int r = 0;
            int k = 0;
            float check = WIDTH_OF_SHADER * HEIGHT_OF_SHADER / (float)numOfContainers + 1;
            for (int i = 0; i < WIDTH_OF_SHADER; i++)
                for (int j = 0; j < HEIGHT_OF_SHADER; j++)
                {
                   // if (r++ > check) { r = 0; k++; }
                    PixelsTabList[rnd.Next()%numOfContainers ].Add(new Point(i, j));
                }
            
            for (int i = 0; i < PixelsTabList.Length; i++)
            {
                Texture2D container = new Texture2D(Game1.graphics.GraphicsDevice, WIDTH_OF_SHADER, HEIGHT_OF_SHADER, false, SurfaceFormat.Color);
                Color[] colorTab = new Color[WIDTH_OF_SHADER * HEIGHT_OF_SHADER];
                foreach(Point point in PixelsTabList[i])
                {
                    colorTab[point.Y * WIDTH_OF_SHADER + point.X] = Color.Black;
                }
               
                container.SetData(colorTab);
                Containers.Add(container);
            }
        }

        float time = 0;
        bool next = false;
        public override void Update(float update)
        {
            time += update;
            if (time > 10 / numOfContainers) { next = true; time = 0; }
        }

        public int k = 0;
        public override void Draw(SpriteBatch sprite)
        {
            if (next) { k++; next = false; }
            for(int i =k;i< Containers.Count(); i++)
            {
                sprite.Draw(Containers[i], Vector2.Zero);
            }
            
        }
    }
}
