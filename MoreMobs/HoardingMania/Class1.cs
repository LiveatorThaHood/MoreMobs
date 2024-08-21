using UnityEngine;
using BepInEx;
using HarmonyLib;
using Unity.Netcode;

[BepInPlugin("MoreMobs", "More Mobs", "1.0.0")]
public class ManEaterMania : BaseUnityPlugin
{
    void Awake()
    {
        var harmony = new Harmony("com.moremobs.moremobs");
        harmony.PatchAll();
        Debug.Log("[MOREMOBS] Successfully Initialized!");
    }

    public void LogInfo(string message)
    {
        Logger.LogInfo(message);
    }
}

[HarmonyPatch(typeof(RoundManager))]
[HarmonyPatch("SpawnEnemyOnServer")]
public static class SpawnEnemyOnServerPatch
{
    static void Prefix(Vector3 spawnPosition, float yRot, int enemyNumber = -1)
    {
        Debug.Log("[MOREMOBS] Spawn Function Called!");
        for (int i = 0; i <= 7; i++)
        {
            Debug.Log("[MOREMOBS] Spawned Entity " + i + " Times!");
            if (RoundManager.Instance.IsServer)
            {
                RoundManager.Instance.SpawnEnemyServerRpc(spawnPosition, yRot, enemyNumber);
            }
            RoundManager.Instance.SpawnEnemyGameObject(spawnPosition, yRot, enemyNumber, null);
        }
    }
}
public static class SpawnEnemyGameObjectPatch
{
    static NetworkObjectReference Prefix(Vector3 spawnPosition, float yRot, int enemyNumber, EnemyType enemyType = null)
    {
        Debug.Log("[MOREMOBS] Spawn Function Called!");

        RoundManager roundManager = RoundManager.Instance;
        NetworkObjectReference lastSpawned = new NetworkObjectReference();

        for (int i = 0; i < 8; i++)
        {
            Debug.Log("[MOREMOBS] Spawned Entity " + i + " Times!");
            if (!roundManager.IsServer)
            {
                return roundManager.currentLevel.Enemies[0].enemyType.enemyPrefab.GetComponent<NetworkObject>();
            }

            if (enemyType != null)
            {
                GameObject gameObject = RoundManager.Instantiate<GameObject>(enemyType.enemyPrefab, spawnPosition, Quaternion.Euler(new Vector3(0f, yRot, 0f)));
                gameObject.GetComponentInChildren<NetworkObject>().Spawn(true);
                roundManager.SpawnedEnemies.Add(gameObject.GetComponent<EnemyAI>());
                lastSpawned = gameObject.GetComponentInChildren<NetworkObject>();
                continue;
            }

            int index = enemyNumber;
            if (enemyNumber == -1)
            {
                index = UnityEngine.Random.Range(0, roundManager.currentLevel.Enemies.Count);
            }
            else if (enemyNumber == -2)
            {
                GameObject gameObject2 = RoundManager.Instantiate<GameObject>(roundManager.currentLevel.DaytimeEnemies[UnityEngine.Random.Range(0, roundManager.currentLevel.DaytimeEnemies.Count)].enemyType.enemyPrefab, spawnPosition, Quaternion.Euler(new Vector3(0f, yRot, 0f)));
                gameObject2.GetComponentInChildren<NetworkObject>().Spawn(true);
                roundManager.SpawnedEnemies.Add(gameObject2.GetComponent<EnemyAI>());
                lastSpawned = gameObject2.GetComponentInChildren<NetworkObject>();
                continue;
            }
            else if (enemyNumber == -3)
            {
                GameObject gameObject3 = RoundManager.Instantiate<GameObject>(roundManager.currentLevel.OutsideEnemies[UnityEngine.Random.Range(0, roundManager.currentLevel.OutsideEnemies.Count)].enemyType.enemyPrefab, spawnPosition, Quaternion.Euler(new Vector3(0f, yRot, 0f)));
                gameObject3.GetComponentInChildren<NetworkObject>().Spawn(true);
                roundManager.SpawnedEnemies.Add(gameObject3.GetComponent<EnemyAI>());
                lastSpawned = gameObject3.GetComponentInChildren<NetworkObject>();
                continue;
            }

            GameObject gameObject4 = RoundManager.Instantiate<GameObject>(roundManager.currentLevel.Enemies[index].enemyType.enemyPrefab, spawnPosition, Quaternion.Euler(new Vector3(0f, yRot, 0f)));
            gameObject4.GetComponentInChildren<NetworkObject>().Spawn(true);
            roundManager.SpawnedEnemies.Add(gameObject4.GetComponent<EnemyAI>());
            lastSpawned = gameObject4.GetComponentInChildren<NetworkObject>();
        }

        return lastSpawned;
    }
}




/*

*/