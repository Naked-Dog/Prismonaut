using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.InputSystem;
using UnityEngine.Animations;

public class Player2DController : MonoBehaviour
{
    public float maxVelocity;
    public float maxCircleVelocity;
    public float acceleration;
    public float deacceleration;
    public float circleDeacceleration;
    public float jumpVelocity;
    public float triangleJumpVelocity;
    public float trianglePowerVelocity;
    public float squarePowerVelocity;
    public float circlePowerVelocity;
    public float gravity;
    public float triangleGravity;
    public float powerDuration = 0.3f;
    public float staggerDuration = 0.5f;
    public float staggerVelocity = 5f;
    public float invulnerabilityTime = 4f;
    public float startingFallDamageDistance = 8f;
    public float intervalFallDamageDistance = 4f;
    public Color circlePowerColor;
    public Color squarePowerColor;
    public Color trianglePowerColor;
    public Sprite circleSprite;
    public Sprite squareSprite;
    public Sprite triangleSprite;
    public TriggerEventHandler groundTrigger;
    public TriggerEventHandler topTrigger;
    public TriggerEventHandler leftTrigger;
    public TriggerEventHandler rightTrigger;
    public Animator animator;
    public SpriteRenderer playerSpriteRender;
    public CinemachineVirtualCamera virtualCamera;

    public AudioClip walkSound;
    public AudioClip switchPowerSound;
    public AudioClip circlePowerSound;
    public AudioClip squarePowerSound;
    public AudioClip trianglePowerSound;
    public AudioClip damageSound;
    public AudioClip breakSound;
    public AudioClip pipeSound;
    public AudioClip collectSound;
    public AudioClip jumpSound;


    private int[] lifes = new int[3] { GameManager.maxLifes, GameManager.maxLifes, GameManager.maxLifes };
    private int prisms = 0;
    private bool isPowerOnCooldown = false;
    private FormState form = FormState.Undefined;
    private Rigidbody2D rb2d;
    private SplineAnimate splineAnimate;
    private int inputDirection = 0;
    private FormState lastPowerUsed = FormState.Undefined;
    private bool isStagger = false;
    private float fallHeight;
    private bool isDead = false;
    private AudioSource audioSource;

    [SerializeField] private SpriteRenderer helmetRenderer;
    [SerializeField] private Sprite circleWhileTubeSprite;
    [SerializeField] private Sprite squarePowerSprite;

    public static event Action<FormState, bool> OnFormChangedEvent;
    public static event Action<FormState, int> OnLifesChangedEvent;
    public static event Action<int> OnPrismsChangedEvent;


    private bool _isMoving = false;
    private bool isMoving
    {
        get { return _isMoving; }
        set
        {
            if (_isMoving == value) return;
            _isMoving = value;
            animator.SetBool("isMoving", value);
        }
    }

    private bool _isFalling = false;
    private bool isFalling
    {
        get { return _isFalling; }
        set
        {
            if (_isFalling == value) return;
            if (!_isFalling && value)
            {
                fallHeight = transform.position.y;
            }
            _isFalling = value;
        }
    }
    private bool _isGrounded;
    private bool isGrounded
    {
        get { return _isGrounded; }
        set
        {
            if (_isGrounded == value) return;
            _isGrounded = value;
            animator.SetBool("isGrounded", value);
            if (value) isFalling = false;
        }
    }

    private bool _isUsingSquarePower;
    private bool _isUsingCirclePower;
    private bool _isUsingTrianglePower;
    private bool isUsingPower
    {
        get
        {
            return _isUsingSquarePower || _isUsingCirclePower || _isUsingTrianglePower;
        }
        set
        {
            if (value)
            {
                if (form == FormState.Square)
                {
                    _isUsingSquarePower = true;
                    animator.SetBool("isUsingSquarePower", true);
                }
                if (form == FormState.Circle)
                {
                    _isUsingCirclePower = true;
                    animator.SetBool("isUsingCirclePower", true);
                }
                if (form == FormState.Triangle)
                {
                    _isUsingTrianglePower = true;
                }
            }
            else
            {
                _isUsingSquarePower = _isUsingCirclePower = _isUsingTrianglePower = false;
                animator.SetBool("isUsingSquarePower", false);
                animator.SetBool("isUsingCirclePower", false);
            }
        }

    }

    private bool _isInvulnerable = false;
    private bool isInvulnerable
    {
        get { return _isInvulnerable; }
        set
        {
            if (_isInvulnerable == value) return;
            _isInvulnerable = value;
            if (value)
            {
                StartCoroutine(invulnerability());
                StartCoroutine(rendererBlink());
            }
        }
    }

    private bool once = false;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction trianglePowerAction;
    private InputAction squarePowerAction;
    private InputAction circlePowerAction;
    private InputAction pauseAction;
    public static Player2DController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        splineAnimate = GetComponent<SplineAnimate>();
        formInput(GameManager.startingForm, false);

