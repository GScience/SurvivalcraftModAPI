using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModAPI
{
    public class BlocksManager
    {
        //储存所有Mod Block
        public static List<System.Reflection.TypeInfo> modBlocks;

        //自动调用，请勿使用
        public static void Initialize()
        {
            //初始化Block
            ModAPI.BlocksManager.modBlocks = new List<System.Reflection.TypeInfo>();

            foreach (System.Reflection.TypeInfo info in System.Reflection.IntrospectionExtensions.GetTypeInfo((Type)typeof(Game.BlocksManager)).Assembly.DefinedTypes)
            {
                ModAPI.BlocksManager.modBlocks.Add(info);
            }
        }
    }
}
