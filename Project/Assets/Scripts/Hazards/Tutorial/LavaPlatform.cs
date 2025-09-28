using System.Collections;
using UnityEngine;

public class LavaPlatform : MonoBehaviour
{
    [SerializeField] private Vector2 initialPos;
    [SerializeField] private Vector2 finalPos;
    [SerializeField] private float platformSize;
    [SerializeField] private int platformIndex;
    [SerializeField] private float timeToMove;
    private Coroutine positionCoroutine = null;
    private Coroutine timeCoroutine = null;
    private PlatformManager platformManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void InitPlatform(PlatformManager platformManager, int amountOfPlatforms)
    {
        this.platformManager = platformManager;
        initialPos = transform.position;
        finalPos = new Vector2(initialPos.x, initialPos.y - platformSize);
        ResetTimeToMove(amountOfPlatforms);
    }

    public void ResetTimeToMove(int amountOfPlatforms)
    {
        timeToMove = amountOfPlatforms + platformIndex;
    }

    public void SetPlatformStart()
    {
        transform.position = initialPos;
        if (positionCoroutine != null)
            StopCoroutine(positionCoroutine);
        positionCoroutine = StartCoroutine(MovePlatform());
    }

    private IEnumerator MovePlatform()
    {
        float elapsedTime = 0f;
        while (elapsedTime < timeToMove)
        {
            float t = elapsedTime / timeToMove;

            transform.position = Vector2.Lerp(initialPos, finalPos, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = finalPos;
    }

    public void UpdateMovement(int currentPlatform)
    {
        //Has To Fall Quickly
        if (currentPlatform > platformIndex) return;

        if (timeCoroutine != null)
            StopCoroutine(timeCoroutine);

        float timeStart = timeToMove;
        float timeTarget = Mathf.Max(timeToMove - (currentPlatform / 3), 0.5f);

        timeCoroutine = StartCoroutine(UpdateTime(timeStart, timeTarget));
    }

    private IEnumerator UpdateTime(float timeStart, float timeTarget, float timeToUpdate = 1)
    {
        float elapsedTime = 0f;
        while (elapsedTime < timeToUpdate)
        {
            float t = elapsedTime / timeToUpdate;
            timeToMove = Mathf.Lerp(timeStart, timeTarget, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public int GetPlatformIndex() => platformIndex;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            platformManager.PlayerSteppedOnPlatform(this);
        }
    }
}
