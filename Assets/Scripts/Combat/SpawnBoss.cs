using UnityEngine;

public class SpawnBoss : MonoBehaviour {

    public Spawner enemyBossSpawner;
    int numOfMonsters;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < transform.childCount; i++)
        {
            Spawner spawner = transform.GetChild(i).GetComponent<Spawner>();
            if (enemyBossSpawner.gameObject != spawner.gameObject)
            {
                numOfMonsters += spawner.enemiesToSpawn.Length;
            }
        }
	}
	
	// Update is called once per frame
	public void EnemyKilled () {
        numOfMonsters--;
        if(numOfMonsters <= 0)
        {
            enemyBossSpawner.gameObject.SetActive(true);
        }
	}
}
