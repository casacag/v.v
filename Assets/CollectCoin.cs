using UnityEngine;

public class CollectCoin : MonoBehaviour
{
    GameManager gameManager;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.tag == "Coin")
        {
            Destroy(other.gameObject);            
            gameManager.UpdateCoinCount();
            Debug.Log("Ecco una moneta");
        }
    }
}
