using System.Collections;
using Cinemachine;
using Unity.Behavior;
using UnityEngine;

public class BullHealth : MonoBehaviour
{
    [System.Serializable]
    public class HealthSegment
    {
        public HealthBar healthBar;
        public float currentHealth;
        public bool locked;
        public Coroutine regenCoroutine;
    }

    [SerializeField] private int healthPerBar = 6;
    [SerializeField] private float regenDelay = 1f;
    [SerializeField] private HealthSegment[] segments;
    [SerializeField] private Transform target;
    [SerializeField] private ScarabullAnimationController animationController;
    [SerializeField] private BehaviorGraphAgent agent;
    [SerializeField] private GameObject finalTrigger;
    [SerializeField] private BoxCollider2D hazardCollider;

    [HideInInspector] public BlackboardVariable<bool> flinchedVar;
    [HideInInspector] public BlackboardVariable<int> currentStageVar;

    private void Start()
    {
        agent.GetVariable("Flinched", out flinchedVar);
        agent.GetVariable("CurrentStage", out currentStageVar);

        foreach (var seg in segments)
        {
            seg.healthBar.Init();
            seg.currentHealth = healthPerBar;
            seg.locked = false;
            seg.healthBar.SetMaxHealth(healthPerBar);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            TakeDamage(4);
        }
        if (target != null)
        {
            transform.position = new Vector3(target.position.x, target.position.y + 3.5f, transform.position.z);
        }
    }

    public void TakeDamage(float amount)
    {
        if (flinchedVar == null || !flinchedVar.Value) return;

        for (int i = segments.Length - 1; i >= 0 && amount > 0; i--)
        {
            var seg = segments[i];
            if (seg.locked) continue;

            float damage = Mathf.Min(amount, seg.currentHealth);
            seg.currentHealth -= damage;
            amount -= damage;

            seg.healthBar.SetHealth(seg.currentHealth);

            animationController.HurtAnimation();

            if (seg.currentHealth <= 0)
            {
                seg.locked = true;
                currentStageVar.Value++;
            }
            else
            {
                if (seg.regenCoroutine != null)
                    StopCoroutine(seg.regenCoroutine);

                seg.regenCoroutine = StartCoroutine(RegenerateSegment(i));
            }
        }

        if (IsDead())
        {
            ShakeManager.Instance.StartShakeSequence(3, 0.5f);
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.Play2DSound(BullSoundsEnum.BossFinishZone, false);

            if (hazardCollider != null)
                hazardCollider.enabled = false;

            if (animationController != null)
                animationController.SetState(ScarabullAnimationState.Death);

            if (agent != null)
                agent.End();

            if (finalTrigger != null)
                finalTrigger.SetActive(false);

            if (gameObject != null)
                gameObject.SetActive(false);
        }
    }

    private IEnumerator RegenerateSegment(int index)
    {
        var seg = segments[index];

        yield return new WaitForSeconds(regenDelay);

        while (!seg.locked && seg.currentHealth < healthPerBar)
        {
            seg.currentHealth++;
            seg.healthBar.SetHealth(seg.currentHealth);
            yield return new WaitForSeconds(regenDelay);
        }
    }

    public bool IsDead()
    {
        foreach (var seg in segments)
        {
            if (!seg.locked)
                return false;
        }
        return true;
    }
}
