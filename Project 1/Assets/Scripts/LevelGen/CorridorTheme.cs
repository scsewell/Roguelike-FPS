using UnityEngine;

[CreateAssetMenu(fileName = "CorridorTheme", menuName = "LevelGen/Corridor Theme", order = 2)]
public class CorridorTheme : ScriptableObject
{
    public Transform[] floorCorridor;
    public Transform[] floorJunction;
    public Transform[] wall;
    public Transform[] wallDeadEnd;
    public Transform[] junctionCorner;
    public Transform[] roof;
    public Transform[] roofToWall;
    public Transform[] roofToCorridor;
    public Transform[] light;
    public Transform[] spinLight;

    public float lightChance = 0.1f;
    public float spinLightProportion = 0.25f;
}