using System;
using UnityEngine;

public class ShineCanvas : MonoBehaviour
{
    public static Action OnShineBackground;
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        OnShineBackground += StartShineCanvasAnimation;
    }

    private void StartShineCanvasAnimation()
    {
        animator.Play("ShineCanvas");
    }
}
