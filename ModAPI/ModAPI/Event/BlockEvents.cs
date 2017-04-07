using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModAPI.Event
{
    public class BlockEvents : Game.SubsystemBlockBehavior
    {
        //自动调用，请勿使用
        public static void Initialize(Dictionary<int, List<Game.SubsystemBlockBehavior>> behaviors)
        {
            for (int i=0;i< behaviors.Count; i++)
            {
                behaviors[i].Add(new BlockEvents());
            }
        }
        public override int[] HandledBlocks
        {
            get
            {
                return new int[0];
            }
        }
    }
}
