using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void Damage(int damageAmount, Vector2 hitDirection = default);

    void Die();

    int MaxHealth { get; set; }
    int CurrentHealth { get; set; }
}