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
    /// A null healthBarTexture is passed to this class if you dont want the basic game object to 
    /// display its health status
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

            if (healthBarTexture != null) {
                healthBar = new HealthBar(healthBarTexture, spriteBatch);
            } else {
                healthBar = null;
            }
            
        }

        /// <summary>
        /// Update method for the basic game object that updates the basic model
        /// and also updates any health bar information associated to it.
        /// </summary>
        /// <param name="gameTime">A reference to the game time</param>
        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            //Update the position of the health bar on the object
            if (healthBar != null) {
                healthBar.Update(currentHealth / maxHealth);
            }
            
        }

        /// <summary>
        /// Draws the basic model and then draws the health bar in the correct position for 
        /// the object position
        /// </summary>
        /// <param name="camera">A reference to the main camera</param>
        /// <param name="graphics">A reference to the graphics device manager
        /// to allow for the drawing of the health bar to the screen</param>
        public override void Draw(Camera camera, GraphicsDeviceManager graphics) {
            base.Draw(camera, graphics);
            if (healthBar != null) {
                //Draw health bar
                Vector3 objectPosition = Game1.PickedPositionTranslation(position);
                //Object position z axis has to be flipped for health bar
                Vector3 objectScreenPosition = graphics.GraphicsDevice.Viewport.Project(new Vector3(objectPosition.X, objectPosition.Y, -objectPosition.Z), camera.projection, camera.view, world);
                healthBar.SetScreenPosition(new Vector2(objectScreenPosition.X - BAR_OFFSET_SIDE, objectScreenPosition.Y - BAR_OFFSET_TOP));
                healthBar.Draw(camera);
            }
        }

        /// <summary>
        /// Will make the object lose the given amount of health
        /// </summary>
        /// <param name="damage">The amount of damage that the object will take</param>
        public virtual void DamageObject(float damage) {
            this.currentHealth -= damage;
        }

        /// <summary>
        /// Will determine if this current game object has been destroyed/killed
        /// </summary>
        /// <returns>Whether the object is destroyed or not</returns>
        public bool IsDead() {
            if (this.currentHealth <= 0) {
                return true;
            } else {
                return false;
            }
        }

    }
}
