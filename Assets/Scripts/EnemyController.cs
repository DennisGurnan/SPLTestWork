using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class EnemyController : MonoBehaviour
{
    [Header("Skin")]
    public Color selectedColor = Color.yellow;
    public Color normalColor = Color.red;
    public MeshRenderer skinRenderer;
    public float selectedTime;

    [Header("Shot")]
    public GameObject bulletPrefab;
    public GameObject bulletOut;
    public float bulletPower;
    public float bulletRechargeTime;

    [Header("Movement")]
    public bool hasMovement;
    public float MovementSpeed = 1;
    public LayerMask ground;

    // private variables
    private GameObject player;
    private Rigidbody rigidbody;
    private CapsuleCollider capsuleCollider;
    private bool isGrounded;
    private float rechargeTimer;
    private Vector3 moveVector = Vector3.zero;
    private Vector3 startPosition;
    private bool isSelected = false;
    private bool iNeedToMove = false;
    private float selectedTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        player = GameObject.FindGameObjectWithTag("Player");
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player.transform);
        Vector3 bottomCenterPoint = new Vector3(capsuleCollider.bounds.center.x, capsuleCollider.bounds.min.y, capsuleCollider.bounds.center.z);
        isGrounded = Physics.CheckCapsule(capsuleCollider.bounds.center, bottomCenterPoint, capsuleCollider.bounds.size.x / 2 * 0.9f, ground);

        // Move to target position
        if (hasMovement && isGrounded)
        {
            if(moveVector == Vector3.zero)
            {
                moveVector = new Vector3(1, 0, 0);
            }
            RaycastHit hit;
            Vector3 hitFrom = transform.position + new Vector3(Mathf.Sign(moveVector.x) * 0.5f, 1, 0);
            // ≈сли по ходу движени€ преп€тствие - мен€ем направление
            Debug.DrawRay(hitFrom, moveVector, Color.red);
            if (Physics.Raycast(hitFrom, moveVector, out hit, 0.1f))
            {
                moveVector.x = -moveVector.x;
            }
            // ≈сли пол заканчиваетс€ - мен€ем направление
            Vector3 floorScanPoint = hitFrom + new Vector3(-Mathf.Sign(moveVector.x) * 0.1f, 1f, 0);
            Vector3 floorScanDir = hitFrom - floorScanPoint;
            Debug.DrawRay(hitFrom, floorScanDir, Color.green);
            if (!Physics.Raycast(hitFrom, floorScanDir, out hit, Vector3.Distance(hitFrom, floorScanDir) + 0.1f))
            {
                moveVector.x = -moveVector.x;
            }
            // ≈сли сместились по Z - возвращаемс€
            if(Mathf.Abs(transform.position.z - startPosition.z) > 0.01f)
            {
                moveVector.z = startPosition.z - transform.position.z;
            }
            else
            {
                moveVector.z = 0.0f;
            }
        }
        else
        {
            // ≈сли столкнули, пытаемс€ вернутьс€ на исходную
            if(Vector3.Distance(startPosition, transform.position) > 0.1f)
            {
                moveVector = startPosition - transform.position;
                moveVector.Normalize();
                iNeedToMove = true;
            }
            else
            {
                iNeedToMove = false;
            }
        }

        // Skin color check
        if (isSelected)
        {
            if (selectedTimer > 0) selectedTimer -= Time.deltaTime;
            else
            {
                isSelected = false;
                skinRenderer.material.color = normalColor;
            }
        }

        // Recharge timer
        if (rechargeTimer > 0) rechargeTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        // Move to target position
        if ((iNeedToMove || hasMovement) && isGrounded)
        {
            rigidbody.MovePosition(rigidbody.position + moveVector * MovementSpeed * Time.fixedDeltaTime);
        }

        // Fire control
        if(rechargeTimer <= 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(bulletOut.transform.position, bulletOut.transform.TransformDirection(Vector3.forward), out hit, 20))
            {
                Debug.DrawRay(bulletOut.transform.position, bulletOut.transform.TransformDirection(Vector3.forward) * 20, Color.yellow);
                if (hit.collider.tag == "Player")
                {
                    rechargeTimer = bulletRechargeTime;
                    GameObject bullet = Instantiate(bulletPrefab, bulletOut.transform.position, bulletOut.transform.rotation);
                    bullet.GetComponent<Rigidbody>().AddForce(bulletOut.transform.forward * bulletPower, ForceMode.Impulse);
                }
            }
        }
    }

    public void OnAim()
    {
        isSelected = true;
        selectedTimer = selectedTime;
        skinRenderer.material.color = selectedColor;
    }
}
