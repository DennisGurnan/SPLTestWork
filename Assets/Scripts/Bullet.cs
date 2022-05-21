using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Tooltip("Ёфект взрыва")]
    public GameObject FxPrefab;
    [Tooltip("ћаксимальна€ дальность полета")]
    public float maxDistance = 10;

    private Vector3 _startPosition;

    void Start()
    {
        _startPosition = transform.position;
    }

    void Update()
    {
        if (Vector3.Distance(_startPosition, transform.position) > maxDistance) DestroyMe();
    }

    private void OnCollisionEnter(Collision collision)
    {
        DestroyMe();
    }

    private void DestroyMe()
    {
        Instantiate(FxPrefab, transform.position, transform.rotation);
        Destroy(gameObject, 0.1f);
    }
}
