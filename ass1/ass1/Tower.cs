using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefence
{
    public class Tower : Building 
    {

        public static float DEFAULT_TOWER_HEALTH = 1000.0f;
        public static float DEFAULT_DAMAGE = 1000.0f;

        Game1 game;

        /// <summary>
        /// Constructor method that passes the tower model and the position to the
        /// parent BasicModel class.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="position"></param>

        public Tower(Model m, Vector3 position, Game1 game, float maxHealth, float maxDamage, Texture2D healthBarTexture, SpriteBatch spriteBatch, Tile builtOnTile) : base(m, position, maxHealth, maxDamage, healthBarTexture, spriteBatch, builtOnTile) {
            //Debug.WriteLine("Turret created at X: " + position.X + " Y: " + position.Y + " Z: " + position.Z);
            builtOnTile.AddTowerToTile(this);
            this.game = game;
            Initiate();
        }

        /// <summary>
        /// A method that must be implemented that initiates the stats of the tower
        /// and also the strings for the description and the name
        /// </summary>
        protected virtual void Initiate()
        {

        }

        /// <summary>
        /// Will return the world matrix of the model
        /// </summary>
        /// <returns>worldMatrix</returns>
        public override Matrix GetWorldMatrix()
        {
            Matrix world;
            world = base.GetWorldMatrix();
            return world;
        }

        /// <summary>
        /// Draws the tower health text to the screen
        /// </summary>
        /// <param name="spriteBatch">A reference to the sprite batch from the game</param>
        /// <param name="font">The SpriteFont that will be used for the text</param>
        public void DrawText(SpriteBatch spriteBatch, SpriteFont font) {
            String text = "Tower Health: " + currentHealth;
            Vector2 textCenter = font.MeasureString(text)/2;
            spriteBatch.DrawString(font, text, new Vector2(game.SCREEN_WIDTH/2, 40), Game1.TEXT_COLOR, 0, textCenter, 1.0f, SpriteEffects.None, 0.5f);
        }

        public override void DamageObject(float damage) {
            base.DamageObject(damage);
            game.TowerTakesDamage();
            if (currentHealth <= (maxHealth*0.2)) {
                game.TowerDangerHealth();
            }
            
        }
    }
}
