using System.Collections;
using System.Collections.Generic;
using Unity.XR.Oculus.Input;
using UnityEngine;
using UnityEngine.Events;


public class PlayerController : MonoBehaviour
{
    [SerializeField] Camera playerCamera;
    [SerializeField] Hit hit;

    private void Awake()
    {
        playerCamera = Camera.main;
        hit = GetComponent<Hit>();
        hit.SetPlayer = this;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        // 카메라가 움직일 때 마다 플레이어의 피격도 움직인다
        transform.position =playerCamera.transform.position;
    }


    public void HitDamage(string name)
    {
        // 일단 로그로 구현
        Debug.Log($"{name} 에게 공격을 받았다!");
    }

}
