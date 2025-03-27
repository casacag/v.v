using UnityEngine;
using TMPro;
using System.Collections;

public class SecondaryMissionManager : MonoBehaviour
{
    public static SecondaryMissionManager Instance { get; private set; }

    [Header("Impostazioni Missione")]
    public float missionTime = 60f;            // Tempo totale in secondi per completare la missione
    private float currentTime;                 // Tempo rimanente

    [Header("UI")]
    public TMP_Text timerText;                 // Componente TextMeshPro per il timer
    public TMP_Text missionInstructionText;    // Messaggio testuale per le istruzioni e i risultati

    [Header("Indicatore Destinazione")]
    public GameObject destinationPoint;        // Punto di destinazione che si illumina

    [Header("Riferimenti Player")]
    public Transform playerTransform;          // Riferimento al transform del player
    public float destinationThreshold = 2f;      // Distanza minima per considerare la missione completata

    [Header("Oggetto Aggiuntivo")]
    public GameObject additionalObject;        // Oggetto da attivare quando la missione parte

    private bool missionActive = false;        // Flag che indica se la missione è attiva

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        // Disattiva inizialmente la UI, il punto di destinazione e l'oggetto aggiuntivo
        if (timerText != null)
            timerText.gameObject.SetActive(false);
        if (missionInstructionText != null)
            missionInstructionText.gameObject.SetActive(false);
        if (destinationPoint != null)
            destinationPoint.SetActive(false);
        if (additionalObject != null)
            additionalObject.SetActive(false);
    }

    void Update()
    {
        if (missionActive)
        {
            // Aggiorna il timer
            currentTime -= Time.deltaTime;
            UpdateTimerUI();

            // Verifica se il player ha raggiunto la destinazione
            if (Vector3.Distance(playerTransform.position, destinationPoint.transform.position) <= destinationThreshold)
            {
                MissionSuccess();
            }

            // Se il tempo scade, la missione fallisce
            if (currentTime <= 0)
            {
                MissionFailed();
            }
        }
    }

    // Aggiorna il testo del timer
    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = "Tempo: " + Mathf.Ceil(currentTime).ToString() + " s";
        }
    }

    // Attiva la missione: imposta il timer, mostra la UI, il punto di destinazione, le istruzioni e attiva l'oggetto aggiuntivo
    public void ActivateMission()
    {
        missionActive = true;
        currentTime = missionTime;

        if (timerText != null)
            timerText.gameObject.SetActive(true);

        if (missionInstructionText != null)
        {
            missionInstructionText.gameObject.SetActive(true);
            missionInstructionText.text = "Raggiungi la destinazione!";
            StartCoroutine(HideInstructionAfterDelay(10f));
        }

        if (destinationPoint != null)
            destinationPoint.SetActive(true);

        if (additionalObject != null)
            additionalObject.SetActive(true);

        Debug.Log("Missione secondaria attivata!");
    }

    // Nasconde il messaggio di istruzioni dopo un delay (in secondi)
    IEnumerator HideInstructionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (missionInstructionText != null)
        {
            missionInstructionText.gameObject.SetActive(false);
        }
    }

    // Chiamata quando la missione viene completata con successo
    void MissionSuccess()
    {
        missionActive = false;
        if (timerText != null)
            timerText.gameObject.SetActive(false);
        if (missionInstructionText != null)
        {
            missionInstructionText.gameObject.SetActive(true);
            missionInstructionText.text = "Missione completata con successo!";
        }
        if (destinationPoint != null)
            destinationPoint.SetActive(false);
        Debug.Log("Missione completata con successo!");
    }

    // Chiamata quando il tempo scade e la missione fallisce
    void MissionFailed()
    {
        missionActive = false;
        if (timerText != null)
            timerText.gameObject.SetActive(false);
        if (missionInstructionText != null)
        {
            missionInstructionText.gameObject.SetActive(true);
            missionInstructionText.text = "Missione fallita!";
        }
        if (destinationPoint != null)
            destinationPoint.SetActive(false);
        Debug.Log("Missione fallita!");
    }
}
