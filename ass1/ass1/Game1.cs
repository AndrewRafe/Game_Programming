using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Diagnostics;
using System.IO;

namespace TowerDefence {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game {

        private const String STATE_MENU = "MENU";
        private const String STATE_GAME = "GAME";
        private const String STATE_GAME_OVER = "GAME_OVER";
        private const String STATE_PAUSE = "PAUSE";
        private const String STATE_LEVEL_EDITOR = "LEVEL_EDITOR";
        private String currentState;

        public int SCREEN_WIDTH;
        public int SCREEN_HEIGHT;

        public static Color TEXT_COLOR = Color.DarkRed;

        public Wave currentWave;

        //World bounds width and height in tiles
        public static int WORLD_BOUNDS_WIDTH = 20;
        public static int WORLD_BOUNDS_HEIGHT = 20;
        public static float TILE_SIZE = 50.0f;

        public static float BASIC_TURRET_RANGE = 500.0f;

        private int timeMinutes;
        private int timeMilliseconds;

        private int prevWaveNumber;

        private bool towerHealthDanger;

        //Sound effects
        private SoundEffect turretDestroyedSound;
        private SoundEffect cannonFire;
        private SoundEffect enemyDeath;
        private SoundEffect siren;
        private SoundEffect towerScream;
        //http://www.bensound.com/royalty-free-music/track/epic
        private Song backgroundMusic;
        private SoundEffectInstance sirenInstance;

        public GraphicsDeviceManager graphics { get; private set; }
        public SpriteBatch spriteBatch;
        BasicEffect effect;
        WorldModelManager worldModelManager;

        Player player;

        Grid grid;

        MouseState prevMouseState;
        KeyboardState prevKeyboardState;

        SpriteFont informationFont;

        public Random rand = new Random();

        int enemiesKilled;

        public Camera camera;

        /// <summary>
        /// Constructor method for the game
        /// </summary>
        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = 1000;
            graphics.PreferredBackBufferWidth = 1900;
            currentState = STATE_MENU;
            Content.RootDirectory = "Content";
            prevWaveNumber = 1;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            camera = new Camera(this, new Vector3(0, 1400, 75), Vector3.Zero, Vector3.Up);
            Components.Add(camera);

            grid = new Grid(Vector3.Zero, WORLD_BOUNDS_WIDTH,WORLD_BOUNDS_HEIGHT, this);
            Debug.WriteLine(grid.ToString());
            worldModelManager = new WorldModelManager(this, graphics, grid);
            Components.Add(worldModelManager);

            player = new Player(this);

            prevMouseState = Mouse.GetState();

            enemiesKilled = 0;

            currentWave = new Wave(1, 500);

            timeMinutes = 0;
            timeMilliseconds = 0;

            towerHealthDanger = false;

            turretDestroyedSound = Content.Load<SoundEffect>(@"Sound\explosion");
            cannonFire = Content.Load<SoundEffect>(@"Sound\cannonFire");
            enemyDeath = Content.Load<SoundEffect>(@"Sound\enemyDeath");
            siren = Content.Load<SoundEffect>(@"Sound\siren");
            towerScream = Content.Load<SoundEffect>(@"Sound\towerScream");
            backgroundMusic = Content.Load<Song>(@"Sound\backgroundMusic");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(backgroundMusic);

            sirenInstance = siren.CreateInstance();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            

            effect = new BasicEffect(GraphicsDevice);

            informationFont = Content.Load<SpriteFont>(@"Fonts\playerInfoFont");

            SCREEN_HEIGHT = graphics.PreferredBackBufferHeight;
            SCREEN_WIDTH = graphics.PreferredBackBufferWidth;

