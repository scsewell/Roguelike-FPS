using UnityEngine;

[CreateAssetMenu(fileName = "LightProfile", menuName = "LevelGen/Light Profile", order = 1)]
public class LightProfile : ScriptableObject
{
    public GeneratedLight.LightMode lightMode = GeneratedLight.LightMode.Static;
    public Color[] colors;
    public float maxIntensity = 1;
    public float minIntensity = 1;
    public float intensityChangeRate = 4;
    public float flickerChance = 1;
    public float flickerMaxDuration = 0.5f;
    public float flickerMinDuration = 0.05f;
}