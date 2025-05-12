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

    public int healthPerBar = 6;
    public float regenDelay = 1f;
    public HealthSegment[] segments;
    public Transform target;

    public BehaviorGraphAgent agent;
    private BlackboardVariable<bool> flinchedVar;
    private void Start()
    {
        agent.GetVariable("Flinched", out flinchedVar);
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
        if(Input.GetKeyDown(KeyCode.F))
        {
            TakeDamage(4);
        }
        if (target != null)
        {
            transform.position = new Vector3(target.position.x, target.position.y + 1.75f, transform.position.z);
        }
    }

    public void TakeDamage(float amount)
    {
        if (flinchedVar == null || flinchedVar.Value == false) return;

        for (int i = segments.Length - 1; i >= 0 && amount > 0; i--)
        {
            var seg = segments[i];

            if (seg.locked) continue;

            float damage = Mathf.Min(amount, seg.currentHealth);
            seg.currentHealth -= damage;
            amount -= damage;

            seg.healthBar.SetHealth(seg.currentHealth);

            if (seg.currentHealth <= 0)
            {
                seg.locked = true;
            }
            else
            {
                // Start regen if not fully depleted
                if (seg.regenCoroutine != null)
                    StopCoroutine(seg.regenCoroutine);

                seg.regenCoroutine = StartCoroutine(RegenerateSegment(i));
            }
        }
    }

    private System.Collections.IEnumerator RegenerateSegment(int index)
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
