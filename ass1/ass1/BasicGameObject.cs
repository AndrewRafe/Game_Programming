using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefence {
    /// <summary>
    /// A class that stores basic information about a game object that has health and damage
    /// attributes
    /// </summary>
    public class BasicGameObject : BasicModel {

        private static int BAR_OFFSET_TOP = 30;
        private static int BAR_OFFSET_SIDE = 30;

        public float maxHealth { get; protected set; }
        public float currentHealth { get; protected set; }
        public float maxDamage { get; protected set; }

        private HealthBar healthBar;

        public BasicGameObject(Model m, Vector3 position, float maxHealth, float maxDamage, Texture2D healthBarTexture, SpriteBatch spriteBatch) : base(m, position) {
            this.maxHealth = maxHealth;
            this.currentHealth = maxHealth;
            this.maxDamage = maxDamage;

            healthBar = new HealthBar(healthBarTexture, spriteBatch);
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            //Update the position of the health bar on the object
            healthBar.Update(currentHealth / maxHealth);
            
        }

        public override void Draw(Camera camera, GraphicsDeviceManager graphics) {
            base.Draw(camera, graphics);
            //Draw health bar
            Vector3 objectScreenPosition = graphics.GraphicsDevice.Viewport.Project(Game1.CorrectedVector(position), camera.projection, camera.view, world);
            healthBar.SetScreenPosition(new Vector2(objectScreenPosition.X - BAR_OFFSET_SIDE, objectScreenPosition.Y - BAR_OFFSET_TOP));
            healthBar.Draw(camera);
        }

        public void DamageObject(float damage) {
            this.currentHealth -= damage;
        }

        public bool IsDead() {
            if (this.currentHealth <= 0) {
                return true;
            } else {
                return false;
            }
        }

    }
}
