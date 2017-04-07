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
