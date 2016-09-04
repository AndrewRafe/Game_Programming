using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ass1 {
    /// <summary>
    /// The ground plane of the world
    /// Inherits from the basic model class
    /// </summary>
    class Ground : BasicModel {


        //A plane that specifies the level that the ground plane is on
        public static Plane groundPlane;

        /// <summary>
        /// Constructor method for the ground. Takes a model and a center position
        /// for the ground plane
        /// </summary>
        /// <param name="m"></param>
        /// <param name="position"></param>
        public Ground(Model m, Vector3 position) : base(m, position){
            groundPlane = new Plane(Vector3.UnitY, position.Y);
        }

        /// <summary>
        /// Overrides the getter method for the world matrix of the ground
        /// </summary>
        /// <returns></returns>
        public override Matrix GetWorldMatrix() {
            Matrix world;
            world = base.GetWorldMatrix();
            return world;
        }
    }
}
