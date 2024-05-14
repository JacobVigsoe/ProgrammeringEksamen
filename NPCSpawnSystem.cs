using System.Collections;
using UnityEngine;

public class NPCSpawnSystem : MonoBehaviour
{
    public GameObject[] npcPrefabs; // Array of NPC prefabs to spawn
    //private NPCSystem npcSystem;
    public Transform[] spawnPoints; // Array of spawn points

    public Transform playerTransform; // Reference to the player's transform
    public float despawnDistance = 50f; // Distance at which road segments are despawned

    public float spawnInterval = 2f; // Time interval between spawns
    private float nextSpawnTime; // Time for the next spawn

    public float spawnedNPCAmount = 0;


    void Start()
    {
        //npcSystem = GameObject.FindAnyObjectByType<NPCSystem>();
        nextSpawnTime = Time.time + spawnInterval; // Set initial spawn time
    }

    void Update()
    {
        // Check if it's time to spawn a new NPC
        if (Time.time >= nextSpawnTime)
        {
            SpawnNPC(); // Spawn a new NPC
            nextSpawnTime = Time.time + spawnInterval; // Set time for the next spawn
           
        }

        if (spawnedNPCAmount == 10)
        {
            StartCoroutine(SpawnNpcDoubleLane());

        }

        // Despawn road segments that are too far behind the player
        DespawnOldNpc();
    }

    private IEnumerator SpawnNpcDoubleLane()
    {

        //npcSystem.randomLane1 = 3;
        //npcSystem.randomLane2 = 3;
        SpawnNPC(); // Spawn a new NPC
        yield return new WaitForSeconds(0.1f);
        //npcSystem.randomLane1 = 2;
        //npcSystem.randomLane2 = 2;
        SpawnNPC(); // Spawn a new NPC
        spawnedNPCAmount = 0;
        yield return new WaitForSeconds(0.1f);

    }

    void SpawnNPC()
    {
        float randomTime = Random.Range(0, 350);
        // Choose a random NPC prefab from the npcPrefabs array
        GameObject npcPrefab = npcPrefabs[Random.Range(0, npcPrefabs.Length)];

        // Choose a random spawn point from the spawnPoints array
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Spawn the NPC at the chosen spawn point with a rotation of 180 degrees
        GameObject spawnedNPC = Instantiate(npcPrefab, spawnPoint.position, Quaternion.Euler(0f, 180f, 0f));
        spawnedNPCAmount ++;
    }

    void DespawnOldNpc()
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
