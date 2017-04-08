using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TemplatesDatabase;

namespace SCLaucher
{
    class testBehavior :Game.SubsystemBlockBehavior
    {
        protected override void Load(ValuesDictionary valuesDictionary)
        {
            base.Load(valuesDictionary);
        }
        public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
        {
            Engine.Vector3 playerLocation = ModAPI.Player.getLocation();

            ModAPI.Block playerBlock = ModAPI.Terrain.getBlock(playerLocation);
            playerBlock.setBlockID(2);

            ModAPI.Entity.EntitySpawner.spawnCreature("Piranha", new Engine.Vector3(x, y, z));
        }

        public override int[] HandledBlocks
        {
            get
            {
                return new int[] { 0x02 };
            }
        }
    }
    class Laucher
    {
        public static void dialogCommandAction(Game.MessageDialogButton button)
        {
            if (button.ToString() == "Button2")
            {
                Game.ScreensManager.SwitchScreen("Loading", null);
            }
        }
        public static void init()
        {
            Game.DialogsManager.ShowDialog(new Game.MessageDialog("Mod API 1.0.0", "Do you like it?", "Yes", "No", dialogCommandAction));
            ModAPI.Event.BlockEvents.register(new testBehavior());
        }
        public static void preInit()
        {
            //添加初始化事件
            ModAPI.ModAction.GameInitAction = new Action(init);
        }
        public static void Main()
        {
            preInit();

            Game.Program.Main();
        }
    }
}
