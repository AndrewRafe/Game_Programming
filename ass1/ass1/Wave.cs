using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefence {
    public class Wave {

        private Random random;

        public int waveNumber { get; private set; }
        public int spawnRate { get; private set; }

        public Wave(int waveNumber, int spawnRate) {
            this.waveNumber = waveNumber;
            this.spawnRate = spawnRate;
            random = new Random();
        }

        /// <summary>
        /// In a used instance will determine if a enemy should be spawned
        /// </summary>
        /// <returns>Whether an enemy should be spawned</returns>
        public bool SpawnEnemy() {
            if (random.Next() % spawnRate == 0) {
                return true;
            } else {
                return false;
            }
        }



    }
}