        groundTrigger.OnTriggerEnter2DAction.AddListener((other) =>
        {
            if (form == FormState.Square && isUsingPower && other.gameObject.tag == "Breakable")
            {
                Destroy(other.gameObject);
                audioSource.PlayOneShot(breakSound);
            }

            if (form == FormState.Square && other.gameObject.tag == "Ground")
            {
                isUsingPower = false;
                playerSpriteRender.color = Color.white;
            }

            if (other.gameObject.tag == "Ground" || other.gameObject.tag == "Breakable")
            {
                isGrounded = true;
                float fallDistance = fallHeight - transform.position.y;
                if (!isInvulnerable && form != FormState.Square && fallDistance > startingFallDamageDistance)
                {
                    reduceLife(1 + (int)Math.Floor((fallDistance - startingFallDamageDistance) / intervalFallDamageDistance));
                    if (!isDead) StartCoroutine(staggerPlayer(Vector2.up));
                }
                lastPowerUsed = FormState.Undefined;
            }
        });
        groundTrigger.OnTriggerExit2DAction.AddListener((other) =>
        {
            if (other.gameObject.tag == "Ground" || other.gameObject.tag == "Breakable")
            {
                isGrounded = false;
            }
        });

        topTrigger.OnTriggerEnter2DAction.AddListener((other) =>
        {
            if (form == FormState.Triangle && isUsingPower && other.gameObject.tag == "Breakable")
            {
                Destroy(other.gameObject);
                audioSource.PlayOneShot(breakSound);
            }
        });

        leftTrigger.OnTriggerEnter2DAction.AddListener((other) =>
        {
            if (form == FormState.Circle && isUsingPower && other.gameObject.tag == "Breakable")
            {
                Destroy(other.gameObject);
                audioSource.PlayOneShot(breakSound);
            }
        });

        rightTrigger.OnTriggerEnter2DAction.AddListener((other) =>
        {
            if (form == FormState.Circle && isUsingPower && other.gameObject.tag == "Breakable")
            {
                Destroy(other.gameObject);
                audioSource.PlayOneShot(breakSound);
            }
        });

