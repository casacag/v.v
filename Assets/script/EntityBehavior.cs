using UnityEngine;

public class EntityBehavior : MonoBehaviour
{
    public enum EntityType { Virtue, Vice }
    [Header("Tipo di Entit�")]
    public EntityType entityType = EntityType.Virtue;

    [Header("Impostazioni Movimento")]
    public float speed = 3f;          // Velocit� di movimento
    public float triggerDistance = 5f; // Distanza alla quale inizia il comportamento

    [Header("Soglia per l'inseguimento")]
    public int chaseThreshold = 3;    // Numero di entit� raccolte (virt� o vizi) per far inseguire i vizi

    [Header("Riferimenti")]
    public Transform player;          // Riferimento al transform del player

    // Altezza base per mantenere il movimento sul piano XZ
    private float baseY;

    // Riferimento al GameManager per leggere i contatori
    private GameManager gm;

    void Start()
    {
        baseY = transform.position.y;

        // Se il player non � assegnato manualmente, cerchiamolo tramite tag "Player"
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        // Trova il GameManager nella scena
        gm = GameObject.FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (player == null)
            return;

        // Calcola le posizioni sul piano XZ (ignorando Y)
        Vector3 entityPosXZ = new Vector3(transform.position.x, baseY, transform.position.z);
        Vector3 playerPosXZ = new Vector3(player.position.x, baseY, player.position.z);
        Vector3 diff = playerPosXZ - entityPosXZ;
        float distance = diff.magnitude;

        // Comportamento in base al tipo di entit�
        if (entityType == EntityType.Virtue)
        {
            // Le virt� scappano se il player si avvicina troppo
            if (distance < triggerDistance)
            {
                Vector3 fleeDirection = -diff.normalized;
                Vector3 newPos = entityPosXZ + fleeDirection * speed * Time.deltaTime;
                transform.position = new Vector3(newPos.x, baseY, newPos.z);
            }
        }
        else if (entityType == EntityType.Vice)
        {
            bool chaseCondition = false;
            // Se il GameManager � presente, controlla i contatori
            if (gm != null)
            {
                // I vizi inizialmente fuggono; se si raccolgono almeno "chaseThreshold" virt� o vizi, passano a inseguire
                if (gm.collectedVirtuesCount >= chaseThreshold || gm.collectedVicesCount >= chaseThreshold)
                {
                    chaseCondition = true;
                }
            }
            // Se il chaseCondition � vera, inseguono il player, altrimenti si comportano come se fuggissero
            if (chaseCondition)
            {
                // Comportamento di inseguimento: muoviti verso il player
                Vector3 chaseDirection = diff.normalized;
                Vector3 newPos = entityPosXZ + chaseDirection * speed * Time.deltaTime;
                transform.position = new Vector3(newPos.x, baseY, newPos.z);
            }
            else
            {
                // Comportamento di fuga: scappa se il player � troppo vicino
                if (distance < triggerDistance)
                {
                    Vector3 fleeDirection = -diff.normalized;
                    Vector3 newPos = entityPosXZ + fleeDirection * speed * Time.deltaTime;
                    transform.position = new Vector3(newPos.x, baseY, newPos.z);
                }
            }
        }
    }

    // Quando l'entit� entra in contatto con il player, notifica il GameManager e distruggi l'oggetto
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





