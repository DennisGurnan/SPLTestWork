using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [Header("Sounds")]
    public AudioSource crySound;
    public AudioSource shotSound;

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

        // Выстрел
        if(Input.GetButton("Fire1") && rechargeTimer <= 0)
        {
            rechargeTimer = bulletRechargeTime;
            GameObject bullet = Instantiate(bulletPrefab, bulletOut.transform.position, bulletOut.transform.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(bulletOut.transform.forward * bulletPower, ForceMode.Impulse);
            if (shotSound != null) shotSound.Play();
        }
        if (rechargeTimer > 0) rechargeTimer -= Time.deltaTime;

        // Я проиграл?
        if (transform.position.y < -2f)
        {
            crySound.Play();
            GUIController.Singleton.ShowDefard();
        }
    }

    private void FixedUpdate()
    {
        rigidbody.MovePosition(rigidbody.position + moveVector * MovementSpeed * Time.fixedDeltaTime);
        // Целеуказатель
        RaycastHit hit;
        if (Physics.Raycast(bulletOut.transform.position, bulletOut.transform.TransformDirection(Vector3.forward), out hit, 20))
        {
            Debug.DrawRay(bulletOut.transform.position, bulletOut.transform.TransformDirection(Vector3.forward) * 20, Color.yellow);
            if(hit.collider.tag == "Enemy")
            {
                hit.collider.GetComponent<EnemyController>().OnAim();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "Finish") GUIController.Singleton.ShowVictory();
    }
}
