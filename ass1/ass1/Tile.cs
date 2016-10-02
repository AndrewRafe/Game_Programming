using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefence {
    /// <summary>
    /// Holds information about a single tile in a grid including all of the tiles
    /// that are adjacent to this tile as well as some scoring information to be used
    /// by a graph search algorithm
    /// </summary>
    class Tile {

        //Attributes for use in search algorithm
        public float gScore { get; private set; }
        public float fScore { get; private set; }
        public Tile cameFrom { get; private set; }

        public List<Tile> adjacentTiles { get; private set; }
        public Vector3 globalPosition { get; private set; }
        public Vector2 localPosition { get; private set; }
        public bool isWalkable { get; private set; }

        /// <summary>
        /// Constructor method for the tile class that will assign its center global
        /// position and also its local position in relation to the rest of the grid
        /// </summary>
        /// <param name="globalPosition">The center position of the tile</param>
        /// <param name="localPosition">The position relative to the rest of the grid</param>
        public Tile(Vector3 globalPosition, Vector2 localPosition) {
            adjacentTiles = new List<Tile>();
            this.globalPosition = globalPosition;
            this.localPosition = localPosition;
            isWalkable = true;
        }

        /// <summary>
        /// Adds a given tile to the list of adjacent tiles to this tile
        /// </summary>
        /// <param name="tile">The tile that is adjacent to this tile</param>
        public void AddAdjacentTile(Tile tile) {
            adjacentTiles.Add(tile);
        }

        /// <summary>
        /// Will remove a given tile from this tiles adjacency list if it exists in the list
        /// </summary>
        /// <param name="tile">The tile to remove from the adjacent tiles</param>
        public void RemoveAdjacentTile(Tile tile) {
            //Remove tile from list
            //If it doesnt exist in the list then nothing will happen
            adjacentTiles.Remove(tile);
        }

        /// <summary>
        /// Makes this tile unwalkable by removing all of its adjacency references
        /// </summary>
        public void MakeUnwalkable() {
            //Loop through all currently adjacent tiles to this tile
            foreach(Tile tile in adjacentTiles) {
                //Remove the reference to this tile from the adjacent tile
                tile.RemoveAdjacentTile(this);
            }
            //Remove all of the adjacent tiles from this tile
            adjacentTiles = new List<Tile>();
            //Set the isWalkable attribute to be false
            isWalkable = false;
        }

        //TODO: Implement MakeWalkable method
        /// <summary>
        /// Makes this tile walkable and adds its adjacencies back to the grid
        /// </summary>
        /// <param name="grid">A reference to the grid</param>
        public void MakeWalkable(Grid grid) {
            Tile adjacentTile;
            foreach (Tile tile in grid.tiles) {
                for (int i = -1; i <= 1; i++) {
                    for (int j = -1; j <= 1; j++) {
                        adjacentTile = grid.GetTile(new Vector2(
                            tile.localPosition.X + i, tile.localPosition.Y + j));
                        if (adjacentTile != null && !tile.Equals(adjacentTile)) {
                            tile.AddAdjacentTile(adjacentTile);
                        }
                    }
                }
            }
            //Set the is walkable attribute to be true
            isWalkable = true;
        }

        /// <summary>
        /// Determines if the given tile is adjacent to this tile
        /// </summary>
        /// <param name="tile">THe tile that needs to be tested for adjacency 
        /// against this tile</param>
        /// <returns>Whether tile is adjacent or not</returns>
        public bool IsAdjacent(Tile tile) {
            if (adjacentTiles.Contains(tile)) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Debug toString override for a visual representation for a single tile and
        /// its adjacent tiles
        /// </summary>
        /// <returns>String representation of tile</returns>
        public override string ToString() {
            String s = "(" + localPosition.X + ", " + localPosition.Y + ") --> ";
            foreach (Tile tile in adjacentTiles) {
                s += "(" + tile.localPosition.X + ", " + tile.localPosition.Y + ") --> ";
            }
            return s;
        }

    }
}
