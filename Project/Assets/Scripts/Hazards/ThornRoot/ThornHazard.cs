using System;
using PlayerSystem;
using UnityEngine;

public class ThornHazard : HazardBase
{
    public Action onParry;
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        HandleCollision(other.gameObject);
    }

    protected override void OnCollisionEnter2D(Collision2D other)
    {
        HandleCollision(other.gameObject);
    }

    protected override bool CheckPrevDmgPlayer(GameObject other)
    {
        PlayerBaseModule player = other.GetComponent<PlayerBaseModule>();
        if (player.state.isParry)
        {
            onParry?.Invoke();
            return true;
        }
        return false;
    }
}
