using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    [SerializeField] GameObject[] enemy;
    [SerializeField] Transform[] spawnPos;
    [SerializeField] int spawnCount;
    [SerializeField] float timer;
    [SerializeField] bool randomPos;
    [SerializeField] bool randomEnemy;

    //private fields
    bool isSpawning;
    bool startSpawning;
    int enemiesSpawned;
    int enemiesCounter;
    int positionsCounter;
    GameObject spawnedEnemy;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(startSpawning && !isSpawning && enemiesSpawned < spawnCount)
        {
            StartCoroutine(spawn());
        }
    }

    IEnumerator spawn()
    {
        isSpawning = true;

        if(randomPos && randomEnemy)
        {
            spawnedEnemy = enemy[Random.Range(0, enemy.Length)];
            Instantiate(spawnedEnemy, 
                        spawnPos[Random.Range(0, spawnPos.Length)].position,
                        spawnedEnemy.transform.rotation);

            enemiesSpawned++;
        }

        if(!randomPos && randomEnemy)
        {
            if(positionsCounter == spawnPos.Length)
                positionsCounter = 0;

            spawnedEnemy = enemy[Random.Range(0, enemy.Length)];
            Instantiate(spawnedEnemy, 
                        spawnPos[positionsCounter].position,
                        spawnedEnemy.transform.rotation);
                
            positionsCounter++;
            enemiesSpawned++;
        }

        if(randomPos && !randomEnemy)
        {
            if(enemiesCounter == enemy.Length)
                enemiesCounter = 0;

            Instantiate(enemy[enemiesCounter],
                        spawnPos[Random.Range(0, spawnPos.Length)].position,
                        enemy[enemiesCounter].transform.rotation);

            enemiesCounter++;
            enemiesSpawned++;
        }

        if(!randomPos && !randomEnemy)
        {
            if(positionsCounter == spawnPos.Length)
                positionsCounter = 0;
            
            if(enemiesCounter == enemy.Length)
                enemiesCounter = 0;

            Instantiate(enemy[enemiesCounter],
                        spawnPos[positionsCounter].position,
                        enemy[enemiesCounter].transform.rotation);
            
            positionsCounter++;
            enemiesCounter++;
            enemiesSpawned++;
        }

        yield return new WaitForSeconds(timer);

        isSpawning = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            startSpawning = true;
    }
}
