using System.Collections;
using System.Collections.Generic;
using PlayerSystem;
using Unity.VisualScripting;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<PlayerBaseModule>())
        {
            Vector3 playerPosition = other.transform.position;
            GameDataManager.Instance.SavePlayerPosition(playerPosition);
        }
    }
}
