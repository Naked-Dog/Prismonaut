using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using PlayerSystem;
using UnityEngine.Splines;


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
        player3DModel.SetActive(true);
        player3DModel.GetComponent<SplineAnimate>().Play();
        yield return new WaitForSeconds(2);
        player3DModel.GetComponent<SplineAnimate>().Pause();
        player3DModel.GetComponent<SplineAnimate>().Restart(false);
        player3DModel.SetActive(false);
        MenuController.Instance.ChangeScene("NewAdventuresScene");
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.GetComponent<PlayerBaseModule>())
        {
            PlayerBaseModule playerBaseModule = collider.GetComponent<PlayerBaseModule>();
            playerBaseModule.transform.GetChild(1).gameObject.SetActive(false);
            StartCoroutine(EnterEndGamePortal(playerBaseModule.transform.position));
        }
    }
}
