using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHandler : MonoBehaviour
{
    public float horisontalSpeed = 1f;
    public float verticalSpeed = 1f;

    private float _xRotation = 0.0f;
    private float _yRotation = 0.0f;
    private Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse Y") * horisontalSpeed;
        float mouseY = Input.GetAxis("Mouse X") * verticalSpeed;

        _xRotation -= mouseX;
        _yRotation += mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90, 90);

        transform.eulerAngles = new Vector3(0.0f, _yRotation, 0.0f);
        camera.transform.eulerAngles = new Vector3(_xRotation, _yRotation, 0.0f);
    }
}
