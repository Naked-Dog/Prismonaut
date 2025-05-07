using System.Collections;
using UnityEngine;

public class BreakableSlime : BaseSlime
{
    [Header("Break Parameters")]
    [SerializeField] private GameObject halfA;
    [SerializeField] private GameObject halfB;
    [SerializeField] private float rejoinDelay = 2f;

    protected override void DoOnReflect(BounceValues bv)
    {
        if (currentRoutine != null && bv.rb)
        {
            bv.rb.bodyType = RigidbodyType2D.Dynamic;
            currentRoutine = null;
            busy.Remove(bv.rb);
        }
        StopAllCoroutines();
        halfA.SetActive(true);
        halfB.SetActive(true);

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        StartCoroutine(Rejoin());
    }

    private IEnumerator Rejoin()
    {
        yield return new WaitForSeconds(rejoinDelay);
        halfA.SetActive(false);
        halfB.SetActive(false);
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<BoxCollider2D>().enabled = true;
    }
}
