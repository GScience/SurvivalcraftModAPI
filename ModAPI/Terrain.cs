using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModAPI
{
    public class Block
    {
        private Game.Block m_block;
        private Engine.Vector3 m_location;
        private Game.SubsystemTerrain m_terrain;

        public Engine.Vector3 location
        {
            get
            {
                return m_location;
            }
        }
        public Block(int blockValue, Engine.Vector3 location, Game.SubsystemTerrain terrain)
        {
            m_location = location;
            m_terrain = terrain;
            m_block = Game.BlocksManager.Blocks[Game.TerrainData.ReplaceLight(blockValue, 0)];
        }
        public void setBlockID(int blockValue)
        {
            m_terrain.ChangeCell((int)location.X, (int)location.Y, (int)location.Z, blockValue, false);
            m_block = Game.BlocksManager.Blocks[Game.TerrainData.ReplaceLight(blockValue, 0)];
        }
        public Game.Block toSCBlock()
        {
            return m_block;
        }
    }
    public class Terrain
    {
        private static Game.SubsystemTerrain terrain;

        //自动调用，请勿使用
        public static void Initialize(Game.SubsystemTerrain scTerrain)
        {
            terrain = scTerrain;
        }

        //获取Block
        public static Block getBlock(int x, int y, int z)
        {
            return new Block(terrain.TerrainData.GetCellValue(x, y, z), new Engine.Vector3(x, y, z), terrain);
        }
        public static Block getBlock(Engine.Vector3 loc)
        {
            return new Block(terrain.TerrainData.GetCellValue((int)loc.X, (int)loc.Y, (int)loc.Z), new Engine.Vector3((int)loc.X, (int)loc.Y, (int)loc.Z), terrain);
        }
    }
}
