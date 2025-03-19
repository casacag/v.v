using UnityEngine;
using UnityEngine.AI;

public class ZombieEscape : MonoBehaviour
{
    public Transform car; // L'auto da cui scappare
    public float escapeDistance = 10f; // Distanza minima per scappare
    public float randomMoveInterval = 2f; // Intervallo per il movimento casuale
    public float randomMoveRadius = 20f; // Raggio entro cui muoversi casualmente

    private NavMeshAgent agent;
    private float timeSinceLastRandomMove;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timeSinceLastRandomMove = 0f;
    }

    void Update()
    {
        float distanceToCar = Vector3.Distance(transform.position, car.position);

        // Se l'auto è entro la distanza di fuga, scappa
        if (distanceToCar < escapeDistance)
        {
            FleeFromCar();
        }
        else
        {
            // Se l'auto è lontana, fai muovere lo zombie in modo casuale
            MoveRandomly();
        }
    }

    void FleeFromCar()
    {
        // Calcola la direzione opposta rispetto all'auto
        Vector3 directionAway = transform.position - car.position;
        Vector3 targetPosition = transform.position + directionAway.normalized * escapeDistance;

        // Fai scappare lo zombie
        agent.SetDestination(targetPosition);
    }

    void MoveRandomly()
    {
        timeSinceLastRandomMove += Time.deltaTime;

        // Ogni 'randomMoveInterval' secondi, fai muovere lo zombie in una direzione casuale
        if (timeSinceLastRandomMove >= randomMoveInterval)
        {
            Vector3 randomDirection = Random.insideUnitSphere * randomMoveRadius;
            randomDirection.y = 0; // Assicurati che lo zombie si muova solo sul piano XZ

            Vector3 randomPosition = transform.position + randomDirection;
            agent.SetDestination(randomPosition);

            timeSinceLastRandomMove = 0f; // Resetta il timer per il prossimo movimento casuale
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        // Se lo zombie tocca il giocatore, distruggilo
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("STO TOCCANDO IL PLAYER");
            Destroy(gameObject); // Distruggi lo zombie
        }
    }
}


