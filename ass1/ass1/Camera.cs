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
    /// An implementation of a basic camera for the game
    /// </summary>
    public class Camera : GameComponent {

        /// <summary>
        /// Constants controlling limitations on camera movement
        /// </summary>
        private static float SCROLL_SPEED = 10.0f;
        private static float MAX_ZOOM = 600.0f;
        private static float MIN_ZOOM = 200.0f;

        /// <summary>
        /// View and Projection matrices for the camera
        /// </summary>
        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }

        //Camera Vectors:
        public Vector3 cameraPosition { get; protected set; }
        Vector3 cameraDirection;
        Vector3 cameraUp;

        //Camera movement properties
        float speed;
        float zoomInterval;

        private int nearPlane;
        private int farPlane;

        //Input Variables
        MouseState prevMouseState;

        /// <summary>
        /// Constructor class for the camera
        /// </summary>
        /// <param name="game">A reference to the game</param>
        /// <param name="pos">The position of the camera</param>
        /// <param name="target">The target that the camera will face</param>
        /// <param name="up">The cameras up vector</param>
        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up) : base(game) {
            //Build the camera view matrix
            cameraPosition = pos;
            cameraDirection = target - pos;

            //Normalize method takes any vector and converts it to a vector with a magnitude of 1
            //Makes it easier to apply speed values when moving the camera in the direction it is facing
            cameraDirection.Normalize();
            cameraUp = up;
            CreateLookAt();

            speed = SCROLL_SPEED;
            zoomInterval = 100.0f;
            //45 degree rotation limit

            nearPlane = 1;
            farPlane = 3000;

            try {
                projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)Game.Window.ClientBounds.Width /
                (float)Game.Window.ClientBounds.Height, nearPlane, farPlane);
            } catch (NullReferenceException) {
                Debug.WriteLine("Projection null reference");
            }
            
        }

        /// <summary>
        /// Initialise the state of the camera as well as set the mouse position
        /// to the middle of the screen on launch
        /// </summary>
        public override void Initialize() {

            //Set mouse position and get the initial state
            Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);
            prevMouseState = Mouse.GetState();

            base.Initialize();
        }

        /// <summary>
        /// Method to create the lookat matrix based on the camera position, direction and up vectors
        /// </summary>
        public void CreateLookAt() {
            view = Matrix.CreateLookAt(cameraPosition, cameraPosition + cameraDirection, cameraUp);
        }

        
        /// <summary>
        /// Allows for the controlling of the camera
        /// </summary>
        /// <param name="gameTime">A reference to the game time</param>
        public override void Update(GameTime gameTime) {

            //Move forward and back
            if (Keyboard.GetState().IsKeyDown(Keys.W)) {
                cameraPosition += (cameraDirection * speed) * (new Vector3(1, 0, 1));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S)) {
                cameraPosition -= (cameraDirection * speed) * (new Vector3(1, 0, 1));
            }

            //Strafe
            if (Keyboard.GetState().IsKeyDown(Keys.A)) {
                cameraPosition += Vector3.Cross(cameraUp, cameraDirection) * speed * (new Vector3(1, 0, 1));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D)) {
                cameraPosition -= Vector3.Cross(cameraUp, cameraDirection) * speed * (new Vector3(1, 0, 1));
            }


            //ZOOMING IN AND OUT
            if (Mouse.GetState().ScrollWheelValue < prevMouseState.ScrollWheelValue) {
                if (cameraPosition.Y <= MAX_ZOOM) {
                    cameraPosition = new Vector3(cameraPosition.X, cameraPosition.Y + zoomInterval, cameraPosition.Z);
                }
            } else if (Mouse.GetState().ScrollWheelValue > prevMouseState.ScrollWheelValue) {
                if (cameraPosition.Y >= MIN_ZOOM) {
                    cameraPosition = new Vector3(cameraPosition.X, cameraPosition.Y - zoomInterval, cameraPosition.Z);
                }
            }

            prevMouseState = Mouse.GetState();

            CreateLookAt();

            base.Update(gameTime);
        }


    }
}
