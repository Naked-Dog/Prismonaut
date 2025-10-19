using DG.Tweening;
using UnityEngine;

public class LavaHazard : HazardBase
{
    protected override void OnHitPlayer(GameObject player)
    {
        base.OnHitPlayer(player);
        if (LavaManager.Instance.eventFinished) return;
        DOVirtual.DelayedCall(0.25f, () =>
        {
            LavaManager.Instance.Reset();
            PlatformManager.Instance.StartSequence();
        });

    }
}
