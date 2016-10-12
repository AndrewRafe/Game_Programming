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

        public const String STATE_ATTACK_TOWER = "ATTACK_TOWER";
        public const String STATE_IDLE = "IDLE";

        private LinkedList<Tile> path;
        private Grid grid;

        public float rewardForKilling { get; protected set; }
        public Vector3 prevPosition { get; private set; }
        public Tile targetTile { get; private set; }
        Tower tower;
        float speed;

        public Enemy(Model m, Vector3 position, float maxHealth, float maxDamage, Texture2D healthBarTexture, Tower tower, Game1 game, Grid grid) : base(m, position, maxHealth, maxDamage, healthBarTexture, game.spriteBatch) {
            this.tower = tower;
            this.maxHealth = maxHealth;
            this.maxDamage = maxDamage;
            this.speed = (float) game.rand.Next(20, 50);
            if (game.currentWave.waveNumber <= 4) {
                this.rewardForKilling = 10.0f;
            } else if (game.currentWave.waveNumber <= 6) {
                this.rewardForKilling = 5.0f;
            } else if (game.currentWave.waveNumber <= 8) {
                this.rewardForKilling = 2.0f;
            } else {
                this.rewardForKilling = 1;
            }

            this.grid = grid;
            this.targetTile = grid.GetTile(tower.GetPosition());
            UpdatePath(grid.GetTile(tower.GetPosition()));

        }

        public virtual void Initiate() {
            
        }

        /// <summary>
        /// Enemy position will be updated based on enemies chasing the position of the castle
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime) {
            Tile currentTile;
            if (path.Count() > 0) {
                currentTile = path.First.Value;
                prevPosition = position;
                position = Behavior.StraightLineChase(this.position, currentTile.globalPosition, gameTime, speed);
                rotation = BasicModel.RotateToFace(position, currentTile.globalPosition, Vector3.UnitX);
                if (currentTile.IsAtCenter(this.position)) {
                    currentTile.RemoveEnemyFromTile(this);
                    path.RemoveFirst();
                    path.First.Value.AddEnemyToTile(this);
                }
            }
            //prevPosition = position;
            //position = Behavior.StraightLineChase(this.position, tower.GetPosition(), gameTime, this.speed);
            //rotation = BasicModel.RotateToFace(position, tower.GetPosition(), new Vector3(0, 0, 1));
            base.Update(gameTime);
        }

        /// <summary>
        /// Will use the astar behavior to create a path to the target
        /// </summary>
        /// <param name="targetGridDestination">The tile that the enemy is trying to get to</param>
        public void UpdatePath(Tile targetGridDestination) {
            path = Behavior.AStarPathFinding(grid.GetTile(position), targetGridDestination, grid);
            //If there is no path to the destination then call a handler function
            if (path == null) {
                //NoValidPath();
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
            return position - prevPosition;
        }

        /// <summary>
        /// Handles the event that the enemy has no valid path to its destination
        /// The enemy will randomly target walls until a path exists
        /// TODO: FIX
        /// </summary>
        /*private void NoValidPath() {
            //If it is stuck on a tile with a wall then damage the wall
            Building buildingOnCurrentTile = grid.GetTile(position).AddModelToTile(;
            Debug.WriteLine(position.ToString() + " AND GRID AT " + grid.GetTile(position).ToString());
            if (buildingOnCurrentTile != null) {
                buildingOnCurrentTile.DamageObject(this.maxDamage);
                this.DamageObject(buildingOnCurrentTile.maxDamage);
            }
            Random rand = new Random();
            while (path != null) {
                path = Behavior.AStarPathFinding(grid.GetTile(position), grid.obstacleTiles.ElementAt(rand.Next(0,grid.obstacleTiles.Count - 1)), grid);
            }
            
        }*/
    }
}
