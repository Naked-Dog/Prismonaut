using UnityEngine;

[CreateAssetMenu(fileName = "Player Health Constans", menuName = "ScriptableObjects/Player/Player Health Values", order = 1)]
public class PlayerHealthScriptable : ScriptableObject
{
    public int maxHealthBars = 3;
    public int currentHealthBars = 3;
    public int healthPerBar = 8;
    public int currentHealth = 8;
    public float hpRegenRate = 2f;

    [Header("Damage")]
    public float hurtTime;
    public float deathTime = 1.5f;
}
