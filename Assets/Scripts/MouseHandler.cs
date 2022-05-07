using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHandler : MonoBehaviour
{
    public float horisontalSpeed = 1f;
    public float verticalSpeed = 1f;

    private float xRotation = 0.0f;
    private float yRotation = 0.0f;
    private Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * horisontalSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * verticalSpeed;

        yRotation += mouseX;
        yRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        camera.transform.eulerAngles = new Vector3(xRotation, yRotation, 0.0f);
    }
}
