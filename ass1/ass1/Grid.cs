using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefence {
    /// <summary>
    /// Will hold a description of a grid square for the map
    /// </summary>
    public class Grid {

        //A list of tiles that are in the grid
        public List<Tile> tiles { get; private set; }

        public LinkedList<Tile> obstacleTiles { get; private set; }
        public List<Enemy> allEnemies { get; private set; }

        public Vector3 centerPosition { get; private set; }
        public int width { get; private set; }
        public int height { get; private set; }

        public Game1 game { get; private set; }

        /// <summary>
        /// Constructor method for the grid which assigns the center position and set
        /// up the grid based on the height and width attributes
        /// </summary>
        /// <param name="centerPosition">The center position of the grid</param>
        /// <param name="width">The number of tiles wide</param>
        /// <param name="height">The number of tiles high</param>
        public Grid(Vector3 centerPosition, int width, int height, Game1 game) {
            tiles = new List<Tile>();
            obstacleTiles = new LinkedList<Tile>();
            this.centerPosition = centerPosition;
            this.height = height;
            this.width = width;
            this.game = game;
            allEnemies = new List<Enemy>();
            GenerateGrid();
        }

        /// <summary>
        /// Returns the tile represented by the given local 2d coordinates
        /// </summary>
        /// <param name="tileLocalPosition">The local 2D position of the tile</param>
        /// <returns>The tile at the given local coordinates or null</returns>
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
        /// Will retrieve the tile based of a given global position
        /// </summary>
        /// <param name="globalPosition"></param>
        /// <returns></returns>
        public Tile GetTile(Vector3 globalPosition) {
            foreach (Tile tile in tiles) {
                if (Math.Abs(tile.globalPosition.X - globalPosition.X) <= Game1.TILE_SIZE/2 &&
                    Math.Abs(tile.globalPosition.Y - globalPosition.Y) <= Game1.TILE_SIZE/2) {
                    return tile;
                }
            }
            return null;
        }

        /// <summary>
        /// Helper method to construct the grid based on the attribute values
        /// </summary>
        private void GenerateGrid() {
            for (int i = -width/2; i <= width/2; i++) {
                for (int j = height/2; j >= -height/2; j--) {
                    Tile tileToAdd = new Tile(new Vector3(i * Game1.TILE_SIZE + centerPosition.X,
                        j * Game1.TILE_SIZE + centerPosition.Y, 0), new Vector2(i, j), this);
                    tiles.Add(tileToAdd);
                    if (i == -width/2 || i == width/2 || j == height/2) {
                        Wall wall = new Wall(game.Content.Load<Model>(@"Models\Buildings\wall"), GetTile(new Vector2(i, j)).globalPosition,
                            Wall.DEFAULT_HEALTH, Wall.DEFAULT_DAMAGE, null, game.spriteBatch,tileToAdd); 
                    }
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

        /// <summary>
        /// Will reset all the pathfinding costs for every tile in the grid
        /// </summary>
        public void ResetTileCosts() {
            foreach (Tile tile in tiles) {
                tile.ResetCost();
            }
        }

        public void AddObstacleTile (Tile tile) {
            obstacleTiles.AddLast(tile);
            tile.MakeUnwalkable();
        }

        public void RemoveObstacleTile(Tile tile) {
            obstacleTiles.Remove(tile);
            tile.MakeWalkable();
        }

        public void HandleTiles(GameTime gameTime) {
            foreach (Tile tile in tiles) {
                tile.HandleTile(gameTime);
            }
        }

        public void DrawTiles() {
            foreach (Tile tile in tiles) {
                tile.DrawTile();
            }
        }

        public void ResetEnemyPath() {
            foreach(Tile tile in tiles) {
                tile.ResetEnemyPath();
            }
        }

        public void AddEnemy(Enemy enemy) {
            allEnemies.Add(enemy);
        }

        public void RemoveEnemy(Enemy enemy) {
            allEnemies.Remove(enemy);
        }

    }
}
