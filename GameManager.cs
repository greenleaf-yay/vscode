using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Enemy Create Info")]
    public Transform[] points;
    public GameObject enemy;
    public float createTime = 2.0f;
    public int maxEnemy = 10;
    public bool isGameOver = false;

    //싱글턴을 위한 객체 선언
    public static GameManager instance = null;

    [Header("Object Pool")]
    public GameObject bulletPrefab;
    public int maxBulletPool = 10;
    public List<GameObject> bulletPool = new List<GameObject>();

    private bool isPause;

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
        CreatePool();
    }

    void Start()
    {
        points = GameObject.Find("SpawnPointGroup")
            .GetComponentsInChildren<Transform>();
        if (points.Length > 0)
        {
            StartCoroutine(this.CreateEnemy());
        }
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
                int idx = Random.Range(1, points.Length);
                Instantiate(enemy, points[idx].position,
                    points[idx].rotation);
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

    public void CreatePool()
    {
        GameObject objectPools = new GameObject("ObjectPools");
        for (int i = 0; i < maxBulletPool; i++)
        {
            var obj = Instantiate<GameObject>(bulletPrefab
                , objectPools.transform);
            obj.name = "Bullet_" + i.ToString("00");
            obj.SetActive(false);
            bulletPool.Add(obj);
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

}
