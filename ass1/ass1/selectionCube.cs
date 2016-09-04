using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ass1 {
    /// <summary>
    /// Class to show the player where they are about to build
    /// on the map. Inherits from the BasicModel class
    /// </summary>
    public class SelectionCube : BasicModel {

        /// <summary>
        /// Constructor method for the selection cube sub class
        /// </summary>
        /// <param name="m"></param>
        /// <param name="position"></param>
        public SelectionCube(Model m, Vector3 position) : base(m,position) {

        }

        /// <summary>
        /// Updates the position attribute of the selection cube
        /// </summary>
        /// <param name="newPosition"></param>
        public void ChangeSelectionPosition(Vector3 newPosition) {
            position = newPosition;
            
        }

        /// <summary>
        /// Will return the world matrix of the model
        /// </summary>
        /// <returns>worldMatrix</returns>
        public override Matrix GetWorldMatrix() {
            Matrix world;
            world = base.GetWorldMatrix();
            
            return world;
        }

    }
}
