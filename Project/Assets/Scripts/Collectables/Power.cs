using System.Collections;
using System.Collections.Generic;
using PlayerSystem;
using UnityEngine;
using UnityEngine.Events;

public class Power : MonoBehaviour, ICollectable
{
    public CollectableType CollectableType { get; set; }
    public UnityEvent collect;
    public PlayerBaseModule playerBaseModule { get; set; }
    [SerializeField] private PlayerSystem.Power power;

    private void Start()
    {
        playerBaseModule = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBaseModule>();
        Debug.Log(playerBaseModule);
        collect.AddListener(delegate { playerBaseModule.powersModule.SetPowerAvailable(power, true); });
    }

    public void Collect()
    {
        gameObject.SetActive(false);
        collect.Invoke();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Collect();
        }
    }
}
