using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using static SuperMarioBros.ContentManager;

namespace SuperMarioBros
{
    public static class ContentManager
    {
        public static Dictionary<string, Texture2D> Textures;
        public static Dictionary<string, SoundEffect> SoundEffects;
        public static Dictionary<string, Song> Songs;
        public static Dictionary<string, Effect> Shaders;
        public static Game game;

        //public int MyProperty { get; private set; }

        public static void Initialize(Game game1)
        {
            Textures = new Dictionary<string, Texture2D>();
            SoundEffects = new Dictionary<string, SoundEffect>();
            Songs = new Dictionary<string, Song>();
            Shaders = new Dictionary<string, Effect>();
            game = game1;
        }

        public static void LoadTextures(params (string nazwa, string file)[] TextureNames)
        {
            foreach (var pair in TextureNames)
            {
                Textures.Add(pair.nazwa, game.Content.Load<Texture2D>(pair.file));
            }
        }

        public static void LoadSoundEffects(params (string nazwa, string file)[] SoundEffectNames)
        {
            foreach (var pair in SoundEffectNames)
            {
                SoundEffects.Add(pair.nazwa, game.Content.Load<SoundEffect>(pair.file));
            }
        }

        public static void LoadSongs(params (string nazwa, string file)[] SongNames)
        {
            foreach (var pair in SongNames)
            {
                Songs.Add(pair.nazwa, game.Content.Load<Song>(pair.file));
            }
        }

        public static void LoadShaders(params (string nazwa, string file)[] ShaderNames)
        {
            foreach (var pair in ShaderNames)
            {
                game.Content.Load<Effect>(pair.file);
                Shaders.Add(pair.nazwa, game.Content.Load<Effect>(pair.file));
            }
        }

        private static Random rnd = new Random((int)System.DateTime.UtcNow.TimeOfDay.TotalSeconds);

        public static Song GetRandomSong()
        {
            return Songs.Values.ElementAt(rnd.Next() % Songs.Count());
        }
        public static SoundEffect GetRandomSoundEffect()
        {
            return SoundEffects.Values.ElementAt(rnd.Next() % SoundEffects.Count());
        }

        public static void ShowListOfTextures()
        {
            foreach (var pair in Textures)
            {
                Console.WriteLine(pair.Key);
            }
        }

        static int magicx = 2484 / 34;
        static int magicy = 716 / 34;
        public static Texture2D[,] tilesTextures = new Texture2D[magicx, magicy];
        public static void GetStageTiles()
        {
            Texture2D stage = Textures["stagetiles"];
            Microsoft.Xna.Framework.Rectangle sourceRect;
            Color[] colorsTab = new Color[32 * 32];
            for (int i = 0; i < magicx; i++)
                for (int j = 0; j < magicy; j++)
                {
                    Texture2D tileTexture = new Texture2D(game.GraphicsDevice, 32, 32);
                    sourceRect = new Microsoft.Xna.Framework.Rectangle(2 + 34 * i, 2 + 34 * j, 32, 32);
                    stage.GetData<Color>(0, sourceRect, colorsTab, 0, colorsTab.Length);
                    tileTexture.SetData<Color>(colorsTab);
                    tilesTextures[i, j] = tileTexture;
                }
        }

    }
    public static class Texture2DExtender
    {
        /// <summary>
        /// Function for extracting a part of a Texture
        /// </summary>
        /// <param name="texture">Texture extracted from</param>
        /// <param name="rect">Part of the texture that we extract from</param>
        /// <returns>Smallet Texture2D</returns>
        public static Texture2D ExtractTexture(this Texture2D texture, Rectangle rect)
        {
            Texture2D result = new Texture2D(ContentManager.game.GraphicsDevice, (int)rect.width, (int)rect.height);
            Color[] colorsTab = new Color[(int)rect.width * (int)rect.height];
            texture.GetData<Color>(0, rect, colorsTab, 0, colorsTab.Length);
            result.SetData<Color>(colorsTab);
            return result;
        }
        public static void SetTexture(this Texture2D texture, Texture2D AddedTexture, Rectangle rect)
        {
            Color[] colorsTab = new Color[(int)rect.width * (int)rect.height];
            AddedTexture.GetData<Color>(colorsTab, 0, colorsTab.Length);
            texture.SetData<Color>(0, rect, colorsTab, 0, colorsTab.Length);
        }
        public static Texture2D FlipTextureVertically(this Texture2D texture)
        {
            Texture2D result = new Texture2D(ContentManager.game.GraphicsDevice, texture.Width, texture.Height);
            Color[] colorsTab = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(colorsTab, 0, colorsTab.Length);
            for (int i = 0; i < texture.Width; i++)
            {
                for (int j = 0; j < texture.Height/2; j++)
                {
                    int a = j * texture.Width + i;
                    int b = (texture.Height - 1 - j) * texture.Width + i;
                    var pom = colorsTab[a];
                    colorsTab[a] = colorsTab[b];
                    colorsTab[b] = pom;
                }
            }
            result.SetData<Color>(colorsTab, 0, colorsTab.Length);
            return result;
        }

