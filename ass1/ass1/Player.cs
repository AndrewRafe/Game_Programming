using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefence {
    class Player {

        public static double STARTING_MONEY = 300.0;

        public double money;
        Game1 game;

        /// <summary>
        /// Constructor method for the player sets up the starting money and 
        /// stores a reference to the game
        /// </summary>
        /// <param name="game"></param>
        public Player(Game1 game) {
            money = STARTING_MONEY;
            this.game = game;
        }

        /// <summary>
        /// Will return true if the player has more money than the cost
        /// and false if they do not have enough money
        /// </summary>
        /// <param name="cost"></param>
        /// <returns></returns>
        public bool HasSuffucientMoney(int cost) {
            if (money - cost >= 0) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Deducts a given amount of money from the players current money
        /// </summary>
        /// <param name="cost"></param>
        public void SpendMoney(double cost) {
            money -= cost;
        }

        /// <summary>
        /// Will give the player the given amount of money
        /// </summary>
        /// <param name="reward"></param>
        public void GiveMoney(double reward) {
            money += reward;
        }

        /// <summary>
        /// Returns a message for the player informing them that they have insufficient
        /// money to make a purchase
        /// </summary>
        /// <returns></returns>
        private String InsufficientMoneyMessage() {
            return "You have insufficient money for your purchase";
        }

        /// <summary>
        /// Draws the amount of money that the player has to the screen
        /// </summary>
        /// <param name="spriteBatch">The Game SpriteBatch</param>
        /// <param name="font">The SpriteFont that the will be used for the message</param>
        public void DrawText(SpriteBatch spriteBatch, SpriteFont font) {
            spriteBatch.DrawString(font, "Money : $" + money, new Vector2(20, 20), Game1.TEXT_COLOR);
        }

    }
}
