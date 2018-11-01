using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    public float speed = 1000.0f;
    public float damage = 20.0f;

    private Transform tr;
    private Rigidbody rb;
    private TrailRenderer trail;

    private void Awake()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
        damage = GameManager.instance._gameData.damage;
    }

    private void OnEnable() // 활성화 될 때마다 호출
    {
        rb.AddForce(transform.forward * speed);
        //StartCoroutine(BulletTimer());
        GameManager.OnItemChange += UpdateSetup;
    }

    void UpdateSetup()
    {
        damage = GameManager.instance._gameData.damage;
    }

    private void OnDisable()
    {
        trail.Clear(); //총알 꼬리를 안보이게 하는것.
        tr.position = Vector3.zero;
        tr.rotation = Quaternion.identity;
        rb.Sleep(); //Awake함수나 onEnable에서 힘을 가지지 않으면 움직이지않게 만듬
    }
}
