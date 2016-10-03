using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefence {
    public class Enemy : BasicGameObject {

        public static float MAX_HEALTH = 300.0f;
        public static float MAX_DAMAGE = 100.0f;

        private LinkedList<Tile> path;
        private Grid grid;

        public double rewardForKilling { get; protected set; }
        public Vector3 prevPosition { get; private set; }
        Tower tower;
        float speed;

        public Enemy(Model m, Vector3 position, float maxHealth, float maxDamage, Texture2D healthBarTexture, Tower tower, Game1 game, Grid grid) : base(m, position, maxHealth, maxDamage, healthBarTexture, game.spriteBatch) {
            this.tower = tower;
            this.maxHealth = maxHealth;
            this.maxDamage = maxDamage;
            this.speed = (float) game.rand.Next(50, 100);
            if (game.currentWave.waveNumber <= 4) {
                this.rewardForKilling = 10.0;
            } else if (game.currentWave.waveNumber <= 6) {
                this.rewardForKilling = 5.0;
            } else if (game.currentWave.waveNumber <= 8) {
                this.rewardForKilling = 2.0;
            } else {
                this.rewardForKilling = 1;
            }

            this.grid = grid;
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
                rotation = BasicModel.RotateToFace(position, currentTile.globalPosition, Vector3.UnitZ);
                if (currentTile.IsAtCenter(this.position)) {
                    path.RemoveFirst();
                }
            }
            //prevPosition = position;
            //position = Behavior.StraightLineChase(this.position, tower.GetPosition(), gameTime, this.speed);
            //rotation = BasicModel.RotateToFace(position, tower.GetPosition(), new Vector3(0, 0, 1));
            base.Update(gameTime);
        }

        public void UpdatePath(Tile targetGridDestination) {
            path = Behavior.AStarPathFinding(grid.GetTile(position), targetGridDestination, grid);
            
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
    }
}
