using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Material material;
    [SerializeField] BoxCollider weaponCollider;
    [SerializeField] Enemy enemy;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();
    }

    // 패링을 할 수 있는 타이밍
    public void ParryingTiming()
    {
        StartCoroutine(ParryTimingRoutine());

        Debug.Log("패링 타이밍!");
    }

    private IEnumerator ParryTimingRoutine()
    {
        // 무기에 콜라이더 활성화
        weaponCollider.enabled = true;
        // 무기 색 변경하고
        material.color =  Color.green ;
        // 애니메이션 속도 느려지고
        animator.speed = 0.05f;
        yield return new WaitForSeconds(1f);
        animator.speed = 1f;
        material.color = Color.white ;
    }

    // 패링 실패시
    public void ParryFailed()
    {
        // 애니메이션 종료 시점에서 발동
        // 무기 콜라이어에 충돌이 없으면
        // 무기 콜라이더 종료
        weaponCollider.enabled = false;
        // 데미지 입히기
        Debug.Log("패링 실패!");
        enemy.ChangeState(Enemy.State.Return);
    }

    // 패링 성공 시
    public void ParrySccuess()
    {
        // 실패 애니메이션
        animator.SetTrigger("HitTrigger");
        // 몬스터 피격
        // 돌아가기
        Debug.Log("패링 성공!");
        material.SetColor("_EmissionColor", Color.black * 1f);
        weaponCollider.enabled = false;
    }

    // 피격 애니메이션이 끝나면 돌아가기
    public void ReturnPosition()
    {
        enemy.ChangeState(Enemy.State.Return);
    }

}
