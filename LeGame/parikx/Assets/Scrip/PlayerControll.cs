using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump=true;

    [Header("KeyBind")]
    public KeyCode jumpkey = KeyCode.Space;
    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
 [SerializeField]   bool grounded;
    [SerializeField] Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody body;

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        body.freezeRotation = true;
    }
    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * .5f + .2f, whatIsGround);
        
        MyInput();
        SpeedControll();
        if(grounded)
        {
            body.drag = groundDrag;
        }
        else
        {
            body.drag = 0;

        }
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(jumpkey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    void SpeedControll()
    {
        Vector3 flatVel = new Vector3(body.velocity.x, 0, body.velocity.z);

        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            body.velocity = new Vector3(limitedVel.x, body.velocity.y, limitedVel.z);

        }
    }
    void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if(grounded)
        {
            body.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        }
        else
        {
            body.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        }
    }

    void Jump()
    {
        body.velocity = new Vector3(body.velocity.x, 0f, body.velocity.z);
        body.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void ResetJump()
    {
        readyToJump = true;
    }
}
