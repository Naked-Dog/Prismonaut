using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using PlayerSystem;
using UnityEngine.Splines;
using UnityEngine.InputSystem;


public class EndGamePortal : MonoBehaviour
{
    private AudioManager audioManager;
    [SerializeField] private GameObject player3DModel;
    // private bool isTeleporting = false;

    private void Awake()
    {
        //audioManager = new AudioManager(gameObject, GetComponent<PortalSoundList>(), GetComponent<AudioSource>());
    }

    private void Start()
    {
        //audioManager.PlayAudioClip("Idle", true);
    }

    private IEnumerator EnterEndGamePortal(PlayerBaseModule playerBaseModule)
    {
        // isTeleporting = true;
        yield return new WaitForSeconds(1);
        playerBaseModule.transform.GetChild(1).gameObject.SetActive(false);
        //audioManager.PlayAudioClip("Entry", true);
        player3DModel.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        player3DModel.GetComponent<SplineAnimate>().Play();
        yield return new WaitForSeconds(5);
        player3DModel.GetComponent<SplineAnimate>().Pause();
        player3DModel.GetComponent<SplineAnimate>().Restart(false);
        MenuController.Instance.ChangeScene("NewAdventuresScene");
        yield return new WaitForSeconds(0.5f);
        InputActionMap playerInput = InputSystem.actions.FindActionMap("Player");
        playerInput.Enable();
        player3DModel.SetActive(false);
        playerBaseModule.transform.GetChild(1).gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.GetComponent<PlayerBaseModule>())
        {
            PlayerBaseModule playerBaseModule = collider.GetComponent<PlayerBaseModule>();
            StartCoroutine(EnterEndGamePortal(playerBaseModule));
        }
    }
}
