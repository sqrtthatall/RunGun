using Unity.VisualScripting;
using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    [SerializeField] private Rigidbody2D[] targetRigidbodies;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            foreach (Rigidbody2D rb in targetRigidbodies)
            {
                if (rb != null)
                {
                    rb.bodyType = RigidbodyType2D.Dynamic;
                }
            }
        }
    }
}
