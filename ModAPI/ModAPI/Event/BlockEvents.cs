using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModAPI.Event
{
    public class BlockEvents
    {
        //若想取消事件则可以返回false
        public static bool blockBreakEvent(Game.Block oldBlock)
        {
            if (oldBlock.ToString() == "Game.DirtBlock")
                return false;

            return true;
        }
    }
}
