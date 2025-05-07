using UnityEngine;

public class RegularSlime : BaseSlime
{
    protected override void DoOnReflect(BounceValues bv) 
    {
        bv.bounceImpulse = ss.maxBounceSpeed;
    }
}
