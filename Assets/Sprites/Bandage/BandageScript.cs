using Unity.VisualScripting;
using UnityEngine;

public class BandageScript : MonoBehaviour
{
    public static int healTo = 50;

    void Start()
    {
        if (Movement.collectedBandages.Contains(gameObject.name))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D colision)
    {
        if (colision.CompareTag("Player"))
        {
            Movement.RegisterCollectedBandage(gameObject.name);
            Movement.Heal(healTo);
            Destroy(gameObject);
            
        }
    }
}