            Debug.WriteLine("Screen height = " + SCREEN_HEIGHT + " Screen width = " + SCREEN_WIDTH);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || 
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //State Machine:
            MouseState mouseState = Mouse.GetState();
            KeyboardState ks = Keyboard.GetState();
            Vector3 pickedPosition = this.PickedPosition();
            if (currentState == STATE_MENU) {
                if (ks.IsKeyDown(Keys.L)) {
                    currentState = STATE_LEVEL_EDITOR;
                }
                if (ks.IsKeyDown(Keys.Space) && prevKeyboardState.IsKeyDown(Keys.Space)) {
                    currentState = STATE_GAME;
                }
                //Level 1
                if (ks.IsKeyDown(Keys.D1)) {
                    ResetGame();
                    LoadLevelData("../../../../Content/level1.txt");
                }

                //Level 2
                if (ks.IsKeyDown(Keys.D2)) {
                    ResetGame();
                    LoadLevelData("../../../../Content/level2.txt");
                }

                //Level 3
                if (ks.IsKeyDown(Keys.D3)) {
                    ResetGame();
                    LoadLevelData("../../../../Content/level3.txt");
                }

            } else if (currentState == STATE_GAME) {

                camera.Update(gameTime);
                worldModelManager.Update(gameTime);
                timeMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
                //If time in milliseconds is greater than one minute
                if (timeMilliseconds > 60000) {
                    timeMinutes++;
                    timeMilliseconds = 0;
                    int newWaveNumber = currentWave.waveNumber + 1;
                    int newSpawnRate = currentWave.spawnRate / currentWave.waveNumber;
                    currentWave = new Wave(newWaveNumber, newSpawnRate);
                }
                //Random enemy creation every frame
                if (currentWave.SpawnEnemy()) {
                    worldModelManager.CreateEnemy();
                }

                try {
                    worldModelManager.selectionCube.ChangeSelectionPosition(grid.GetTile(PickedPositionTranslation(pickedPosition)).globalPosition);
                }
                catch (NullReferenceException) {
                    Debug.WriteLine("Mouse outside world bounds");
                }

                try {
                    //Debug.WriteLine("Cube position is now: X: " + pickedPosition.X + " Y: " + -pickedPosition.Z + " Z: " + pickedPosition.Y);
                    //CREATION OF THE TURRET ON CLICK
                    if (mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released) {
                        if (player.HasSuffucientMoney(Turret.COST)) {
                            worldModelManager.CreateTurret(grid.GetTile(PickedPositionTranslation(pickedPosition)).globalPosition);
                            player.SpendMoney(Turret.COST);
                        }
                        else {
                            //Player does not have enough money for turret
                        }

                    }

                }
                catch (NullReferenceException) {
                    Debug.WriteLine("Tried to build outside world bounds. That is not allowed");
                }

                if (worldModelManager.tower.IsDead()) {
                    currentState = STATE_GAME_OVER;
                }

                if (ks.IsKeyDown(Keys.Space) && !prevKeyboardState.IsKeyDown(Keys.Space)) {
                    currentState = STATE_PAUSE;
                }
            } else if (currentState == STATE_PAUSE) {
                if (ks.IsKeyDown(Keys.Space) && !prevKeyboardState.IsKeyDown(Keys.Space)) {
                    currentState = STATE_GAME;
                }
                if (ks.IsKeyDown(Keys.X)) {
                    currentState = STATE_MENU;
                    ResetGame();
                }
            } else if (currentState == STATE_GAME_OVER) {
                if (currentWave != null) {
                    ResetGame();
                }

                if (ks.IsKeyDown(Keys.Space) && !prevKeyboardState.IsKeyDown(Keys.Space)) {
                    currentState = STATE_MENU;
                }
            } else if (currentState == STATE_LEVEL_EDITOR) {
                camera.Update(gameTime);
                worldModelManager.Update(gameTime);

                try {
                    worldModelManager.selectionCube.ChangeSelectionPosition(grid.GetTile(PickedPositionTranslation(pickedPosition)).globalPosition);
                }
                catch (NullReferenceException) {
                    Debug.WriteLine("Mouse outside world bounds");
                }

                try {

                    if (mouseState.LeftButton == ButtonState.Pressed) {
                        Debug.WriteLine(pickedPosition.ToString());
                        Tile currentMouseOverTile = grid.GetTile(PickedPositionTranslation(pickedPosition));
                        worldModelManager.CreateWall(grid.GetTile(PickedPositionTranslation(pickedPosition)).globalPosition);
                    }
                }
                catch (NullReferenceException) {
                    Debug.WriteLine("Tried to build outside world bounds. That is not allowed");
                }

                if (ks.IsKeyDown(Keys.Enter)) {
                    WriteLevelToFile("../../../../Content/level3.txt", worldModelManager.grid);
                    
                    currentState = STATE_MENU;
                }
            }

            effect.EnableDefaultLighting();
            prevMouseState = mouseState;
            prevKeyboardState = ks;
        }

        /// <summary>
        /// Private helper method to conver the mouse picked position to the global coordinate system
        /// </summary>
        /// <param name="pickedPosition">The picked position of the mouse</param>
        /// <returns>The fix to fit the global coordinate system</returns>
        public static Vector3 PickedPositionTranslation(Vector3 pickedPosition) {
            return new Vector3(pickedPosition.X, -pickedPosition.Z, pickedPosition.Y);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);

