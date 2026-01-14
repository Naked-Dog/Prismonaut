using DG.Tweening;
using UnityEngine;

public class LavaHazard : HazardBase
{
    protected override void OnHitPlayer(GameObject player)
    {
        base.OnHitPlayer(player);
        DOVirtual.DelayedCall(0.25f, () =>
        {
            if (!LavaManager.Instance.IsFinished)
            {
                LavaManager.Instance.Reset();
                LavaManager.Instance.StartLava();
                PlatformManager.Instance.StartSequence();
            }
        });

    }
}
