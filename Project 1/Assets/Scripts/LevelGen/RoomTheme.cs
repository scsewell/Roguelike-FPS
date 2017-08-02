using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelGen
{
    [CreateAssetMenu(fileName = "RoomTheme", menuName = "LevelGen/Room Theme", order = 2)]
    public class RoomTheme : ScriptableObject
    {
        [SerializeField]
        private Transform[] m_floors;
        public Transform[] Floors
        {
            get { return m_floors; }
        }

        [SerializeField]
        private Transform[] m_walls;
        public Transform[] Walls
        {
            get { return m_walls; }
        }

        [SerializeField]
        private Transform[] m_roofs;
        public Transform[] Roofs
        {
            get { return m_roofs; }
        }
    }
}