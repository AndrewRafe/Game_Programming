using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ass1 {
    class Building : BasicModel {

        public int health { get; protected set; }

        public Building(Model m, Vector3 position, int health) : base(m, position) {
            this.health = health;
        }

    }
}
