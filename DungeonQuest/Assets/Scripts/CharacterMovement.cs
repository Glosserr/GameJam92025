using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float speed;
    public float gravity;
    public float jumpForce;
    float velocityY = 0;

    CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        velocityY += gravity * Time.deltaTime;

        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");  
        
        Vector3 move = (transform.right * inputX + transform.forward * inputZ).normalized;
     
        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            velocityY = jumpForce;
            Debug.Log(move);
            if (move == Vector3.zero)
            {
                speed = jumpForce;
            }
        }

        Vector3 velocity = move * speed;

        velocity.y = velocityY;

        controller.Move(velocity * Time.deltaTime);

        if (controller.isGrounded)
        {
            velocityY = 0;
        }
    }
}