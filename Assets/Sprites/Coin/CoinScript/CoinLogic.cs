using UnityEngine;

public class CoinLogic : MonoBehaviour
{
    private void Start()
    {

        if (Movement.collectedCoins.Contains(gameObject.name))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D colision)
    {
        if (colision.CompareTag("Player"))
        {
            Movement.RegisterCollectedCoin(gameObject.name);
            Movement.AddCoin();
            Destroy(gameObject);
            Debug.Log("Coin Added! " + Movement.GetCoinsValue().ToString());
        }
    }
}