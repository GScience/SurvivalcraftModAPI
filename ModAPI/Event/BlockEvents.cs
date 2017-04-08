using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplatesDatabase;

namespace ModAPI.Event
{
    public class BlockEvents
    {
        private static List<Game.SubsystemBlockBehavior> globalModBlockBehavoirs = new List<Game.SubsystemBlockBehavior>();
        private static Dictionary<int, List<Game.SubsystemBlockBehavior>> modBlockBehavoirs = new Dictionary<int, List<Game.SubsystemBlockBehavior>>();

        //自动调用，请勿使用
        public static void Initialize(Dictionary<int, List<Game.SubsystemBlockBehavior>> behaviors)
        {
            for (int i=0;i< behaviors.Count; i++)
            {
                behaviors[i].AddRange(globalModBlockBehavoirs);
            }
        }
        public static void register(Game.SubsystemBlockBehavior blockBehavior)
        {
            globalModBlockBehavoirs.Add(blockBehavior);
        }
    }
}
