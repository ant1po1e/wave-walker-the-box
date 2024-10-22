using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private float speed;
    public float walkSpeed = 8f;
    public float runSpeed = 15f;
    public Transform cam;
    public CinemachineFreeLook cinemachineCam;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity = 0.1f;

    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    private float verticalVelocity = 0f;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private bool isGrounded;

    public Transform groundCheck;

    public float normalFOV = 40f;
    public float runFOV = 60f;
    public float fovSmoothSpeed = 10f;

    private float targetFOV;

    void Start() {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        speed = walkSpeed;

        targetFOV = normalFOV;
        cinemachineCam.m_Lens.FieldOfView = normalFOV;
    }

    void Update() {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; 
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            targetFOV = runFOV;
            speed = runSpeed;
        }
        else
        {
            targetFOV = normalFOV;
            speed = walkSpeed;
        }

        cinemachineCam.m_Lens.FieldOfView = Mathf.Lerp(cinemachineCam.m_Lens.FieldOfView, targetFOV, fovSmoothSpeed * Time.deltaTime);

        verticalVelocity += gravity * Time.deltaTime;

        controller.Move(new Vector3(0, verticalVelocity, 0) * Time.deltaTime);
    }
}
