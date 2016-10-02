using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefence {
    /// <summary>
    /// Basic model manager interface
    /// </summary>
    public class ModelManager : DrawableGameComponent {

        /// <summary>
        /// Stores a list of all the models in the scene
        /// </summary>
        public List<BasicModel> models = new List<BasicModel>();

        public GraphicsDeviceManager graphics;

        /// <summary>
        /// Constructor method for the Model Manager
        /// Supplies the game to the superclass DrawableGameComponent
        /// </summary>
        /// <param name="game"></param>
        public ModelManager(Game game, GraphicsDeviceManager graphics) : base(game) {
            this.graphics = graphics;
        }

        /// <summary>
        /// Update method will call the update method for each of the models
        /// stored in the model manager
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime) {
            //Loop through all the models and call update
            for (int i = 0; i < models.Count; ++i) {
                models[i].Update(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Override for the load content of the DrawableGameComponent
        /// </summary>
        protected override void LoadContent() {
            
            base.LoadContent();
        }

        /// <summary>
        /// Will loop through and call the draw method for every single model 
        /// stored in the model manager
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime) {

            foreach(BasicModel model in models) {
                model.Draw(GetCamera(), graphics);
            }

            base.Draw(gameTime);
        }

        /// <summary>
        /// Helper method to retrieve the camera from the game class
        /// </summary>
        /// <returns></returns>
        protected Camera GetCamera() {
            return ((Game1)Game).camera;
        }

    }
}
