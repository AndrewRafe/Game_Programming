﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefence {
    class Wall : Building {

        public static float DEFAULT_COST = 0.0f;
        public static float DEFAULT_DAMAGE = 0.0f;
        public static float DEFAULT_HEALTH = 100.0f;

        public float cost { get; private set; }

        public Wall(Model m, Vector3 position, float maxHealth, float maxDamage, Texture2D healthBarTexture, SpriteBatch spriteBatch, Tile builtOnTile) : base(m, position, maxHealth, maxDamage, healthBarTexture, spriteBatch, builtOnTile) {
            this.cost = DEFAULT_COST;
        }

    }
}
