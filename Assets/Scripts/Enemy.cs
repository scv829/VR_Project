using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // 1. 처음에 대기 Idle 상태
    // 2. 그다음 일정 시간만큼 왼 / 오 방향으로 이동
    // 3. 그리고 플레이어한테 돌진
    // 4. 돌진하기 전에 자신의 원래 위치 기억
    // 5. 플레이어 한테 돌진 후 공격 범위안에 들어왔으면 
    // 6. 플레이어 공격 실행
    // 7. 플레이어 공격 후 다시 돌아가기 기억한 위치로 돌아가기
    // 8. 2번으로 돌아가기


    [SerializeField] PlayerController player;       
    [SerializeField] Vector3 returnPos;             // 돌진 전 위치
    [SerializeField] float moveSpeed;               // 이동 속도 
    [SerializeField] Animator animator;             // 애니메이터
    [SerializeField] CinemachineDollyCart cart;     // 시네머시 돌리카트
    [SerializeField] AttackArea attackArea;
    [SerializeField] Vector3 startPos;              // 시작 위치
    [SerializeField] bool isStart;                  // 시작했는지 확인

    [Header("State")]
    [SerializeField] State curState;
    public enum State { Idle, Move, Trace, Attack, Return, Size }
    private BaseState[] states = new BaseState[(int)State.Size];
    public void StartMove() => isStart = true;

    private void Awake()
    {
        states[(int)State.Idle] = new IdleState(this);
        states[(int)State.Move] = new MoveState(this);
        states[(int)State.Trace] = new TraceState(this);
        states[(int)State.Attack] = new AttackState(this);
        states[(int)State.Return] = new ReturnState(this);

        animator = gameObject.GetComponent<Animator>();
        startPos = gameObject.transform.position;
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        states[(int)curState].Enter();
    }

    private void Update()
    {
        states[(int)curState].Update();
    }

    public void ChangeState(State state)
    {
        states[(int)curState].Exit();
        curState = state;
        states[(int)curState].Enter();
    }

    public void Reset()
    {
        transform.position = startPos;
        ChangeState(State.Idle);
        isStart = false;
    }

    private class IdleState : BaseState
    {
        private Enemy enemy;

        public IdleState(Enemy enemy)
        {
            this.enemy = enemy;
        }

        public override void Update()
        {
            // 시작 했으면 움직이기
            if(enemy.isStart)
            {
                enemy.ChangeState(State.Move);
            }
        }
    }


    /// <summary>
    /// 좌우로 움직이는 상태
    /// 역할: 일정시간 만큼 좌우로 움직이다가 시간이 다 지나면 돌격
    /// </summary>
    private class MoveState : BaseState
    {
        private Enemy enemy;
        private float refeatTime;
        private float delayTime;
        private bool traceStart;

        private Coroutine moveRoutine;

        public MoveState(Enemy enemy)
        {
            this.enemy = enemy;
            moveRoutine = null;
        }

        public override void Enter()
        {
            // 좌우 반복할 횟수
            refeatTime = Random.Range(1, 4);
            traceStart = false;
            enemy.animator.SetFloat("Turn", 0);
        }

        public override void Update()
        {
            if (moveRoutine == null)
            {
                moveRoutine = enemy.StartCoroutine(MoveRoutine());
            }

            if (traceStart)
            {
                enemy.ChangeState(State.Trace);
            }
        }

        public override void Exit()
        {
            if (moveRoutine != null)
            {
                enemy.StopCoroutine(moveRoutine);
                moveRoutine = null;
            }
            enemy.animator.SetFloat("Turn", 0);
        }

        private IEnumerator MoveRoutine()
        {
            

            bool moveDir = true;    // +: 우측 , -: 좌측
            for (int i = 0; i < refeatTime; i++)
            {
                float delay = Random.Range(1f, 3f);  // 1~3초 만큼 돌리트랙에서 이동
                float value = 0f;                       
                enemy.animator.SetFloat("Turn", (moveDir ? 1 : -1));
                while (value < delay)
                {
                    enemy.cart.m_Position +=  (moveDir ? 1 : -1 ) * enemy.moveSpeed * Time.deltaTime;
                    value += Time.deltaTime;
                    enemy.transform.LookAt(enemy.player.gameObject.transform.position);
                    yield return null;
                }
                moveDir = !moveDir;
                yield return null;
            }
            traceStart = true;
        }
    }

    /// <summary>
    /// 돌진 상태
    /// 역할: 플레이어에게 돌진 후 공격 범위 안에 들어 오면 공격
    /// </summary>
    private class TraceState : BaseState
    {
        private Enemy enemy;
        private float traceSpeed;   // 돌진 속도

        public TraceState(Enemy enemy) 
        { 
            this.enemy = enemy;
            traceSpeed = 6f;
        }

        public override void Enter()
        {
            // 돌진 애니메이션 실행
            enemy.animator.SetTrigger("TraceTrigger");
            // 돌진 하기 전 지금 위치
            enemy.returnPos = enemy.transform.position;
        }

        public override void Update()
        {
            // 플레이어를 향해 돌진
            enemy.transform.LookAt(enemy.player.gameObject.transform.position);
            Vector3 playerPos = new Vector3(enemy.player.transform.position.x, 0f, enemy.player.transform.position.z);
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, playerPos, traceSpeed * Time.deltaTime);

            // 만약 공격 범위에 들어 왔으면 공격 실행
            if (enemy.attackArea.Target is not null)
            {
                enemy.ChangeState(State.Attack);
            }
        }

    }

    /// <summary>
    /// 공격 상태
    /// 역할 : 플레이어를 공격
    /// 공격 성공: 플레이어 피격 후 돌아가기
    /// 공격 실패: 돌아가기
    /// </summary>
    private class AttackState : BaseState
    {

        private Enemy enemy;

        public AttackState(Enemy enemy)
        {
            this.enemy = enemy;
        }

        public override void Enter()
        {
            // 공격 애니메이션 실행
            enemy.animator.SetTrigger("AttackTrigger");
        }

    }

    private class ReturnState : BaseState
    {
        private Enemy enemy;
        private float returnSpeed;

        Collider[] cartCollider;

        public ReturnState(Enemy enemy)
        {
            this.enemy = enemy;
            returnSpeed = enemy.moveSpeed * 2;
            cartCollider = new Collider[1];
        }

        public override void Enter()
        {
            // 돌아가는 애니메이션 실행
            enemy.animator.SetBool("Return", true);
            enemy.animator.speed = returnSpeed;
            cartCollider[0] = null;
        }

        public override void Update()
        {
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, enemy.returnPos, returnSpeed * Time.deltaTime);
            
            if(Physics.OverlapSphereNonAlloc(enemy.transform.position, 1f, cartCollider, LayerMask.GetMask("Cart")) == 1)
            {
                enemy.transform.position = cartCollider[0].gameObject.transform.position;
                enemy.ChangeState(State.Move);
            }

        }

        public override void Exit()
        {
            enemy.animator.SetBool("Return", false);
            enemy.animator.speed = 1;   
        }
    }

}
