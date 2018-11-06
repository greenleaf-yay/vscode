using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum State { PATROL, TRACE, ATTACK, DIE }
    public State _state = State.PATROL;
    public float attackDist = 5.0f;
    public float traceDist = 10.0f;
    public bool isDie = false;
    //public bool showHpBar = false;

    private Transform playerTr;
    private Transform enemyTr;
    private WaitForSeconds ws;
    private MoveAgent mvAgent;
    private Animator _animator;
    private EnemyFire _enemyFire;
    private EnemyDamage _enemyDamage;
    private readonly int hashMove = Animator.
        StringToHash("IsMove");
    private readonly int hashSpeed = Animator.
        StringToHash("Speed");
    private readonly int hashDie = Animator.
        StringToHash("Die");
    private readonly int hashDieIdx = Animator.
        StringToHash("DieIdx");
    private readonly int hashOffset = Animator.
        StringToHash("Offset");
    private readonly int hashWalkSpeed = Animator.
        StringToHash("WalkSpeed");
    private readonly int hashPlayerDie = Animator.
        StringToHash("PlayerDie");

    private void Awake()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTr = player.GetComponent<Transform>();
        }
        enemyTr = GetComponent<Transform>();
        mvAgent = GetComponent<MoveAgent>();
        _animator = GetComponent<Animator>();
        _enemyFire = GetComponent<EnemyFire>();
        _enemyDamage = GetComponent<EnemyDamage>();
        ws = new WaitForSeconds(0.3f);
        _animator.SetFloat(hashOffset, Random.Range(0.0f, 1.0f));
        _animator.SetFloat(hashWalkSpeed, 
            Random.Range(1.0f, 1.2f));
    }

    private void OnEnable() // 해당 오브젝트가 활성화 될때 실행
    {
        StartCoroutine(CheckState());
        StartCoroutine(Action());
        Damage.OnPlayerDie += this.OnPlayerDie;

        this.gameObject.tag = "ENEMY";
        isDie = false;
        _enemyFire.isFire = false;
        //mvAgent.Stop();
        GetComponent<CapsuleCollider>().enabled = true;
    }

    private void OnDisable() // 해당 오브젝트가 비활성화 될때 실행
    {
        Damage.OnPlayerDie -= this.OnPlayerDie;
    }

    IEnumerator CheckState()
    {
        while(!isDie)
        {
            if (_state == State.DIE) yield break;
            float dist = Vector3.Distance(playerTr.position,
                enemyTr.position);
            if (dist <= attackDist)
            {
                _state = State.ATTACK;
            }
            else if (dist <= traceDist)
            {
                _state = State.TRACE;
            }
            else
            {
                _state = State.PATROL;
            }
            yield return ws;
        }
    }

    IEnumerator Action()
    {
        while(!isDie)
        {
            yield return ws;
            switch (_state)
            {
                case State.PATROL:
                    _enemyFire.isFire = false;
                    mvAgent.Patrolling = true;
                    _animator.SetBool(hashMove, true);
                    break;
                case State.TRACE:
                    _enemyFire.isFire = false;
                    mvAgent.TraceTarget = playerTr.position;
                    _animator.SetBool(hashMove, true);
                    break;
                case State.ATTACK:
                    _enemyFire.isFire = true;
                    mvAgent.Stop();
                    _animator.SetBool(hashMove, false);
                    break;
                case State.DIE:

                    this.gameObject.tag = "Untagged";
                    //StopAllCoroutines();
                    isDie = true;
                    _enemyFire.isFire = false;
                    mvAgent.Stop();
                    _animator.SetInteger(hashDieIdx,
                        Random.Range(0, 3));
                    _animator.SetTrigger(hashDie);
                    GetComponent<CapsuleCollider>().enabled = false;
                    StartCoroutine(PushEnemyPool());

                    break;
            }
        }
    }

    IEnumerator PushEnemyPool()
    {
        yield return new WaitForSeconds(5.0f);
        isDie = false;
        _enemyDamage.hp = 100.0f;

        _enemyDamage.ShowHpBar();

        gameObject.tag = "ENEMY";
        _state = State.PATROL;
        GetComponent<CapsuleCollider>().enabled = true;
        gameObject.SetActive(false);
    }

    void Update()
    {
        _animator.SetFloat(hashSpeed, mvAgent.Speed);

        //if (GameManager.instance.show)
        //{
        //    _enemyDamage.ShowHpBar();
        //    GameManager.instance.show = false;
        //}
    }

    public void OnPlayerDie()
    {
        mvAgent.Stop();
        _enemyFire.isFire = false;
        StopAllCoroutines();

        _animator.SetTrigger(hashPlayerDie);
    }
}