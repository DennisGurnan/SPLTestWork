using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [Header("Shot")]
    public GameObject bulletPrefab;
    public GameObject bulletOut;
    public float bulletPower;
    public float bulletRechargeTime;
    [Header("Movement")]
    public float MovementSpeed = 1;
    public float JumpHeiht = 1;
    public LayerMask ground;

    private Camera camera;
    private float rechargeTimer;
    private Vector3 moveVector;

    private Rigidbody rigidbody;
    private CapsuleCollider capsuleCollider;

    void Start()
    {
        camera = Camera.main;
        rigidbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }


    void Update()
    {
        // Персонаж находится на замле
        Vector3 bottomCenterPoint = new Vector3(capsuleCollider.bounds.center.x, capsuleCollider.bounds.min.y, capsuleCollider.bounds.center.z);
        // Для прыжков "в воздухе" уменьшить коэфициент 0.9f
        bool isGrounded = Physics.CheckCapsule(capsuleCollider.bounds.center, bottomCenterPoint, capsuleCollider.bounds.size.x / 2 * 0.9f, ground);

        // Направление движения
        moveVector = camera.transform.right * Input.GetAxis("Horizontal") + camera.transform.forward * Input.GetAxis("Vertical");
        
        // Прыжок
        if(Input.GetButton("Jump") && isGrounded)
        {
            rigidbody.AddForce(Vector3.up * Mathf.Sqrt(-JumpHeiht * Physics.gravity.y), ForceMode.VelocityChange);
        }
    }

    private void FixedUpdate()
    {
        rigidbody.MovePosition(rigidbody.position + moveVector * MovementSpeed * Time.fixedDeltaTime);
    }

}
