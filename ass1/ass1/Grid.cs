using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ass1 {
    /// <summary>
    /// Will hold a description of a grid square for the map
    /// </summary>
    class Grid {

        //A list of tiles that are in the grid
        public List<Tile> tiles { get; private set; }
        public Vector3 centerPosition { get; private set; }
        public int width { get; private set; }
        public int height { get; private set; }

        /// <summary>
        /// Constructor method for the grid which assigns the center position and set
        /// up the grid based on the height and width attributes
        /// </summary>
        /// <param name="centerPosition">The center position of the grid</param>
        /// <param name="width">The number of tiles wide</param>
        /// <param name="height">The number of tiles high</param>
        public Grid(Vector3 centerPosition, int width, int height) {
            tiles = new List<Tile>();
            this.centerPosition = centerPosition;
            this.height = height;
            this.width = width;
            GenerateGrid();
        }

        /// <summary>
        /// Returns the tile represented by the given local 2d coordinates
        /// </summary>
        /// <param name="tileLocalPosition">The local 2D position of the tile</param>
        /// <returns>The tile at the given local coordinates or nul;</returns>
        public Tile GetTile(Vector2 tileLocalPosition) {
            foreach (Tile tile in tiles) {
                if (tile.localPosition.X == tileLocalPosition.X &&
                    tile.localPosition.Y == tileLocalPosition.Y) {
                    return tile;
                }
            }
            Debug.WriteLine("Tile with a local position " + tileLocalPosition.ToString() +
                " does not exist");
            return null;
        }

        /// <summary>
        /// Helper method to construct the grid based on the attribute values
        /// </summary>
        private void GenerateGrid() {
            for (int i = -width/2; i < width/2; i++) {
                for (int j = height/2; j > -height/2; j--) {
                    tiles.Add(new Tile(new Vector3(i * Game1.TILE_SIZE + centerPosition.X,
                        0, j * Game1.TILE_SIZE + centerPosition.Y), new Vector2(i, j)));
                }
            }
            GenerateAdjacency(true);
        }

        /// <summary>
        /// Will generate all the adjacency lists for each tile
        /// </summary>
        /// <param name="diagonalsAllowed">Whether or not the diagonal are adjacent</param>
        private void GenerateAdjacency(bool diagonalsAllowed) {
            Tile adjacentTile;
            foreach(Tile tile in tiles) {
                for (int i = -1; i <= 1; i++) {
                    for (int j = -1; j <= 1; j++) {
                        adjacentTile = GetTile(new Vector2(
                            tile.localPosition.X + i, tile.localPosition.Y + j));
                        if (adjacentTile != null && !tile.Equals(adjacentTile)) {
                            tile.AddAdjacentTile(adjacentTile);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// To string method for a text based representation of the grid
        /// </summary>
        /// <returns></returns>
        public override String ToString() {
            String returnString = "";
            foreach(Tile tile in tiles) {
                returnString += tile.ToString() + "\n";
            }
            return returnString;
        }

    }
}
