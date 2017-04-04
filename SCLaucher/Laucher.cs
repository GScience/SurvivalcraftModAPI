using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SCLaucher
{
    class Laucher
    {
        public static void Main()
        {
            ModAPI.BlocksManager.modBlocks = new List<System.Reflection.TypeInfo>();

            foreach (TypeInfo info in IntrospectionExtensions.GetTypeInfo((Type)typeof(Game.BlocksManager)).Assembly.DefinedTypes)
            {
                ModAPI.BlocksManager.modBlocks.Add(info);
            }
            Game.Program.Main();
        }
    }
}
