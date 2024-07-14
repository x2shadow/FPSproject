using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemySpawnContoller : MonoBehaviour
{
    public int initialEnemiesPerWave = 5;
    public int currentEnemiesPerWave;

    public float spawnDelay = 0.5f;

    public int currentWave = 0;
    public float waveCooldown = 10.0f;

    public bool inCooldown;
    public float cooldownCounter = 0; 

    public List<Enemy> currentEnemiesAlive;

    public GameObject enemyPrefab;

    private void Start()
    {
        currentEnemiesPerWave = initialEnemiesPerWave;

        StartNextWave();
    }

    private void StartNextWave()
    {
        currentEnemiesAlive.Clear();

        currentWave++;

        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < currentEnemiesPerWave; i++)
        {
            // Generate a random offset within a specified range
            Vector3 spawnOffset = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
            Vector3 spawnPosition = transform.position + spawnOffset;

            // Instantiate the Enemy
            var enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            // Get Enemy script
            Enemy enemyScript  = enemy.GetComponent<Enemy>();

            // Track this enemy
            currentEnemiesAlive.Add(enemyScript);

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void Update()
    {
        // Get all dead enemies
        List<Enemy> enemiesToRemove = new List<Enemy>();

        foreach (Enemy enemy in currentEnemiesAlive)
        {
            if (enemy.isDead)
            {
                enemiesToRemove.Add(enemy);
            }
        }

        // Actually remove all dead enemies
        foreach (Enemy enemy in enemiesToRemove)
        {
            currentEnemiesAlive.Remove(enemy);
        }

        enemiesToRemove.Clear();

        // Start Cooldown if all enemies are dead
        if (currentEnemiesAlive.Count == 0 && inCooldown == false)
        {
            // Start cooldown for next wave
            StartCoroutine(WaveCooldown());
        }

        // Run the cooldown counter
        if (inCooldown)
        {
            cooldownCounter -= Time.deltaTime;
        }
        else
        {
            // Reset the counter
            cooldownCounter = waveCooldown;
        }
    }

    private IEnumerator WaveCooldown()
    {
        inCooldown = true;

        yield return new WaitForSeconds(waveCooldown);

        inCooldown = false;

        currentEnemiesPerWave *= 2;

        StartNextWave();
    }
}
