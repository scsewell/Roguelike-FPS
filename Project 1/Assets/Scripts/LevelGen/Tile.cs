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

        private Level m_level;

        public Tile(Level level, int x, int y, int z)
        {
            m_level = level;
            this.x = x;
            this.y = y;
            this.z = z;

            m_type = TileType.Wall;
        }

        public void MakeRoom(Room room)
        {
            m_type = TileType.Room;
            room.Tiles.Add(this);
            m_room = room;
            m_level.RoomTiles.Add(this);
        }

        public void MakeCorridor()
        {
            m_type = TileType.Corridor;
            m_level.CorridorTiles.Add(this);
        }

        public override string ToString()
        {
            return Position + " Type: " + m_type;
        }
    }
}
