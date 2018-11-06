using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // 클래스는 해당 어트리뷰트를 명시해야 인스펙터에 노출됨
public class PlayerAnim
{
    public AnimationClip idle;
    public AnimationClip runF;
    public AnimationClip runB;
    public AnimationClip runL;
    public AnimationClip runR;
}

public class PlayerCtrl : MonoBehaviour
{
    //// 스크립트 실행 시 한 번만 호출되는 함수. Start 함수 호출 전에 호출됨.
    //// 스크립트가 비활성화 되어 있어도 호출됨. 코루틴 함수 호출 불가.
    //private void Awake()
    //{

    //}

    //// Update 함수 호출 전에 한 번만 호출.
    //private void Start()
    //{

    //}

    //// 매 프레임마다 호출
    //private void Update()
    //{

    //}

    //// 모든 Update 함수가 호출 된 후에 호출
    //private void LateUpdate()
    //{

    //}

    //// 물리엔진의 시뮬레이션 계산 주기로 기본값은 0.02초
    //private void FixedUpdate()
    //{

    //}

    //// 게임오브젝트 또는 스크립트 활성화 시 호출
    //private void OnEnable()
    //{

    //}

    //// 게임오브젝트 또는 스크립트 비 활성화 시 호출
    //private void OnDisable()
    //{

    //}

    private float h = 0.0f;
    private float v = 0.0f;
    private float r = 0.0f;

    private Transform tr;

    public float moveSpeed = 10.0f;
    public float rotSpeed = 80.0f;
    public PlayerAnim _playerAnim;
    public Animation anim;

    private void OnEnable()
    {
        GameManager.OnItemChange += UpdateSetup;
    }

    void UpdateSetup()
    {
        moveSpeed = GameManager.instance._gameData.speed;
    }

    private void Start()
    {
        tr = this.gameObject.GetComponent<Transform>(); // 캐쉬 처리
        anim = GetComponent<Animation>();
        anim.clip = _playerAnim.idle;
        anim.Play();
        moveSpeed = GameManager.instance._gameData.speed;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (Input.GetKeyDown("escape"))
            Cursor.lockState = CursorLockMode.None;

        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        r = Input.GetAxis("Mouse X");

        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime,
            Space.Self);
        tr.Rotate(Vector3.up * r * rotSpeed * Time.deltaTime);

        if (v >= 0.1f)
        {
            //anim.clip = _playerAnim.runF;
            anim.CrossFade(_playerAnim.runF.name, 0.3f);
        }
        else if (v <= -0.1f)
        {
            anim.CrossFade(_playerAnim.runB.name, 0.3f);
        }
        else if (h >= 0.1f)
        {
            anim.CrossFade(_playerAnim.runR.name, 0.3f);
        }
        else if (h <= -0.1f)
        {
            anim.CrossFade(_playerAnim.runL.name, 0.3f);
        }
        else
        {
            anim.CrossFade(_playerAnim.idle.name, 0.3f);
        }

        //Debug.Log("h = " + h.ToString());
        //Debug.Log("v = " + v.ToString());
    }
}
