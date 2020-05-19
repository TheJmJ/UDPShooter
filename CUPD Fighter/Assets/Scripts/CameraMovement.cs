using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Mouse Movement
        Vector2 mouseVector = Vector2.zero;
        mouseVector.x = Input.GetAxis("Mouse X");
        mouseVector.y = -Input.GetAxis("Mouse Y");

        // Rotation
        transform.parent.Rotate(Vector3.up, mouseVector.x);
        transform.Rotate(Vector3.right, mouseVector.y);

    }
}
