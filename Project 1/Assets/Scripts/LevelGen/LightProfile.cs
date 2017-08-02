using UnityEngine;

namespace LevelGen
{
    [CreateAssetMenu(fileName = "LightProfile", menuName = "LevelGen/Light Profile", order = 1)]
    public class LightProfile : ScriptableObject
    {
        public GeneratedLight.LightMode lightMode = GeneratedLight.LightMode.Static;
        public Color[] colors;

        [Range(0, 4)]
        public float maxIntensity = 1;

        [Range(0, 4)]
        public float minIntensity = 1;
        
        [Range(0, 20)]
        public float intensityChangeRate = 4;

        [Range(0, 20)]
        public float flickerChance = 1;

        [Range(0, 5)]
        public float flickerMaxDuration = 0.5f;

        [Range(0, 5)]
        public float flickerMinDuration = 0.05f;
    }
}