﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Diagnostics;

namespace TowerDefence {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game { 

        public int SCREEN_WIDTH;
        public int SCREEN_HEIGHT;

        public static Color TEXT_COLOR = Color.DarkRed;

        public Wave currentWave;

        public static int WORLD_BOUNDS_WIDTH = 1500;
        public static int WORLD_BOUNDS_HEIGHT = 1500;
        public static float TILE_SIZE = 25.0f;

        public static float BASIC_TURRET_RANGE = 500.0f;

        private int timeMinutes;
        private int timeMilliseconds;

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

        GraphicsDeviceManager graphics;
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

        bool gameOver;
        bool pause; // sushmita

        /// <summary>
        /// Constructor method for the game
        /// </summary>
        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = 1000;
            graphics.PreferredBackBufferWidth = 1900;

            Content.RootDirectory = "Content";
            gameOver = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            camera = new Camera(this, new Vector3(0, 200, 75), Vector3.Zero, Vector3.Up);
            Components.Add(camera);

            grid = new Grid(Vector3.Zero, WORLD_BOUNDS_WIDTH / (int)TILE_SIZE,
                (int)WORLD_BOUNDS_HEIGHT / (int)TILE_SIZE);
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

            pause = false;

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

            // TODO: Add your update logic here
            //THE LOGIC FOR DETERMINING THE POSITION OF THE MOUSE RELATIVE TO GROUND PLANE
            Vector3 pickedPosition = this.PickedPosition();
            MouseState mouseState = Mouse.GetState();
            KeyboardState ks = Keyboard.GetState();
            worldModelManager.selectionCube.ChangeSelectionPosition(grid.GetTile(PickedPositionTranslation(pickedPosition)).globalPosition);
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

            if (mouseState.RightButton == ButtonState.Pressed) {
                //worldModelManager.CreateWall(new Vector3(pickedPosition.X, -pickedPosition.Z, pickedPosition.Y));
                Debug.WriteLine(pickedPosition.ToString());
                Tile currentMouseOverTile = grid.GetTile(PickedPositionTranslation(pickedPosition));
                if (currentMouseOverTile != null) {
                    Debug.WriteLine(currentMouseOverTile.ToString());
                }
                if (player.HasSuffucientMoney(Wall.DEFAULT_COST)) {
                    worldModelManager.CreateWall(grid.GetTile(PickedPositionTranslation(pickedPosition)).globalPosition);
                    player.SpendMoney(Wall.DEFAULT_COST);
                }
            }

            prevMouseState = mouseState;

            //Random enemy creation every frame, 1 in 100 chance of spawing
            if (currentWave.SpawnEnemy()) {
                worldModelManager.CreateEnemy();
            }

            if (!gameOver && !pause) {
                timeMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
                //If time in milliseconds is greater than one minute
                if (timeMilliseconds > 60000) {
                    timeMinutes++;
                    timeMilliseconds = 0;
                    int newWaveNumber = currentWave.waveNumber + 1;
                    int newSpawnRate = currentWave.spawnRate / currentWave.waveNumber;
                    currentWave = new Wave(newWaveNumber, newSpawnRate);
                }
            }

            if(ks.IsKeyDown(Keys.Space) && !prevKeyboardState.IsKeyDown(Keys.Space))
            {
                if (pause == false)
                    pause = true;
                else
                    pause = false;
            }

            prevKeyboardState = ks;
            if(pause ==false && !gameOver)
            base.Update(gameTime);
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
            if (currentWave.waveNumber == 1) {
                Tutorial();
            }
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        /// <summary>
        /// Sets the game over value to be true. Will be called by another class when a losing
        /// condition is triggered
        /// </summary>
        public void GameOver() {
            gameOver = true;
            pause = true;
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

            //Player and Tower information on screen
            player.DrawText(spriteBatch, informationFont);
            worldModelManager.tower.DrawText(spriteBatch, informationFont);

            //Message displayed when game is over
            if (gameOver) {
                String gameOverString = "THE TOWER HAS BEEN DESTROYED";
                String waveNumberGameOverString = "YOU MADE IT TO WAVE: " + currentWave.waveNumber;
                Vector2 gameOverCenterVector = informationFont.MeasureString(gameOverString) / 2;
                Vector2 waveNumberGameOverCenterVector = informationFont.MeasureString(waveNumberGameOverString) / 2;
                spriteBatch.DrawString(informationFont, gameOverString, new Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2 - 100), Game1.TEXT_COLOR, 0, gameOverCenterVector, 1.0f, SpriteEffects.None, 0.5f);
                spriteBatch.DrawString(informationFont, waveNumberGameOverString, new Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2 + 100), Game1.TEXT_COLOR, 0, waveNumberGameOverCenterVector, 1.0f, SpriteEffects.None, 0.5f);

            }

            String timeString;
            if (timeMilliseconds < 10000) {
                timeString = "Time: " + timeMinutes + ":0" + timeMilliseconds / 1000;
            }
            else {
                timeString = "Time: " + timeMinutes + ":" + timeMilliseconds / 1000;
            }

            spriteBatch.DrawString(informationFont, timeString, new Vector2(SCREEN_WIDTH - 150, 20), TEXT_COLOR);

            spriteBatch.DrawString(informationFont, "Wave: " + currentWave.waveNumber, new Vector2(SCREEN_WIDTH - 150, SCREEN_HEIGHT - 100), Game1.TEXT_COLOR);

            spriteBatch.End();
        }

        public void Tutorial() {
            String alertText;
            //Wave number alert
            if (pause == true && !gameOver) {
                alertText = "Game is Paused. Please press space to continue";
            }
            else if (timeMilliseconds < 5000 && currentWave.waveNumber != 1) {
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
            spriteBatch.Begin();
            spriteBatch.DrawString(informationFont, alertText, new Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2), Game1.TEXT_COLOR, 0, fontOrigin, 1.5f, SpriteEffects.None, 0.5f);
            spriteBatch.End();
        }

    }


}
