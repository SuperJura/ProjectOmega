using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public GameObject[] objectToEnableAfterDeath;
    public GameObject[] objectToDisableAfterDeath;
    public AudioClip musicOnEnable;

    public EnemyInfo[] enemiesToSpawn;
    [Range(0.2f, 2)]
    public float spawnRange = 0.5f;

    int numberOfAliveEnemies;

	// Use this for initialization
	void Start () {
        if (musicOnEnable != null) Game.instance.ChangeMusic(musicOnEnable);
        for (int i = 0; i < enemiesToSpawn.Length; i++)
        {
            GameObject enemy = Instantiate(Resources.Load<GameObject>("Enemy"));
            enemy.transform.SetParent(transform);
            enemy.GetComponent<SpriteRenderer>().sprite = enemiesToSpawn[i].sprites.idleSprite;
            enemy.AddComponent<BoxCollider2D>();
            enemy.transform.Find("Canvas").GetComponent<Canvas>().worldCamera = Camera.main;

            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            enemyComponent.info = enemiesToSpawn[i];
            enemyComponent.mySpawner = gameObject;
            enemyComponent.Init();

            Vector2 positionOffset = Random.insideUnitCircle * spawnRange;
            enemy.transform.position = new Vector2(transform.position.x + positionOffset.x, transform.position.y + positionOffset.y);
        }
        numberOfAliveEnemies = enemiesToSpawn.Length;

    }
    
    public void EnemyKilled()
    {
        //Enemy je djete spawnera koji je djete SpawnBossa
        transform.parent.GetComponent<SpawnBoss>().EnemyKilled();
        numberOfAliveEnemies -= 1;

        if(numberOfAliveEnemies <= 0)
        {
            for (int i = 0; i < objectToEnableAfterDeath.Length; i++)
            {
                objectToEnableAfterDeath[i].SetActive(true);
            }

            for (int i = 0; i < objectToDisableAfterDeath.Length; i++)
            {
                objectToDisableAfterDeath[i].SetActive(false);
            }
            if (musicOnEnable != null) Game.instance.ResetMusic();
        }
    }
}