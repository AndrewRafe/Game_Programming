using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ass1 {
    public class Enemy : BasicModel {

        public int health { get; protected set; }
        public double rewardForKilling { get; protected set; }
        Tower tower;
        float speed;
        int damage;

        public Enemy(Model m, Vector3 position, Tower tower, Game1 game) : base(m, position) {
            this.tower = tower;
            this.health = 300;
            this.speed = (float) game.rand.Next(50, 100);
            if (game.waveNumber <= 4) {
                this.rewardForKilling = 5.0;
            } else if (game.waveNumber <= 6) {
                this.rewardForKilling = 2.0;
            } else if (game.waveNumber <= 8) {
                this.rewardForKilling = 1.0;
            } else {
                this.rewardForKilling = 0.5;
            }
            this.damage = 10;
        }

        public virtual void Initiate() {
            
        }

        /// <summary>
        /// Enemy position will be updated based on enemies chasing the position of the castle
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime) {
            position = Behavior.StraightLineChase(this.position, tower.GetPosition(), gameTime, this.speed);
            rotation = BasicModel.RotateToFace(position, tower.GetPosition(), new Vector3(0, 0, 1));
            base.Update(gameTime);
        }

        public int GetDamage() {
            return this.damage;
        }

        public void DamageEnemy(int damage) {
            health -= damage;
        }

        ///<summary>
        ///Return the speed of the enemy
        ///</summary>
        public float GetSpeed() {
            return this.speed;
        }
    }
}
