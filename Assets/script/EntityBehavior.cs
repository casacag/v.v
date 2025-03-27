using UnityEngine;
using UnityEngine.AI;

public class EntityBehavior : MonoBehaviour
{
    public enum EntityType { Virtue, Vice }

    [Header("Tipo di Entità")]
    public EntityType entityType = EntityType.Virtue;

    [Header("Impostazioni Movimento")]
    public float speed = 3f;          // Velocità di movimento
    public float triggerDistance = 5f; // Distanza alla quale inizia il comportamento

    [Header("Soglia per l'inseguimento")]
    public int chaseThreshold = 3;    // Numero di entità raccolte per far inseguire i vizi

    [Header("Riferimenti")]
    public Transform player;          // Riferimento al transform del player
    private NavMeshAgent agent;
    private GameManager gm;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        gm = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (player == null || agent == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (entityType == EntityType.Virtue)
        {
            // Le virtù scappano sempre dal player
            FleeFromPlayer();
        }
        else if (entityType == EntityType.Vice)
        {
            bool chaseCondition = gm != null && (gm.collectedVirtuesCount >= chaseThreshold || gm.collectedVicesCount >= chaseThreshold);

            if (chaseCondition)
            {
                ChasePlayer();
            }
            else if (distance < triggerDistance)
            {
                FleeFromPlayer();
            }
            else
            {
                Wander(); // Movimento casuale se non c'è il player vicino
            }
        }
    }

    void ChasePlayer()
    {
        if (player != null)
            agent.SetDestination(player.position);
    }

    void FleeFromPlayer()
    {
        Vector3 fleeDirection = (transform.position - player.position).normalized;
        Vector3 fleeTarget = transform.position + fleeDirection * triggerDistance;
        agent.SetDestination(fleeTarget);
    }

    void Wander()
    {
        if (!agent.hasPath || agent.remainingDistance < 1f)
        {
            Vector3 randomDirection = Random.insideUnitSphere * triggerDistance;
            randomDirection += transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, triggerDistance, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gm != null)
                gm.RemoveEntity(gameObject, entityType);
            Destroy(gameObject);
        }
    }
}

