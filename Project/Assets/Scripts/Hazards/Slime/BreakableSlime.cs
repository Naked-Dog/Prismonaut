using System.Collections;
using UnityEngine;

public class BreakableSlime : BaseSlime
{
    [SerializeField] private float rejoinDelay = 2f;

    protected override void DoOnReflect(BounceValues bv)
    {
        if (currentRoutine != null && bv != null && bv.rb)
        {
            bv.rb.bodyType = RigidbodyType2D.Dynamic;
            currentRoutine = null;
            busy.Remove(bv.rb);
        }
        StopAllCoroutines();

        anim.Play("Separate");

        StartCoroutine(Rejoin());
    }

    private IEnumerator Rejoin()
    {
        yield return new WaitForSeconds(rejoinDelay);

        anim.Play("Rejoin");
    }
}
