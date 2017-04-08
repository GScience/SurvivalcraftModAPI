using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModAPI
{
    public class Terrain
    {
        private static Game.SubsystemTerrain terrain;

        //自动调用，请勿使用
        public static void Initialize(Game.SubsystemTerrain scTerrain)
        {
            terrain = scTerrain;
        }
        public static Game.SubsystemTerrain getTerrain()
        {
            return terrain;
        }
    }
}
