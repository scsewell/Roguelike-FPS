using UnityEngine;

[CreateAssetMenu(fileName = "BulletSetting", menuName = "Bullet Settings", order = 2)]
public class BulletSettings : ScriptableObject
{
    [SerializeField]
    private Bullet m_bulletPrefab;
    public Bullet BulletPrefab { get { return m_bulletPrefab; } }
    
    [SerializeField]
    private ParticleSystem m_sparksParticles;
    public ParticleSystem Sparks { get { return m_sparksParticles; } }

    [SerializeField]
    private ParticleSystem m_bloodParticles;
    public ParticleSystem Blood { get { return m_bloodParticles; } }

    [SerializeField]
    private Decal m_bulletHole;
    public Decal BulletHole { get { return m_bulletHole; } }

    [SerializeField]
    private LayerMask m_hitLayers;
    public LayerMask HitLayers { get { return m_hitLayers; } }
    
    [SerializeField] [Range(0, 1000)]
    private float m_range = 100.0f;
    public float Range { get { return m_range; } }

    [SerializeField] [Range(0, 800)]
    private float m_speed = 400.0f;
    public float Speed { get { return m_speed; } }

    [SerializeField]
    private bool m_useGravity = true;
    public bool UseGravity { get { return m_useGravity; } }

    [SerializeField] [Range(0, 1000)]
    private float m_damage = 10.0f;
    public float Damage { get { return m_damage; } }

    [SerializeField] [Range(0, 1000)]
    private float m_force = 300.0f;
    public float Force { get { return m_force; } }

    [SerializeField] [Range(0, 0.4f)]
    private float m_baseInaccuracy = 0.0125f;
    public float BaseInaccuracy { get { return m_baseInaccuracy; } }

    [SerializeField] [Range(1, 32f)]
    [Tooltip("Larger values bias shots to perfect accuracy.")]
    private float m_spread = 3.0f;
    public float Spread { get { return m_spread; } }
}
