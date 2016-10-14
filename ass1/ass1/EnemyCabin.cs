using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TowerDefence
{
    public class EnemyCabin : BasicModel
    {
        public EnemyCabin(Model m, Vector3 position): base(m, position)
        { 

        }

        public override Matrix GetWorldMatrix()
        {
            Matrix world;
            world = base.GetWorldMatrix();

            return world;
        }
    }
}
