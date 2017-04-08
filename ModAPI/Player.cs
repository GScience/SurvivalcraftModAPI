using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModAPI
{
    public class Player
    {
        private static Game.ComponentPlayer player;
        private static GameEntitySystem.Entity playerEntity;

        //自动调用，请勿使用
        public static void Initialize(Game.ComponentPlayer scPlayer)
        {
            player = scPlayer;
            playerEntity = player.Entity;
        }

        //获取物品栏
        public static Game.IInventory getInventory()
        {
            return player.ComponentMiner.Inventory;
        }

        public static Engine.Vector3 getLocation()
        {
            return (Engine.Vector3)((player.ComponentBody.BoundingBox.Min + player.ComponentBody.BoundingBox.Max) / 2f);
        }
    }
}
