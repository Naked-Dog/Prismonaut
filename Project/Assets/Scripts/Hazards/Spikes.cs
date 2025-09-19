using System;
using System.Collections;
using System.Collections.Generic;
using PlayerSystem;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] private int damage = 3;
    [SerializeField] private bool willWarp = true;
    [SerializeField] private bool willDestroy = false;
    public event Action onSpikeDestroy;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerBaseModule>()?.healthModule.SpikeDamage(damage, willWarp);
        }
        if (willDestroy && !collision.gameObject.CompareTag("SpikeD")) DestroySpike();
    }
    public void DestroySpike()
    {
        onSpikeDestroy.Invoke();
        Destroy(gameObject);
    }

}
