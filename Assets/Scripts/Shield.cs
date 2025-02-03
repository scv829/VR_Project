using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] Vector3 pos;
    [SerializeField] Quaternion quaternion;

    private void Awake()
    {
        pos = transform.position;
        quaternion = transform.rotation;
    }

    private void Update()
    {
        if(transform.position.y < -1f)
        {
            Reset();
        }
    }

    public void Reset()
    {
        transform.position = pos;
        transform.rotation = quaternion;
    }
}
