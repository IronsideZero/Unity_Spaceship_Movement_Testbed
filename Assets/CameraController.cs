using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float rotateSpeed = 50f;
    private float tiltAngle = 0f;
    private float rotateAngle = 0f;

    public KeyCode moveUpKey = KeyCode.W;
    public KeyCode moveDownKey = KeyCode.S;
    public KeyCode moveLeftKey = KeyCode.A;
    public KeyCode moveRightKey = KeyCode.D;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(moveUpKey))
        {
            transform.Translate(Vector3.up * movementSpeed * Time.deltaTime);
        }
        if (Input.GetKey(moveDownKey))
        {
            transform.Translate(Vector3.down * movementSpeed * Time.deltaTime);
        }
        if (Input.GetKey(moveLeftKey))
        {
            transform.Translate(Vector3.left * movementSpeed * Time.deltaTime);
        }
        if (Input.GetKey(moveRightKey))
        {
            transform.Translate(Vector3.right * movementSpeed * Time.deltaTime);
        }

        // Tilt the camera up and down
        if (Input.GetKey(KeyCode.R))
        {
            tiltAngle += rotateSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.F))
        {
            tiltAngle -= rotateSpeed * Time.deltaTime;
        }
        tiltAngle = Mathf.Clamp(tiltAngle, -90f, 90f);
        transform.localRotation = Quaternion.Euler(tiltAngle, 0f, 0f);

        // Rotate the camera left and right on the Y-axis
        if (Input.GetKey(KeyCode.Q))
        {
            rotateAngle -= rotateSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            rotateAngle += rotateSpeed * Time.deltaTime;
        }
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, rotateAngle, transform.rotation.eulerAngles.z);
    }
}
