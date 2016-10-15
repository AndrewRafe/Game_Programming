using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefence {
    /// <summary>
    /// A variety of static behaviors to be used by non controllable characters
    /// </summary>
    class Behavior {

        /// <summary>
        /// Static method that takes a current position and returns its new position based on 
        /// a simple chase mechanic. A speed of the agent and a reference to the gametime ensures
        /// smooth movement from frame to frame
        /// </summary>
        /// <param name="currentPosition">The current position of the agent</param>
        /// <param name="targetPosition">The position that the agent is chasing</param>
        /// <param name="gameTime">A reference to the current game time</param>
        /// <param name="speed">The speed that the agent is moving</param>
        /// <returns>newPosition</returns>
        public static Vector3 ChaseLocation(Vector3 currentPosition, Vector3 targetPosition, GameTime gameTime, float speed) {

            Vector3 newPosition = new Vector3(currentPosition.X, currentPosition.Y, currentPosition.Z);

            //Y position remains unchanged
            newPosition.Y = currentPosition.Y;

            if (targetPosition.X < currentPosition.X) {
                newPosition.X = currentPosition.X - speed * gameTime.ElapsedGameTime.Milliseconds/1000;
            } else if (targetPosition.X > currentPosition.X) {
                newPosition.X = currentPosition.X + speed * gameTime.ElapsedGameTime.Milliseconds/1000;
            } else {
                //Is in the correct X position therefore DO NOTHING
            }

            if (targetPosition.Y < currentPosition.Y) {
                newPosition.Y = currentPosition.Y - speed * gameTime.ElapsedGameTime.Milliseconds/1000;
            } else if (targetPosition.Y > currentPosition.Y) {
                newPosition.Y = currentPosition.Y + speed * gameTime.ElapsedGameTime.Milliseconds/1000;
            } else {
                //Is in the correct Z position therefore DO NOTHING
            }

            return newPosition;
        }

        /// <summary>
        /// A static method that takes a current position and returns a new position based on the
        /// target destination. This particular method will find the next spot in a straight line
        /// path to the destination
        /// </summary>
        /// <param name="currentPosition">The current position of the agent</param>
        /// <param name="targetPosition">The target position for the agent to head towards</param>
        /// <param name="gameTime">The current game time</param>
        /// <param name="speed">The speed that the agent will move at</param>
        /// <returns></returns>
        public static Vector3 StraightLineChase(Vector3 currentPosition, Vector3 targetPosition, GameTime gameTime, float speed) {

            Vector3 directionOfTravel = Vector3.Normalize(targetPosition - currentPosition);
            Vector3 newPosition = currentPosition + directionOfTravel * speed * gameTime.ElapsedGameTime.Milliseconds / 1000;

            return newPosition;
        }

        /// <summary>
        /// Predict the position that the given target will be
        /// </summary>
        /// <param name="targetPosition">The position of the target</param>
        /// <param name="targetVelocityVector">The velocity of the target</param>
        /// <param name="gameTime">A reference to the game time</param>
        /// <param name="predictionAccuracy">A prediction accuracy variable</param>
        /// <returns>The position that the target will be</returns>
        public static Vector3 PredictTargetPosition(Vector3 targetPosition, Vector3 targetVelocityVector, GameTime gameTime, float predictionAccuracy) {
            //Predict where the target will be according to its current velocity and prediction accuracy
            Vector3 predictionPosition = targetPosition + targetVelocityVector * predictionAccuracy;
            return predictionPosition;
        }

        //TODO: ASTAR PATHFINDING IMPLEMENTATION
        public static LinkedList<Tile> AStarPathFinding(Tile currentTile, Tile destinationTile, Grid grid) {
            grid.ResetTileCosts();
            LinkedList<Tile> closedSet = new LinkedList<Tile>();
            LinkedList<Tile> openSet = new LinkedList<Tile>();
            float tentativeGScore = 0;
            openSet.AddLast(currentTile);
            currentTile.gScore = 0;
            currentTile.fScore = Heuristic(currentTile, destinationTile);
            LinkedList<Tile> prevTiles = new LinkedList<Tile>();
            Tile current;
            while (openSet.Count != 0) {
                current = openSet.First.Value;
                if (!current.isWalkable) {
                    openSet.Remove(current);
                    continue;
                }
                foreach (Tile tile in openSet) {
                    if (tile.fScore < current.fScore) {
                        current = tile;
                    }
                }
                if (current.Equals(destinationTile)) {
                    break;
                }
                openSet.Remove(current);
                closedSet.AddLast(current);

                foreach (Tile tile in current.adjacentTiles) {
                    if (closedSet.Contains(tile)) {
                        continue;
                    }

                    tentativeGScore = current.gScore + Vector2.Distance(current.localPosition, tile.localPosition);
                    if (!(openSet.Contains(tile))) {
                        openSet.AddLast(tile);
                    }
                    else if (tentativeGScore >= tile.gScore) {
                        continue;
                    }

                    tile.cameFrom = current;
                    tile.gScore = tentativeGScore;
                    tile.fScore = tile.gScore + Heuristic(current, destinationTile);

                }

            }

            LinkedList<Tile> path = new LinkedList<Tile>();
            Tile workBackTile = destinationTile;
            while (workBackTile != currentTile) {
                if (workBackTile == null) {
                    Debug.WriteLine("There is no path to the target");
                    return new LinkedList<Tile>();
                }
                path.AddFirst(workBackTile);
                workBackTile = workBackTile.cameFrom;
            }

            return path;


        }
        
        private static float Heuristic(Tile currentTile, Tile destinationTile) {
            return (Vector2.Distance(currentTile.localPosition, destinationTile.localPosition));
        }

    }
}
