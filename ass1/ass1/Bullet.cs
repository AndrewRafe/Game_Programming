using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ass1 {
    /// <summary>
    /// Bullet class to maintain the position and the characteristics of a bullet
    /// </summary>
    class Bullet : BasicModel {

        public int damage { get; private set; }

        private Vector3 directionOfTravel;

        float speed;
        Enemy targetEnemy;
        Tower tower; // new added

        public Bullet(Model m, Vector3 position, Enemy targetEnemy, Tower tower, GameTime gameTime) : base(m, position) {
            this.targetEnemy = targetEnemy;
            this.speed = 250.0f;
            damage = 100;
            this.tower = tower;
            CreateDirectionOfTravel(); // committed by sushmita
        }

        
        private void CreateDirectionOfTravel() {
            directionOfTravel = Vector3.Normalize(targetEnemy.GetPosition() - position);
        } 

        public override void Update(GameTime gameTime) {
            this.position += directionOfTravel * speed * gameTime.ElapsedGameTime.Milliseconds / 1000;
            base.Update(gameTime);
        }

        ///<summary>
        ///Return the estimated current position of the enemy
        ///</summary>
        private Vector3 EstimateCurrentPosition(GameTime gameTime)
        {
            Vector3 enemyCurrentPosition = targetEnemy.GetPosition();
            Vector3 enemyTargetPosition = tower.GetPosition();
            //float distance = Vector3.Distance(currentPosition, targetPosition);
            Vector3 direction = Vector3.Normalize(enemyTargetPosition - enemyCurrentPosition);
            Vector3 updatedPosition = direction * targetEnemy.GetSpeed() * gameTime.ElapsedGameTime.Milliseconds / 1000;
            return updatedPosition;
        }

        /*
        private void CreateDirectionOfTravel(GameTime gameTime)
        {
            directionOfTravel = Vector3.Normalize(EstimateCurrentPosition(gameTime) - position);
        } */
        

        /*
        public override void Update(GameTime gameTime)
        {
            //CreateDirectionOfTravel(gameTime);
            this.position += directionOfTravel * speed * gameTime.ElapsedGameTime.Milliseconds / 1000;
            base.Update(gameTime);
        } */
        
    }
}
