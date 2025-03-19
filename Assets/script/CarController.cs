using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    public float acceleration = 500f;   // Forza di accelerazione
    public float maxSpeed = 20f;       // Velocit� massima
    public float turnSpeed = 5f;       // Velocit� di sterzata
    public float brakeForce = 1000f;   // Forza frenante
    public bool isMobile = false;      // Attiva i controlli touch per dispositivi mobili

    private Rigidbody rb;
    private float inputVertical;
    private float inputHorizontal;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0); // Abbassa il centro di massa per stabilit�
    }

    void Update()
    {
        if (isMobile)
        {
            HandleMobileInput();
        }
        else
        {
            HandlePCInput();
        }

        Debug.Log($"Vertical Input: {inputVertical}");
    }


    void FixedUpdate()
    {
        // Movimento in avanti/indietro
        if (inputVertical != 0 && rb.linearVelocity.magnitude < maxSpeed)
        {
            rb.AddForce(transform.forward * inputVertical * acceleration * Time.fixedDeltaTime, ForceMode.Acceleration);
        }

        // Sterzata
        if (inputHorizontal != 0)
        {
            float turn = inputHorizontal * turnSpeed * Time.fixedDeltaTime;
            Quaternion rotation = Quaternion.Euler(0, turn, 0);
            rb.MoveRotation(rb.rotation * rotation);
        }

        // Frenata naturale (decelerazione)
        if (inputVertical == 0)
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, 0.05f);
        }
    }


    void HandlePCInput()
    {
        inputVertical = Input.GetAxis("Vertical");
        inputHorizontal = Input.GetAxis("Horizontal");
    }

    void HandleMobileInput()
    {
        // Usa il sistema di input touch per accelerare e sterzare
        inputVertical = 0f;
        inputHorizontal = 0f;

        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.position.x < Screen.width / 2) // Lato sinistro per accelerare
                {
                    inputVertical = 1f;
                }
                else if (touch.position.x > Screen.width / 2) // Lato destro per sterzare
                {
                    if (touch.position.y > Screen.height / 2)
                        inputHorizontal = 1f; // Sterza a destra
                    else
                        inputHorizontal = -1f; // Sterza a sinistra
                }
            }
        }
    }
}