using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.InputSystem;
using System;
using Cinemachine;

public class DimensionTransition : MonoBehaviour
{
    [SerializeField] private Vector2 secondTravelPoint = Vector2.zero;
    [SerializeField] private GameObject player3DModel;
    private Vector3 secondTravelWorldPoint 
    { 
        get 
        {
            return new Vector3(transform.position.x + secondTravelPoint.x, transform.position.y + secondTravelPoint.y, 0);
        }
    }
    private BoxCollider2D[] box2DColliders{ get => GetComponents<BoxCollider2D>(); }
    private Vector3 firstTravelPoint;
    private bool isTraveling = false;
    private CinemachineVirtualCamera virtualCamera;
    private Player2DController playerController = null;
    private InputAction trianglePowerAction;
    private InputAction squarePowerAction;
    private InputAction circlePowerAction;

    void Awake()
    {
        CheckCollidersNumber();
    }

    void Start()
    {
        firstTravelPoint = transform.position;
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        SetBoxColliders();

        trianglePowerAction = InputSystem.actions.FindAction("TrianglePower");
        squarePowerAction = InputSystem.actions.FindAction("SquarePower");
        circlePowerAction = InputSystem.actions.FindAction("CirclePower");
    }

    void Update() 
    {
        bool isActionPressed = trianglePowerAction.WasPressedThisFrame() || squarePowerAction.WasPressedThisFrame() || circlePowerAction.WasPressedThisFrame();
        if(playerController != null && isActionPressed && !isTraveling)
        {
            float dist1 = Math.Abs((playerController.transform.position - firstTravelPoint).x);
            float dist2 = Math.Abs((playerController.transform.position - secondTravelWorldPoint).x);
            StartCoroutine(TravelTransition(playerController, dist1 < dist2 ? secondTravelWorldPoint : firstTravelPoint));
        }
    }

    private void CheckCollidersNumber()
    {
        if(box2DColliders.Length < 2) 
        {
            for (int i = box2DColliders.Length; i < 2; i++)
            {
                gameObject.AddComponent<BoxCollider2D>();
            }
        } 
        else 
        {
            for (int i = box2DColliders.Length - 1; i > 1; i--)
            {
                Destroy(box2DColliders[i]);
            }
        }
    }

    private void SetBoxColliders()
    {
        box2DColliders[0].offset = Vector2.zero;
        box2DColliders[1].offset = secondTravelPoint;
        foreach(BoxCollider2D box in box2DColliders)
        {
            box.isTrigger = true;
        }
    }
    public IEnumerator TravelTransition(Player2DController player, Vector3 targetPosition)
    {
        isTraveling = true;
        player.gameObject.SetActive(false);
        GameObject model3D = Instantiate(player3DModel, player.transform.position, Quaternion.identity);
        model3D.transform.localScale = new  Vector3(0.5f,0.5f,0.5f);
        model3D.transform.LookAt(targetPosition);
        virtualCamera.Follow = model3D.transform;

        Tween travel = model3D.transform.DOMove(targetPosition, 1.5f).SetEase(Ease.Linear);
        yield return travel.WaitForCompletion();
        
        Destroy(model3D);
        player.transform.position = targetPosition;
        virtualCamera.Follow = player.transform;
        player.gameObject.SetActive(true);
        isTraveling = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.15f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(secondTravelWorldPoint, 0.15f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position,secondTravelWorldPoint);
    }


    void OnTriggerEnter2D(Collider2D collider) 
    {
        if(collider.GetComponent<Player2DController>()){
            playerController = collider.GetComponent<Player2DController>();
        }
    }

    void OnTriggerExit2D(Collider2D collider) 
    {
        if(collider.GetComponent<Player2DController>()){
            playerController = null;
        }
    }

    public void OnValidate()
    {
        SetBoxColliders();
    }

}
