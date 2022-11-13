using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

//this cannot be initialized without xyz component
public class PlayerController : MonoBehaviour
{
    Rigidbody2D body;
    BoxCollider2D boxCollider;
    LayerMask platformMask;
    LayerMask hookableMask;
    PlayerCollisions collisions;
    PlayerStateMachine machine;
    [SerializeField] Camera playerCamera;

    #region Input Variables
    PlayerControls playerControls;
    float runInput;
    bool jumpInput;
    bool hookInput;
    #endregion

    #region Movement Variables
    public Vector2 move; // difficulty making it private
    [SerializeField] float jumpDuration;
    [SerializeField] float jumpHeight;
    float gravity; //should maybe be const
    float initialJumpVelocity;
    float runSpeed; //const
    float acceleration;
    [SerializeField] float maxSpeed;
    [SerializeField] float timeToMaxSpeed;
    Vector2 mousePositionOnScreen;
    Vector2 mousePositionInWorld;
    RaycastHit2D hookableHitBox;
    [SerializeField] float hookSpeed;
    Vector2 hookDirection;
    #endregion

    #region Properties
    public Rigidbody2D Body
    {
        get
        {
            return body;
        }
    }

    public BoxCollider2D BoxCollider
    {
        get
        {
            return boxCollider;
        }
    }

    public LayerMask PlatformMask
    {
        get
        {
            return platformMask;
        }
    }

    public PlayerCollisions Collisions
    {
        get
        {
            return collisions;
        }
    }

    public float RunInput
    {
        get
        {
            return runInput;
        }
    }

    public bool JumpInput
    {
        get
        {
            return jumpInput;
        }
    }

    public bool HookInput
    {
        get
        {
            return hookInput;
        }
    }

    public float Gravity
    {
        get
        {
            return gravity;
        }
    }

    public float InitialJumpVelocity
    {
        get
        {
            return initialJumpVelocity;
        }
    }

    public float RunSpeed
    {
        get
        {
            return runSpeed;
        }
        set
        {
            runSpeed = value;
        }
    }

    public float Acceleration
    {
        get
        {
            return acceleration;
        }
    }

    public float MaxSpeed
    {
        get
        {
            return maxSpeed;
        }
    }

    public Vector2 MousePositionInWorld
    {
        get
        {
            return mousePositionInWorld;
        }
    }

    public float HookSpeed
    {
        get
        {
            return hookSpeed;
        }
    }

    public Vector2 HookDirection
    {
        get
        {
            return hookDirection;
        }
        set
        {
            hookDirection = value;
        }
    }

    public RaycastHit2D HookHit // needs a description
    {
        get
        {
            return Physics2D.BoxCast(
            body.position,
                boxCollider.size,
                0.0f,
                Vector2.zero,
            0.0f,
                hookableMask);
        }
    }
    #endregion

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        platformMask = LayerMask.GetMask("Platforms"); //could use a private exposed string
        hookableMask = LayerMask.GetMask("Hookables");

        playerControls = new PlayerControls();
        playerControls.Enable();

        float jumpApex = jumpDuration / 2;
        gravity = (-2 * jumpHeight) / Mathf.Pow(jumpApex, 2);
        initialJumpVelocity = -gravity * jumpApex;

        acceleration = maxSpeed / timeToMaxSpeed;

        machine = new PlayerStateMachine(this);
        collisions = new PlayerCollisions(this);
    }
    void Update()
    {
        ReadInput();
    }

    void ReadInput()
    {
        mousePositionOnScreen = playerControls.Player.MousePosition.ReadValue<Vector2>();

        runInput = playerControls.Player.Running.ReadValue<float>();
        jumpInput = playerControls.Player.Jumping.IsPressed();
        hookInput = playerControls.Player.Hookshot.IsPressed();
    }

    void FixedUpdate() //researched ellapsed time; having it here locked the framerate
    {
        MovePlayer(); // MOVES PLAYER
        machine.UpdateMachine(); // DOES MOVEMENT CALCULATIONS
        collisions.UpdateCollisions(); // DOES COLLISIONS CALCULATIONS
    }

    void MovePlayer()
    {
        body.position = body.position + (move * Time.fixedDeltaTime);
    }

    public bool TryToHook()
    {
        mousePositionInWorld = playerCamera.ScreenToWorldPoint(mousePositionOnScreen);
        hookableHitBox = Physics2D.CircleCast(mousePositionInWorld, 0.2f, Vector2.zero, 0.0f, hookableMask); //in big games, watch out for many points

        if (hookableHitBox.collider == null) return false;
        
        float distance = Vector3.Distance(body.transform.position, hookableHitBox.collider.transform.position);
        Vector2 direction = (mousePositionInWorld - body.position).normalized;

        RaycastHit2D platformRay = Physics2D.Raycast(body.position, direction, distance, PlatformMask);

        if (platformRay.collider == null) return true;
        else return false;
    }
}