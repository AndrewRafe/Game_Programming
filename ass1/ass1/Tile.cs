using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ass1 {
    /// <summary>
    /// Holds information about a single tile in a grid including all of the tiles
    /// that are adjacent to this tile as well as some scoring information to be used
    /// by a graph search algorithm
    /// </summary>
    class Tile {

        public List<Tile> adjacentTiles { get; private set; }
        public Vector3 globalPosition { get; private set; }
        public Vector2 localPosition { get; private set; }

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
        }

        /// <summary>
        /// Adds a given tile to the list of adjacent tiles to this tile
        /// </summary>
        /// <param name="tile">The tile that is adjacent to this tile</param>
        public void AddAdjacentTile(Tile tile) {
            adjacentTiles.Add(tile);
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
