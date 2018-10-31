using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Enemy Create Info")]
    public Transform[] points;
    public GameObject enemy;
    public float createTime = 2.0f;
    public int maxEnemy = 10;
    public bool isGameOver = false;

    //public bool show = false;

    //싱글턴을 위한 객체 선언
    public static GameManager instance = null;

    [Header("Object Pool")]
    public GameObject bulletPrefab;
    public int maxBulletPool = 10;
    public List<GameObject> bulletPool = new List<GameObject>();
    public GameObject enemyPrefab;
    public int maxEnemyPool = 10;
    public List<GameObject> enemyPool = new List<GameObject>();
    public CanvasGroup inventoryCG;

    [HideInInspector] public int killCount;
    public Text killCountTxt;

    private bool isPause = false;    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject); //다른 씬에서도 삭제안됨
        LoadGameData();
        CreatePool();
    }

    void LoadGameData()
    {
        killCount = PlayerPrefs.GetInt("KILL_COUNT", 0);
        killCountTxt.text = "KILL " + killCount.ToString("0000");
    }

    void Start()
    {
        OnInventoryOpen(false);
        points = GameObject.Find("SpawnPointGroup")
            .GetComponentsInChildren<Transform>();
        if (points.Length > 0)
        {
            StartCoroutine(this.CreateEnemy());
        }
    }

    public void OnInventoryOpen(bool isOpen)
    {
        inventoryCG.alpha = (isOpen) ? 1.0f : 0.0f;
        inventoryCG.interactable = isOpen;
        inventoryCG.blocksRaycasts = isOpen;
    }

    IEnumerator CreateEnemy()
    {
        while (!isGameOver)
        {
            int enemyCount = (int)GameObject
                .FindGameObjectsWithTag("ENEMY").Length;
            if (enemyCount < maxEnemy)
            {
                yield return new WaitForSeconds(createTime);
                //int idx = Random.Range(1, points.Length);
                //Instantiate(enemy, points[idx].position,
                //    points[idx].rotation);
                var _enemy = GetEnemy();
                if (_enemy != null)
                {
                    int idx = Random.Range(1, points.Length);
                    _enemy.transform.position = points[idx].position;
                    _enemy.transform.rotation = points[idx].rotation;
                    _enemy.SetActive(true);

                    //_enemy.SendMessage("ShowHpBar"
                    //    , SendMessageOptions.DontRequireReceiver);

                    //show = true;
                }
            }
            else yield return null;            
        }
    }

    public GameObject GetBullet()
    {
        for (int i = 0; i < bulletPool.Count; i++)
        {
            if (bulletPool[i].activeSelf == false)
            {
                return bulletPool[i];
            }
        }
        return null;
    }

    public GameObject GetEnemy()
    {
        for (int i = 0; i < enemyPool.Count; i++)
        {
            if (enemyPool[i].activeSelf == false)
            {
                return enemyPool[i];
            }
        }
        return null;
    }

    public void CreatePool()
    {
        GameObject bulletPools = new GameObject("BulletPools");
        for (int i = 0; i < maxBulletPool; i++)
        {
            var obj = Instantiate<GameObject>(bulletPrefab
                , bulletPools.transform);
            obj.name = "Bullet_" + i.ToString("00");
            obj.SetActive(false);
            bulletPool.Add(obj);
        }
        GameObject enemyPools = new GameObject("EnemyPools");
        for (int i = 0; i < maxEnemyPool; i++)
        {
            var obj = Instantiate<GameObject>(enemyPrefab
                , enemyPools.transform);
            obj.name = "Enemy_" + i.ToString("00");
            obj.SetActive(false);
            enemyPool.Add(obj);
        }
    }

    public void OnPauseClick()
    {
        isPause = !isPause;
        Time.timeScale = (isPause) ? 0.0f : 1.0f;
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        var scripts = playerObj.GetComponents<MonoBehaviour>();
        foreach (var item in scripts)
        {
            item.enabled = !isPause;
        }

        var canvasGroup = GameObject.Find("Panel-Weapon")
            .GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = !isPause;
    }
    
    public void AddKillCount()
    {
        ++killCount;
        killCountTxt.text = "KILL " + killCount.ToString("0000");
        PlayerPrefs.SetInt("KILL_COUNT", killCount);
    }
}