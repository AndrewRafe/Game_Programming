using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefence {
    /// <summary>
    /// A more specific model manager class which keeps track of various aspects of the world
    /// variables by storing globally static objects and also storing seperate model managers
    /// for the various categories of dynamic objects like enemies, turrets, etc.
    /// </summary>
    public class WorldModelManager : ModelManager {

        public static int MODEL_OFFSET = 30;

        public Ground ground;
        public SelectionCube selectionCube;
        public Tower tower;
        public Turret towerTurret;

        Game1 game;
        public Grid grid;

        //Stores the state of the mouse in the previous frame
        MouseState prevMouseState;

        //Separate model managers to maintain the dynamic objects in the world
        Random rand = new Random();

        List<Enemy> allEnemies;

        /// <summary>
        /// Constructor method that sets up the separate model managers for each of the dynamic
        /// objects in the game.
        /// </summary>
        /// <param name="game"></param>
        public WorldModelManager(Game1 game, GraphicsDeviceManager graphics, Grid grid) : base(game, graphics) {
            prevMouseState = Mouse.GetState();
            this.game = game;
            this.grid = grid;
            this.allEnemies = new List<Enemy>();
        }

        /// <summary>
        /// Load the content for the global models in the scene
        /// </summary>
        protected override void LoadContent() {

            ground = new Ground(Game.Content.Load<Model>(@"GroundModels\ground"), new Vector3(0, 0, 0));
            models.Add(ground);
            selectionCube = new SelectionCube(Game.Content.Load<Model>(@"Models\selectionCube"), new Vector3(0, 0, MODEL_OFFSET));
            models.Add(selectionCube);
            tower = new Tower(Game.Content.Load<Model>(@"Models\Buildings\Tower\tower"), grid.GetTile(new Vector3(0, Game1.WORLD_BOUNDS_HEIGHT / 3 - MODEL_OFFSET, MODEL_OFFSET)).globalPosition, game, Tower.DEFAULT_TOWER_HEALTH, Tower.DEFAULT_DAMAGE, Game.Content.Load<Texture2D>(@"HealthTexture"), game.spriteBatch, grid.GetTile(new Vector3(0, Game1.WORLD_BOUNDS_HEIGHT / 3 - MODEL_OFFSET, MODEL_OFFSET)));
            models.Add(tower);
            //towerTurret = new Turret(Game.Content.Load<Model>(@"Models\Turrets\cannon2"), tower.GetPosition(), Game.Content.Load<Model>(@"Models\Turrets\Bullets\cannonBall"), this, Game1.BASIC_TURRET_RANGE, Tower.DEFAULT_TOWER_HEALTH, Turret.DEFAULT_DAMAGE, Game.Content.Load<Texture2D>(@"HealthTexture"), game.spriteBatch, grid.GetTile(tower.GetPosition()));
            //allTurrets.models.Add(towerTurret);
            
            CreateEnemy();
            base.LoadContent();
        }

        /// <summary>
        /// Checks to see if any of the models held in this model manager experienced collisions
        /// and takes appropriate action depending on the type of collision
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            grid.HandleTiles(gameTime);

            /*toBeKilled = new List<Enemy>();
            turretsToBeDestroyed = new List<Turret>();

            EnemyLogic(gameTime);
            TurretLogic(gameTime);
            WallLogic();*/

            
        }

        /// <summary>
        /// Draws this model manager and all of the separate model managers holding the dynamic
        /// objects in the scene
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime) {
            base.Draw(gameTime);
            ground.Draw(game.camera, game.graphics);
            selectionCube.Draw(game.camera, game.graphics);
            tower.Draw(game.camera, game.graphics);
            grid.DrawTiles();
        }

        /// <summary>
        /// Creates an enemy model at a random location along the spawning zone
        /// </summary>
        public void CreateEnemy() {
            Vector3 startingEnemyPosition = new Vector3(rand.Next(-Game1.WORLD_BOUNDS_WIDTH / 2, Game1.WORLD_BOUNDS_WIDTH / 2), -Game1.WORLD_BOUNDS_HEIGHT / 2, 0);
            Enemy enemy = new Enemy(Game.Content.Load<Model>(@"Models\Enemy\enemy"), 
                startingEnemyPosition, Enemy.MAX_HEALTH, Enemy.MAX_DAMAGE, Game.Content.Load<Texture2D>(@"HealthTexture"), tower, game, grid);
            grid.GetTile(startingEnemyPosition).AddEnemyToTile(enemy);
            grid.AddEnemy(enemy);
        }

        /// <summary>
        /// Creates a turret at a given position
        /// </summary>
        /// <param name="position"></param>
        public void CreateTurret(Vector3 position) {
            if (position.X > Game1.WORLD_BOUNDS_WIDTH/2 || position.X < -Game1.WORLD_BOUNDS_WIDTH/2 || position.Y > Game1.WORLD_BOUNDS_HEIGHT/2 || position.Y < -Game1.WORLD_BOUNDS_HEIGHT/2) {
                game.InvalidTurretPlacement();
                return;
            }
            Tile placementTile = grid.GetTile(position);
            Turret turret = new Turret(game.Content.Load<Model>(@"Models\Turrets\cannon2"), new Vector3(position.X, position.Y, 0), 
                game.Content.Load<Model>(@"Models\Turrets\Bullets\cannonBall"), this, Game1.BASIC_TURRET_RANGE, Turret.DEFAULT_HEALTH, Turret.DEFAULT_DAMAGE, null, game.spriteBatch,
                placementTile);
            placementTile.AddTurretToTile(turret);
        }

        /// <summary>
        /// Returns the closest enemy to a given position
        /// Will return NULL if there are no enemies
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <returns>closestEnemy</returns>
        /*public Enemy GetClosestEnemy(Vector3 currentPosition) {
            if (enemies.models.Count == 0) {
                return null;
            }
            Enemy closestEnemy = (Enemy) enemies.models.ElementAt(0);
            foreach (Enemy enemy in enemies.models) {
                //Pythagoras Thereom
                if (Math.Sqrt(Math.Pow(enemy.GetPosition().X - currentPosition.X, 2) + 
                    Math.Pow(enemy.GetPosition().Y - currentPosition.Y, 2)) <
                    Math.Sqrt(Math.Pow(closestEnemy.GetPosition().X - currentPosition.X, 2) + 
                    Math.Pow(closestEnemy.GetPosition().Y - currentPosition.Y, 2))) {
                    closestEnemy = enemy;
                }
            }

            return closestEnemy;
        }
        */
        /// <summary>
        /// Creates a wall at a given position
        /// </summary>
        /// <param name="position"></param>
        public void CreateWall(Vector3 position) {
            if (position.X > Game1.WORLD_BOUNDS_WIDTH / 2 || position.X < -Game1.WORLD_BOUNDS_WIDTH / 2 || position.Y > Game1.WORLD_BOUNDS_HEIGHT / 2 || position.Y < -Game1.WORLD_BOUNDS_HEIGHT / 2) {
                game.InvalidTurretPlacement();
                return;
            }
            Tile placementTile = grid.GetTile(position);
            Wall wall = new Wall(game.Content.Load<Model>(@"Models\selectionCube"), new Vector3(position.X, position.Y, position.Z),
                Wall.DEFAULT_HEALTH, Wall.DEFAULT_DAMAGE, null, game.spriteBatch,
                placementTile);
            grid.AddObstacleTile(placementTile);
            grid.ResetEnemyPath();
            placementTile.AddBuildingToTile(wall);

        }

        /// <summary>
        /// Let the game know that the cannon has been fired
        /// </summary>
        public void CannonFire() {
            game.CannonFire();
        }
    }
}
