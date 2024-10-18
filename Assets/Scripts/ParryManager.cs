using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryManager : MonoBehaviour
{

    private void OnEnable()
    {
        Debug.Log("방패 장착!");
    }

    private void OnDisable()
    {
        Debug.Log("방패 해제!");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.collider.gameObject);
            Debug.Log($"패링 성공! 막은 공격 오브젝트의 이름 : {collision.collider.name} ");
        }
            Debug.Log($"현재 접촉중인 오브젝트 이름 : {collision.collider.name} ");
    }
}
