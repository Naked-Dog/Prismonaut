using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using PlayerSystem;


public class EndGamePortal : MonoBehaviour
{
    private AudioManager audioManager;
    [SerializeField] private GameObject player3DModel;

    private IEnumerator EnterEndGamePortal(Vector3 playerPosition)
    {
        GameObject model3D = Instantiate(player3DModel, playerPosition, Quaternion.identity);
        model3D.transform.localScale = Vector3.one * 0.5f;
        yield return model3D.transform.DOMove(gameObject.transform.position, 0.5f).WaitForCompletion();
        yield return model3D.transform.DOScale(Vector3.zero, 1f).WaitForCompletion();
        MenuController.Instance.ChangeScene("NewAdventuresScene");
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.GetComponent<PlayerBaseModule>())
        {
            PlayerBaseModule playerBaseModule = collider.GetComponent<PlayerBaseModule>();
            if (playerBaseModule.state.activePower == PlayerSystem.Power.Circle)
            {
                Debug.Log("EndGame");
                playerBaseModule.transform.GetChild(1).gameObject.SetActive(false);

                StartCoroutine(EnterEndGamePortal(playerBaseModule.transform.position));
            }
        }
    }

}
