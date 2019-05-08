using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace SuperMarioBros
{
    public abstract class Chunk
    {
        public Texture2D texture;
        public List<Tile> ListOfTiles;
        protected int sizeOfTile;
        public Scene scene;
        private int myVar;

        public int SizeOfTile
        {
            get { return sizeOfTile; }
            set { sizeOfTile = value; }
        }

        public void MakeChunk(Vector2 origin)
        {

            foreach (var tile1 in ListOfTiles)
            {
                Tile tile = new Tile(tile1);
                tile.rectangle.X += origin.X;
                tile.rectangle.Y += origin.Y;
                scene.AddGameObject(tile);
            }
        }

    }
    public class Pyramid : Chunk
    {
        public int size;
        public Pyramid(Texture2D texture, int sizeoftile,Scene scene,int size)
        {
            ListOfTiles = new List<Tile>();
            this.scene = scene;
            this.texture = texture;
            this.sizeOfTile = sizeoftile;
            this.size = size;

            for(int i = 0; i < size; i++)
                for(int j=i; j < 2*size -i; j++)
                {
                    ListOfTiles.Add(new Tile( new Rectangle(j*sizeOfTile,sizeOfTile *(size - i),sizeOfTile,sizeOfTile), this.texture , true) );
                }
        }

    }
    
    public class VerticalWall : Chunk
    {
        public int size;
        public VerticalWall(Texture2D texture, int sizeoftile, Scene scene, int size)
        {
            ListOfTiles = new List<Tile>();
            this.scene = scene;
            this.texture = texture;
            this.sizeOfTile = sizeoftile;
            this.size = size;

            for (int i = 0; i < size; i++)
                {
                    ListOfTiles.Add(new Tile(new Rectangle(0, sizeOfTile * i, sizeOfTile, sizeOfTile), this.texture, true));
                }
        }

    }

    public class HorizontalWall : Chunk
    {
        public int size;
        public HorizontalWall(Texture2D texture, int sizeoftile, Scene scene, int size)
        {
            ListOfTiles = new List<Tile>();
            this.scene = scene;
            this.texture = texture;
            this.sizeOfTile = sizeoftile;
            this.size = size;

            for (int i = 0; i < size; i++)
            {
                ListOfTiles.Add(new Tile(new Rectangle(sizeOfTile * i, 0 , sizeOfTile, sizeOfTile), this.texture, true));
            }
        }

    }
}