            ShowBasicInformation();

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        /// <summary>
        /// Is called when a player has killed an enemy
        /// </summary>
        /// <param name="rewardForKilled"></param>
        public void EnemyKilled(float rewardForKilled) {
            player.GiveMoney(rewardForKilled);
            enemyDeath.Play();
            enemiesKilled++;
        }

        public void TurretDestroyed() {
            turretDestroyedSound.Play();
        }

        public void CannonFire() {
            cannonFire.Play();
        }

        public void TowerDangerHealth() {
            if (!towerHealthDanger) {
                sirenInstance.IsLooped = true;
                sirenInstance.Play();
                towerHealthDanger = true;
            } else if (towerHealthDanger) {
                sirenInstance.Stop();
                towerHealthDanger = false;
            }
        }

        public void TowerTakesDamage() {
            towerScream.Play();
        }

        public void InvalidTurretPlacement() {
            player.GiveMoney(Turret.COST);
        }

        public void InvalidWallPlacement() {
            player.GiveMoney(Wall.DEFAULT_COST);
        }

        /// <summary>
        /// Will find the current position of the mouse cursor on the ground plane
        /// </summary>
        /// <returns>The position of the cursor in the world</returns>
        public Vector3 PickedPosition() {
            //THE LOGIC FOR DETERMINING THE POSITION OF THE MOUSE RELATIVE TO GROUND PLANE
            KeyboardState ks = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            Vector3 nearsource = new Vector3((float)mouseState.Position.X, (float)mouseState.Position.Y, 0f);
            Vector3 farsource = new Vector3((float)mouseState.Position.X, (float)mouseState.Position.Y, 1f);

            Matrix world = Matrix.CreateTranslation(0, 0, 0);

            Vector3 nearPoint = GraphicsDevice.Viewport.Unproject(nearsource, camera.projection, camera.view, world);
            Vector3 farPoint = GraphicsDevice.Viewport.Unproject(farsource, camera.projection, camera.view, world);

            // Create a ray from the near clip plane to the far clip plane.
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            Ray pickRay = new Ray(nearPoint, direction);

            // calcuate distance of plane intersection point from ray origin
            float? distance = pickRay.Intersects(Ground.groundPlane);

            if (distance != null) {
                Vector3 pickedPosition = nearPoint + direction * (float)distance;
                return pickedPosition;
            } else {
                return Vector3.Zero;
            }
            

        }

