using System;
using System.IO;
namespace SuperMarioBros
{
    public class Game1 : Game
    {
        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        public static bool GameActive = true;
        Scene scene = new Scene();
        public Renderer renderer;
        Player player;
        CollisionDetector collisionDetector;
        Scene startingScene;
        public static SpriteFont font;
        static string goback = "../../../../Levels/";
        public static Level level1 = new Level(goback + "1.1.png");
        public static Level level2 = new Level(goback + "1.2.png");
        public static Level level3 = new Level(goback + "1.3.png");
        public static Level level4 = new Level(goback + "1.4.png");
        Map map;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            ContentManager.Initialize(this);
            Animations.Initalize();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            collisionDetector = new CollisionDetector(scene);
            renderer = new Renderer(scene, spriteBatch, graphics);
            Map.graphics = graphics;
            Level.collisionDetector = collisionDetector;
            Level.renderer = renderer;
            this.IsMouseVisible = true;
            base.Initialize();
            map = Map.GetMap();




            System.Timers.Timer timer = new System.Timers.Timer(1000);
            timer.Elapsed += Timer_Elapsed2;
            timer.Start();
        }

        int frames = 0;
        int updates = 0;

        protected override void LoadContent()
        {
            /*                   Load Content Begin          */
            LoadTextures(("mario", "MarioAnimations"), ("floor", "nicetile"), ("mushroom", "Shroom"), ("turtle1", "TurtleAnimations"), ("turtle2", "TurtleAnimations2"), ("turtle3", "TurtleAnimations3"), ("plant", "plant"), ("walker1", "walker1"), ("walker2", "walker2"));
            LoadTextures(("bullet", "bullet"), ("big mario", "big mario"), ("coin", "coin"), ("stagetiles", "StageTiles"), ("level_icons", "level_icons"), ("star1", "Touch a star/star1"), ("star2", "Touch a star/star2"));
            LoadTextures(("feather", "Images/feather"), ("raccoon", "Images/marioanims"), ("block1", "Images/block1"), ("block2", "Images/block2"), ("block3", "Images/block3"), ("speedbar", "Images/speedbar"));
            LoadTextures(("background", "Images/background"), ("pyramid_background", "Images/pyramid_background"), ("sky_background", "Images/sky_background"), ("mapTile", "Images/MapScreenTile"));
            LoadTextures(("FullAnims", "Images/EpicTomekWork"), ("map1", "Images/map1"), ("mapmario", "Images/mapmario"), ("sparkle", "Images/CoinSpark"), ("fireball", "Images/fireball"), ("cloud_anim", "Images/cloud_anim"), ("flower", "Images/flower"));
            LoadSoundEffects(("jumpSound", "Super Mario Bros 3 Real NES Sound Effects/Bump V1 SFX"));
            LoadSoundEffects(("jumpxs", "Super Mario Bros 3 Real NES Sound Effects/1-Up SFX"));
            LoadSoundEffects(("coin", "Super Mario Bros 3 Real NES Sound Effects/Coin Obtained SFX"));
            LoadSongs(("themeSong", "Theme Song"));
            LoadShaders(("water", "shaders/water"), ("circle", "shaders/circle"), ("linear", "shaders/linear"), ("fade", "shaders/fade"), ("hypno", "shaders/hypno"), ("barrel", "shaders/barrel"));
            //// Loader ///
            LoadStuff();
            font = Content.Load<SpriteFont>("Score");
            ContentManager.GetStageTiles();
            /*                   Load Content End           */
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(GetRandomSong());
            level1.LoadLevel(level1.file_name, Textures["background"], Songs["Running.Around"]);
            level2.LoadLevel(level2.file_name, Textures["pyramid_background"], Songs["Grass.Land"]);
            level3.LoadLevel(level3.file_name, Textures["background"], Songs["The.World.in.the.Sky"]);
            level4.LoadLevel(level4.file_name, Textures["sky_background"], Songs["Up.Above"]);
            MainView.SwitchMode(Mode.main_screen);

        }
        protected override void UnloadContent() { }

