using System.Collections;
using System.Collections.Generic;
using PlayerSystem;
using Unity.VisualScripting;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private bool isOpen;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<PlayerBaseModule>())
        {
            GameDataManager.Instance.SavePlayerPosition(transform.position);
            isOpen = true;
            animator.SetBool("isOpen", isOpen);
        }
    }
}
