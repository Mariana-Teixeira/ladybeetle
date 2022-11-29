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
    PlayerCollisions playerCollisions;
    PlayerMovementStates playerMovement;
    PlayerAccelerationStates playerAcceleration;

    Rigidbody2D body;
    BoxCollider2D boxCollider;
    LayerMask platformMask;
    [SerializeField] Camera playerCamera;

    #region Input Variables
    PlayerControls playerControls;
    float runInput;
    bool jumpInput;
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
    #endregion

    public float direction; // used in acceleration

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
            return playerCollisions;
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
    #endregion

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        platformMask = LayerMask.GetMask("Platforms"); //could use a private exposed string

        playerControls = new PlayerControls();
        playerControls.Enable();

        float jumpApex = jumpDuration / 2;
        gravity = (-2 * jumpHeight) / Mathf.Pow(jumpApex, 2);
        initialJumpVelocity = -gravity * jumpApex;

        acceleration = maxSpeed / timeToMaxSpeed;

        playerMovement = new PlayerMovementStates(this);
        playerCollisions = new PlayerCollisions(this);
        playerAcceleration = new PlayerAccelerationStates(this);
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
    }

    void FixedUpdate() //researched ellapsed time; having it here locked the framerate
    {
        MovePlayer(); // MOVES PLAYER
        playerAcceleration.UpdateMachine(); // DOES ACCELERATION CALCULATIONS (associated with the movement script)
        playerMovement.UpdateMachine(); // DOES MOVEMENT CALCULATIONS
        playerCollisions.UpdateCollisions(); // DOES COLLISIONS CALCULATIONS
    }

    void MovePlayer()
    {
        body.position = body.position + (move * Time.fixedDeltaTime);
    }
}