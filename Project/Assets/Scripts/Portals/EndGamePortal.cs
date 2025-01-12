using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;


public class EndGamePortal : MonoBehaviour, IPlayerPowerInteractable
{
    private AudioManager audioManager;
    [SerializeField] private GameObject player3DModel;


    public void PlayerPowerInteraction(PlayerSystem.PlayerState playerState)
    {
        if (playerState.activePower == PlayerSystem.Power.Circle)
        {
            Debug.Log("EndGame");
            playerState.playerGameObject.transform.GetChild(1).gameObject.SetActive(false);

            StartCoroutine(EnterEndGamePortal(playerState.playerGameObject.transform.position));
        }
    }

    private IEnumerator EnterEndGamePortal(Vector3 playerPosition)
    {
        GameObject model3D = Instantiate(player3DModel, playerPosition, Quaternion.identity);
        model3D.transform.localScale = Vector3.one * 0.5f;
        yield return model3D.transform.DOMove(gameObject.transform.position, 0.5f).WaitForCompletion();
        yield return model3D.transform.DOScale(Vector3.zero, 1f).WaitForCompletion();
        MenuController.Instance.ChangeScene("NewAdventuresScene");
    }
}
