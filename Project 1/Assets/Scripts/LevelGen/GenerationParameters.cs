using UnityEngine;

[CreateAssetMenu(fileName = "LevelParameters", menuName = "LevelGen/Level Parameters", order = 2)]
public class GenerationParameters : ScriptableObject
{
    [Header("Main Level")]

    [SerializeField] [Range(10, 500)]
    private int m_maxLevelWidth = 100;
    public int MaxLevelWidth { get { return m_maxLevelWidth; } }

    [SerializeField] [Range(10, 500)]
    private int m_maxLevelLength = 100;
    public int MaxLevelLength { get { return m_maxLevelLength; } }
        
    [SerializeField]
    private bool m_levelIsElliptical = false;
    public bool levelIsElliptical { get { return m_levelIsElliptical; } }

    [Header("Rooms")]

    [SerializeField] [Range(1, 200)]
    private int m_minRoomCount = 10;
    public int MinRoomCount { get { return m_minRoomCount; } }

    [SerializeField] [Range(1, 200)]
    private int m_maxRoomCount = 30;
    public int MaxRoomCount { get { return m_maxRoomCount; } }
    
    [SerializeField] [Range(1, 100)]
    private float m_noiseScale = 65.0f;
    public float NoiseScale { get { return m_noiseScale; } }

    [SerializeField] [Range(0, 1)]
    private float m_noiseThreshold = 0.625f;
    public float NoiseThreshold { get { return m_noiseThreshold; } }

    [SerializeField] [Range(1, 30)]
    private int m_minRoomSize = 3;
    public int MinRoomSize { get { return m_minRoomSize; } }

    [SerializeField] [Range(1, 30)]
    private int m_maxRoomSize = 14;
    public int MaxRoomSize { get { return m_maxRoomSize; } }

    [SerializeField]
    private AnimationCurve m_roomSizeDistribution = new AnimationCurve(new Keyframe[] { new Keyframe(0,0), new Keyframe(1, 1) });
    public AnimationCurve RoomSizeDistribution { get { return m_roomSizeDistribution; } }

    [Header("Corridor")]
    
    [SerializeField] [Range(0, 20)]
    private float m_closeRoomBias = 2;
    public float CloseRoomBias { get { return m_closeRoomBias; } }

    [SerializeField] [Range(-50, 50)]
    private float m_wallCarveCost = 10;
    public float WallCarveCost { get { return m_wallCarveCost; } }

    [SerializeField] [Range(-50, 50)]
    private float m_roomCarveCost = 0;
    public float RoomCarveCost { get { return m_roomCarveCost; } }

    [SerializeField] [Range(-50, 50)]
    private float m_changeDirectionCost = 2f;
    public float ChangeDirectionCost { get { return m_changeDirectionCost; } }

    [SerializeField] [Range(-10, 10)]
    private float m_noiseCost = 3;
    public float NoiseCost { get { return m_noiseCost; } }
}
