using System;
using UnityEngine;

public class SpikeGiveDamageSystem : MonoBehaviour
{

    public int spikeDamage = 30;

    private void OnTriggerEnter2D(Collider2D colision)
    {
        if (colision.CompareTag("Player"))
        {
            Movement.Damage(spikeDamage);
            Debug.Log(Movement.Health);
        }
    }

}
