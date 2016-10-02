using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefence {
    class HealthBar {

        private static int MAX_WIDTH = 60;
        private static int MAX_HEIGHT = 5;

        public SpriteBatch spriteBatch { get; private set; }
        public Texture2D barTexture { get; private set; }

        public Vector2 screenPosition { get; private set; }

        public Rectangle bar;

        public HealthBar(Texture2D barTexture, SpriteBatch spriteBatch) {
            this.barTexture = barTexture;
            this.spriteBatch = spriteBatch;
            bar = new Rectangle(0, 0, MAX_WIDTH, MAX_HEIGHT);
        }

        public void Draw(Camera camera) {
            spriteBatch.Begin();
            spriteBatch.Draw(barTexture, bar, Color.White);
            spriteBatch.End();
        }

        public void Update(float healthPercentage) {
            bar.Width = (int) (healthPercentage * MAX_WIDTH);
        }

        /// <summary>
        /// Will change the screen position to the given vector
        /// </summary>
        /// <param name="newScreenPosition">The updated screen position of the health bar</param>
        public void SetScreenPosition(Vector2 newScreenPosition) {
            screenPosition = newScreenPosition;
            bar.X = (int) screenPosition.X;
            bar.Y = (int) screenPosition.Y;
        }

    }
}
