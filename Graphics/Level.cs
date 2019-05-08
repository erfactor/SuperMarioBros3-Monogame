using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Drawing;
using static SuperMarioBros.ContentManager;
using Microsoft.Xna.Framework.Graphics;

namespace SuperMarioBros
{
    public class Level : ViewMode
    {
        public Scene scene;
        public Scene startingScene;
        public Player player;
        public string file_name;
        public static Renderer renderer;
        public static CollisionDetector collisionDetector;
        public List<Rectangle> boundaries = new List<Rectangle>();
        Texture2D background;
        Song song;

        public Level(string file_name)
        {
            this.file_name = file_name;
        }

        public void ReloadLevel()
        {
            scene.LoadScene(startingScene);
            player = new Player(new Rectangle(startingScene.player.rectangle.X, startingScene.player.rectangle.Y, 32, 32), Textures["FullAnims"]);
            renderer.SetMario(player);
            renderer.speedBar = new SpeedBar();
            scene.player = player;
            scene.AddGameObject(player);
            collisionDetector.scene = scene;
            player.scene = scene;
            renderer.scene = scene;
            MediaPlayer.Play(song);
            Camera.boundaries = this.boundaries.ToList();
        }

        public void LoadLevel(String path, Texture2D background,Song song)
        {
            this.song = song;
            this.background = background;
            scene = new Scene();
            uint[,] pixels;
            Bitmap img = new Bitmap(path);
            pixels = new uint[img.Height, img.Width];
            int width, height;

            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    Color pixel = img.GetPixel(i, j);
                    pixels[j, i] = (uint)pixel.ToArgb();
                }
            }

            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    uint color = pixels[j, i];
                    //uint a = color >> 24;
                    //uint r = (color >> 16) & 0b11111111;
                    //uint g = (color >> 😎 & 0b11111111;
                    //uint b = color & 0b11111111;
                    switch (color)
                    {
                        case 4294967295://white 255 255 255 255
                            break;
                        case uint n when (n.GREEN() == 20 && n.RED() == 40):
                            Tile[] tiles = {
                                new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[23, 11], true),
                                new Tile(new Rectangle(i * 32 + 32, 32 * j, 32, 32), ContentManager.tilesTextures[24, 11], true),
                                new Tile(new Rectangle(i * 32 + 64, 32 * j, 32, 32), ContentManager.tilesTextures[25, 11], true)
                            };
                            Platform p = new Platform(new Rectangle(i * 32, 32 * j, 32 * 3, 32), tiles, n.BLUE() * 32);
                            scene.mobs.Add(p);
                            break;
                        case uint n when (n.GREEN() == 75 && n.RED() == 75 && n.BLUE() < 30)://black 255 0 0 0
                            switch (n.BLUE())
                            {
                                case 1:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[26, 9], true));
                                    break;
                                case 2:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[27, 9], true));
                                    break;
                                case 3:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[28, 9], true));
                                    break;
                                case 4:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[26, 10], true));
                                    break;
                                case 5:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[27, 10], true));
                                    break;
                                case 6:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[28, 10], true));
                                    break;
                                case 7://default tile
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[59, 1], true));
                                    break;
                                case 8:
                                    DestroyableTile destroyablex = new DestroyableTile(new Rectangle(i * 32, j * 32, 32, 32), tilesTextures[61, 0]);
                                    scene.AddGameObject(destroyablex);
                                    break;
                                case 9:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[41, 0], true));
                                    break;
                                case 10:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[62, 11], true));
                                    break;
                                case 11:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[20, 17], true));
                                    break;
                                case 12:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[21, 17], true));
                                    break;
                                case 13:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[22, 17], true));
                                    break;
                                case 14:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[20, 18], true));
                                    break;
                                case 15:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[21, 18], true));
                                    break;
                                case 16:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[22, 18], true));
                                    break;
                                case 17:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[26, 11], true));
                                    break;
                                case 18:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[27, 11], true));
                                    break;
                                case 19:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[28, 11], true));
                                    break;
                                case 20:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[26, 12], true));
                                    break;
                                case 21:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[27, 12], true));
                                    break;
                                case 22:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[28, 12], true));
                                    break;
                                case 23://pyramid background 23-26
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[28, 4], false));
                                    break;
                                case 24:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[29, 4], false));
                                    break;
                                case 25:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[28, 5], false));
                                    break;
                                case 26:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[29, 5], false));
                                    break;//pyramid
                                case 27://black pyramid 27-28
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[38, 3], false));
                                    break;
                                case 28:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[39, 3], false));
                                    break;//black pyramid
                                case 29://sand tile
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[30, 4], true));
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case uint n when (n.GREEN() == 255 && n.BLUE() > 0 && n.BLUE() < 18)://bushes etc.
                            switch (n.BLUE())
                            {
                                case 1:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[18, 18], false));
                                    break;
                                case 2:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[19, 18], false));
                                    break;
                                case 3:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[18, 20], false));
                                    break;
                                case 4:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[19, 20], false));
                                    break;
                                case 5:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[18, 19], false));
                                    break;
                                case 6:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[19, 19], false));
                                    break;
                                case 7:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[20, 19], false));
                                    break;
                                case 8:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[22, 19], false));
                                    break;
                                case 9:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[42, 4], false));
                                    break;
                                case 10:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[43, 4], false));
                                    break;
                                case 11:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[42, 5], false));
                                    break;
                                case 12:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[43, 5], false));
                                    break;
                                case 13://13-16 palm tree
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[24, 4], false));
                                    break;
                                case 14:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[25, 4], false));
                                    break;
                                case 15:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[26, 5], false));
                                    break;
                                case 16:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[27, 5], false));
                                    break;
                                case 17://cactus
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[22, 4], false));
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case uint n when (n.GREEN() == 200 && n.RED() == 200 && n.BLUE() > 199 && n.BLUE() < 202)://black 255 0 0 0
                            switch (n.BLUE())
                            {
                                case 200:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[41, 2], false));
                                    break;
                                case 201:
                                    scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), ContentManager.tilesTextures[41, 5], false));
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case 4278190080://black 255 0 0 0
                            scene.tiles.Add(new Tile(new Rectangle(i * 32, 32 * j, 32, 32), Textures["floor"], true));
                            break;
                        case 4278255360://green 255 0 255 0
                            player = new Player(new Rectangle(i * 32, j * 32, 32, 32), Textures["FullAnims"]);
                            renderer.SetMario(player);
                            scene.player = player;
                            scene.AddGameObject(player);
                            collisionDetector.scene = scene;
                            renderer.scene = scene;
                            float xoffset = Camera.PLAYER_BOX_WIDTH + 25;
                            float yoffset = Camera.PLAYER_BOX_HEIGHT + 16;
                            boundaries.Add(new Rectangle(0 * 32 + xoffset, 0 * 32 + yoffset, pixels[0, 0].RED() * 32 - xoffset * 2, pixels[0, 1].RED() * 32 - yoffset * 2));
                            boundaries.Add(new Rectangle(((pixels[1, 0] & 0x00ff0000) >> 16) * 32 + xoffset, ((pixels[1, 1] & 0x00ff0000) >> 16) * 32 + yoffset, (pixels[2, 0].RED() - pixels[1,0].RED()) * 32 - xoffset * 2, (((pixels[2, 1] & 0x00ff0000) >> 16) - ((pixels[1, 1] & 0x00ff0000) >> 16)) * 32 - yoffset * 2));
                            uint pipe1 = pixels[3, 0], pipe2 = pixels[3, 1];
                            Console.WriteLine(pipe1.RED() + " " + pipe1.GREEN()+" " + pipe1.BLUE());
                            if (pipe1.BLUE() != 0) { scene.Teleports_Info.Add((pipe1.RED(), pipe1.GREEN(), Tp_From_int(pipe1.BLUE()))); }
                            if (pipe2.BLUE() != 0) { scene.Teleports_Info.Add((pipe2.RED(), pipe2.GREEN(), Tp_From_int(pipe2.BLUE()))); }
                            break;
                        case 4286578688:
                            Turtle turtle = new Turtle(new Rectangle(i * 32, j * 32 - 32, 32, 64), null);
                            scene.AddGameObject(turtle);
                            break;
                        case 4294901760:
                            Walker walker = new Walker(new Rectangle(i * 32, j * 32, 32, 32), null);
                            scene.AddGameObject(walker);
                            break;
                        case 4294967040:
                            scene.AddGameObject(new Coin(new Rectangle(i * 32 + 8, j * 32 + 8, 24, 24), Textures["coin"]));
                            break;
                        case 4278222848:
                            Shroom shroom = new Shroom(new Rectangle(i * 32, j * 32, 32, 32), Textures["mushroom"]);
                            scene.AddGameObject(shroom);
                            break;
                        case 4294934528:
                            Plant plant = new Plant(new Rectangle(i * 32 + 16, j * 32 + 32, 32, 64), Textures["plant"]);
                            scene.AddGameObject(plant);
                            break;
                        case 0xff008080:
                            SpawningMob spawningShroom = new SpawningMob(new Rectangle(i * 32, j * 32, 32, 32), tilesTextures[67, 2], WhatIsSpawned.shroom);
                            scene.AddGameObject(spawningShroom);
                            break;
                        case 0xff00807f:
                            SpawningMob spawningFeather = new SpawningMob(new Rectangle(i * 32, j * 32, 32, 32), tilesTextures[32, 3], WhatIsSpawned.feather);
                            scene.AddGameObject(spawningFeather);
                            break;
                        case 0xff00807e:
                            SpawningMob spawningCoin = new SpawningMob(new Rectangle(i * 32, j * 32, 32, 32), tilesTextures[32, 3], WhatIsSpawned.coin);
                            scene.AddGameObject(spawningCoin);
                            break;
                        case 0xff00807d:
                            SpawningMob spawningFlower = new SpawningMob(new Rectangle(i * 32, j * 32, 32, 32), tilesTextures[32, 3], WhatIsSpawned.flower);
                            scene.AddGameObject(spawningFlower);
                            break;
                        case 0xff3c3c3c:
                            Star star = new Star(new Rectangle(i * 32, j * 32, 64, 64));
                            scene.AddGameObject(star);
                            break;
                        case 0xff_3c_64_64:
                            Cloud cloud = new Cloud(new Rectangle(i * 32, j * 32, 32 * 3, 32 * 2));
                            scene.AddGameObject(cloud);
                            break;
                        case 0xff_3c_64_65:
                            Cloud cloudcombo = new Cloud(new Rectangle(i * 32, j * 32, 32 * 12, 32 * 2), true);
                            scene.AddGameObject(cloudcombo);
                            break;
                        case uint n when (n > 0xff_01_00_00 && n <= 0xff_01_ff_ff):
                            width = n.GREEN(); height = n.BLUE();
                            Block block = new Block(new Microsoft.Xna.Framework.Point(i * 32, j * 32), width, height);
                            scene.AddGameObject(block);
                            break;
                        case uint m when (m > 0xff_02_00_00 && m <= 0xff_02_ff_ff):
                            int number = m.GREEN(); height = m.BLUE();
                            Pipe pipe = new Pipe(new Rectangle(i * 32, j * 32, 2 * 32, height * 32), Texture2DExtender.ProperPipe(2, height), number);
                            //PipeManager.AddPipe(number, pipe);
                            scene.AddGameObject(pipe);
                            break;
                        case 4278190335:
                            DestroyableTile destroyable = new DestroyableTile(new Rectangle(i * 32, j * 32, 32, 32), tilesTextures[61, 0]);
                            scene.AddGameObject(destroyable);
                            break;
                        case 4278190334:
                            DestroyableTile destroyable1 = new DestroyableTile(new Rectangle(i * 32, j * 32, 32, 32), tilesTextures[32, 14]);
                            scene.AddGameObject(destroyable1);
                            break;
                        case 4278190333:
                            DestroyableTile destroyable2 = new DestroyableTile(new Rectangle(i * 32, j * 32, 32, 32), tilesTextures[43, 16]);
                            scene.AddGameObject(destroyable2);
                            break;
                        case 4278190332:
                            DestroyableTile destroyable3 = new DestroyableTile(new Rectangle(i * 32, j * 32, 32, 32), tilesTextures[66, 4]);
                            scene.AddGameObject(destroyable3);
                            break;
                        default:
                            Console.WriteLine("The color is unknown. Value: " + color);
                            break;
                    }
                }
            }
            startingScene = scene.Clone();
        }

        private TeleportType Tp_From_int(int v)
        {
            switch (v)
            {
                case 1:
                    return TeleportType.Top_Top;
                case 2:
                    return TeleportType.Top_Bot;
                case 3:
                    return TeleportType.Bot_Bot;
                case 4:
                    return TeleportType.Bot_Top;
            }
            return TeleportType.None;
        }

        internal void Update(ref float accumulator)
        {
            
            scene.DeleteDeadEnemys();
            scene.CheckQuery();
            scene.ClearToUpdateLists();
            scene.SelectObjectsInCameraRange(renderer.camera);


            if (accumulator > 1 / 60f)
            {
                float deltaTime = accumulator;
                accumulator = 0;
                if (!Game1.GameActive) return;



                scene.shader.Update(deltaTime);

                player.input();
                foreach (var mob in scene.mobsToUpdate)
                {
                    mob.Update();
                }
                foreach (var enemy in scene.enemiesToUpdate)
                {
                    enemy.Update();
                }

                collisionDetector.MarioEnemysCollisions(deltaTime);
                collisionDetector.DetectCollisions(deltaTime);
                collisionDetector.MarioMobsCollisions(deltaTime);
                collisionDetector.DamageCollisions();

                foreach (var enemy in scene.enemiesToUpdate)
                {
                    enemy.Move(deltaTime);
                }
                foreach (var mob in scene.mobsToUpdate)
                {
                    mob.Move(deltaTime);
                }

                foreach (var movable in scene.tilesToUpdate.OfType<IMovable>())
                {
                    movable.Update(deltaTime);
                }
                // base.Update(gameTime);
                
            }
        }

        void ViewMode.Load()
        {
            ReloadLevel();
            //PipeManager.AddTeleport(4, 5, TeleportType.Bot_Bot);
            //PipeManager.AddTeleport(6, 7, TeleportType.Bot_Bot);
            Camera.currentBounds = 0;
            scene.shader = new VerticalShader();
            scene.SetShader();
        }

        void ViewMode.Update(ref float accumulator)
        {
            this.Update(ref accumulator);
        }

        void ViewMode.Draw()
        {
            renderer.Render(background);
        }
    }
}
