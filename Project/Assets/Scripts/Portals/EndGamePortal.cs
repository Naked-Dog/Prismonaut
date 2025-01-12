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
    private bool isTeleporting = false;

    private void Awake()
    {
        audioManager = new AudioManager(gameObject, GetComponent<PortalSoundList>(), GetComponent<AudioSource>());
    }

    private void Start()
    {
        audioManager.PlayAudioClip("Idle", true);
    }

    private IEnumerator EnterEndGamePortal(Vector3 playerPosition)
    {
        isTeleporting = true;
        audioManager.PlayAudioClip("Entry", true);
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
            if (playerBaseModule.state.activePower != PlayerSystem.Power.None)
            {
                playerBaseModule.transform.GetChild(1).gameObject.SetActive(false);

                StartCoroutine(EnterEndGamePortal(playerBaseModule.transform.position));
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.GetComponent<PlayerBaseModule>() && !isTeleporting)
        {
            PlayerBaseModule playerBaseModule = collider.GetComponent<PlayerBaseModule>();
            if (playerBaseModule.state.activePower != PlayerSystem.Power.None)
            {
                playerBaseModule.transform.GetChild(1).gameObject.SetActive(false);

                StartCoroutine(EnterEndGamePortal(playerBaseModule.transform.position));
            }
        }
    }
}
