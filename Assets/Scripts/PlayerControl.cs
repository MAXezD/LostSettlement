using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerControl : Singleton<PlayerControl>
{
    // Start is called before the first frame update
    [Header("Player Movement")]
    public MovementState State;
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    bool readyToSprint;
    [Space]
    private float horizontalInput;
    private float verticalInput;
    public Transform orientation;
    [Space]
    private Vector3 inputDirection;
    private Rigidbody rb;
    public float groundDrag = 5F;
    public float airDrag = 1F;

    [Header("GroundCheck")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool isGrounded;

    [Header("Jumping")]
    public float jumpPower;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("StepClimb")]
    [SerializeField] private GameObject stepRayUpper;
    [SerializeField] private GameObject stepRayLower;

    [SerializeField] float stepHeight;
    [SerializeField] float stepSmooth;

    [Header("KeyBinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Stamina")]
    private float stamina;
    public float maxStamina;
    public float staminaRegenRate;
    public float staminaCostJump;
    public float staminaCostSprint;
    public Slider staminaBar;

    [Header("Audio")]
    [SerializeField] GameObject walkSound;
    [SerializeField] GameObject runSound;
    public enum MovementState
    {
        walk,
        sprint,
        air,
    }
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        stepRayUpper.transform.position = new Vector3(stepRayUpper.transform.position.x, stepRayLower.transform.position.y + stepHeight, stepRayUpper.transform.position.z);
        ResetJump();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Wendigo.Instance.startedAttack)
        {
            rb.isKinematic = true;
            transform.position = new Vector3(Wendigo.Instance.jumpScareCam.transform.position.x, Wendigo.Instance.jumpScareCam.transform.position.y - 0.3f , Wendigo.Instance.jumpScareCam.transform.position.z);
            transform.rotation = Quaternion.Euler(Wendigo.Instance.jumpScareCam.transform.rotation.x, Wendigo.Instance.jumpScareCam.transform.rotation.y, Wendigo.Instance.jumpScareCam.transform.rotation.z);
        }
        else
        {
            rb.isKinematic = false;
        }
        
        // if player got catch, player will froze in place
        MoveDirectionInput();
        MovePlayer();
        StepClimb();
        GroundCheck();
        MoveStateHandler();
        StaminaManager();
    }
    public void MoveDirectionInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && isGrounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    public void MovePlayer() 
    {

        inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if(isGrounded)
        {
            rb.AddForce(inputDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!isGrounded)
        {
            rb.AddForce(inputDirection.normalized * moveSpeed * airMultiplier * 10f, ForceMode.Force);
        }
        
        Vector3 XZVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (XZVelocity.magnitude > moveSpeed) // if it's more than max speed
        {
            // Clamp the speed to be maximum speed
            XZVelocity = XZVelocity.normalized * moveSpeed;
            rb.velocity = new Vector3(XZVelocity.x, rb.velocity.y, XZVelocity.z);
        }
    }
    private void MoveStateHandler()
    {
        if (isGrounded && Input.GetKey(sprintKey) && readyToSprint)
        {
            if (rb.velocity != Vector3.zero)
            {
                stamina -= staminaCostSprint;
                State = MovementState.sprint;
                moveSpeed = sprintSpeed;
            }

        }
        else if (isGrounded)
        {
            State = MovementState.walk;
            moveSpeed = walkSpeed;
        }
        else
        {
            State = MovementState.air;
        }
        
    }
    private void Footsteps()
    {
        walkSound.SetActive(State == MovementState.walk && rb.velocity.sqrMagnitude >= 0.5f);
        runSound.SetActive(State == MovementState.sprint);
    }

    public void Update()
    {
        Footsteps();
        if(isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
            rb.AddForce(Vector3.down * 9.8f, ForceMode.Force);//Real Gravity
        }
        
    }

    public void GroundCheck()
    {
        Debug.DrawRay(transform.position, Vector3.down, Color.red, playerHeight * 0.5f + 0.2f);
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f , whatIsGround);
    }

    private void Jump()
    {
        stamina -= staminaCostJump;
        rb.velocity = new Vector3(rb.velocity.x, 0f , rb.velocity.z);
        rb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
        
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
    bool activated;
    private void StaminaManager()
    {
        staminaBar.value = stamina;
        if (State == MovementState.walk || rb.velocity == Vector3.zero) stamina += Time.deltaTime * staminaRegenRate;
        if (stamina > maxStamina) stamina = maxStamina;
        if (stamina <= 0f)stamina = 0f;

        if (stamina <= 0f) readyToSprint = false;
        else if (stamina >= (maxStamina/2)) readyToSprint = true;
        
        if (stamina <= staminaCostJump)
        {
            readyToJump = false;
            activated = false;
        }
        if (stamina > staminaCostJump && !activated)
        {
            activated = true;
            readyToJump = true;
        }
    }
    private void StepClimb()
    {
        //testing
        Debug.DrawRay(stepRayLower.transform.position, stepRayLower.transform.TransformDirection(Vector3.forward),Color.green,0.1f);
        Debug.DrawRay(stepRayUpper.transform.position, stepRayUpper.transform.TransformDirection(Vector3.forward),Color.red, 0.2f);
        //
        RaycastHit hitLower;
        if (Physics.Raycast(stepRayLower.transform.position,stepRayLower.transform.TransformDirection(Vector3.forward), out hitLower, 0.1f , LayerMask.NameToLayer("Default")))
        {
            RaycastHit hitUpper;
            if (!Physics.Raycast(stepRayUpper.transform.position,stepRayUpper.transform.TransformDirection(Vector3.forward), out hitUpper, 0.2f))
            {
                rb.position -= new Vector3(0f , -stepSmooth , 0f);
            }
        }
        RaycastHit hitLower45;
        if (Physics.Raycast(stepRayLower.transform.position, stepRayLower.transform.TransformDirection(1.5f,0,1), out hitLower45, 0.1f ,LayerMask.NameToLayer("Default")))
        {
            RaycastHit hitUpper45;
            if (!Physics.Raycast(stepRayUpper.transform.position, stepRayUpper.transform.TransformDirection(1.5f, 0, 1), out hitUpper45, 0.2f))
            {
                rb.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }
        RaycastHit hitLowerMinus45;
        if (Physics.Raycast(stepRayLower.transform.position, stepRayLower.transform.TransformDirection(-1.5f,0,1), out hitLowerMinus45, 0.1f, LayerMask.NameToLayer("Default")))
        {
            RaycastHit hitUpperMinus45;
            if (!Physics.Raycast(stepRayUpper.transform.position, stepRayUpper.transform.TransformDirection(-1.5f, 0, 1), out hitUpperMinus45, 0.2f))
            {
                rb.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }
    }
}
