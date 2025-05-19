using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField]
    private Vector3 rotationDirection;
    private Rigidbody rb;

    private void Awake()
    {
        rb=GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Quaternion deltarotation = Quaternion.Euler(rotationDirection * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * deltarotation);
    }
}
