using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Player : MonoBehaviour
{
    [Header("Sounds")]
    public AudioSource crySound;
    public AudioSource shotSound;

    [Header("Shot")]
    [Tooltip("Префаб снаряда")]
    public GameObject bulletPrefab;
    [Tooltip("Точка вылета снаряда")]
    public GameObject bulletOut;
    [Tooltip("Энергия вылета снаряда")]
    public float bulletPower;
    [Tooltip("Время перезарядки")]
    public float bulletRechargeTime;

    [Header("Movement")]
    public float MovementSpeed = 1;
    public float JumpHeiht = 1;
    [Tooltip("Слой земли")]
    public LayerMask groundLayer;

    private Camera _camera;
    private float _rechargeTimer;
    private Vector3 _moveVector;

    private Rigidbody _rigidbody;
    private CapsuleCollider _capsuleCollider;

    void Start()
    {
        _camera = Camera.main;
        _rigidbody = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }


    void Update()
    {
        // Персонаж находится на замле
        Vector3 bottomCenterPoint = new Vector3(_capsuleCollider.bounds.center.x, _capsuleCollider.bounds.min.y, _capsuleCollider.bounds.center.z);
        // Для прыжков "в воздухе" уменьшить коэфициент 0.9f
        bool isGrounded = Physics.CheckCapsule(_capsuleCollider.bounds.center, bottomCenterPoint, _capsuleCollider.bounds.size.x / 2 * 0.9f, groundLayer);

        // Направление движения
        _moveVector = _camera.transform.right * Input.GetAxis("Horizontal") + _camera.transform.forward * Input.GetAxis("Vertical");
        
        // Прыжок
        if(Input.GetButton("Jump") && isGrounded)
        {
            _rigidbody.AddForce(Vector3.up * Mathf.Sqrt(-JumpHeiht * Physics.gravity.y), ForceMode.VelocityChange);
        }

        // Выстрел
        if(Input.GetButton("Fire1") && _rechargeTimer <= 0)
        {
            _rechargeTimer = bulletRechargeTime;
            GameObject bullet = Instantiate(bulletPrefab, bulletOut.transform.position, bulletOut.transform.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(bulletOut.transform.forward * bulletPower, ForceMode.Impulse);
            if (shotSound != null) shotSound.Play();
        }
        if (_rechargeTimer > 0) _rechargeTimer -= Time.deltaTime;

        // Я проиграл?
        if (transform.position.y < -2f)
        {
            crySound.Play();
            GUIManager.Singleton.ShowDefard();
        }
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(_rigidbody.position + _moveVector * MovementSpeed * Time.fixedDeltaTime);
        // Целеуказатель
        RaycastHit hit;
        if (Physics.Raycast(bulletOut.transform.position, bulletOut.transform.TransformDirection(Vector3.forward), out hit, 20))
        {
            Debug.DrawRay(bulletOut.transform.position, bulletOut.transform.TransformDirection(Vector3.forward) * 20, Color.yellow);
            if(hit.collider.tag == "Enemy")
            {
                hit.collider.GetComponent<Enemy>().OnAim();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "Finish") GUIManager.Singleton.ShowVictory();
    }
}
