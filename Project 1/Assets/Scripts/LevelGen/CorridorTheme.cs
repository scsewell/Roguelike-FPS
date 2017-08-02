using UnityEngine;

namespace LevelGen
{
    [CreateAssetMenu(fileName = "CorridorTheme", menuName = "LevelGen/Corridor Theme", order = 2)]
    public class CorridorTheme : ScriptableObject
    {
        [SerializeField]
        private Transform[] m_corridorFloors;
        public Transform[] CorridorFloors
        {
            get { return m_corridorFloors; }
        }

        [SerializeField]
        private Transform[] m_junctionFloors;
        public Transform[] JunctionFloors
        {
            get { return m_junctionFloors; }
        }

        [SerializeField]
        private Transform[] m_walls;
        public Transform[] Walls
        {
            get { return m_walls; }
        }

        [SerializeField]
        private Transform[] m_deadEndWalls;
        public Transform[] DeadEndWalls
        {
            get { return m_deadEndWalls; }
        }

        [SerializeField]
        private Transform[] m_junctionCorners;
        public Transform[] JunctionCorners
        {
            get { return m_junctionCorners; }
        }

        [SerializeField]
        private Transform[] m_roofs;
        public Transform[] Roofs
        {
            get { return m_roofs; }
        }

        [SerializeField]
        private Transform[] m_roofToWalls;
        public Transform[] RoofToWalls
        {
            get { return m_roofToWalls; }
        }

        [SerializeField]
        private Transform[] m_roofToCorridors;
        public Transform[] RoofToCorridors
        {
            get { return m_roofToCorridors; }
        }

        [SerializeField] [Range(0,1)]
        private float m_lightChance = 0.05f;
        public float LightChance
        {
            get { return m_lightChance; }
        }

        [SerializeField]
        private Transform[] m_lights;
        public Transform[] Lights
        {
            get { return m_lights; }
        }

        [SerializeField]
        private Transform[] m_altLights;
        public Transform[] AltLights
        {
            get { return m_altLights; }
        }

        [SerializeField] [Range(0, 1)]
        private float m_altProportion = 0.2f;
        public float AltLightProportion
        {
            get { return m_altProportion; }
        }
    }
}