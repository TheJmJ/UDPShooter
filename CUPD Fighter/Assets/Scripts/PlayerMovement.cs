using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float rawMovementSpeed = 0.2f;
    [SerializeField] float runMultiplier = 1.5f;
    Vector3 direction;

    float jumpTime;
    float jumpForce = 10f;
    float jumpCD = 2f;


    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Keyboard movement
        float movementSpeed = rawMovementSpeed;
        direction = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += -Vector3.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += -Vector3.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector3.right;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed *= runMultiplier;
        }
        if(Input.GetKeyDown(KeyCode.Space) && Time.time - jumpTime > jumpCD)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpTime = Time.time;
        }
        transform.Translate(direction * movementSpeed, Space.Self);
        rb.AddForce(transform.InverseTransformDirection(direction) * movementSpeed);

    }
}
