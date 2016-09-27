using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace ass1 {
    /// <summary>
    /// The basic class for a model
    /// Contains methods to draw and load in a model
    /// </summary>
    public class BasicModel {

        public Model model { get; protected set; }
        protected Matrix world = Matrix.Identity;
        protected Vector3 position;

        public Quaternion rotation;

        /// <summary>
        /// Constructor method for the basic model class that takes a model
        /// </summary>
        /// <param name="m"></param>
        public BasicModel(Model m, Vector3 position) {
            model = m;
            this.position = position;
            this.rotation = new Quaternion();
        }

        /// <summary>
        /// Update method to be overriden by classes that derive BasicModel
        /// </summary>
        public virtual void Update(GameTime gameTime) {

        }

        /// <summary>
        /// Basic draw method for a generic model object
        /// </summary>
        /// <param name="camera"></param>
        public virtual void Draw(Camera camera) {

            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            Matrix worldMatrix = GetWorldMatrix();

            foreach (ModelMesh mesh in model.Meshes) {
                foreach (BasicEffect effect in mesh.Effects) {
                    effect.EnableDefaultLighting();
                    effect.Projection = camera.projection;
                    effect.View = camera.view;
                    effect.World = worldMatrix * mesh.ParentBone.Transform;
                }
                mesh.Draw();
            }
        }

        /// <summary>
        /// Method to retrieve the world matrix for this object
        /// Method can be overriden by classes that derive the Basic Model Class
        /// </summary>
        /// <returns>worldMatrix</returns>
        public virtual Matrix GetWorldMatrix() {
            Matrix world;
            world = Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position);
            return world;
        }

        /// <summary>
        /// Determines whether or not this object is colliding with the given model and that models world matrix
        /// </summary>
        /// <param name="otherModel"></param>
        /// <param name="otherWorld"></param>
        /// <returns></returns>
        public bool CollidesWith(Model otherModel, Matrix otherWorld) {
            //Loop through each model meash in both objects and compare all the bounding spheres
            //for collisions
            foreach (ModelMesh thisModelMeshes in model.Meshes) {
                foreach(ModelMesh otherModelMeshes in otherModel.Meshes) {
                    if (thisModelMeshes.BoundingSphere.Transform(GetWorldMatrix()).Intersects(otherModelMeshes.BoundingSphere.Transform(otherWorld))) {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Creates a quaternion to face the direction of another position in the world
        /// Is a static method so both the objects position and the target position must be provided
        /// </summary>
        /// <param name="objectPosition">The current position of the model</param>
        /// <param name="targetPosition">The position that the object would like to face</param>
        /// <param name="up">The up vector of the object that you are hoping to rotate</param>
        /// <returns>A Quaternion of the direction that is needed to face the position</returns>
        public static Quaternion RotateToFace(Vector3 objectPosition, Vector3 targetPosition, Vector3 up) {
            Vector3 direction = (objectPosition - targetPosition);
            Vector3 relativeRight = Vector3.Cross(up, direction);
            Vector3.Normalize(ref relativeRight, out relativeRight);
            Vector3 relativeBackwards = Vector3.Cross(relativeRight, up);
            Vector3.Normalize(ref relativeBackwards, out relativeBackwards);
            Vector3 newUp = Vector3.Cross(relativeBackwards, relativeRight);
            Matrix rot = new Matrix(relativeRight.X, relativeRight.Y, relativeRight.Z, 0, newUp.X, newUp.Y, newUp.Z, 0, relativeBackwards.X, relativeBackwards.Y, relativeBackwards.Z, 0, 0, 0, 0, 1);
            Quaternion rotation = Quaternion.CreateFromRotationMatrix(rot);
            return rotation;
        }

        /// <summary>
        /// Getter method to return the position of the current model
        /// </summary>
        /// <returns>The position of the current model</returns>
        public Vector3 GetPosition() {
            return this.position;
        }

    }
}
