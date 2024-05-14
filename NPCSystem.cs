using UnityEngine;
using System.Collections;

public class NPCSystem : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float explosionForce = 10f;
    [SerializeField] private GameObject explosionParticlesPrefab;
    [SerializeField] private GameObject lightLeft;
    [SerializeField] private GameObject lightRight;

    public float laneWidth = 2f;
    public float laneChangeSpeed = 50f;
    public int randomLaneStart = -1;
    public int randomLaneEnd = 3;
    public LayerMask npcLayer;

    private int currentLane = 1;
    private int targetLane = 1;
    private bool isCollidingWithTrigger;
    private bool isSwitchingLanes;
    private bool blinkLeft;
    private bool blinkRight;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        lightLeft.SetActive(false);
        lightRight.SetActive(false);

        int randomLaneIndex = Random.Range(randomLaneStart, randomLaneEnd);
        ChangeLane(randomLaneIndex - currentLane);
    }

    private void FixedUpdate()
    {
        MoveForward();
        HandleLaneChange();
    }

    private void MoveForward()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void HandleLaneChange()
    {
        if (currentLane != targetLane)
        {
            float targetX = (targetLane - 1) * laneWidth;
            float newX = Mathf.MoveTowards(transform.position.x, targetX, laneChangeSpeed * Time.deltaTime);
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);

            if (Mathf.Approximately(transform.position.x, targetX))
            {
                currentLane = targetLane;
            }
        }
        else if (!isSwitchingLanes && !isCollidingWithTrigger)
        {
            float randomTime = Random.Range(0, 350);

            if (randomTime == 250 && currentLane != 2)
            {
                StartCoroutine(BlinkAndChangeLane(1, true));
            }
            else if (randomTime == 125 && currentLane != 0)
            {
                StartCoroutine(BlinkAndChangeLane(-1, false));
            }
        }
    }

    private IEnumerator BlinkAndChangeLane(int laneChange, bool isLeft)
    {
        isSwitchingLanes = true;
        blinkLeft = isLeft;
        blinkRight = !isLeft;

        for (int i = 0; i < 4; i++)
        {
            if (blinkLeft)
            {
                lightLeft.SetActive(true);
            }
            else
            {
                lightRight.SetActive(true);
            }

            yield return new WaitForSeconds(0.5f);

            lightLeft.SetActive(false);
            lightRight.SetActive(false);

            yield return new WaitForSeconds(0.5f);
        }

        ChangeLane(laneChange);
        isSwitchingLanes = false;
    }

    private void ChangeLane(int direction)
    {
        int newLane = Mathf.Clamp(currentLane + direction, 0, 2);
        targetLane = newLane;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            isCollidingWithTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            isCollidingWithTrigger = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ambulance"))
        {
            AudioManageryTest.instance.PlaySFX("CrashHP");

            Vector3 explosionDirection = Random.onUnitSphere;
            explosionDirection.y = Mathf.Abs(explosionDirection.y);

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(explosionDirection * explosionForce, ForceMode.Impulse);
                GetComponent<Collider>().enabled = false;
            }

            if (explosionParticlesPrefab != null)
            {
                Instantiate(explosionParticlesPrefab, collision.contacts[0].point, Quaternion.identity);
                Destroy(explosionParticlesPrefab);
                gameManager.RemoveHP();
            }
        }
    }
}