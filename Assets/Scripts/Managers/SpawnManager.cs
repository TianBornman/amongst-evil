using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public enum GameMode { Waves, Timer }

public class SpawnManager : Singleton<SpawnManager>
{
    [Header("Enemy Prefabs")]
    public List<Character> enemyPrefabs;

    [Header("Wave Settings")]
    public GameMode gameMode = GameMode.Waves;
    public int totalWaves = 10;
    public int baseEnemyCount = 30;
    public int enemyCountScalingPerWave = 10;

    [Header("Spawn Settings")]
    public float initialSpawnInterval = 1.5f;
    public float minSpawnInterval = 0.12f;
    public float spawnRadius = 25f;
    public float minSpawnRadius = 15f;
    public float healthScalingPerWave = 0.3f;

    // Public
    [HideInInspector] public List<Character> spawnedCharacters = new();
    [HideInInspector] public int currentWave = 0;

    // Private
    private int totalEnemies;
    private int enemiesToSpawn;
    private int aliveEnemies;

    // Public Methods
    public void RemoveCharacter(Character character)
    {
        spawnedCharacters.Remove(character);
        aliveEnemies--;

        UpdateEnemyUI();

        if (enemiesToSpawn <= 0 && aliveEnemies <= 0)
            OnWaveCleared();
    }

    // Private Methods
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (GameManager.Instance.AtHub)
            return;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        spawnedCharacters.Clear();
        currentWave = 0;
        enemiesToSpawn = 0;
        aliveEnemies = 0;

        StartCoroutine(FirstWaveRoutine());
    }

    private IEnumerator FirstWaveRoutine()
    {
        yield return null; // party positions settle
        StartWave();
    }

    private void StartWave()
    {
        currentWave++;
        spawnedCharacters.Clear();
        aliveEnemies = 0;

        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            Debug.LogError("[SpawnManager] No enemy prefabs assigned in Inspector!");
            return;
        }

        totalEnemies = baseEnemyCount + (currentWave - 1) * enemyCountScalingPerWave;
        enemiesToSpawn = totalEnemies;

        UiManager.Instance.SetWaveText(currentWave, totalWaves);
        UpdateEnemyUI();

        StartCoroutine(SpawnWaveRoutine());
    }

    private IEnumerator SpawnWaveRoutine()
    {
        float interval = Mathf.Max(minSpawnInterval, initialSpawnInterval * Mathf.Pow(0.8f, currentWave - 1));

        while (enemiesToSpawn > 0)
        {
            Character enemy = SpawnOneEnemy();
            enemiesToSpawn--;

            yield return null; // let enemy Start() run

            if (enemy != null)
                AssignPartyTargets(enemy);

            UpdateEnemyUI();

            yield return new WaitForSeconds(interval);
        }
    }

    private Character SpawnOneEnemy()
    {
        Vector3 partyPos = PartyManager.Instance.Center.position;
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float radius = Random.Range(minSpawnRadius, spawnRadius);
        Vector3 candidate = partyPos + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);

        if (!NavMesh.SamplePosition(candidate, out NavMeshHit hit, 50f, NavMesh.AllAreas))
        {
            enemiesToSpawn++; // retry — don't lose the count
            return null;
        }

        var prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        var enemy = Instantiate(prefab, hit.position, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));

        float waveScale = 1f + (currentWave - 1) * healthScalingPerWave;
        enemy.baseStats.maxHealth *= waveScale;

        spawnedCharacters.Add(enemy);
        aliveEnemies++;

        return enemy;
    }

    private void AssignPartyTargets(Character enemy)
    {
        var partyMembers = PartyManager.Instance.PartyMembers;
        if (partyMembers == null) return;

        foreach (var member in partyMembers)
            if (member.IsAlive && !enemy.targets.Contains(member))
                enemy.targets.Add(member);

        enemy.ReevaluateTarget();

        if (enemy.target != null)
            enemy.SetState(new MoveState(enemy));
    }

    private void UpdateEnemyUI()
    {
        UiManager.Instance.SetEnemiesText(aliveEnemies + enemiesToSpawn, totalEnemies);
    }

    private void OnWaveCleared()
    {
        if (currentWave >= totalWaves)
        {
            PartyManager.Instance.CalculateStats();
            UiManager.Instance.ShowVictoryScreen();
        }
        else
            StartWave();
    }
}
