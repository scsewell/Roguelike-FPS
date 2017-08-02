using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LevelGen
{
    public class Level
    {
        private int m_minX;
        private int m_maxX;
        private int m_minZ;
        private int m_maxZ;
        private int m_floorCount;

        private Tile[][][] m_level;
        
        private List<Room> m_rooms = new List<Room>();
        public List<Room> Rooms
        {
            get { return m_rooms; }
        }

        private List<Tile> m_roomTiles = new List<Tile>();
        public List<Tile> RoomTiles
        {
            get { return m_roomTiles; }
        }

        private List<Tile> m_corridorTiles = new List<Tile>();
        public List<Tile> CorridorTiles
        {
            get { return m_corridorTiles; }
        }

        public Level(List<Room> rooms)
        {
            m_rooms = new List<Room>(rooms);

            m_minX = rooms.Min(r => r.MinX);
            m_minZ = rooms.Min(r => r.MinZ);

            m_maxX = rooms.Max(r => r.MaxX);
            m_maxZ = rooms.Max(r => r.MaxZ);
            m_floorCount = rooms.Max(r => r.Floor + r.FloorCount);

            int width = m_maxX - m_minX;
            int length = m_maxZ - m_minZ;

            m_level = new Tile[width][][];
            for (int x = 0; x < width; x++)
            {
                m_level[x] = new Tile[length][];
                for (int z = 0; z < length; z++)
                {
                    m_level[x][z] = new Tile[m_floorCount];
                    for (int y = 0; y < m_floorCount; y++)
                    {
                        m_level[x][z][y] = new Tile(this, x + m_minX, y, z + m_minZ);
                    }
                }
            }

            foreach (Room room in rooms)
            {
                for (int x = room.MinX; x < room.MaxX; x++)
                {
                    for (int z = room.MinZ; z < room.MaxZ; z++)
                    {
                        for (int y = room.Floor; y < room.FloorCount; y++)
                        {
                            GetTile(x, y, z).MakeRoom(room);
                        }
                    }
                }
            }
        }

        private static Vector3[] DIRECTIONS = { Vector3.right, Vector3.left, Vector3.forward, Vector3.back };

        public List<Tile> GetAdjacent(Tile tile, List<Tile> resultList)
        {
            resultList.Clear();
            for (int i = 0; i < DIRECTIONS.Length; i++)
            {
                Tile adjacent = GetTile(tile.Position + DIRECTIONS[i]);
                if (adjacent != null)
                {
                    resultList.Add(adjacent);
                }
            }
            return resultList;
        }

        public void DrawDebug(float scale)
        {
            Gizmos.color = Color.yellow;
            IterateTiles((t) =>
            {
                if (t.Type != TileType.Wall)
                {
                    switch (t.Type)
                    {
                        case TileType.Room: Gizmos.color = new Color(0, 0.5f, 0); break;
                        case TileType.Corridor: Gizmos.color = new Color(0.5f, 0.5f, 0); break;
                    }
                    Gizmos.DrawWireCube((t.Position + (0.5f * Vector3.up)) * scale, Vector3.one * scale);
                }
            });
        }

        public void IterateTiles(Action<Tile> tileFunc)
        {
            for (int x = m_minX; x < m_maxX; x++)
            {
                for (int z = m_minZ; z < m_maxZ; z++)
                {
                    for (int y = 0; y < m_floorCount; y++)
                    {
                        tileFunc(GetTile(x, y, z));
                    }
                }
            }
        }

        public Tile GetTile(Vector3 pos)
        {   
            return GetTile(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));
        }

        public Tile GetTile(int x, int y, int z)
        {
            if (InBounds(x, y, z))
            {
                return m_level[x - m_minX][z - m_minZ][y];
            }
            return null;
        }

        private bool InBounds(int x, int y, int z)
        {
            return m_minX <= x && x < m_maxX && m_minZ <= z && z < m_maxZ && 0 <= y && y < m_floorCount;
        }
    }
}