        FindInputActions();
    }
    private void Update()
    {
        velocityUpdate();

        if (circlePowerAction.WasPressedThisFrame()) formInput(FormState.Circle);
        if (trianglePowerAction.WasPressedThisFrame()) formInput(FormState.Triangle);
        if (squarePowerAction.WasPressedThisFrame()) formInput(FormState.Square);
        if (pauseAction.WasPressedThisFrame()) MenuController.Instance.DisplayGamePanel();

        isMoving = Mathf.Abs(rb2d.velocity.x) > 0.1f;
        playerSpriteRender.flipX = rb2d.velocity.x < 0;
    }

    private void LateUpdate()
    {
        isFalling = !isGrounded && rb2d.velocity.y < 0f;
    }

    private void velocityUpdate()
    {
        float horizontalInput = moveAction.ReadValue<Vector2>().x;
        float formMaxVelocity = form == FormState.Circle ? maxCircleVelocity : maxVelocity;
        float formDeacceleration = form == FormState.Circle ? circleDeacceleration : deacceleration;

        float newHorizontalVelocity = rb2d.velocity.x;
        if (!isStagger)
        {
            // If the player exceeds the max horizontal velocity, set player input to zero to prevent further acceleration
            if (Math.Sign(rb2d.velocity.x) * Math.Sign(horizontalInput) > 0 &&
                (rb2d.velocity.x < -formMaxVelocity || rb2d.velocity.x > formMaxVelocity))
            {
                horizontalInput = 0;
            }

            newHorizontalVelocity = rb2d.velocity.x + horizontalInput * Time.deltaTime * acceleration;
            if (Math.Sign(rb2d.velocity.x) * Math.Sign(horizontalInput) <= 0) newHorizontalVelocity *= 1f - Time.deltaTime * formDeacceleration;
        }
        if (horizontalInput != 0) inputDirection = Math.Sign(horizontalInput);

        float newVerticalVelocity = rb2d.velocity.y;
        if (form != FormState.Circle || !isUsingPower)
        {
            newVerticalVelocity = rb2d.velocity.y - (form == FormState.Triangle ? triangleGravity : gravity) * Time.deltaTime;
            if (jumpAction.IsPressed() && isGrounded)
            {
                newVerticalVelocity = form == FormState.Triangle ? triangleJumpVelocity : jumpVelocity;
                audioSource.PlayOneShot(jumpSound);
            }
        }

        rb2d.velocity = new Vector2(newHorizontalVelocity, newVerticalVelocity);
    }

    private void formInput(FormState newForm, bool isAnimated = true)
    {
        if (form == newForm)
        {
            StartCoroutine(usePower());
        }
        else
        {
            SetForm(newForm, isAnimated);
        }
    }

    private void SetForm(FormState newForm, bool isAnimated = true)
    {
        switch (newForm)
        {
            case FormState.Circle:
                helmetRenderer.GetComponent<SpriteRenderer>().sprite = circleSprite;
                break;
            case FormState.Square:
                helmetRenderer.GetComponent<SpriteRenderer>().sprite = squareSprite;
                break;
            case FormState.Triangle:
                helmetRenderer.GetComponent<SpriteRenderer>().sprite = triangleSprite;
                break;
        }

        GetComponent<CircleCollider2D>().enabled = newForm == FormState.Circle;
        GetComponent<BoxCollider2D>().enabled = newForm != FormState.Circle;

        rb2d.velocity = new Vector2(rb2d.velocity.x / 2f, rb2d.velocity.y / 2f);
        form = newForm;
        isUsingPower = false;
        playerSpriteRender.color = Color.white;
        audioSource.PlayOneShot(switchPowerSound);

        if (OnFormChangedEvent != null) OnFormChangedEvent(form, isAnimated);
        if (OnLifesChangedEvent != null) OnLifesChangedEvent(form, lifes[(int)newForm]);
    }

    private IEnumerator usePower()
    {
        if (isPowerOnCooldown || form == lastPowerUsed) yield break;
        Color powerColor;
        switch (form)
        {
            case FormState.Circle:
                rb2d.velocity = new Vector2(inputDirection * circlePowerVelocity, 0f);
                audioSource.PlayOneShot(circlePowerSound);
                powerColor = circlePowerColor;
                break;
            case FormState.Square:
                if (isGrounded) yield break;
                rb2d.velocity = new Vector2(rb2d.velocity.x, -squarePowerVelocity);
                audioSource.PlayOneShot(squarePowerSound);
                powerColor = squarePowerColor;
                break;
            case FormState.Triangle:
            default:
                rb2d.velocity = new Vector2(rb2d.velocity.x, trianglePowerVelocity);
                audioSource.PlayOneShot(trianglePowerSound);
                powerColor = trianglePowerColor;
                break;
        }

        isUsingPower = isPowerOnCooldown = true;
        playerSpriteRender.color = powerColor;
        lastPowerUsed = form;
        yield return new WaitForSecondsRealtime(powerDuration);
        isPowerOnCooldown = false;
        if (isGrounded) lastPowerUsed = FormState.Undefined;
        if (form != FormState.Square)
        {
            isUsingPower = false;
            playerSpriteRender.color = Color.white;
        }
    }

    private void reduceLife(int value)
    {
        audioSource.PlayOneShot(damageSound);
        lifes[(int)form] -= value;
        OnLifesChangedEvent(form, lifes[(int)form]);
        if (lifes[(int)form] > 0)
        {
            isInvulnerable = true;
        }
        else
        {
            isDead = true;
            MenuController.Instance.DisplayLosePanel();
            rb2d.velocity = Vector2.zero;
            this.enabled = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Spike" && !isInvulnerable)
        {
            reduceLife(1);
            if (!isDead) StartCoroutine(staggerPlayer(other.contacts[0].normal.normalized));
        }
    }

    private IEnumerator staggerPlayer(Vector2 direction)
    {
        rb2d.velocity = new Vector2(direction.x * staggerVelocity, 3f + direction.y * staggerVelocity);
        isStagger = true;
        yield return new WaitForSeconds(staggerDuration);
        isStagger = false;
    }

    private IEnumerator levelFinishAnimation()
    {
        yield return new WaitForSeconds(2f);
        MenuController.Instance.ChangeScene("3D");
    }

    private IEnumerator invulnerability()
    {
        yield return new WaitForSeconds(invulnerabilityTime);
        isInvulnerable = false;
    }

    private IEnumerator rendererBlink()
    {
        while (isInvulnerable)
        {
            playerSpriteRender.enabled = helmetRenderer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            playerSpriteRender.enabled = helmetRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == form.ToString() + "Tube")
        {
            splineAnimate.Container = other.GetComponent<SplineContainer>();
            splineAnimate.Play();
            enabled = false;
            audioSource.PlayOneShot(pipeSound);
            splineAnimate.Completed += onCompletedSpline;
        }

        if (other.gameObject.tag == "Prism")
        {
            prisms++;
            Destroy(other.gameObject);
            audioSource.PlayOneShot(collectSound);
            OnPrismsChangedEvent(prisms);
        }

        if (other.gameObject.tag == "FinishTrigger" && !once)
        {
            once = true;
            splineAnimate.Container = other.GetComponent<SplineContainer>();
            splineAnimate.MaxSpeed = 25f;
            splineAnimate.Play();
            StartCoroutine(levelFinishAnimation());
        }
    }

    private void onCompletedSpline()
    {
        splineAnimate.Container = null;
        splineAnimate.NormalizedTime = 0f;
        rb2d.velocity = Vector2.zero;
        enabled = true;
        splineAnimate.Completed -= onCompletedSpline;
    }

    private void FindInputActions()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        trianglePowerAction = InputSystem.actions.FindAction("TrianglePower");
        squarePowerAction = InputSystem.actions.FindAction("SquarePower");
        circlePowerAction = InputSystem.actions.FindAction("CirclePower");
        pauseAction = InputSystem.actions.FindAction("Pause");
    }

}