        public void ShowBasicInformation() {
            spriteBatch.Begin();

           

            //Message displayed when game is over
            if (currentState == STATE_GAME_OVER) {
                String gameOverString = "THE TOWER HAS BEEN DESTROYED";
                String waveNumberGameOverString = "YOU MADE IT TO WAVE: " + prevWaveNumber;
                String instructionText = "Hold Space to Continue";
                Vector2 gameOverCenterVector = informationFont.MeasureString(gameOverString) / 2;
                Vector2 waveNumberGameOverCenterVector = informationFont.MeasureString(waveNumberGameOverString) / 2;
                Vector2 instructionTextVector = informationFont.MeasureString(instructionText)/2;
                spriteBatch.DrawString(informationFont, gameOverString, new Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2 - 100), Game1.TEXT_COLOR, 0, gameOverCenterVector, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(informationFont, waveNumberGameOverString, new Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2), Game1.TEXT_COLOR, 0, waveNumberGameOverCenterVector, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(informationFont, instructionText, new Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2 +100), Game1.TEXT_COLOR, 0, instructionTextVector, 1.0f, SpriteEffects.None, 0.5f);

            } else if (currentState == STATE_PAUSE) {
                String alertText;
                alertText = "Game is Paused. Please press space to continue";
                //Find the center of the string
                Vector2 fontOrigin = informationFont.MeasureString(alertText) / 2;
                //Draw the String
                spriteBatch.DrawString(informationFont, alertText, new Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2), Game1.TEXT_COLOR, 0, fontOrigin, 1.5f, SpriteEffects.None, 0.5f);

            } else if (currentState == STATE_GAME) {
                //Player and Tower information on screen
                player.DrawText(spriteBatch, informationFont);
                worldModelManager.tower.DrawText(spriteBatch, informationFont);
                String timeString;
                if (timeMilliseconds < 10000) {
                    timeString = "Time: " + timeMinutes + ":0" + timeMilliseconds / 1000;
                }
                else {
                    timeString = "Time: " + timeMinutes + ":" + timeMilliseconds / 1000;
                }
                spriteBatch.DrawString(informationFont, timeString, new Vector2(SCREEN_WIDTH - 150, 20), TEXT_COLOR);
                spriteBatch.DrawString(informationFont, "Wave: " + currentWave.waveNumber, new Vector2(SCREEN_WIDTH - 150, SCREEN_HEIGHT - 100), Game1.TEXT_COLOR);
                if (currentWave.waveNumber == 1) {
                    Tutorial();
                }

            } else if (currentState == STATE_MENU) {
                String alertText = "DEFEND THE TOWER";
                String instructionText = "Press Space to Start, L to start Level Editor";
                //Find the center of the string
                Vector2 fontOrigin = informationFont.MeasureString(alertText) / 2;
                Vector2 instructionTextVector = informationFont.MeasureString(instructionText) / 2;
                //Draw the String
                spriteBatch.DrawString(informationFont, alertText, new Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2), Game1.TEXT_COLOR, 0, fontOrigin, 1.5f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(informationFont, instructionText, new Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2 + 100), Game1.TEXT_COLOR, 0, instructionTextVector, 1.0f, SpriteEffects.None, 0.5f);
            }


            spriteBatch.End();
        }

        public void Tutorial() {
            String alertText;
            //Wave number alert
            if (currentState == STATE_PAUSE) {
                return;
            }
            if (timeMilliseconds < 5000 && currentWave.waveNumber != 1) {
                alertText = "Wave " + currentWave.waveNumber;
            }
            else if (currentWave.waveNumber > 1) {
                alertText = "";
            }
            else if (currentWave.waveNumber == 1 && timeMilliseconds < 10000) {
                alertText = "YOU MUST DEFEND YOUR TOWER";
            }
            else if (currentWave.waveNumber == 1 && timeMilliseconds < 20000) {
                alertText = "Left Click to place a Cannon. A Cannon costs $100";
            }
            else if (currentWave.waveNumber == 1 && timeMilliseconds < 28000) {
                alertText = "But be careful, the enemy can destroy your Cannons";
            }
            else if (currentWave.waveNumber == 1 && timeMilliseconds < 37000) {
                alertText = "You can earn more money by killing Enemies";
            }
            else if (currentWave.waveNumber == 1 && timeMilliseconds < 46000) {
                alertText = "A wave will last 1 Minute";
            }
            else if (currentWave.waveNumber == 1 && timeMilliseconds < 55000) {
                alertText = "Each wave will increase the spawn rate of enemies";
            }
            else if (currentWave.waveNumber == 1 && timeMilliseconds < 60000) {
                alertText = "HOW LONG WILL YOU LAST?";
            }
            else {
                alertText = "";
            }

            //Find the center of the string
            Vector2 fontOrigin = informationFont.MeasureString(alertText) / 2;
            //Draw the String
            spriteBatch.DrawString(informationFont, alertText, new Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2), Game1.TEXT_COLOR, 0, fontOrigin, 1.5f, SpriteEffects.None, 0.5f);
        }

        private void ResetGame() {
            prevWaveNumber = currentWave.waveNumber;
            timeMilliseconds = 0;
            timeMinutes = 0;
            Components.Remove(worldModelManager);
            sirenInstance.Dispose();
            MediaPlayer.Stop();
            Initialize();
        }

        /// <summary>
        /// Will write the map to a text file
        /// </summary>
        /// <param name="filename">The filename that the map will be stored</param>
        /// <param name="grid">The grid that is being saved</param>
        private void WriteLevelToFile(String filename, Grid grid) {
            String isWallString = "";
            int i = 0;
            foreach (Tile tile in grid.tiles) {
                if (tile.buildingsOnTile.Count > 0) {
                    isWallString += "1";
                } else {
                    isWallString += "0";
                }
                i++;
            }
            System.IO.File.WriteAllText(filename, isWallString);
            //Debug.WriteLine(isWallString);


        }

        private void LoadLevelData(String filename) {
            String mapData;
            using (StreamReader reader = new StreamReader(filename)) {
                mapData = reader.ReadLine();
            }
            int i = -WORLD_BOUNDS_WIDTH/2;
            int j = -WORLD_BOUNDS_HEIGHT/2;
            foreach(char isWall in mapData) {
                
                if (isWall.ToString() == "1") {
                    worldModelManager.CreateWall(worldModelManager.grid.GetTile(new Vector2(j, -i)).globalPosition);
                }
                i++;
                if (i > WORLD_BOUNDS_WIDTH/2) {
                    j++;
                    i = -WORLD_BOUNDS_WIDTH / 2;
                } 
            }

        }

    }


}
