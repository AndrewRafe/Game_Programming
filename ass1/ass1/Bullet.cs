using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefence {
    /// <summary>
    /// Bullet class to maintain the position and the characteristics of a bullet
    /// </summary>
    public class Bullet : BasicModel {

        public float damage { get; private set; }

        private Vector3 directionOfTravel;

        Tile onTile;

        public float speed { get; private set; }
        public Enemy targetEnemy { get; private set; }

        /// <summary>
        /// Constructor method for the bullet class
        /// Takes the regular basic model parameters and sets the bullets course towards an enemy
        /// </summary>
        /// <param name="m">Bullet Model</param>
        /// <param name="position">Starting position of the bullet</param>
        /// <param name="targetEnemy">The enemy that the bullet is directed at</param>
        /// <param name="tower">The tower needed to be protected</param>
        /// <param name="gameTime">A reference to the game time</param>
        public Bullet(Model m, Vector3 position, Enemy targetEnemy, float damage, GameTime gameTime, Grid grid) : base(m, position) {
            Debug.WriteLine("BULLET CREATED");
            this.targetEnemy = targetEnemy;
            this.speed = 150.0f;
            this.damage = damage;
            onTile = grid.GetTile(position);
            onTile.AddBulletToTile(this);
            CreateDirectionOfTravel(gameTime);
        }
        
        /// <summary>
        /// Determines the direction that the bullet is going to travel
        /// </summary>
        private void CreateDirectionOfTravel(GameTime gameTime) {
            //prediction accuracy is based off the distance between the start position and the enemy
            //The closer the enemy the lower the prediction accuracy must be
            directionOfTravel = Vector3.Normalize(Behavior.PredictTargetPosition(targetEnemy.GetPosition(), targetEnemy.GetVelocityVector(), gameTime, CalculatePredictionAccuracy()) - position);
        } 

        /// <summary>
        /// The algorithm used to calculate the accuracy variable for the prediction of the moving enemy
        /// </summary>
        /// <returns></returns>
        private int CalculatePredictionAccuracy() {
            return (int)Vector3.Distance(position, targetEnemy.GetPosition())/2;
        }

        /// <summary>
        /// Updates the position of the bullet based on its current trajectory
        /// </summary>
        /// <param name="gameTime">A reference to the game time</param>
        public override void Update(GameTime gameTime) {
            this.position += directionOfTravel * speed * gameTime.ElapsedGameTime.Milliseconds / 1000;
            base.Update(gameTime);
        }

        ///<summary>
        ///Estimates the enemy position based on its current trajectory
        ///TODO: Currently does not work as intended: NEEDS FIX
        ///</summary>
        private Vector3 EstimateCurrentPosition(GameTime gameTime)
        {
            Vector3 enemyCurrentPosition = targetEnemy.GetPosition();
            Vector3 enemyTargetPosition = targetEnemy.targetTile.globalPosition;
            //float distance = Vector3.Distance(currentPosition, targetPosition);
            Vector3 direction = Vector3.Normalize(enemyTargetPosition - enemyCurrentPosition);
            Vector3 updatedPosition = direction * targetEnemy.GetSpeed() * gameTime.ElapsedGameTime.Milliseconds / 1000;
            return updatedPosition;
        }

        public Tile getCurrentTile(Grid grid) {
            Tile tile = grid.GetTile(position);
            onTile = tile;
            return tile;
        }
        
    }
}
