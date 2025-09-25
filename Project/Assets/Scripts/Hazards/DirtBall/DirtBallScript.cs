using System.Collections;
using System.Collections.Generic;
using PlayerSystem;
using UnityEngine;

public class DirtBallScript : MonoBehaviour
{
    [SerializeField] private float impulseLimit;
    [SerializeField] private float maxSpeed = 10;
    [SerializeField] private List<GameObject> rocks = new List<GameObject>();
    private Rigidbody2D rb;
    public DirtSpawner spawner;
    private Animator anim;
    private string randomAnim;
    private string currentAnim;
    const float startTime = 1;
    private float t = startTime;
    private const float baseDeathTime = 1;
    public bool check = true;
    private bool isDead;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        randomAnim = Random.Range(0, 2) == 0 ? "RollingRight" : "RollingLeft";
    }

    private void Update()
    {
        CheckVelocity();
    }

    private void CheckVelocity()
    {
        if (!check) return;

        if (Mathf.Abs(rb.linearVelocityX) + Mathf.Abs(rb.linearVelocityY) < 0.1f)
        {
            t -= Time.deltaTime;
            if (t < 0) Death();
        }
        else t = startTime;
    }

    private void LateUpdate()
    {
        ControlRollingAnim();
    }

    public void setInitialSpeed(Vector2 direction, float speed)
    {
        rb.linearVelocity = direction * speed;
        rb.AddForce(direction * speed, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float normalImpulse = collision.contacts[0].normalImpulse;
        PlayerBaseModule player = collision.gameObject.GetComponent<PlayerBaseModule>();
        if (player && player.state.activePower == PlayerSystem.Power.Drill) return;
        if (normalImpulse > impulseLimit)
        {
            Death();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<DirtSpawner>())
        {
            StartCoroutine(DieAfterTime(baseDeathTime));
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<DirtSpawner>()) StopAllCoroutines();
    }

    IEnumerator DieAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Death();
    }

    private void Death()
    {
        if (isDead) return;
        isDead = true;

        if (spawner != null)
        {
            spawner.SpawnDirtBall(spawner.reloadTime);
        }

        int maxAmountRocks = Random.Range(2, 6);
        List<GameObject> rocksList = new List<GameObject>();

        for (int i = 0; i < rocks.Count; i++)
        {
            bool shouldBeUsed = true;

            if (Random.value > 0.5f || rocksList.Count > maxAmountRocks)
                shouldBeUsed = false;

            if (shouldBeUsed && rocks[i] != null)
            {
                rocks[i].transform.parent = null;
                rocks[i].SetActive(true);
                rocksList.Add(rocks[i]);
            }
        }

        if (spawner != null)
        {
            spawner.AddRocksToTheList(rocksList);
        }

        Vector3 deathPos = transform.position;
        AudioManager.Instance?.Play3DSountAtPosition(RocksSounds.Destroy, deathPos);

        Destroy(gameObject, 0.1f);
    }


    private void ControlRollingAnim()
    {
        string animClip;
        if (rb.linearVelocityX > 0)
        {
            animClip = "RollingRight";
        }
        else if (rb.linearVelocityX < 0)
        {
            animClip = "RollingLeft";
        }
        else
        {
            animClip = randomAnim;
        }

        if (currentAnim != animClip)
        {
            currentAnim = animClip;
            anim.Play(animClip);
        }

        float minSpeed = Mathf.Abs(rb.linearVelocityY) > 0 ? 0.3f : 0;
        float LerpSpeed = Mathf.InverseLerp(0, maxSpeed, Mathf.Abs(rb.linearVelocityX));
        float animationSpeed = Mathf.Max(LerpSpeed, minSpeed);
        anim.speed = animationSpeed;
    }
}
