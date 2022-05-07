using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float MovementSpeed = 1;
    public float JumpSpeed = 1;
    public float Gravity = 9.8f;

    private Vector3 moveDirection;
    private CharacterController characterController;
    private Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (characterController.isGrounded)
        {
            moveDirection = camera.transform.right * Input.GetAxis("Horizontal") + camera.transform.forward * Input.GetAxis("Vertical");
            moveDirection *= MovementSpeed;
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = JumpSpeed;
            }
        }

        moveDirection.y -= Gravity * Time.deltaTime;
        characterController.Move(moveDirection * Time.deltaTime);
    }
}
