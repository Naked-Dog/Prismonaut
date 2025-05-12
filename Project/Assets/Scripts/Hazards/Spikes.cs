using System.Collections;
using System.Collections.Generic;
using PlayerSystem;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] private int damage = 3;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerBaseModule>()?.healthModule.SpikeDamage(damage);
        }
    }


}
