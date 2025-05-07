using DG.Tweening.Core.Easing;
using PlayerSystem;
using UnityEngine;

public class Slime2Test : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerBaseModule player = other.attachedRigidbody.GetComponent<PlayerBaseModule>();
        if(player.state.isParry)
        {
            gameObject.SetActive(false);
        }
        else
        {
            //Slime bounce logic
        }
    }
}
