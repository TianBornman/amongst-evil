using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameMode { Waves, Timer }

public class SpawnManager : Singleton<SpawnManager>
{
    [Header("References")]
    public List<Encounter> encounters;
    public SpawnPoint partySpawnPoint;

    [Header("Settings")]
    public GameMode gameMode = GameMode.Waves;
    public int totalWaves = 10;
    public float spawnRadius = 28f;
    public float minSpawnRadius = 18f;

    // Public Variables
    [HideInInspector] public List<Character> spawnedCharacters = new();
    [HideInInspector] public int currentWave = 0;

    // Private Variables
    private int totalEnemies;

    // Public Methods
    public void RemoveCharacter(Character character)
    {
        spawnedCharacters.Remove(character);
        UiManager.Instance.SetEnemiesText(spawnedCharacters.Count, totalEnemies);

        if (spawnedCharacters.Count <= 0)
            OnWaveCleared();
    }

    // Private Methods
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (GameManager.Instance.AtHub)
            return;

        var spawnPoints = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
        partySpawnPoint = spawnPoints.FirstOrDefault(spawn => spawn.partySpawnPoint);

        spawnedCharacters.Clear();
        currentWave = 0;
        StartCoroutine(FirstWaveRoutine());
    }

    private IEnumerator FirstWaveRoutine()
    {
        yield return null; // party positions settle
        SpawnNextWave();
        yield return null; // enemy Start() runs
        AssignPartyTargets();
    }

    private IEnumerator NextWaveRoutine()
    {
        SpawnNextWave();
        yield return null; // enemy Start() runs
        AssignPartyTargets();
    }

    private void SpawnNextWave()
    {
        currentWave++;
        spawnedCharacters.Clear();

        if (encounters == null || encounters.Count == 0)
        {
            Debug.LogError("[SpawnManager] No encounters assigned in Inspector!");
            return;
        }

        int encounterCount = Mathf.Clamp(Mathf.CeilToInt(currentWave / 3f), 1, 4);
        float baseAngle = Random.Range(0f, 360f);

        for (int i = 0; i < encounterCount; i++)
        {
            var encounter = encounters[Random.Range(0, encounters.Count)];
            float angle = (baseAngle + 360f / encounterCount * i + Random.Range(-20f, 20f)) * Mathf.Deg2Rad;
            float radius = Random.Range(minSpawnRadius, spawnRadius);
            Vector3 partyPos = PartyManager.Instance.Center.position;
            Vector3 spawnCenter = partyPos + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
            encounter.Spawn(spawnCenter);
        }

        totalEnemies = spawnedCharacters.Count;
        UiManager.Instance.SetEnemiesText(spawnedCharacters.Count, totalEnemies);
        UiManager.Instance.SetWaveText(currentWave, totalWaves);
    }

    private void AssignPartyTargets()
    {
        var partyMembers = PartyManager.Instance.PartyMembers;
        if (partyMembers == null) return;

        foreach (var enemy in spawnedCharacters)
        {
            foreach (var member in partyMembers)
                if (member.IsAlive && !enemy.targets.Contains(member))
                    enemy.targets.Add(member);

            enemy.ReevaluateTarget();

            if (enemy.target != null)
                enemy.SetState(new MoveState(enemy));
        }
    }

    private void OnWaveCleared()
    {
        if (currentWave >= totalWaves)
        {
            PartyManager.Instance.CalculateStats();
            UiManager.Instance.ShowVictoryScreen();
        }
        else
            StartCoroutine(NextWaveRoutine());
    }
}