        public static Texture2D ProperBlockTexture(Texture2D texture, int width, int height)
        {
            Texture2D result = new Texture2D(ContentManager.game.GraphicsDevice, width * 32, height * 32);
            ////////////////
            Texture2D TopLeft = texture.ExtractTexture(new Rectangle(0, 0, 32, 32));
            Texture2D TopRight = texture.ExtractTexture(new Rectangle(32, 0, 32, 32));
            Texture2D BotLeft = texture.ExtractTexture(new Rectangle(0, 32, 32, 32));
            Texture2D BotRight = texture.ExtractTexture(new Rectangle(32, 32, 32, 32));
            /////////////////////////
            Texture2D Top = texture.ExtractTexture(new Rectangle(16, 0, 32, 32));
            Texture2D Bot = texture.ExtractTexture(new Rectangle(16, 32, 32, 32));
            Texture2D Left = texture.ExtractTexture(new Rectangle(0, 16, 32, 32));
            Texture2D Right = texture.ExtractTexture(new Rectangle(32, 16, 32, 32));
            Texture2D Mid = texture.ExtractTexture(new Rectangle(16, 16, 32, 32));

            for (int i = 1; i < width - 1; i++) result.SetTexture(Top, new Rectangle(32 * i, 0, 32, 32));
            for (int i = 1; i < height - 1; i++) result.SetTexture(Right, new Rectangle((width - 1) * 32, 32 * i, 32, 32));
            for (int i = 1; i < width - 1; i++) result.SetTexture(Bot, new Rectangle(32 * i, (height - 1) * 32, 32, 32));
            for (int i = 1; i < height - 1; i++) result.SetTexture(Left, new Rectangle(0, 32 * i, 32, 32));

            for (int i = 1; i < width - 1; i++)
                for (int j = 1; j < height - 1; j++)
                {
                    result.SetTexture(Mid, new Rectangle(32 * i, 32 * j, 32, 32));
                }

            result.SetTexture(TopLeft, new Rectangle(0, 0, 32, 32));
            result.SetTexture(TopRight, new Rectangle((width - 1) * 32, 0, 32, 32));
            result.SetTexture(BotLeft, new Rectangle(0, (height - 1) * 32, 32, 32));
            result.SetTexture(BotRight, new Rectangle((width - 1) * 32, (height - 1) * 32, 32, 32));

            return result;
        }

        public static Random rnd = new Random();

        public static Texture2D ProperPipe(int width, int height)
        {
            int ia = rnd.Next(2) == 0 ? 0 : 4;
            int j = 2+ 2*rnd.Next(6);

            Texture2D result = new Texture2D(ContentManager.game.GraphicsDevice, width * 32, height * 32);
            ////////////////
            Texture2D TopLeft = tilesTextures[ia, j];
            Texture2D TopRight = tilesTextures[ia + 1, j];
            Texture2D BotLeft = TopLeft.FlipTextureVertically();
            Texture2D BotRight = TopRight.FlipTextureVertically();
            /////////////////////////
            Texture2D Left = tilesTextures[ia, j+1];
            Texture2D Right = tilesTextures[ia + 1, j+1];

            for (int i = 1; i < height - 1; i++) result.SetTexture(Right, new Rectangle((width - 1) * 32, 32 * i, 32, 32));
            for (int i = 1; i < height - 1; i++) result.SetTexture(Left, new Rectangle(0, 32 * i, 32, 32));
            
            result.SetTexture(TopLeft, new Rectangle(0, 0, 32, 32));
            result.SetTexture(TopRight, new Rectangle((width - 1) * 32, 0, 32, 32));
            result.SetTexture(BotLeft, new Rectangle(0, (height - 1) * 32, 32, 32));
            result.SetTexture(BotRight, new Rectangle((width - 1) * 32, (height - 1) * 32, 32, 32));

            return result;
        }

        public static Texture2D ProperCloud(int width)
        {
            Texture2D result = new Texture2D(ContentManager.game.GraphicsDevice, width * 32 + 64, 64);
            ////////////////
            Texture2D TopLeft = tilesTextures[42, 0];
            Texture2D TopRight = tilesTextures[44, 0];
            Texture2D BotLeft = tilesTextures[42, 1];
            Texture2D BotRight = tilesTextures[44, 1];
            /////////////////////////
            Texture2D Top = tilesTextures[43,0];
            Texture2D Bot = tilesTextures[43,1];

            for (int i = 0; i < width; i++) result.SetTexture(Top, new Rectangle((i + 1) * 32,  0, 32, 32));
            for (int i = 0; i < width; i++) result.SetTexture(Bot, new Rectangle((i + 1) * 32, 32, 32, 32));

            result.SetTexture(TopLeft, new Rectangle(0, 0, 32, 32));
            result.SetTexture(TopRight, new Rectangle((width + 1) * 32, 0, 32, 32));
            result.SetTexture(BotLeft, new Rectangle(0, 32, 32, 32));
            result.SetTexture(BotRight, new Rectangle((width + 1) * 32,  32, 32, 32));

            return result;
        }

        public static Texture2D GetTextureRectangleFromTable(Texture2D[,] Textures,int x1,int y1,int x2,int y2)
        {
            int width = (x2 - x1 + 1) * 32;
            int height = (y2 - y1 + 1) * 32;
            Texture2D result = new Texture2D(ContentManager.game.GraphicsDevice, (int)width, (int)height);

            for(int i = x1; i <= x2; i++)
            for(int j = y1; j <= y2; j++)
                {
                    result.SetTexture(Textures[i, j], new Rectangle((i - x1)*32, (j - y1)*32, 32, 32));
                }

            return result;
        }

    }
}

