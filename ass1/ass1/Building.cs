using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefence {
    public class Building : BasicGameObject {

        //The tile that the building is on
        public Tile onTile { get; private set; }

        public Building(Model m, Vector3 position, float maxHealth, float maxDamage, Texture2D healthBarTexture, SpriteBatch spriteBatch, Tile builtOnTile) : base(m, position, maxHealth, maxDamage, healthBarTexture, spriteBatch) {
            builtOnTile.AddBuildingToTile(this);
            onTile = builtOnTile;
        }

        public override void DamageObject(float damage) {
            base.DamageObject(damage);

        }

    }
}
