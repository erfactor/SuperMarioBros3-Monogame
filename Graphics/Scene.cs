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
    public class Scene
    {
        public List<Tile> tiles;
        public List<Tile> tilesToUpdate;
        public List<Mob> mobs;
        public List<Mob> mobsToUpdate;
        public List<Enemy> enemies;
        public List<Enemy> enemiesToUpdate;
        private List<GameObject> QueriedGameObjects;
        public Player player;
        public float waterLevel = 1000f;
        public VerticalShader shader = new VerticalShader();
        public List<(int from, int to, TeleportType tp_type)> Teleports_Info = new List<(int from, int to, TeleportType tp_type)>();

        public Scene()
        {
            tiles = new List<Tile>();
            mobs = new List<Mob>();
            enemies = new List<Enemy>();
            tilesToUpdate = new List<Tile>();
            mobsToUpdate = new List<Mob>();
            enemiesToUpdate = new List<Enemy>();
            QueriedGameObjects = new List<GameObject>();
        }

        public void SelectObjectsInCameraRange(Camera camera)
        {
            Rectangle forMobs = camera.playerBox + new Rectangle(0, 0, 640, 2000);
            Rectangle forTiles = camera.playerBox + new Rectangle(0, 0, 800, 600);
            foreach (Mob m in mobs)
            {
                if(CollisionDetector.isIntersect(forMobs, m.rectangle))
                {
                    mobsToUpdate.Add(m);
                }
            }
            foreach (Enemy e in enemies)
            {
                if (CollisionDetector.isIntersect(forMobs, e.rectangle))
                {
                    enemiesToUpdate.Add(e);
                }
            }
            foreach (Tile t in tiles)
            {
                if (CollisionDetector.isIntersect(forTiles, t.rectangle))
                {
                    tilesToUpdate.Add(t);
                }
            }
        }

        public void ClearToUpdateLists()
        {
            mobsToUpdate.Clear();
            tilesToUpdate.Clear();
            enemiesToUpdate.Clear();
        }


        public void AddGameObject(params GameObject[] GameObjects)
        {
            
            foreach (var gameObject in GameObjects)
            {
                if (gameObject is Tile tile) { tiles.Add(tile); tile.scene = this;}
                else if (gameObject is Enemy enemy) { enemies.Add(enemy); enemy.SetScene(this); }
                else if (gameObject is Mob mob) mobs.Add(mob);
            }
        }

        public void QueryAddGameObject(GameObject gameObject)
        {
            QueriedGameObjects.Add(gameObject);
        }
        public void CheckQuery()
        {
            if (QueriedGameObjects.Any())
            {
                this.AddGameObject(QueriedGameObjects.ToArray());

                QueriedGameObjects.Clear();
            }
        }
        
        public Scene Clone()
        {
            fuzzy = false;
            Scene NewScene = new Scene();

            foreach(var tile in tiles)
            {
                switch (tile)
                {
                    case Tile t when (t.GetType() == typeof(Tile)):
                        NewScene.tiles.Add(new Tile(tile.rectangle.Clone(), tile.texture, tile.solid));
                        break;
                    case Tile t when (t.GetType() == typeof(Block)):
                        NewScene.tiles.Add(new Block(tile.rectangle.Clone(), tile.texture));
                        break;
                    case Tile t when (t.GetType() == typeof(Coin)):
                        NewScene.tiles.Add(new Coin(tile.rectangle.Clone(), tile.texture));
                        break;
                    case Tile t when (t.GetType() == typeof(Star)):
                        NewScene.tiles.Add(new Star(tile.rectangle.Clone()));
                        break;
                    case Tile t when (t.GetType() == typeof(Cloud)):
                        Cloud cloud = t as Cloud;
                        NewScene.tiles.Add(new Cloud(tile.rectangle.Clone(),cloud.combo));
                        break;
                    case Tile t when (t.GetType() == typeof(DestroyableTile)):
                        NewScene.tiles.Add(new DestroyableTile(tile.rectangle.Clone(), tile.texture));
                        break;
                    case Tile t when (t.GetType() == typeof(Pipe)):
                        Pipe last = t as Pipe;
                        NewScene.tiles.Add(new Pipe(tile.rectangle.Clone(), tile.texture, last.number));
                        break;
                    case Tile t when (t.GetType() == typeof(SpawningMob)):
                        SpawningMob sm = (SpawningMob)tile;
                        NewScene.tiles.Add(new SpawningMob(tile.rectangle.Clone(), tile.texture,sm.what == WhatIsSpawned.feather? WhatIsSpawned.feather: sm.what == WhatIsSpawned.coin? WhatIsSpawned.coin: sm.what == WhatIsSpawned.shroom? WhatIsSpawned.shroom:WhatIsSpawned.flower ));
                        break;

                }
                
            }
            Random rnd = new Random();
            foreach (var it in mobs)
            {
                if(it is Player p)
                {
                    //NewScene.mobs.Add(new Player((Rectangle)it.rectangle.Clone(), it.texture));
                    NewScene.player = new Player(new Rectangle(player.rectangle.X, player.rectangle.Y, 32, 32), ContentManager.Textures["FullAnims"]);
                    //NewScene.player.SetScene(this);
                    continue;
                }
                if (it is Shroom sh)
                {
                    NewScene.mobs.Add(new Shroom((Rectangle)it.rectangle.Clone(), it.texture));
                }
                if (it is Platform pl)
                {
                    NewScene.mobs.Add(pl.Clone());
                }

            }
            foreach (Enemy enemy in enemies)
            {
                switch (enemy)
                {
                    case Turtle t:
                        NewScene.enemies.Add(new Turtle((Rectangle)enemy.rectangle.Clone(), enemy.texture));
                        break;
                    case Plant p:
                        NewScene.enemies.Add(new Plant((Rectangle)enemy.rectangle.Clone(), enemy.texture));
                        break;
                    case Walker w:
                        NewScene.enemies.Add(new Walker((Rectangle)enemy.rectangle.Clone(), null));
                        break;
                }
                    
            }

            NewScene.Teleports_Info = this.Teleports_Info.ToList();
            return NewScene;

        }

        public void LoadScene(Scene copy)
        {
            fuzzy = false;
            PipeManager.pipes.Clear();
            Random rnd = new Random();
            this.ClearScene();
            Player player = new Player(new Rectangle(copy.player.rectangle.X, copy.player.rectangle.Y, 32, 32), ContentManager.Textures["FullAnims"]);
            player.SetScene(this);
            this.player = player;
            foreach (var tile in copy.tiles)
            {
                switch (tile)
                {
                    case Tile t when (t.GetType() == typeof(Tile)):
                        tiles.Add(new Tile(tile.rectangle.Clone(), tile.texture, tile.solid));
                        break;
                    case Tile t when (t.GetType() == typeof(Block)):
                        tiles.Add(new Block(tile.rectangle.Clone(), tile.texture));
                        break;
                    case Tile t when (t.GetType() == typeof(Cloud)):
                        Cloud cloud = t as Cloud;
                        tiles.Add(new Cloud(tile.rectangle.Clone(),cloud.combo ));
                        break;
                    case Tile t when (t.GetType() == typeof(Coin)):
                        Coin coin = new Coin(tile.rectangle.Clone(), tile.texture);
                        coin.scene = this;
                        tiles.Add(coin);
                        break;
                    case Tile t when (t.GetType() == typeof(Star)):
                        Star star = new Star(tile.rectangle.Clone());
                        star.scene = this;
                        tiles.Add(star);
                        break;
                    case Tile t when (t.GetType() == typeof(DestroyableTile)):
                        DestroyableTile destroyable = new DestroyableTile(tile.rectangle.Clone(), tile.texture);
                        destroyable.scene = this;
                        tiles.Add(destroyable);
                        break;
                    case Tile t when (t.GetType() == typeof(Pipe)):
                        Pipe last = tile as Pipe;
                        Pipe pipe = new Pipe(tile.rectangle.Clone(), tile.texture,last.number);
                        PipeManager.AddPipe(pipe.number,pipe);
                        pipe.scene = this;
                        tiles.Add(pipe);
                        break;
                    case Tile t when (t.GetType() == typeof(SpawningMob)):
                        SpawningMob sm = (SpawningMob)tile;
                        sm = new SpawningMob(tile.rectangle.Clone(), tile.texture, sm.what == WhatIsSpawned.feather ? WhatIsSpawned.feather : sm.what == WhatIsSpawned.coin ? WhatIsSpawned.coin : sm.what == WhatIsSpawned.shroom?WhatIsSpawned.shroom:WhatIsSpawned.flower);
                        sm.scene = this;
                        tiles.Add(sm);
                        break;

                }
            }
            foreach (var it in copy.mobs)
            {
                if (it is Shroom sh)
                {
                    Shroom shroom = new Shroom((Rectangle)it.rectangle.Clone(), it.texture);
                    shroom.SetScene(this);
                    mobs.Add(shroom);
                }
                if (it is Platform pl)
                {
                    Platform plat = pl.Clone();
                    plat.AddToScene(this);
                    mobs.Add(plat);
                }
            }
            foreach (Enemy enemy in copy.enemies)
            {
                
                switch (enemy)
                {
                    case Turtle t:
                        Turtle newt = new Turtle((Rectangle)enemy.rectangle.Clone(), enemy.texture);
                        newt.SetScene(this);
                        enemies.Add( newt);
                        break;
                    case Plant p:
                        Plant newp = new Plant((Rectangle)enemy.rectangle.Clone(), enemy.texture);
                        newp.SetScene(this);
                        enemies.Add(newp);
                        break;
                    case Walker w:
                        Walker neww = new Walker((Rectangle)enemy.rectangle.Clone(), null);
                        neww.SetScene(this);
                        enemies.Add(neww);
                        break;
                }
            }
            this.player.SetScene(this);
            this.Teleports_Info = copy.Teleports_Info.ToList();
            foreach(var tuple in Teleports_Info)
            {
                if (tuple.tp_type == TeleportType.None) continue;
                PipeManager.AddTeleport(tuple.from, tuple.to, tuple.tp_type);
            }
        }

        public void ClearScene()
        {
            mobs.Clear();
            tiles.Clear();
            enemies.Clear();
            Teleports_Info.Clear();
            PipeManager.ClearPipes();
            player = null;
            fuzzy = false;
        }

        public void SetShader()
        {
            //shader.k = 0;
            shader.GenerateShader(new Vector2(400,340));
        }

        public bool fuzzy = false;
        public void MakeFuzzy()
        {
            fuzzy = true;
        }

        public void DeleteDeadEnemys()
        {
            var ToBeKilled = (from enemy in enemies
                             where enemy.enemydead
                             select enemy).ToArray();
            foreach(Enemy enemy in ToBeKilled)
            {
                enemies.Remove(enemy);
            }

            var MobsToBeKilled = (from mob in mobs
                              where mob.enemydead
                              select mob).ToArray();
            foreach (Mob mob in MobsToBeKilled)
            {
                mobs.Remove(mob);
            }

            
            var TilesToBeKilled = (from d in tiles
                                   where !d.alive
                                  select d).ToArray();
            foreach (var d in TilesToBeKilled)
            {
                tiles.Remove(d);
            }
        }
        
    }
    
}
