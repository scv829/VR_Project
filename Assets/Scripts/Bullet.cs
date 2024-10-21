using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float moveSpeed;

    public void Setting(Transform target, float moveSpeed)
    {
        this.target = target;
        this.moveSpeed = moveSpeed;

        transform.LookAt(target);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

}
