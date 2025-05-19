using StarterAssets;
using System.Collections.Generic;
using TMPro; // Assicurati di includere il namespace del ThirdPersonController
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Liste Entità")]
    public List<GameObject> virtuesList;
    public List<GameObject> vicesList;
    public int collectedVirtuesCount = 0;
    public int collectedVicesCount = 0;


    [Header("Barre UI")]
    public Slider serotoninBar;
    public Slider dopamineBar;

    [Header("Valori di Evoluzione")]
    public float serotonin = 0f;
    public float dopamine = 0f;
    public float maxBarValue = 100f;

    [Header("Incrementi/Decrementi per Entità Raccolta")]
    public float serotoninIncrement = 20f;
    public float dopamineIncrement = 20f;
    public float serotoninDecrement = 10f;
    public float dopamineDecrement = 10f;

    [Header("Bonus sul Player")]
    // Bonus che vengono applicati se tutte le virtù o tutti i vizi sono raccolti.
    public float bonusSpeed = 2f;       // Incremento di MoveSpeed e SprintSpeed per i vizi
    public float bonusJumpHeight = 0.5f;  // Incremento di JumpHeight per le virtù

    private ThirdPersonController thirdPersonController; // Riferimento al controller del player


    //gestione delle monete
    [Header("Manager Monete")]
    public int startingCoinCount = 0;
    public int targetCountCoin = 5;
    public static byte currentCointCount;
    [SerializeField]
    private TextMeshProUGUI cointText;

    void Start()
    {
        // Se le liste non sono assegnate manualmente, troviamo le entità tramite tag
        if (virtuesList == null || virtuesList.Count == 0)
        {
            GameObject[] virtues = GameObject.FindGameObjectsWithTag("Virtù");
            virtuesList = new List<GameObject>(virtues);
        }
        if (vicesList == null || vicesList.Count == 0)
        {
            GameObject[] vices = GameObject.FindGameObjectsWithTag("Vizio");
            vicesList = new List<GameObject>(vices);
        }

        // Configura le barre UI
        if (serotoninBar != null)
        {
            serotoninBar.maxValue = maxBarValue;
            serotoninBar.value = serotonin;
        }
        if (dopamineBar != null)
        {
            dopamineBar.maxValue = maxBarValue;
            dopamineBar.value = dopamine;
        }

        // Trova il player e ottieni il componente ThirdPersonController
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            thirdPersonController = playerObj.GetComponent<ThirdPersonController>();
        }

        cointText.text = "0/" + targetCountCoin.ToString();
    }

    // Metodo chiamato da un'entità quando viene raccolta
    public void RemoveEntity(GameObject entity, EntityBehavior.EntityType type)
    {
        if (type == EntityBehavior.EntityType.Virtue)
        {
            if (virtuesList.Contains(entity))
            {
                virtuesList.Remove(entity);
                collectedVirtuesCount++;
                IncreaseSerotonin();
                DecreaseDopamine();
            }
        }
        else if (type == EntityBehavior.EntityType.Vice)
        {
            if (vicesList.Contains(entity))
            {
                vicesList.Remove(entity);
                collectedVicesCount++;
                IncreaseDopamine();
                DecreaseSerotonin();
            }
        }

        CheckAllCollected();
    }



    void IncreaseSerotonin()
    {
        serotonin += serotoninIncrement;
        if (serotonin > maxBarValue)
            serotonin = maxBarValue;
        if (serotoninBar != null)
            serotoninBar.value = serotonin;
    }

    void IncreaseDopamine()
    {
        dopamine += dopamineIncrement;
        if (dopamine > maxBarValue)
            dopamine = maxBarValue;
        if (dopamineBar != null)
            dopamineBar.value = dopamine;
    }

    void DecreaseSerotonin()
    {
        serotonin -= serotoninDecrement;
        if (serotonin < 0f)
            serotonin = 0f;
        if (serotoninBar != null)
            serotoninBar.value = serotonin;
    }

    void DecreaseDopamine()
    {
        dopamine -= dopamineDecrement;
        if (dopamine < 0f)
            dopamine = 0f;
        if (dopamineBar != null)
            dopamineBar.value = dopamine;
    }

    // Verifica se una delle liste è vuota: se sì, distrugge tutte le altre entità e applica bonus al player.
    void CheckAllCollected()
    {
        // Se una delle liste è vuota, distruggi tutte le entità residue
        if (virtuesList.Count == 0 || vicesList.Count == 0)
        {
            DestroyAllEntities();
            SecondaryMissionManager.Instance?.ActivateMission();
            SecondaryMissionManager sMM = FindObjectOfType<SecondaryMissionManager>();
            if (sMM != null)
            {
                sMM.ActivateMission();
                Debug.Log("Ho avviato missione secondaria");
            }
            else
            {
                Debug.Log("Non trovo oggetto SecondaryMissionManager");
            }

        }

        // Se tutte le virtù sono state raccolte, porta la barra della serotonina al massimo
        // e applica un bonus al salto del player.
        if (virtuesList.Count == 0)
        {
            serotonin = maxBarValue;
            if (serotoninBar != null)
                serotoninBar.value = serotonin;

            if (thirdPersonController != null)
            {
                thirdPersonController.JumpHeight += bonusJumpHeight;
                Debug.Log("Bonus salto applicato: " + bonusJumpHeight);
            }
        }

        // Se tutti i vizi sono stati raccolti, porta la barra della dopamina al massimo
        // e applica un bonus alla velocità del player.
        if (vicesList.Count == 0)
        {
            dopamine = maxBarValue;
            if (dopamineBar != null)
                dopamineBar.value = dopamine;

            if (thirdPersonController != null)
            {
                thirdPersonController.MoveSpeed += bonusSpeed;
                thirdPersonController.SprintSpeed += bonusSpeed;
                Debug.Log("Bonus velocità applicato: " + bonusSpeed);
            }
        }

    }

    // Distrugge tutte le entità residue presenti in scena (sia virtù che vizi)
    void DestroyAllEntities()
    {
        // Trova e distruggi tutte le virtù
        GameObject[] remainingVirtues = GameObject.FindGameObjectsWithTag("Virtù");
        foreach (GameObject v in remainingVirtues)
        {
            Destroy(v);
        }

        // Trova e distruggi tutti i vizi
        GameObject[] remainingVices = GameObject.FindGameObjectsWithTag("Vizio");
        foreach (GameObject v in remainingVices)
        {
            Destroy(v);
        }
    }
    public void UpdateCoinCount()
    {
        currentCointCount++;
        cointText.text = currentCointCount.ToString() + "/" + targetCountCoin.ToString();
        CheckCoinCount();
        Debug.Log(currentCointCount);
    }

    private void CheckCoinCount()
    {
        if (currentCointCount == targetCountCoin) cointText.text = "All Coin Collected!";
    }
}