        Random rnd = new Random((int)System.DateTime.UtcNow.TimeOfDay.TotalSeconds);
        float accumulator = 0;
        bool hypnose = false;
        protected override void Update(GameTime gameTime)
        {
            updates++;
            //if (MediaPlayer.State == MediaState.Stopped) MediaPlayer.Play(Songs.Values.ElementAt(rnd.Next() % Songs.Count()));

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                SwitchMode(Mode.main_screen);

            if (MainView.CurrentMode is Level level)
            {
                if (!level.scene.player.alive)
                {
                    if (!hypnose)
                    {
                        renderer.hypnoactive = true; renderer.ResetHypnoShader(); hypnose = true; GameActive = false;
                        MediaPlayer.Play(Songs["You.have.died"]);
                    }
                    if (!renderer.hypnoactive & hypnose)
                    {
                        hypnose = false;
                        GameActive = true;
                        Map.ResetFade();
                        Renderer.circleSize = 0;
                        if (level == level1) MainView.SwitchMode(Mode.level_1);
                        else if (level == level2) MainView.SwitchMode(Mode.level_2);
                        else if (level == level3) MainView.SwitchMode(Mode.level_3);
                        else if (level == level4) MainView.SwitchMode(Mode.level_4);
                    }
                }
                if (level.scene.player.finished)
                {
                    if (!hypnose)
                    {
                        renderer.hypnoactive = true; renderer.ResetHypnoShader(); hypnose = true; GameActive = false;
                        MediaPlayer.Play(Songs["Level.Completed"]);
                    }
                    if (!renderer.hypnoactive & hypnose) { hypnose = false; SwitchMode(Mode.map); GameActive = true; Map.ResetFade(); }
                }
            }
            // Console.WriteLine(hypnose + "   "+ renderer.hypnoactive);
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                SwitchMode(Mode.textures);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                SwitchMode(Mode.map);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D1) && canrestart)
            {
                TimeoutRestart();
                SwitchMode(Mode.level_1);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D2) && canrestart)
            {
                TimeoutRestart();
                SwitchMode(Mode.level_2);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D3) && canrestart)
            {
                TimeoutRestart();
                SwitchMode(Mode.level_3);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D4) && canrestart)
            {
                TimeoutRestart();
                SwitchMode(Mode.level_4);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.G))
            {
                SwitchMode(Mode.main_screen);
            }

            //if (Keyboard.GetState().IsKeyDown(Keys.R) || MainLevel.player.rectangle.Y >= 2000  ||!MainLevel.player.alive)
            //{
            //   MainLevel = level_1;
            //   MainLevel.LoadLevel();
            //}

            accumulator += (float)gameTime.ElapsedGameTime.TotalSeconds;
            MainView.CurrentMode.Update(ref accumulator);
        }

        private void TimeoutRestart()
        {
            canrestart = false;
            System.Timers.Timer timer = new System.Timers.Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = false;
            timer.Start();
        }

        bool canrestart = true;
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            canrestart = true;
        }

        private void Timer_Elapsed2(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("Frames: " + frames);
            Console.WriteLine("Updates: " + updates);
            updates = 0;
            frames = 0;
        }



        protected override void Draw(GameTime gameTime)
        {
            frames++;
            GraphicsDevice.Clear(Color.Azure);
            MainView.CurrentMode.Draw();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Loading Sounds and Sound Effects
        /// </summary>
        public void LoadStuff()
        {
            DirectoryInfo directory = new DirectoryInfo(".\\..\\..\\..\\..\\Content\\Mario Songs");
            foreach (var file in directory.GetFiles())
            {
                string str = file.Name.Remove(file.Name.Length - 4, 4);

                LoadSongs((file.Name.Remove(file.Name.Length - 4, 4), "Mario Songs/" + str));
            }

            DirectoryInfo directory1 = new DirectoryInfo(".\\..\\..\\..\\..\\Content\\Super Mario Bros 3 Real NES Sound Effects");
            foreach (var file in directory1.GetFiles())
            {
                string str1 = file.Name.Remove(file.Name.Length - 4, 4);
                string str2 = file.Name.Remove(file.Name.Length - 8, 8);
                LoadSoundEffects((str2, "Super Mario Bros 3 Real NES Sound Effects/" + str1));
            }
        }

    }
}
