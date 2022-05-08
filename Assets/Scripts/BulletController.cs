using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public GameObject FxPrefab;
    public float maxDistance = 10;

    private Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(playerTransform.position, transform.position) > maxDistance) DestroyMe();
    }

    private void OnCollisionEnter(Collision collision)
    {
        DestroyMe();
    }

    private void DestroyMe()
    {
        Instantiate(FxPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
