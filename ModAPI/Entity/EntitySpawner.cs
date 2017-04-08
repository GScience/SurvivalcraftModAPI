using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ModAPI.Entity
{
    public class EntitySpawner
    {
        private static Game.SubsystemCreatureSpawn m_subsystemCreatureSpawn;

        //自动调用，请勿使用
        public static void Initialize(Game.SubsystemCreatureSpawn subsystemCreatureSpawn)
        {
            m_subsystemCreatureSpawn = subsystemCreatureSpawn;
        }
        public static void spawnCreature(string name, Engine.Vector3 location)
        {
            m_subsystemCreatureSpawn.GetType().GetMethod("SpawnCreature",BindingFlags.NonPublic | BindingFlags.Instance).Invoke(m_subsystemCreatureSpawn, new object[] { name, location });
        }
    }
}
