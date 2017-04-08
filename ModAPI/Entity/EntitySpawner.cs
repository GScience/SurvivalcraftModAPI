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
        private static Game.SubsystemEntityFactory m_subsystemEntityFactory;

        //自动调用，请勿使用
        public static void Initialize(Game.SubsystemEntityFactory subsystemEntityFactory)
        {
            m_subsystemEntityFactory = subsystemEntityFactory;
        }
        public static GameEntitySystem.Entity spawnEntity(string name, Engine.Vector3 location)
        {
            GameEntitySystem.Entity newEntity = m_subsystemEntityFactory.CreateEntity(name, true);

            newEntity.FindComponent<Game.ComponentBody>(true).Position = location;

            m_subsystemEntityFactory.Project.AddEntity(newEntity);

            return newEntity;
        }
    }
}
