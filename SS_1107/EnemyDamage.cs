using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDamage : MonoBehaviour
{
    private const string bulletTag = "BULLET";

    public float hp = 100.0f;

    private float initHp = 100.0f;
    private GameObject bloodEffect;
    private Canvas uiCanvas;
    private Image hpBarImage;

    public GameObject hpBarPrefab;
    public Vector3 hpBarOffset = new Vector3(0.0f, 2.2f, 0.0f);

    private GameObject hpBar;

    void Start()
    {
        bloodEffect = Resources.Load<GameObject>
            ("BulletImpactFleshBigEffect");

        //hpBar = Instantiate<GameObject>
        //    (hpBarPrefab, uiCanvas.transform);

        SetHpBar();
    }

    void SetHpBar()
    {
        uiCanvas = GameObject.Find("UI Canvas")
            .GetComponent<Canvas>();

        hpBar = Instantiate<GameObject>
            (hpBarPrefab, uiCanvas.transform);

        //GameObject hpBar = Instantiate<GameObject>
        //    (hpBarPrefab, uiCanvas.transform);

        //hpBarImage = hpBar.GetComponentsInChildren<Image>()[1];
        //var _hpBar = hpBar.GetComponent<EnemyHpBar>();

        //_hpBar.targetTr = this.gameObject.transform;
        //_hpBar.offset = hpBarOffset;

        ShowHpBar();
    }

    public void ShowHpBar()
    {
        hpBarImage = hpBar.GetComponentsInChildren<Image>()[1];
        hpBarImage.GetComponentsInParent<Image>()[1]
                    .color = Color.black;
        hpBarImage.fillAmount = hp / initHp;
        var _hpBar = hpBar.GetComponent<EnemyHpBar>();

        _hpBar.targetTr = this.gameObject.transform;
        _hpBar.offset = hpBarOffset;
    }

    void OnDamage(object[] _params)
    {
        ShowBloodEffect(_params);
        hp -= (float)_params[2];

        hpBarImage.fillAmount = hp / initHp;
        if (hp <= 0.0f)
        {
            GetComponent<EnemyAI>()._state
                = EnemyAI.State.DIE;
            hpBarImage.GetComponentsInParent<Image>()[1]
                .color = Color.clear;
            GameManager.instance.AddKillCount();
            GetComponent<CapsuleCollider>().enabled = false;
        }
    }

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.tag == bulletTag)
        {
            object[] _params = new object[2];
            _params[0] = coll.contacts[0].point;
            _params[1] = coll.contacts[0].normal;
            ShowBloodEffect(_params);
            //Destroy(coll.gameObject);
            coll.gameObject.SetActive(false);
            hp -= coll.gameObject.GetComponent<BulletCtrl>().
                damage;
            hpBarImage.fillAmount = hp / initHp;
            if (hp <= 0.0f)
            {
                GetComponent<EnemyAI>()._state
                    = EnemyAI.State.DIE;
                hpBarImage.GetComponentsInParent<Image>()[1]
                    .color = Color.clear;
                GameManager.instance.AddKillCount();
                GetComponent<CapsuleCollider>().enabled = false;
            }
        }
    }

    void ShowBloodEffect(object[] _params)
    {
        Vector3 pos = (Vector3)_params[0];
        Vector3 _normal = (Vector3)_params[1];
        Quaternion rot = Quaternion.FromToRotation
            (-Vector3.forward, _normal);
        GameObject blood = Instantiate<GameObject>
            (bloodEffect, pos, rot);
        Destroy(blood, 1.0f);
    }
}