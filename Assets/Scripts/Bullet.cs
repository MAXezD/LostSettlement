using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        StartCoroutine(DestroyBullet());
    }
    IEnumerator DestroyBullet()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
    }
}
