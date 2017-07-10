using System.Collections.Generic;
using UnityEngine;

namespace LevelGen
{
    public class Tile
    {
        public int x;
        public int y;
        public int z;

        public Vector3 Position
        {
            get { return new Vector3(x, y, z); }
        }

        private TileType m_type;
        public TileType Type
        {
            get { return m_type; }
        }

        private Room m_room;
        public Room Room
        {
            get { return m_room; }
        }

        public Tile(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;

            m_type = TileType.Wall;
        }

        public void MakeRoom(Room room)
        {
            m_type = TileType.Room;
            m_room = room;
        }

        public void MakeCorridor()
        {
            m_type = TileType.Corridor;
        }

        public override string ToString()
        {
            return Position + " Type: " + m_type;
        }
    }
}
