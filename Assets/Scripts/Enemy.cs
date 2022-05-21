using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Enemy : MonoBehaviour
{
    [Header("Sounds")]
    public AudioSource crySound;
    public AudioSource shotSound;

    [Header("Skin")]
    public Color selectedColor = Color.yellow;
    public Color normalColor = Color.red;
    [Tooltip("Цвет чего изменяем")]
    public MeshRenderer skinRenderer;
    [Tooltip("Время, которое объект считается выбраным")]
    public float selectedTime;

    [Header("Shot")]
    [Tooltip("Префаб снаряда")]
    public GameObject bulletPrefab;
    [Tooltip("Точка вылета снаряда")]
    public GameObject bulletOut;
    [Tooltip("Энергия вылета снаряда")]
    public float bulletPower;
    [Tooltip("Время перезарядки")]
    public float bulletRechargeTime;
    [Tooltip("Дистанция видимости противника")]
    public float sightRange = 10.0f;
    [Tooltip("Время контузии")]
    public float hitedTime = 0.3f;

    [Header("Movement")]
    public bool hasMovement;
    public float MovementSpeed = 1;
    [Tooltip("Слой земли")]
    public LayerMask groundLayer;

    // private variables
    private GameObject _player;
    private Rigidbody _rigidbody;
    private CapsuleCollider _capsuleCollider;
    private bool _isGrounded;
    private float _rechargeTimer; // Таймер перезарядки оружия
    private Vector3 _moveVector = Vector3.zero;
    private Vector3 _startPosition;
    private bool _isSelected = false;
    private bool _iNeedToMove = false;
    private float _selectedTimer = 0; // Таймер проверки цвета кожи
     private float _hitedTimer = 0; // Таймер паузы при попадании

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(_player.transform);
        Vector3 bottomCenterPoint = new Vector3(_capsuleCollider.bounds.center.x, _capsuleCollider.bounds.min.y, _capsuleCollider.bounds.center.z);
        _isGrounded = Physics.CheckCapsule(_capsuleCollider.bounds.center, bottomCenterPoint, _capsuleCollider.bounds.size.x / 2 * 0.9f, groundLayer);

        // Move to target position
        if (hasMovement && _isGrounded)
        {
            if(_moveVector == Vector3.zero)
            {
                _moveVector = new Vector3(1, 0, 0);
            }

            RaycastHit hit;
            Vector3 hitFrom = transform.position + new Vector3(Mathf.Sign(_moveVector.x) * 0.5f, 1, 0);
            // Если по ходу движения препятствие - меняем направление
            if (Physics.Raycast(hitFrom, _moveVector, out hit, 0.1f))
            {
                _moveVector.x = -_moveVector.x;
            }

            // Если пол заканчивается - меняем направление
            Vector3 floorScanPoint = hitFrom + new Vector3(-Mathf.Sign(_moveVector.x) * 0.1f, 1f, 0);
            Vector3 floorScanDir = hitFrom - floorScanPoint;
            if (!Physics.Raycast(hitFrom, floorScanDir, out hit, Vector3.Distance(hitFrom, floorScanDir) + 0.1f))
            {
                _moveVector.x = -_moveVector.x;
            }

            // Если сместились по Z - возвращаемся
            if(Mathf.Abs(transform.position.z - _startPosition.z) > 0.01f)
            {
                _moveVector.z = _startPosition.z - transform.position.z;
            }
            else
            {
                _moveVector.z = 0.0f;
            }
        }
        else
        {
            // Если столкнули, пытаемся вернуться на исходную
            if(Vector3.Distance(_startPosition, transform.position) > 0.1f)
            {
                _moveVector = _startPosition - transform.position;
                _moveVector.Normalize();
                if(_hitedTimer <= 0) _iNeedToMove = true;
            }
            else
            {
                _iNeedToMove = false;
            }
        }

        // Skin color check
        if (_isSelected)
        {
            if (_selectedTimer > 0) _selectedTimer -= Time.deltaTime;
            else
            {
                _isSelected = false;
                skinRenderer.material.color = normalColor;
            }
        }

        // Recharge timer
        if (_rechargeTimer > 0) _rechargeTimer -= Time.deltaTime;
        // Hited timer
        if (_hitedTimer > 0) _hitedTimer -= Time.deltaTime;

        // Кажется я упал?
        if (transform.position.y < -1f) IAmDie();
    }

    private void FixedUpdate()
    {
        // Move to target position
        if ((_iNeedToMove || hasMovement) && _isGrounded && (_hitedTimer <= 0))
        {
            _rigidbody.MovePosition(_rigidbody.position + _moveVector * MovementSpeed * Time.fixedDeltaTime);
        }

        // Fire control
        if(_rechargeTimer <= 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(bulletOut.transform.position, bulletOut.transform.TransformDirection(Vector3.forward), out hit, sightRange))
            {
                Debug.DrawRay(bulletOut.transform.position, bulletOut.transform.TransformDirection(Vector3.forward) * sightRange, Color.yellow);
                if (hit.collider.tag == "Player")
                {
                    _rechargeTimer = bulletRechargeTime;
                    GameObject bullet = Instantiate(bulletPrefab, bulletOut.transform.position, bulletOut.transform.rotation);
                    bullet.GetComponent<Rigidbody>().AddForce(bulletOut.transform.forward * bulletPower, ForceMode.Impulse);
                    if (shotSound != null) shotSound.Play();
                }
            }
        }
    }

    public void OnAim()
    {
        _isSelected = true;
        _selectedTimer = selectedTime;
        skinRenderer.material.color = selectedColor;
    }

    private void IAmDie()
    {
        GameObject.Destroy(gameObject, 1);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Bullet")
        {
            if (crySound != null) crySound.Play();
            //hitedTimer = hitedTime;
        }
    }
}
