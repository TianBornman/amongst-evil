using System;
using System.Collections;
using System.Collections.Generic;
using Midevil.Mission;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public enum GameMode { Waves, Timer }

public class SpawnManager : Singleton<SpawnManager>
{
    [Header("Enemy Prefabs")]
    public List<Character> enemyPrefabs;

    [Header("Wave Settings (fallback when no Mission set)")]
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

    [Header("Mission")]
    public Relic relicPrefab;
    public float relicMinDistance = 30f;
    public float relicMaxDistance = 50f;
    public int relicPlacementAttempts = 30;

    // Public
    [HideInInspector] public List<Character> spawnedCharacters = new();
    [HideInInspector] public int currentWave = 0;

    public MissionConfig MissionConfig => missionConfig;
    public IMissionRunner MissionRunner => missionRunner;

    // Private
    private MissionConfig missionConfig;
    private IMissionRunner missionRunner;
    private int totalEnemies;
    private int enemiesToSpawn;
    private int aliveEnemies;
    private bool runEnded;

    // Public Methods
    public void RemoveCharacter(Character character)
    {
        spawnedCharacters.Remove(character);
        aliveEnemies--;

        UpdateEnemyUI();

        missionRunner?.OnEnemyDied(character);

        if (enemiesToSpawn <= 0 && aliveEnemies <= 0)
            OnWaveCleared();
    }

    public void PlaceRelic(Action onTouched)
    {
        if (relicPrefab == null)
        {
            Debug.LogError("[SpawnManager] Relic Recovery mission but no relicPrefab assigned!");
            return;
        }

        Vector3 partyPos = PartyManager.Instance.Center.position;

        for (int i = 0; i < relicPlacementAttempts; i++)
        {
            float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            float radius = UnityEngine.Random.Range(relicMinDistance, relicMaxDistance);
            Vector3 candidate = partyPos + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 50f, NavMesh.AllAreas))
            {
                var relic = Instantiate(relicPrefab, hit.position, Quaternion.identity);
                relic.onTouched = onTouched;
                return;
            }
        }

        Debug.LogError("[SpawnManager] Could not place relic on NavMesh after " + relicPlacementAttempts + " attempts.");
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
        runEnded = false;

        ConfigureMission();

        StartCoroutine(FirstWaveRoutine());
    }

    private void ConfigureMission()
    {
        var mission = PartyManager.Instance.CurrentMission;

        if (mission != null)
        {
            missionConfig = MissionConfig.Build(mission);
            missionRunner = MissionRunnerFactory.Create(mission.type);
        }
        else
        {
            missionConfig = new MissionConfig
            {
                mission = null,
                waveCount = totalWaves,
                baseEnemyCount = baseEnemyCount,
                enemyCountScalingPerWave = enemyCountScalingPerWave,
                healthMul = 1f,
                damageMul = 1f,
                spawnIntervalMul = 1f,
                chaosTimerSeconds = 300f
            };
            missionRunner = new PurgeMissionRunner();
        }

        missionRunner.Begin(this, missionConfig);

        UiManager.Instance.SetObjectiveText(missionRunner.ObjectiveText);
    }

    private void Update()
    {
        if (runEnded || missionRunner == null) return;
        if (GameManager.Instance.AtHub || GameManager.Instance.IsGamePaused) return;

        missionRunner.Tick(Time.deltaTime);

        if (missionRunner.ShouldEndRun(out bool victory))
            EndRun(victory);
    }

    private IEnumerator FirstWaveRoutine()
    {
        yield return null; // party positions settle
        StartWave();
    }

    private void StartWave()
    {
        if (runEnded) return;

        currentWave++;
        spawnedCharacters.Clear();
        aliveEnemies = 0;

        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            Debug.LogError("[SpawnManager] No enemy prefabs assigned in Inspector!");
            return;
        }

        totalEnemies = missionConfig.baseEnemyCount + (currentWave - 1) * missionConfig.enemyCountScalingPerWave;
        enemiesToSpawn = totalEnemies;

        UiManager.Instance.SetWaveText(currentWave, missionRunner.IsEndless ? -1 : missionConfig.waveCount);
        UiManager.Instance.SetObjectiveText(missionRunner.ObjectiveText);
        UpdateEnemyUI();

        StartCoroutine(SpawnWaveRoutine());
    }

    private IEnumerator SpawnWaveRoutine()
    {
        float baseInterval = initialSpawnInterval * Mathf.Pow(0.8f, currentWave - 1) * missionConfig.spawnIntervalMul;
        float interval = Mathf.Max(minSpawnInterval, baseInterval);

        while (enemiesToSpawn > 0 && !runEnded)
        {
            Character enemy = SpawnOneEnemy();
            enemiesToSpawn--;

            yield return null;

            if (enemy != null)
                AssignPartyTargets(enemy);

            UpdateEnemyUI();

            yield return new WaitForSeconds(interval);
        }
    }

    private Character SpawnOneEnemy()
    {
        Vector3 partyPos = PartyManager.Instance.Center.position;
        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
        float radius = UnityEngine.Random.Range(minSpawnRadius, spawnRadius);
        Vector3 candidate = partyPos + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);

        if (!NavMesh.SamplePosition(candidate, out NavMeshHit hit, 50f, NavMesh.AllAreas))
        {
            enemiesToSpawn++;
            return null;
        }

        var prefab = enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Count)];
        var enemy = Instantiate(prefab, hit.position, Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f));

        float waveScale = 1f + (currentWave - 1) * healthScalingPerWave;
        enemy.baseStats.maxHealth *= waveScale * missionConfig.healthMul;
        enemy.baseStats.damage *= missionConfig.damageMul;

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
        if (runEnded) return;

        missionRunner.OnWaveCleared();

        if (missionRunner.ShouldEndRun(out bool victory))
        {
            EndRun(victory);
            return;
        }

        StartWave();
    }

    private void EndRun(bool victory)
    {
        if (runEnded) return;
        runEnded = true;

        StopAllCoroutines();

        if (victory)
        {
            PartyManager.Instance.CalculateStats();
            UiManager.Instance.ShowVictoryScreen();
        }
    }
}
