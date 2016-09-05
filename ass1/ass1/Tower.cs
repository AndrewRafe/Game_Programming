using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ass1
{
    public class Tower : BasicModel 
    {
        protected int health;
        protected String name;
        protected String description;

        Game1 game;

        /// <summary>
        /// Constructor method that passes the tower model and the position to the
        /// parent BasicModel class.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="position"></param>

        public Tower(Model m, Vector3 position, Game1 game) : base(m, position) {
            //Debug.WriteLine("Turret created at X: " + position.X + " Y: " + position.Y + " Z: " + position.Z);
            this.game = game;
            Initiate();
        }

        /// <summary>
        /// A method that must be implemented that initiates the stats of the tower
        /// and also the strings for the description and the name
        /// </summary>
        protected virtual void Initiate()
        {
            name = "Main Tower";
            description = "The Main Tower - USED FOR TESTING";
            health = 100;
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
        /// Will return the amount of damage of the tower
        /// </summary>
        public int GetTowerHealth()
        {
            return health;
        }

        /// <summary>
        /// Will increase the amount of the tower
        /// </summary>
        public void DamageTower(int damage)
        {

            game.TowerTakesDamage();

            if (health - damage <= 0 ) {
                health = 0;
                TowerDestroyed();
            } else {
                health -= damage;
            }

            if (health <= 20) {
                game.TowerDangerHealth();
            }
        }

        /// <summary>
        /// Draws the tower health text to the screen
        /// </summary>
        /// <param name="spriteBatch">A reference to the sprite batch from the game</param>
        /// <param name="font">The SpriteFont that will be used for the text</param>
        public void DrawText(SpriteBatch spriteBatch, SpriteFont font) {
            String text = "Tower Health: " + this.health;
            Vector2 textCenter = font.MeasureString(text)/2;
            spriteBatch.DrawString(font, text, new Vector2(game.SCREEN_WIDTH/2, 40), Game1.TEXT_COLOR, 0, textCenter, 1.0f, SpriteEffects.None, 0.5f);
        }

        /// <summary>
        /// Lets the game know when the tower has been destroyed
        /// </summary>
        public void TowerDestroyed() {
            game.GameOver();
        }
    }
}
