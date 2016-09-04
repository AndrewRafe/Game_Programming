using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ass1 {
    /// <summary>
    /// A more specific model manager class which keeps track of various aspects of the world
    /// variables by storing globally static objects and also storing seperate model managers
    /// for the various categories of dynamic objects like enemies, turrets, etc.
    /// </summary>
    class WorldModelManager : ModelManager {

        public static int MODEL_OFFSET = 30;

        public Ground ground;
        public SelectionCube selectionCube;
        public Tower tower;
        public Turret towerTurret;

        Game1 game;

        //Stores the state of the mouse in the previous frame
        MouseState prevMouseState;

        //Separate model managers to maintain the dynamic objects in the world
        public ModelManager enemies;
        public ModelManager turretsToBeDrawn;
        public ModelManager allTurrets;
        public ModelManager walls;
        Random rand = new Random();

        /// <summary>
        /// Constructor method that sets up the separate model managers for each of the dynamic
        /// objects in the game.
        /// </summary>
        /// <param name="game"></param>
        public WorldModelManager(Game1 game) : base(game) {
            prevMouseState = Mouse.GetState();
            enemies = new ModelManager(game);
            allTurrets = new ModelManager(game);
            turretsToBeDrawn = new ModelManager(game);
            walls = new ModelManager(game);
            this.game = game;
        }

        /// <summary>
        /// Load the content for the global models in the scene
        /// </summary>
        protected override void LoadContent() {

            ground = new Ground(Game.Content.Load<Model>(@"GroundModels\ground"), new Vector3(0, 0, 0));
            models.Add(ground);
            selectionCube = new SelectionCube(Game.Content.Load<Model>(@"Models\selectionCube"), new Vector3(0, 0, MODEL_OFFSET));
            models.Add(selectionCube);
            tower = new Tower(Game.Content.Load<Model>(@"Models\Buildings\Tower\tower"), new Vector3(0, Game1.WORLD_BOUNDS_HEIGHT/2 - MODEL_OFFSET, MODEL_OFFSET), game);
            models.Add(tower);
            towerTurret = new Turret(Game.Content.Load<Model>(@"Models\Turrets\cannon2"), tower.GetPosition(), Game.Content.Load<Model>(@"Models\Turrets\Bullets\cannonBall"), this);
            allTurrets.models.Add(towerTurret);
            
            CreateEnemy();
            base.LoadContent();
        }

        /// <summary>
        /// Checks to see if any of the models held in this model manager experienced collisions
        /// and takes appropriate action depending on the type of collision
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime) {
            List<Enemy> toBeKilled = new List<Enemy>();
            List<Turret> turretsToBeDestroyed = new List<Turret>();
            foreach (Enemy enemy in enemies.models) {
                enemy.Update(gameTime);
                //If an enemy collides with the tower, the tower takes damage and the enemy is destroyed
                if (enemy.CollidesWith(tower.model, tower.GetWorldMatrix())) {
                    tower.DamageTower(enemy.GetDamage());
                    toBeKilled.Add(enemy);
                }

                foreach (Turret turret in turretsToBeDrawn.models) {
                    if (enemy.CollidesWith(turret.model, turret.GetWorldMatrix())) {
                        turret.DamageTurret(enemy.GetDamage());
                        toBeKilled.Add(enemy);
                        if (turret.health <= 0) {
                            turretsToBeDestroyed.Add(turret);
                            game.TurretDestroyed();
                        }
                    }
                }

            }


            foreach (Turret turret in allTurrets.models) {
                List<Bullet> bulletsToBeDestroyed = new List<Bullet>();
                turret.Update(gameTime);
                foreach (Bullet bullet in turret.bullets.models) {
                    foreach (Enemy enemy in enemies.models) {
                        //When a bullet collides with an enemy
                        if (bullet.CollidesWith(enemy.model, enemy.GetWorldMatrix())) {
                            enemy.DamageEnemy(bullet.damage);
                            bulletsToBeDestroyed.Add(bullet);
                            if (enemy.health <= 0) {
                                toBeKilled.Add(enemy);
                                game.EnemyKilled(enemy.rewardForKilling);
                            }
                        //If the bullet is outside the world bounds
                        } else if (bullet.GetPosition().X > Game1.WORLD_BOUNDS_WIDTH/2 ||
                            bullet.GetPosition().X < -Game1.WORLD_BOUNDS_WIDTH/2 ||
                            bullet.GetPosition().Y > Game1.WORLD_BOUNDS_HEIGHT/2 ||
                            bullet.GetPosition().Y < -Game1.WORLD_BOUNDS_HEIGHT/2) {

                            bulletsToBeDestroyed.Add(bullet);

                        }
                    }
                }

                //Remove all bullets that collided this frame
                foreach (Bullet bullet in bulletsToBeDestroyed) {
                    turret.bullets.models.Remove(bullet);
                }
            }


            //Kill all enemies that need to be killed
            foreach (Enemy enemy in toBeKilled) {
                enemies.models.Remove(enemy);
            }

            //Destroy all turrets that need to be destroyed
            foreach (Turret turret in turretsToBeDestroyed) {
                turretsToBeDrawn.models.Remove(turret);
                allTurrets.models.Remove(turret);
            }


            base.Update(gameTime);
        }

        /// <summary>
        /// Draws this model manager and all of the separate model managers holding the dynamic
        /// objects in the scene
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime) {
            base.Draw(gameTime);
            enemies.Draw(gameTime);
            turretsToBeDrawn.Draw(gameTime);
            towerTurret.bullets.Draw(gameTime);
            foreach (Turret turret in allTurrets.models) {
                turret.bullets.Draw(gameTime);
            }
        }

        /// <summary>
        /// Creates an enemy model at a random location along the spawning zone
        /// </summary>
        public void CreateEnemy() {

            Enemy enemy = new Enemy(Game.Content.Load<Model>(@"Models\Enemy\enemy"), 
                new Vector3(rand.Next(-Game1.WORLD_BOUNDS_WIDTH/2, Game1.WORLD_BOUNDS_WIDTH/2), -Game1.WORLD_BOUNDS_HEIGHT/2, MODEL_OFFSET), tower, game);
            enemies.models.Add(enemy);
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
            Turret turret = new Turret(game.Content.Load<Model>(@"Models\Turrets\cannon2"), new Vector3(position.X, position.Y, position.Z + MODEL_OFFSET), 
                game.Content.Load<Model>(@"Models\Turrets\Bullets\cannonBall"), this);
            foreach (Turret otherTurret in allTurrets.models) {
                if (otherTurret.CollidesWith(turret.model, turret.GetWorldMatrix())) {
                    game.InvalidTurretPlacement();
                    return;
                }
            }
            turretsToBeDrawn.models.Add(turret);
            allTurrets.models.Add(turret);
        }

        /// <summary>
        /// Returns the closest enemy to a given position
        /// Will return NULL if there are no enemies
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <returns>closestEnemy</returns>
        public Enemy GetClosestEnemy(Vector3 currentPosition) {
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

        /// <summary>
        /// Creates a wall at a given position
        /// </summary>
        /// <param name="position"></param>
        public void CreateWall(Vector3 position) {
            walls.models.Add(new Wall(game.Content.Load<Model>(@"Models\Buildings\wall"), position, 100));
        }

        /// <summary>
        /// Let the game know that the cannon has been fired
        /// </summary>
        public void CannonFire() {
            game.CannonFire();
        }
        
    }
}
