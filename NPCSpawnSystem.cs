using UnityEngine;
using System.Collections;

public class NPCSpawnSystem : MonoBehaviour
{
    [SerializeField] private GameObject[] npcPrefabs; // Array of NPC prefabs to spawn
    [SerializeField] private Transform[] spawnPoints; // Array of spawn points
    [SerializeField] private Transform playerTransform; // Reference to the player's transform
    [SerializeField] private float despawnDistance = 50f; // Distance at which road segments are despawned
    [SerializeField] private float spawnInterval = 2f; // Time interval between spawns
    [SerializeField] private int maxNPCsToSpawnInDoubleLane = 10; // Maximum number of NPCs to spawn before spawning in double lane

    private float nextSpawnTime; // Time for the next spawn
    private int spawnedNPCAmount; // Number of NPCs spawned since the last double lane spawn

    private void Start()
    {
        nextSpawnTime = Time.time + spawnInterval; // Set initial spawn time
    }

    private void Update()
    {
        SpawnNPCs();
        DespawnOldNPCs();
    }

    private void SpawnNPCs()
    {
        // Check if it's time to spawn a new NPC
        if (Time.time >= nextSpawnTime)
        {
            SpawnSingleNPC(); // Spawn a new NPC
            nextSpawnTime = Time.time + spawnInterval; // Set time for the next spawn

            // Check if the maximum number of NPCs has been reached
            if (++spawnedNPCAmount >= maxNPCsToSpawnInDoubleLane)
            {
                StartCoroutine(SpawnNPCsInDoubleLane());
            }
        }
    }

    private IEnumerator SpawnNPCsInDoubleLane()
    {
        SpawnSingleNPC(); // Spawn a new NPC in the first lane
        yield return new WaitForSeconds(0.1f);
        SpawnSingleNPC(); // Spawn a new NPC in the second lane
        spawnedNPCAmount = 0; // Reset the spawned NPC count
        yield return new WaitForSeconds(0.1f);
    }

    private void SpawnSingleNPC()
    {
        // Choose a random NPC prefab from the npcPrefabs array
        GameObject npcPrefab = npcPrefabs[Random.Range(0, npcPrefabs.Length)];

        // Choose a random spawn point from the spawnPoints array
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Spawn the NPC at the chosen spawn point with a rotation of 180 degrees
        Instantiate(npcPrefab, spawnPoint.position, Quaternion.Euler(0f, 180f, 0f));
    }

    private void DespawnOldNPCs()
    {
        // Iterate through all spawned NPCs
        foreach (GameObject npc in GameObject.FindGameObjectsWithTag("NPC"))
        {
            // Check if the NPC is too far behind the player
            if (npc.transform.position.z < playerTransform.position.z - despawnDistance)
            {
                Destroy(npc); // Despawn the NPC
            }
        }
    }
}