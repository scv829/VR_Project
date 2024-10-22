using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.Primitives;

/// <summary>
/// 플레이어를 향해 공격을 하는 오브젝트
/// </summary>
public class Tower : MonoBehaviour
{
    [SerializeField] Bullet bulletPrefab;   // 총알 프리팹
    [SerializeField] Transform muzzlePoint;    // 총알이 나가는 총구

    [SerializeField] float moveSpeed;           // 총알이 날아가는 속도
    [SerializeField] Transform playerTransform; // 플레이어의 위치

    [SerializeField] float delayTime;           // 총알 생성 시간

    private Coroutine shootCoroutine;

    private void Awake()
    {
        shootCoroutine = null;
    }

    public void StartShooting()
    {
        if (shootCoroutine == null)
        {
            shootCoroutine = StartCoroutine(ShootRoutine());
        }
    }

    public void StopShooting()
    {
        if (shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
            shootCoroutine = null;
        }
    }

    private IEnumerator ShootRoutine()
    {
        WaitForSeconds delay = new WaitForSeconds(delayTime);

        while (true) 
        { 
            Bullet bullet = Instantiate(bulletPrefab);
            bullet.Setting(playerTransform, moveSpeed);
            Destroy(bullet, 10f);
            yield return delay;
        }
    }

}
