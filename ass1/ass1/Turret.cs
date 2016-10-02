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
    /// A base class for a turret
    /// A turret inherits from a basic model class
    /// </summary>
    class Turret : BasicModel {

        public static int COST = 100;

        public ModelManager bullets;

        public int health { get; protected set; }

        //Number of shots per second
        protected float fireRate;
        protected Model bullet;
        protected String name;
        protected String description;

        private WorldModelManager worldModelManager;

        //How many miliseconds since last fire
        private int lastFired;

        /// <summary>
        /// Constructor method that passes the turret model and the position to the
        /// parent BasicModel class. Also takes in a bullet model that this turret
        /// will fire.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="position"></param>
        /// <param name="bullet"></param>
        public Turret(Model m, Vector3 position, Model bullet, WorldModelManager worldModelManager) : base(m, position) {
            this.bullet = bullet;
            this.worldModelManager = worldModelManager;
            bullets = new ModelManager(worldModelManager.Game, worldModelManager.graphics);
            lastFired = 0;
            Enemy closestEnemy = worldModelManager.GetClosestEnemy(position);
            if (closestEnemy != null) {
                rotation = RotateToFace(position, worldModelManager.GetClosestEnemy(position).GetPosition(), new Vector3(0, 0, 1));
            } else {
                rotation = RotateToFace(position, new Vector3(0, -Game1.WORLD_BOUNDS_HEIGHT, 0), new Vector3(0,0,1));
            }
            
            //Debug.WriteLine("Turret created at X: " + position.X + " Y: " + position.Y + " Z: " + position.Z);
            Initiate();
        }

        /// <summary>
        /// A method that must be implemented that initiates the stats of the turret
        /// and also the strings for the description and the name
        /// </summary>
        protected virtual void Initiate() {
            health = 10;
            fireRate = 1.5f;
            name = "Basic Turret";
        }

        /// <summary>
        /// Determines the positioning of all the bullets related to this turret
        /// and spawns new bullets depending on its fire rate
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime) {
            
            foreach(Bullet bullet in bullets.models) {
                bullet.Update(gameTime);
            }

            if (!(worldModelManager.enemies.models.Count <= 0)) {
                rotation = BasicModel.RotateToFace(position, worldModelManager.GetClosestEnemy(position).GetPosition(),
                        new Vector3(0, 0, 1));
            }
            

            if (lastFired > fireRate * 1000.0f && worldModelManager.GetClosestEnemy(position) != null) {
                if (worldModelManager.enemies.models.Count <= 0) {
                } else {
                    bullets.models.Add(new Bullet(bullet, this.position, worldModelManager.GetClosestEnemy(position), worldModelManager.tower, gameTime));
                    worldModelManager.CannonFire();
                    lastFired = 0;
                }
                
            } else {
                lastFired += gameTime.ElapsedGameTime.Milliseconds;
            }

            base.Update(gameTime);
        }

        public void DamageTurret(int damage) {
            health -= damage;
        }

        public override Matrix GetWorldMatrix() {
            world = base.GetWorldMatrix() ;
            return world;
        }

    }
}
