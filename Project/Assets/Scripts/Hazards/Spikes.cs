using System;
using DG.Tweening;
using PlayerSystem;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] private int damage = 3;
    [SerializeField] private bool willWarp = true;
    [SerializeField] private bool willDestroy = false;
    [SerializeField] public bool isLava = false;

    public event Action onSpikeDestroy;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerBaseModule>()?.healthModule.HazardDamage(damage, willWarp);
            if (isLava)
            {
                DOVirtual.DelayedCall(0.25f, () =>
                {
                    LavaManager.Instance.Reset();
                    PlatformManager.Instance.StartSequence();
                }, false);
            }
        }
        if (willDestroy && !collision.gameObject.CompareTag("SpikeD")) DestroySpike();
    }

    public void DestroySpike()
    {
        onSpikeDestroy.Invoke();
        Destroy(gameObject);
    }
}
