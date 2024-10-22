using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    [SerializeField] Transform player;

    public Transform Target { get { return player; } }

    private void Awake()
    {
        player = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") &&
            player == null)
        {
            player = other.gameObject.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") &&
    player != null)
        {
            player = null;
        }
    }
}
