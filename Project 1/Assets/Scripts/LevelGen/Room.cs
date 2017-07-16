using UnityEngine;

namespace LevelGen
{
    public struct Room
    {
        private Bounds m_bounds;
        public Bounds Bounds { get { return m_bounds; } }

        private int m_minX;
        public int MinX { get { return m_minX; } }

        private int m_maxX;
        public int MaxX { get { return m_maxX; } }

        private int m_minZ;
        public int MinZ { get { return m_minZ; } }

        private int m_maxZ;
        public int MaxZ { get { return m_maxZ; } }

        private int m_floor;
        public int Floor { get { return m_floor; } }

        private int m_floorCount;
        public int FloorCount { get { return m_floorCount; } }

        public Room(int centerX, int centerZ, int width, int length, int floor, int floorCount)
        {
            // int center does not always line up even sizes, so randomly offset it
            float offsetCenterX = (width % 2 == 0) ? 0 : 0.5f;
            float offsetCenterZ = (length % 2 == 0) ? 0 : 0.5f;

            Vector3 center = new Vector3(
                centerX + offsetCenterX,
                floor + (floorCount / 2.0f),
                centerZ + offsetCenterZ);

            Vector3 size = new Vector3(width, floorCount, length);

            m_bounds = new Bounds(center, size);

            m_minX = Mathf.RoundToInt(m_bounds.min.x);
            m_maxX = Mathf.RoundToInt(m_bounds.max.x);

            m_minZ = Mathf.RoundToInt(m_bounds.min.z);
            m_maxZ = Mathf.RoundToInt(m_bounds.max.z);

            m_floor = floor;
            m_floorCount = floorCount;
        }

        public Vector3 GetRandomPointInsideRoom()
        {
            return new Vector3(
                Random.Range(m_minX, m_maxX),
                m_floor + Random.Range(0, m_floorCount),
                Random.Range(m_minZ, m_maxZ)
                );
        }

        public bool IsBeside(Vector3 point)
        {
            Bounds expanded = new Bounds(m_bounds.center, m_bounds.size + (2 * Vector3.one));
            return expanded.Contains(point) && !m_bounds.Contains(point);
        }

        public bool IsAlongsideFace(Vector3 point)
        {
            bool inWidth = (m_bounds.min.x <= point.x && point.x <= m_bounds.max.x);
            bool inHeight = (m_bounds.min.y <= point.y && point.y <= m_bounds.max.y);
            bool inLength = (m_bounds.min.z <= point.z && point.z <= m_bounds.max.z);

            return (inWidth && inLength) || (inWidth && inHeight) || (inLength && inHeight);
        }
    }
}
