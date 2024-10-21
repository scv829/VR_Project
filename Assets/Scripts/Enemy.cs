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


    [SerializeField] private PlayerController player;
    [SerializeField] private Vector3 returnPos;     // 돌진 전 위치
    [SerializeField] private float moveSpeed;       // 이동 속도 
    [SerializeField] Animator animator;             // 애니메이터 

    [Header("State")]
    [SerializeField] State curState;
    public enum State { Idle, Trace, Attack, Return, Die, Size }
    private BaseState[] states = new BaseState[(int)State.Size];

    private void Awake()
    {
        states[(int)State.Idle] = new IdleState(this);
        states[(int)State.Trace] = new TraceState(this);
        states[(int)State.Attack] = new AttackState(this);
        states[(int)State.Return] = new ReturnState(this);
        states[(int)State.Die] = new DieState(this);

        animator = gameObject.GetComponent<Animator>();
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

    /// <summary>
    /// 대기 상태
    /// 역할: 일정시간 만큼 좌우로 움직이다가 시간이 다 지나면 돌격
    /// </summary>
    private class IdleState : BaseState
    {
        private Enemy enemy;
        private float refeatTime;
        private float delayTime;
        private bool traceStart;

        private Coroutine moveRoutine;

        public IdleState(Enemy enemy)
        {
            this.enemy = enemy;
            moveRoutine = null;
        }

        public override void Enter()
        {
            // 좌우 반복할 횟수
            refeatTime = Random.Range(1, 4);
            traceStart = false;
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
        }

        private IEnumerator MoveRoutine()
        {
            // 좌우로 이동하다가 출발

/*            bool moveDir = true;    // +: 우측 , -: 좌측
            for (int i = 0; i < refeatTime; i++)
            {
                float distance = Random.Range(5f, 15f);  // 5~15 만큼이동
                Vector3 pos = enemy.transform.position + ((moveDir) ? Vector3.right : Vector3.left) * enemy.moveSpeed * distance;
                enemy.animator.SetBool("Turn", moveDir);
                while (true)
                {
                    enemy.transform.Translate(((moveDir) ? Vector3.right : Vector3.left) * enemy.moveSpeed * Time.deltaTime);
                    if (enemy.transform.position.Equals(pos)) break;
                }
                moveDir = !moveDir;
            }*/

            traceStart = true;
                yield return null;
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
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, enemy.player.gameObject.transform.position, traceSpeed * Time.deltaTime);

            // 만약 공격 범위에 들어 왔으면 공격 실행
            if (Physics.OverlapSphere(enemy.transform.position, 2f, LayerMask.NameToLayer("Player")).Length > 0)
            {
                enemy.ChangeState(State.Attack);
            }
        }

        public override void Exit()
        {
            base.Exit();
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

        public override void Update()
        {
            // 공격로직
            Debug.Log("공격 로직!");
            enemy.ChangeState(State.Return);
        }

    }

    private class ReturnState : BaseState
    {
        private Enemy enemy;

        public ReturnState(Enemy enemy)
        {
            this.enemy = enemy;
        }

        public override void Enter()
        {
            // 돌아가는 애니메이션 실행
            enemy.animator.SetBool("Return", true);
        }

        public override void Update()
        {
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, enemy.returnPos, enemy.moveSpeed * Time.deltaTime);
            
            if(enemy.transform.position.Equals(enemy.returnPos))
            {
                enemy.ChangeState(State.Idle);
            }
        }

        public override void Exit()
        {
            enemy.animator.SetBool("Return", false);
        }
    }

    private class DieState : BaseState
    {
        private Enemy enemy;

        public DieState(Enemy enemy)
        {
            this.enemy = enemy;
        }

        public override void Enter()
        {
            // 사망 애니메이션 실행
            Debug.Log($"{enemy.name} 사망!");
        }

    }

}
