﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TowerDefence {
    public class Enemy : BasicGameObject {

        public static float MAX_HEALTH = 500.0f;
        public static float MAX_DAMAGE = 100.0f;
        public const float DEFAULT_REWARD_FOR_KILLING = 10.0f;

        private const String STATE_ATTACK_TOWER = "ATTACK_TOWER";
        private const String STATE_ATTACK_WALL = "ATTACK_WALL";
        private const String STATE_IDLE = "IDLE";
        private const String STATE_RUN_AWAY = "RUN_AWAY";

        private const String CONDITION_PATH_TO_TOWER = "PATH_TO_TOWER";
        private const String CONDITION_LOW_HEALTH = "LOW_HEALTH";
        private const String CONDITION_MAX_HEALTH = "MAX_HEALTH";
        private String currentState;
        private String currentCondition;

        private LinkedList<Tile> path;
        private Grid grid;

        private bool hasPathToTower;

        public float rewardForKilling { get; protected set; }
        public Vector3 prevPosition { get; private set; }
        public Tile targetTile { get; private set; }
        public Tile spawnTile { get; private set; }
        Tower tower;
        float speed;
        XElement xml;
        

        public Enemy(Model m, Vector3 position, float maxHealth, float maxDamage, Texture2D healthBarTexture, Tower tower, Game1 game, Grid grid, XElement xml) : base(m, position, maxHealth, maxDamage, healthBarTexture, game.spriteBatch) {
            this.tower = tower;
            this.maxHealth = maxHealth;
            this.maxDamage = maxDamage;
            this.speed = (float) game.rand.Next(10, 50);
            this.rewardForKilling = DEFAULT_REWARD_FOR_KILLING;
            hasPathToTower = false;
            currentState = STATE_IDLE;
            currentCondition = "";
            this.grid = grid;
            this.targetTile = grid.GetTile(tower.GetPosition());
            this.spawnTile = grid.GetTile(position);
            UpdatePath(grid.GetTile(tower.GetPosition()));
            this.xml = xml;

        }

        public virtual void Initiate() {
            
        }

        /// <summary>
        /// Enemy position will be updated based on enemies chasing the position of the castle
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime) {

            //Debug.WriteLine("CurrentState :" + currentState);
            //string stateFromXml = "";
            //foreach (XElement state in xml.Elements()) {
            //    stateFromXml = state.Attribute("fromState").Value;
            //    if (stateFromXml.Equals(currentState)) {
            //        if (stateFromXml.Equals(STATE_IDLE)) {
            //            UpdatePath(tower.onTile);
            //            foreach (XElement transaction in state.Elements()) {
            //                string conditionFromXml = transaction.Attribute("condition").Value;
            //                if (conditionFromXml.Equals(currentCondition)) {
            //                    currentState = transaction.Attribute("toState").Value;
            //                }
            //            }
            //        }
            //        else if (stateFromXml.Equals(STATE_ATTACK_TOWER)) {
            //            MoveOnPath(gameTime);
            //            foreach (XElement transaction in state.Elements()) {
            //                bool forCheck = isLowHealth();
            //                string conditionFromXml = transaction.Attribute("condition").Value;
            //                Debug.WriteLine("Condition: " + currentCondition);
            //                if (conditionFromXml.Equals(currentCondition)) {
            //                    currentState = transaction.Attribute("toState").Value;
            //                    RunAwayPathUpdate();
            //                }
            //            }
            //        }
            //        else if (stateFromXml.Equals(STATE_RUN_AWAY)) {
            //            foreach (XElement transaction in state.Elements()) {
            //                string conditionFromXml = transaction.Attribute("condition").Value;
            //                bool forCheck = isMaxHealth();
            //                if (conditionFromXml.Equals(currentCondition)) {
            //                    currentState = transaction.Attribute("toState").Value;
            //                }
            //            }
            //            if (grid.GetTile(position) == spawnTile) {
            //                RegenerateHealth(gameTime);
            //            }
            //            MoveOnPath(gameTime);
            //        }
            //    }
            //}


            if (currentState == STATE_IDLE)
            {
                UpdatePath(tower.onTile);
                if (hasPathToTower)
                {
                    currentState = STATE_ATTACK_TOWER;
                }
            }
            else if (currentState == STATE_ATTACK_TOWER)
            {
                MoveOnPath(gameTime);
                if (isLowHealth())
                {
                    currentState = STATE_RUN_AWAY;
                    RunAwayPathUpdate();
                }
            }
            else if (currentState == STATE_RUN_AWAY)
            {
                if (isMaxHealth())
                {
                    currentState = STATE_IDLE;
                }
                if (grid.GetTile(position) == spawnTile)
                {
                    RegenerateHealth(gameTime);
                }
                MoveOnPath(gameTime);
            }


            //prevPosition = position;
            //position = Behavior.StraightLineChase(this.position, tower.GetPosition(), gameTime, this.speed);
            //rotation = BasicModel.RotateToFace(position, tower.GetPosition(), new Vector3(0, 0, 1));
            base.Update(gameTime);
        }

        private void MoveOnPath(GameTime gameTime) {
            Tile currentTile;
            if (path.Count() > 0) {
                currentTile = path.First.Value;
                prevPosition = position;
                position = Behavior.StraightLineChase(this.position, currentTile.globalPosition, gameTime, speed);
                rotation = BasicModel.RotateToFace(position, currentTile.globalPosition, Vector3.UnitX);
                if (currentTile.IsAtCenter(this.position)) {
                    currentTile.RemoveEnemyFromTile(this);
                    path.RemoveFirst();
                    if (path.Count != 0) {
                        path.First.Value.AddEnemyToTile(this);
                    }
                }
            }
        }

        /// <summary>
        /// Will use the astar behavior to create a path to the target
        /// </summary>
        /// <param name="targetGridDestination">The tile that the enemy is trying to get to</param>
        public void UpdatePath(Tile targetGridDestination) {
            path = Behavior.AStarPathFinding(grid.GetTile(position), targetGridDestination, grid);
            //If there is no path to the destination then call a handler function
            if (path == null) {
                hasPathToTower = false;
                RunAwayPathUpdate();
            } else {
                hasPathToTower = true;
                currentCondition = CONDITION_PATH_TO_TOWER;
            }
        }

        public float GetDamage() {
            return this.maxDamage;
        }

        public void DamageEnemy(float damage) {
            DamageObject(damage);
        }

        ///<summary>
        ///Return the speed of the enemy
        ///</summary>
        public float GetSpeed() {
            return this.speed;
        }

        /// <summary>
        /// Gets the current velocity vector
        /// </summary>
        /// <returns>The velocity of the tank</returns>
        public Vector3 GetVelocityVector() {
            return (position - prevPosition);
        }

        /// <summary>
        /// Determines whether the enemy is on low health
        /// </summary>
        /// <returns></returns>
        public bool isLowHealth() {
            if (currentHealth <= maxHealth/2) {
                currentCondition = CONDITION_LOW_HEALTH;
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Determines whether the enemy is on max health
        /// </summary>
        /// <returns></returns>
        public bool isMaxHealth() {
            if (currentHealth == maxHealth) {
                currentCondition = CONDITION_MAX_HEALTH;
                return true;
            } else {
                return false;
            }
        }

        public void RunAwayPathUpdate() {
            Tile currentTile = grid.GetTile(position);
            path = Behavior.AStarPathFinding(currentTile, spawnTile, grid);
        }

        public void SetSpawnTile(Tile spawnTile) {
            this.spawnTile = spawnTile;
        }

        /// <summary>
        /// Will be called when the enemy is eligible to regain health
        /// </summary>
        /// <param name="gameTime"></param>
        public void RegenerateHealth(GameTime gameTime) {
            currentHealth += gameTime.ElapsedGameTime.Milliseconds/5;
            if (currentHealth > maxHealth) {
                currentHealth = maxHealth;
            }
        }

    }
}
