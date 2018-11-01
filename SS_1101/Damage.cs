using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI를 활용하기 위한 네임스페이스

public class Damage : MonoBehaviour
{
    private const string bulletTag = "BULLET";
    private const string enemyTag = "ENEMY";
    private float initHp = 100.0f;
    private readonly Color initColor =
        new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
    private Color currColor;

    public float currHp;
    public Image bloodScreen;
    public Image hpBar;

    public delegate void PlayerDieHandler(); //델리게이트 선언
    public static event PlayerDieHandler OnPlayerDie; //이벤트 선언

    private void OnEnable()
    {
        GameManager.OnItemChange += UpdateSetup;
    }

    void UpdateSetup()
    {
        initHp = GameManager.instance._gameData.hp;
        currHp += GameManager.instance._gameData.hp - currHp;
    }

    void Start()
    {
        initHp = GameManager.instance._gameData.hp;
        currHp = initHp;
        hpBar.color = initColor;
        currColor = initColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == bulletTag)
        {
            Destroy(other.gameObject);
            StartCoroutine(ShowBloodScreen());
            currHp -= 5.0f;
            //Debug.Log("Player HP = " + currHp.ToString());
            DisplayHpbar();
            if (currHp <= 0.0f)
            {
                PlayerDie();
            }
        }
    }

    void DisplayHpbar()
    {
        if ((currHp / initHp) > 0.5f)
        {
            currColor.r = (1 - (currHp / initHp)) * 2.0f;
        }
        else currColor.g = (currHp / initHp) * 2.0f;
        hpBar.color = currColor;
        hpBar.fillAmount = (currHp / initHp);
    }

    IEnumerator ShowBloodScreen()
    {
        bloodScreen.color = new Color(1, 0, 0
            , Random.Range(0.2f, 0.3f));
        yield return new WaitForSeconds(0.1f);
        bloodScreen.color = Color.clear;
    }

    void PlayerDie()
    {
        OnPlayerDie(); // 이벤트 호출
        GameManager.instance.isGameOver = true; //싱글턴 사용
        GameManager.instance.StopAllCoroutines();
        //Debug.Log("Player Die!!!");
        //GameObject[] enemies = GameObject.
        //    FindGameObjectsWithTag(enemyTag);
        //for (int i = 0; i < enemies.Length; i++)
        //{
        //    enemies[i].SendMessage("OnPlayerDie"
        //        , SendMessageOptions.DontRequireReceiver);
        //}
    }

    void Update()
    {

    }
}
