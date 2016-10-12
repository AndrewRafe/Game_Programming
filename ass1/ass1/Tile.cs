using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefence {
    /// <summary>
    /// Holds information about a single tile in a grid including all of the tiles
    /// that are adjacent to this tile as well as some scoring information to be used
    /// by a graph search algorithm
    /// </summary>
    public class Tile {

        public static float INFINITY = 1000000.0f;

        //Attributes for use in search algorithm
        public float gScore;
        public float fScore;

        public Tile cameFrom;

        public List<BasicGameObject> modelsOnTile { get; private set; }

        public List<Building> buildingsOnTile { get; private set; }
        public List<Enemy> enemiesOnTile { get; private set; }
        public Turret turretOnTile { get; private set; }
        public List<Bullet> bulletsOnTile { get; private set; }
        public Tower towerOnTile { get; private set; }

        public List<Tile> adjacentTiles { get; private set; }
        public Vector3 globalPosition { get; private set; }
        public Vector2 localPosition { get; private set; }
        public bool isWalkable { get; private set; }
        public Grid grid { get; private set; }

        /// <summary>
        /// Constructor method for the tile class that will assign its center global
        /// position and also its local position in relation to the rest of the grid
        /// </summary>
        /// <param name="globalPosition">The center position of the tile</param>
        /// <param name="localPosition">The position relative to the rest of the grid</param>
        public Tile(Vector3 globalPosition, Vector2 localPosition, Grid grid) {
            adjacentTiles = new List<Tile>();
            this.globalPosition = globalPosition;
            this.localPosition = localPosition;
            isWalkable = true;
            buildingsOnTile = new List<Building>();
            enemiesOnTile = new List<Enemy>();
            turretOnTile = null;
            bulletsOnTile = new List<Bullet>();
            towerOnTile = null;
            this.grid = grid;
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
            //Tell the grid that this is an obstacle tile
            
        }

        //TODO: Implement MakeWalkable method
        /// <summary>
        /// Makes this tile walkable and adds its adjacencies back to the grid
        /// </summary>
        public void MakeWalkable() {
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

        /// <summary>
        /// Will reset all the pathfinding costs for this tile
        /// </summary>
        public void ResetCost() {
            this.gScore = INFINITY;
            this.fScore = INFINITY;
            this.cameFrom = null;
        }

        /// <summary>
        /// Determines whether a given global position is close enough to the center of this tile
        /// </summary>
        /// <returns></returns>
        public bool IsAtCenter(Vector3 objectPosition) {
            if (Math.Abs(objectPosition.X - globalPosition.X) <= 10 &&
                    Math.Abs(objectPosition.Y - globalPosition.Y) <= 10) {
                return true;
            } else {
                return false;
            }
        }
        
        public void DrawTile() {
            foreach(Enemy enemy in enemiesOnTile) {
                enemy.Draw(grid.game.camera, grid.game.graphics);
            }

            if (turretOnTile != null) {
                
                turretOnTile.Draw(grid.game.camera, grid.game.graphics);
                Debug.WriteLine("Turret Exists");
            } else {
                
            }
            
         
            foreach(Building building in buildingsOnTile) {
                building.Draw(grid.game.camera, grid.game.graphics);
            }

            foreach(Bullet bullet in bulletsOnTile) {
                bullet.Draw(grid.game.camera, grid.game.graphics);
            }

            if (towerOnTile != null) {
                towerOnTile.Draw(grid.game.camera, grid.game.graphics);
            }


        }

        public void AddEnemyToTile(Enemy enemy) {
            enemiesOnTile.Add(enemy);
            
        }

        public void RemoveEnemyFromTile(Enemy enemy) {
            enemiesOnTile.Remove(enemy);
        }

        public void AddBulletToTile(Bullet bullet) {
            bulletsOnTile.Add(bullet);
        }

        public void RemoveBulletFromTile(Bullet bullet) {
            bulletsOnTile.Remove(bullet);
        }

        public void AddTurretToTile(Turret turret) {
            turretOnTile = turret;
        }

        public void RemoveTurretFromTile(Turret turret) {
            turretOnTile = null;
        }

        public void AddBuildingToTile(Building building) {
            buildingsOnTile.Add(building);
            MakeUnwalkable();
        }

        public void RemoveBuildingFromTile(Building building) {
            buildingsOnTile.Remove(building);
            if (buildingsOnTile.Count == 0) {
                MakeWalkable();
            }
        }

        public void AddTowerToTile(Tower tower) {
            towerOnTile = tower;
        }

        public void HandleTile(GameTime gameTime) {
            TurretLogic(gameTime);
            EnemyLogic(gameTime);
            BulletLogic(gameTime);
        }

        /// <summary>
        /// The logic of all the turrets in the game world
        /// </summary>
        /// <param name="gameTime"></param>
        public void TurretLogic(GameTime gameTime) {

            try {
                if (turretOnTile != null) {
                    turretOnTile.Update(gameTime);
                    if (turretOnTile.IsDead()) {
                        buildingsOnTile.Remove(turretOnTile);
                        turretOnTile = null;
                        grid.game.TurretDestroyed();
                    }
                    if (grid.allEnemies.Count <= 0) {
                        return;
                    }
                    else {
                        Enemy closestEnemy = null;
                        foreach (Enemy enemy in grid.allEnemies) {
                            if (closestEnemy == null) {
                                closestEnemy = enemy;
                                continue;
                            }
                            else {
                                if (Vector3.Distance(turretOnTile.GetPosition(), closestEnemy.GetPosition()) > Vector3.Distance(turretOnTile.GetPosition(), enemy.GetPosition())) {
                                    closestEnemy = enemy;
                                }
                            }
                        }

                        if (turretOnTile.isReadyToFire && closestEnemy != null && Vector3.Distance(closestEnemy.GetPosition(),turretOnTile.GetPosition()) <= turretOnTile.range) {
                            Debug.WriteLine("Ready to fire");
                            bulletsOnTile.Add(turretOnTile.FireTurret(closestEnemy, gameTime, grid));
                            grid.game.CannonFire();
                        }

                        turretOnTile.FaceEnemy(closestEnemy);
                        
                    }
                }
            }
            catch (NullReferenceException) {


            }

        }

        /// <summary>
        /// The logic of a single frame for the enemy on this tile
        /// </summary>
        /// <param name="gameTime"></param>
        public void EnemyLogic(GameTime gameTime) {
            List<Enemy> enemiesToBeKilled = new List<Enemy>();
            List<Enemy> survivingEnemies = new List<Enemy>();
            foreach (Enemy enemy in enemiesOnTile) {
                //If an enemy collides with the tower, the tower takes damage and the enemy is destroyed
                foreach (Building building in buildingsOnTile) {
                    if (enemy.CollidesWith(building.model, building.GetWorldMatrix())) {
                        building.DamageObject(enemy.maxDamage);
                        enemy.DamageObject(building.maxDamage);
                    }
                }
                if (turretOnTile!=null && enemy.CollidesWith(turretOnTile.model, turretOnTile.GetWorldMatrix())) {
                    turretOnTile.DamageObject(enemy.maxDamage);
                    if (turretOnTile.IsDead()) {
                        //turretOnTile = null;
                        grid.game.TurretDestroyed();
                    }
                    enemy.DamageObject(enemy.currentHealth);
                }

                if (towerOnTile != null && enemy.CollidesWith(towerOnTile.model, towerOnTile.GetWorldMatrix())) {
                    towerOnTile.DamageObject(enemy.maxDamage);
                    enemy.DamageObject(towerOnTile.maxDamage);
                }

                //Handle enemy getting stuck on impassable tile
                if (adjacentTiles.Count == 0) {
                    enemy.DamageObject(Enemy.MAX_HEALTH);
                }

                if (enemy.IsDead()) {
                    enemiesToBeKilled.Add(enemy);
                    grid.RemoveEnemy(enemy);
                } else {
                    survivingEnemies.Add(enemy);
                }
            }

            foreach(Enemy enemy in enemiesToBeKilled) {
                enemiesOnTile.Remove(enemy);
            }
            foreach(Enemy enemy in survivingEnemies) {
                enemy.Update(gameTime);
            }

        }

        public void ResetEnemyPath() {
            foreach (Enemy enemy in enemiesOnTile) {
                enemy.UpdatePath(enemy.targetTile);
            }
        }

        public void BulletLogic(GameTime gameTime) {
            Tile currentBulletTile;
            List<Bullet> toBeRemoved = new List<Bullet>();
            List<Enemy> toBeKilled = new List<Enemy>();
            foreach (Bullet bullet in bulletsOnTile) {
                bullet.Update(gameTime);
                currentBulletTile = bullet.getCurrentTile(grid);
                if (currentBulletTile == null) {
                    toBeRemoved.Add(bullet);
                    
                } else if (currentBulletTile != this) {
                    currentBulletTile.AddBulletToTile(bullet);
                    toBeRemoved.Add(bullet);
                }


                foreach (Enemy enemy in enemiesOnTile) {
                    if (bullet.CollidesWith(enemy.model, enemy.GetWorldMatrix())) {
                        enemy.DamageObject(bullet.damage);
                        if (enemy.IsDead()) {
                            grid.game.EnemyKilled(enemy.rewardForKilling);
                        }
                        toBeRemoved.Add(bullet);
                        break;
                    }
                }
                //Check the surrounding tiles for collision as well
                if (!toBeRemoved.Contains(bullet)) {
                    foreach (Tile tile in adjacentTiles) {
                        foreach (Enemy enemy in tile.enemiesOnTile) {
                            if (bullet.CollidesWith(enemy.model, enemy.GetWorldMatrix())) {
                                enemy.DamageObject(bullet.damage);
                                if (enemy.IsDead()) {
                                    grid.game.EnemyKilled(enemy.rewardForKilling);
                                }
                                toBeRemoved.Add(bullet);
                                break;
                            }
                        }
                        if (toBeRemoved.Contains(bullet)) break;
                    }
                }

            }

            foreach (Bullet bullet in toBeRemoved) {
                bulletsOnTile.Remove(bullet);
            }
        }

    }
}
