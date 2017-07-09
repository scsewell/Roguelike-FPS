using UnityEngine;
using System.Collections.Generic;
using Framework;

public class EnemyManager : ComponentSingleton<EnemyManager>
{
    [SerializeField]
    private ExoEnemy m_exoEnemyPrefab;
    [SerializeField]
    private int m_enemyCount = 0;

    private List<ExoEnemy> m_exoEnemies = new List<ExoEnemy>();
    private Transform m_player;

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

		if (m_exoEnemies.Count < m_enemyCount)
        {
            Vector3 randomPos = m_player.position + Quaternion.Euler(0, Random.value * 360, 0) * (Random.Range(20, 50) * Vector3.forward);
            Vector3 spawnPos = CorridorGraph.Instance.ClosestTilePos(randomPos);
            Quaternion spawnRot = Quaternion.Euler(0, Random.value * 360, 0);
            m_exoEnemies.Add(Instantiate(m_exoEnemyPrefab, spawnPos, spawnRot));
        }
	}

    public void RemoveExoEnemy(ExoEnemy enemy)
    {
        m_exoEnemies.Remove(enemy);
    }
}
