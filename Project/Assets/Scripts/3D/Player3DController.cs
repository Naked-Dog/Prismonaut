using System;
using System.Collections;
using UnityEngine;

public enum PlayerState {
    Moving,
    Damage,
    Impulse,
    Pass
}

public class Player3DController : MonoBehaviour
{
    [SerializeField] private Game3DController game3DController;
    [SerializeField] private float horizontalSpeed = 3.0f;
    [SerializeField] private float fordwardSpeed = 3.0f;
    [SerializeField] private MeshFilter playerMesh;
    [SerializeField] private Mesh[] FigureMeshes;
    [SerializeField] private float damageHitForce;
    [SerializeField] private float abilityForce;
    [SerializeField] private Animator animator;

    [SerializeField] private AudioClip change;
    [SerializeField] private AudioClip impulse;
    [SerializeField] private AudioClip damage;

    [SerializeField] private AudioSource audioSource;

    public FormState currentPlayerForm = FormState.Circle;

    private PlayerState playerState;
    private Rigidbody rgbd;
    private Coroutine abilityImpulse;
    private int[] lifes = new int[3] { GameManager.maxLifes, GameManager.maxLifes, GameManager.maxLifes };

    public static event Action<FormState, bool> OnFormChangedEvent;
    public static event Action<FormState, int> OnLifesChangedEvent;

    public static Player3DController Instance;

    private void Awake(){
        if(Instance == null){
            Instance = this;
        } else{
            Destroy(gameObject);
        }
    }

    private void Start(){
        rgbd = gameObject.GetComponent<Rigidbody>();
        playerState = PlayerState.Moving;
        UpdateMesh();
    }

    private void Update(){
        if(game3DController.state == GameState.Playing && playerState != PlayerState.Damage){
            if(playerState == PlayerState.Moving){
                MoveHorizontal();
            }

            if(playerState != PlayerState.Impulse){
                MoveFordward();
            }
            
             
            CheckFigureInputs();
        }
    }

    public PlayerState GetPlayerSate(){
        return playerState;
    }

    private void MoveHorizontal(){
        float rightAxis = Input.GetAxis("Horizontal");
        Vector3 translation = Vector3.right * rightAxis * horizontalSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + translation;
        newPosition.x = Mathf.Clamp(newPosition.x, -3.5f, 3.5f);
        transform.position = newPosition;
        animator.SetFloat("Horizontal", rightAxis);
    }

    private void MoveFordward(){
        transform.position += fordwardSpeed * Time.deltaTime * Vector3.forward;
    }

    private void CheckFigureInputs(){
        if(Input.GetKeyDown(KeyCode.J)){
            ChangeForm(FormState.Circle);
        }

        if(Input.GetKeyDown(KeyCode.K)){
            ChangeForm(FormState.Triangle);
        }

        if(Input.GetKeyDown(KeyCode.L)){
            ChangeForm(FormState.Square);
        }
    }

    private void ChangeForm(FormState newForm){
        if(newForm == currentPlayerForm){
            abilityImpulse = StartCoroutine(nameof(AbilityImpulse));
            audioSource.Stop();
            audioSource.PlayOneShot(impulse);
            return;
        }
        audioSource.Stop();
        audioSource.PlayOneShot(change);
        currentPlayerForm = newForm;
        UpdateMesh();
        if (OnFormChangedEvent != null) OnFormChangedEvent(currentPlayerForm, true);
        if (OnLifesChangedEvent != null) OnLifesChangedEvent(currentPlayerForm, lifes[(int)newForm]);
    }

    private void UpdateMesh(){
        switch(currentPlayerForm){
            case FormState.Circle:
                playerMesh.mesh = FigureMeshes[0];
            break;
            case FormState.Triangle:
                playerMesh.mesh = FigureMeshes[1];
            break;
            case FormState.Square:
                playerMesh.mesh = FigureMeshes[2];
            break;
        }
    }

    public void HitObstacle(FormState obstacleForm, ObstacleType obstacleType, bool isCenter = true){
        if(currentPlayerForm != obstacleForm || obstacleType == ObstacleType.Block || !isCenter){
            if(abilityImpulse != null){
                StopCoroutine(abilityImpulse);
                playerState = PlayerState.Moving;
            } 
            if(playerState == PlayerState.Damage) return;
            reduceLife(1);
            if(lifes[(int)currentPlayerForm] <= 0){
                game3DController.CheckGameState(true);
                return;
            }
            StartCoroutine(nameof(DamageReaction));
        } 
        else 
        {
            if(obstacleType == ObstacleType.Asteroid){
                if(playerState != PlayerState.Impulse){
                    reduceLife(1);
                    if(lifes[(int)currentPlayerForm] <= 0){
                        game3DController.CheckGameState(true);
                        return;
                    }
                    StartCoroutine(nameof(DamageReaction));
                    return;
                }
            }
            if(obstacleType == ObstacleType.Hollow)playerState = PlayerState.Pass;
            game3DController.CheckGameState(false);
        }
    }

    private void reduceLife(int value)
    {
        lifes[(int)currentPlayerForm] -= value;
        OnLifesChangedEvent(currentPlayerForm, lifes[(int)currentPlayerForm]);
    }

    private IEnumerator DamageReaction(){
        playerState = PlayerState.Damage;
        audioSource.Stop();
        audioSource.PlayOneShot(damage);
        float timer = 0;
        while (timer <= 0.5f)
        {
            transform.position -= Vector3.forward * damageHitForce * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }
        playerState = PlayerState.Moving;
    }

    private IEnumerator AbilityImpulse(){
        if(abilityImpulse != null) StopCoroutine(abilityImpulse);
        playerState = PlayerState.Impulse;
        float timer = 0;
        while (timer <= 0.5f)
        {
            transform.position += Vector3.forward * abilityForce * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }
        abilityImpulse = null;
        playerState = PlayerState.Moving;

    }

    public void PassObstacle(){
        if(playerState == PlayerState.Pass) playerState = PlayerState.Moving;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.transform.CompareTag("Starship")){
            game3DController.CheckGameState(false);
        }
    }
}
