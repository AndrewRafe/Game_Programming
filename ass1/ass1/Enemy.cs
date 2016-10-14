using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefence {
    public class Enemy : BasicGameObject {

        public static float MAX_HEALTH = 300.0f;
        public static float MAX_DAMAGE = 100.0f;

        private const String STATE_ATTACK_TOWER = "ATTACK_TOWER";
        private const String STATE_ATTACK_WALL = "ATTACK_WALL";
        private const String STATE_IDLE = "IDLE";
        private const String STATE_RUN_AWAY = "RUN_AWAY";
        private String currentState;

        private LinkedList<Tile> path;
        private Grid grid;

        private bool hasPathToTower;

        public float rewardForKilling { get; protected set; }
        public Vector3 prevPosition { get; private set; }
        public Tile targetTile { get; private set; }
        public Tile spawnTile { get; private set; }
        Tower tower;
        float speed;

        public Enemy(Model m, Vector3 position, float maxHealth, float maxDamage, Texture2D healthBarTexture, Tower tower, Game1 game, Grid grid) : base(m, position, maxHealth, maxDamage, healthBarTexture, game.spriteBatch) {
            this.tower = tower;
            this.maxHealth = maxHealth;
            this.maxDamage = maxDamage;
            this.speed = (float) game.rand.Next(10, 50);
            if (game.currentWave.waveNumber <= 4) {
                this.rewardForKilling = 10.0f;
            } else if (game.currentWave.waveNumber <= 6) {
                this.rewardForKilling = 5.0f;
            } else if (game.currentWave.waveNumber <= 8) {
                this.rewardForKilling = 2.0f;
            } else {
                this.rewardForKilling = 1;
            }
            hasPathToTower = false;
            currentState = STATE_IDLE;
            this.grid = grid;
            this.targetTile = grid.GetTile(tower.GetPosition());
            this.spawnTile = grid.GetTile(position);
            UpdatePath(grid.GetTile(tower.GetPosition()));

        }

        public virtual void Initiate() {
            
        }

        /// <summary>
        /// Enemy position will be updated based on enemies chasing the position of the castle
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime) {
            if (currentState == STATE_IDLE) {
                UpdatePath(tower.onTile);
                if (hasPathToTower) {
                    currentState = STATE_ATTACK_TOWER;
                } else {
                    currentState = STATE_ATTACK_WALL;
                }
            } else if (currentState == STATE_ATTACK_TOWER) {
                MoveOnPath(gameTime);
                if (isLowHealth()) {
                    currentState = STATE_RUN_AWAY;
                    RunAwayPathUpdate();
                }

            } else if (currentState == STATE_ATTACK_WALL) {
                if (isLowHealth()) {
                    currentState = STATE_RUN_AWAY;
                    RunAwayPathUpdate();
                }
                Debug.WriteLine("NO PATH TO TOWER");
            } else if (currentState == STATE_RUN_AWAY) {
                if (isMaxHealth()) {
                    currentState = STATE_IDLE;
                }
                if (grid.GetTile(position) == spawnTile) {
                    RegenerateHealth(gameTime);
                }
                MoveOnPath(gameTime);
            }
            
            //prevPosition = position;
            //position = Behavior.StraightLineChase(this.position, tower.GetPosition(), gameTime, this.speed);
            //rotation = BasicModel.RotateToFace(position, tower.GetPosition(), new Vector3(0, 0, 1));
            base.Update(gameTime);
        }

        private void MoveOnPath(GameTime gameTime) {
            Tile currentTile;
            if (path.Count() > 0) {
                currentTile = path.First.Value;
                prevPosition = position;
                position = Behavior.StraightLineChase(this.position, currentTile.globalPosition, gameTime, speed);
                rotation = BasicModel.RotateToFace(position, currentTile.globalPosition, Vector3.UnitX);
                if (currentTile.IsAtCenter(this.position)) {
                    currentTile.RemoveEnemyFromTile(this);
                    path.RemoveFirst();
                    if (path.Count != 0) {
                        path.First.Value.AddEnemyToTile(this);
                    }
                }
            }
        }

        /// <summary>
        /// Will use the astar behavior to create a path to the target
        /// </summary>
        /// <param name="targetGridDestination">The tile that the enemy is trying to get to</param>
        public void UpdatePath(Tile targetGridDestination) {
            path = Behavior.AStarPathFinding(grid.GetTile(position), targetGridDestination, grid);
            //If there is no path to the destination then call a handler function
            if (path == null) {
                hasPathToTower = false;
                RunAwayPathUpdate();
            } else {
                hasPathToTower = true;
            }
        }

        public float GetDamage() {
            return this.maxDamage;
        }

        public void DamageEnemy(float damage) {
            DamageObject(damage);
        }

        ///<summary>
        ///Return the speed of the enemy
        ///</summary>
        public float GetSpeed() {
            return this.speed;
        }

        /// <summary>
        /// Gets the current velocity vector
        /// </summary>
        /// <returns>The velocity of the tank</returns>
        public Vector3 GetVelocityVector() {
            return (position - prevPosition);
        }

        /// <summary>
        /// Determines whether the enemy is on low health
        /// </summary>
        /// <returns></returns>
        public bool isLowHealth() {
            if (currentHealth <= maxHealth/2) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Determines whether the enemy is on max health
        /// </summary>
        /// <returns></returns>
        public bool isMaxHealth() {
            if (currentHealth == maxHealth) {
                return true;
            } else {
                return false;
            }
        }

        public void RunAwayPathUpdate() {
            Tile currentTile = grid.GetTile(position);
            path = Behavior.AStarPathFinding(currentTile, spawnTile, grid);
        }

        public void SetSpawnTile(Tile spawnTile) {
            this.spawnTile = spawnTile;
        }

        /// <summary>
        /// Will be called when the enemy is eligible to regain health
        /// </summary>
        /// <param name="gameTime"></param>
        public void RegenerateHealth(GameTime gameTime) {
            currentHealth += gameTime.ElapsedGameTime.Milliseconds/5;
            if (currentHealth > maxHealth) {
                currentHealth = maxHealth;
            }
        }

    }
}
