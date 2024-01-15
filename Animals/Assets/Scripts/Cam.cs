using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Cam : MonoBehaviour
{
    public float movementSpeed;
    public float rotationSpeed;


    void Update()
    {
        // Move camera with WASD keys
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontal, 0.0f, vertical) * movementSpeed * Time.deltaTime;
        transform.Translate(movement);

        // Rotate camera with mouse movement
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");
        Vector3 rotation = new Vector3(mouseY, mouseX, 0.0f) * rotationSpeed;
        transform.Rotate(rotation);

        // Clamp vertical rotation to prevent camera flipping
        Vector3 currentRotation = transform.rotation.eulerAngles;
        currentRotation.z = 0.0f; // Set vertical rotation to zero
        transform.rotation = Quaternion.Euler(currentRotation);
    }
}