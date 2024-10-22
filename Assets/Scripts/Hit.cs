using System.Collections;
using System.Collections.Generic;
using Unity.XR.Oculus.Input;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Hit : MonoBehaviour
{
    [SerializeField] PlayerController player;

    public PlayerController SetPlayer { set { player = value; } }

    // 트리거가 작동하고
    private void OnTriggerEnter(Collider other)
    {
        // 들어온 오브젝트가 적의 공격이라면
        if (other.gameObject.CompareTag("Bullet"))
        {
           
            // 공격받는 로직
            player.HitDamage(other.gameObject.name);

            // 해당 오브젝트를 사자리게 한다
            Destroy(other.gameObject);
        }
    }
}
