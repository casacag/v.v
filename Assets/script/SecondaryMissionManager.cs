using UnityEngine;
using TMPro;

public class SecondaryMissionManager : MonoBehaviour
{
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

    private bool missionActive = false;        // Flag che indica se la missione è attiva

    void Start()
    {
        // Disattiva inizialmente la UI e l'indicatore della destinazione
        if (timerText != null)
            timerText.gameObject.SetActive(false);
        if (missionInstructionText != null)
            missionInstructionText.gameObject.SetActive(false);
        if (destinationPoint != null)
            destinationPoint.SetActive(false);
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

    // Attiva la missione: imposta il timer, mostra la UI e il punto di destinazione e le istruzioni
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
        }
        if (destinationPoint != null)
            destinationPoint.SetActive(true);
        Debug.Log("Missione secondaria attivata!");
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

