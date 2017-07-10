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

        public Level(List<Room> rooms)
        {
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
                        m_level[x][z][y] = new Tile(x + m_minX, y, z + m_minZ);
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

        public Tile this[int x, int y, int z]
        {
            get
            {
                return GetTile(x, y, z);
            }
        }

        public Tile GetTile(Vector3 v)
        {
            return GetTile(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
        }

        public Tile GetTile(int x, int y, int z)
        {
            if (InBounds(x, y, z))
            {
                return m_level[x - m_minX][z - m_minZ][y];
            }
            return null;
        }

        public bool InBounds(int x, int y, int z)
        {
            return m_minX <= x && x < m_maxX && m_minZ <= z && z < m_maxZ && 0 <= y && y < m_floorCount;
        }

        private static Vector3[] DIRECTIONS = { Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.forward, Vector3.back };

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

        public void DrawDebug()
        {
            Gizmos.color = Color.yellow;
            for (int x = m_minX; x < m_maxX; x++)
            {
                for (int z = m_minZ; z < m_maxZ; z++)
                {
                    for (int y = 0; y < m_floorCount; y++)
                    {
                        Tile tile = GetTile(x, y, z);

                        if (tile.Type != TileType.Wall)
                        {
                            switch (tile.Type)
                            {
                                case TileType.Room:     Gizmos.color = Color.green; break;
                                case TileType.Corridor: Gizmos.color = Color.yellow; break;
                            }
                            Gizmos.DrawWireCube(new Vector3(x, y, z) + (0.5f * Vector3.one), Vector3.one);
                        }
                    }
                }
            }
        }
    }
}
