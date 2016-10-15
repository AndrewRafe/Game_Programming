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
    public class Turret : Building {

        public static int COST = 100;
        public static float DEFAULT_DAMAGE = 50.0f;
        public static float DEFAULT_HEALTH = 100.0f;

        //Number of shots per second
        protected float fireRate;
        protected Model bullet;
        public float range;

        //How many miliseconds since last fire
        private int lastFired;

        public bool isReadyToFire { get; private set; }

        /// <summary>
        /// Constructor method that passes the turret model and the position to the
        /// parent BasicModel class. Also takes in a bullet model that this turret
        /// will fire.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="position"></param>
        /// <param name="bullet"></param>
        public Turret(Model m, Vector3 position, Model bullet, WorldModelManager worldModelManager, float range, float maxHealth, float maxDamage, Texture2D healthBarTexture, SpriteBatch spriteBatch, Tile builtOnTile) : base(m, position, maxHealth, maxDamage, healthBarTexture, spriteBatch, builtOnTile) {
            this.bullet = bullet;
            lastFired = 0;
            this.range = range;
            FaceEnemy(null);
            //Debug.WriteLine("Turret created at X: " + position.X + " Y: " + position.Y + " Z: " + position.Z);
            Initiate();
        }

        /// <summary>
        /// A method that must be implemented that initiates the stats of the turret
        /// and also the strings for the description and the name
        /// </summary>
        protected virtual void Initiate() {
            fireRate = 1.5f;
        }

        public override void Draw(Camera camera, GraphicsDeviceManager graphics) {
            base.Draw(camera, graphics);
        }

        public override void Update(GameTime gameTime) {
         

            if (lastFired >= fireRate * 1000.0f) {
                isReadyToFire = true;
                lastFired = (int) fireRate * 1000;
                
            } else {
                isReadyToFire = false;
                lastFired += gameTime.ElapsedGameTime.Milliseconds;
            }

            base.Update(gameTime);
        }

        public override Matrix GetWorldMatrix() {
            world = base.GetWorldMatrix() ;
            return world;
        }

        public Bullet FireTurret(Enemy enemy, GameTime gameTime, Grid grid) {
            if (isReadyToFire) {
                lastFired = 0;
                isReadyToFire = false;
                return new Bullet(bullet, position, enemy, DEFAULT_DAMAGE, gameTime, grid);
            } else {
                return null;
            }
        }

        public void FaceEnemy(Enemy enemy) {
            if (enemy != null) {
                rotation = RotateToFace(position, enemy.GetPosition(), new Vector3(0, 0, 1));
            } else {
                rotation = RotateToFace(position, new Vector3(0, -Game1.WORLD_BOUNDS_HEIGHT, 0), new Vector3(0,0,1));
            }
}

    }
}
