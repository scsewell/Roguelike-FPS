using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private Transform m_exoEnemyPrefab;

    private static List<Transform> m_exoEnemies;

    private CorridorGraph m_level;
    private Transform m_player;
    
	private void Start()
    {
        m_level = GetComponent<CorridorGraph>();

        m_exoEnemies = new List<Transform>();
    }
	
	private void FixedUpdate()
    {
        if (m_player == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                m_player = player.transform;
            }
        }

		if (m_exoEnemies.Count < 1)
        {
            Vector3 randomPos = m_player.position + Quaternion.Euler(0, Random.value * 360, 0) * (Random.Range(20, 50) * Vector3.forward);
            Vector3 spawnPos = m_level.ClosestTilePos(randomPos);
            Quaternion spawnRot = Quaternion.Euler(0, Random.value * 360, 0);
            Transform enemy = Instantiate(m_exoEnemyPrefab, spawnPos, spawnRot) as Transform;
            m_exoEnemies.Add(enemy);
        }
	}

    public static void RemoveExoEnemy(Transform t)
    {
        if (m_exoEnemies != null && m_exoEnemies.Contains(t))
        {
            m_exoEnemies.Remove(t);
        }
    }
}
