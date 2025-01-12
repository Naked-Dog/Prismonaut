using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private GameObject DoorSprite;
    [SerializeField] private float movementDuration = 2f;

    private Animator animator;
    private Vector3 startPosition;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void Move()
    {
        startPosition = DoorSprite.transform.position;
        StartCoroutine(LerpToTarget());
    }

    private IEnumerator LerpToTarget()
    {
        float elapsedTime = 0f;

        while (elapsedTime < movementDuration)
        {
            float t = elapsedTime / movementDuration;
            DoorSprite.transform.position = Vector3.Lerp(startPosition, target.position, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the object reaches the exact target position at the end
        DoorSprite.transform.position = target.position;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        animator.Play("ClosingMovement");
    }
    public void OpenDoor()
    {
        animator.Play("OpeningMovement");
    }
}
