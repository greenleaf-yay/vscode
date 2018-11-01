using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DataInfo;


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
    public GameObject bulletPrefab; //bulletPrefab 연결하겠다. 
    public int maxBulletPool = 10; //총알 10발만 만들어지게하겠다.
    public List<GameObject> bulletPool = new List<GameObject>(); // bulletPool = GameObject를 집어넣을 수 있는 오브젝트
    public CanvasGroup inventoryCG;

    private bool isPause = false;

    //[HideInInspector] public int killCount;
    [Header("GameData")]
    public Text killCountTxt;
    public GameData _gameData;
    public delegate void ItemChangeDelegate();
    public static event ItemChangeDelegate OnItemChange;
    public GameObject[] itemObjects;


    private DataManager dataMgr;
    private GameObject slotList;


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
        dataMgr = GetComponent<DataManager>();
        dataMgr.Initialize();

        slotList = inventoryCG.transform.Find("SlotList").gameObject;

        LoadGameData();
        CreatePool();
    }

    void LoadGameData()
    {
        //killCount = PlayerPrefs.GetInt("KILL_COUNT", 0);
        //killCountTxt.text = "KILL "+ killCount.ToString("0000");
        GameData data = dataMgr.Load();
        _gameData.hp = data.hp;
        _gameData.damage = data.damage;
        _gameData.speed = data.speed;
        _gameData.killCount = data.killCount;
        _gameData.equipItem = data.equipItem;

        if (_gameData.equipItem.Count > 0)
        {
            OnInventorySetup();
        }

        killCountTxt.text = "KILL "
            + _gameData.killCount.ToString("0000"); //killCount.ToString("0000");출력형식 네자리
    }



    void SaveGameData()
    {
        dataMgr.Save(_gameData);
    }

    void OnInventorySetup()
    {
        var slots = slotList.GetComponentsInChildren<Transform>();
        for (int i = 0; i < _gameData.equipItem.Count; i++)
        {
            for (int j = 1; j < slots.Length; j++) //자기자신은 배제 1번부터
            {
                if (slots[j].childCount > 0) continue; //현재슬롯이 아이템이 
                int itemIdx = (int)_gameData.equipItem[i]._itemType;
                itemObjects[itemIdx].GetComponent<ItemInfo>().itemData = _gameData.equipItem[i];
                break;
            }
        }
    }

    public void AddItem(Item _item)
    {
        if (_gameData.equipItem.Contains(_item)) return;//Contains bool type 있는지 없는지
        _gameData.equipItem.Add(_item);
        switch (_item._itemType)
        {
            case Item.ItemType.HP:
                if (_item._itemCalc == Item.ItemCalc.INC_VALUE)
                    _gameData.hp += _item.value;
                else
                    _gameData.hp += _gameData.hp * _item.value;
                break;
            case Item.ItemType.SPEED:
                if (_item._itemCalc == Item.ItemCalc.INC_VALUE)
                    _gameData.speed += _item.value;
                else
                    _gameData.speed += _gameData.speed * _item.value;
                break;
            case Item.ItemType.GRENADE:
                break;
            case Item.ItemType.DAMAGE:

                if (_item._itemCalc == Item.ItemCalc.INC_VALUE)
                    _gameData.damage += _item.value;
                else
                    _gameData.damage += _gameData.damage * _item.value;
                break;
        }
        OnItemChange();
    }

    public void RemoveItem(Item _item)
    {
        _gameData.equipItem.Remove(_item);
        switch (_item._itemType)
        {
            case Item.ItemType.HP:
                if (_item._itemCalc == Item.ItemCalc.INC_VALUE)
                    _gameData.hp -= _item.value;
                else
                    _gameData.hp = _gameData.hp / (1.0f + _item.value);
                break;
            case Item.ItemType.SPEED:
                if (_item._itemCalc == Item.ItemCalc.INC_VALUE)
                    _gameData.speed -= _item.value;
                else
                    _gameData.speed = _gameData.speed / (1.0f + _item.value);
                break;
            case Item.ItemType.GRENADE:
                break;
            case Item.ItemType.DAMAGE:

                if (_item._itemCalc == Item.ItemCalc.INC_VALUE)
                    _gameData.damage -= _item.value;
                else
                    _gameData.damage = _gameData.damage / (1.0f + _item.value);
                break;
        }
    }

    void Start()

    {
        OnInventoryOpen(false);
        points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
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

            int enemyCount = (int)GameObject.FindGameObjectsWithTag("ENEMY").Length;
            if (enemyCount < maxEnemy)
            {
                yield return new WaitForSeconds(createTime);
                int idx = Random.Range(1, points.Length);
                Instantiate(enemy, points[idx].position, points[idx].rotation);
            }
            else yield return null;
        }
    }

    public GameObject GetBullet()
    {
        for (int i = 0; i < bulletPool.Count; i++)
        {
            if (bulletPool[i].activeSelf == false) // 비활성화된 총알을 주는것
            {
                return bulletPool[i];
            }
        }
        return null;
    }

    public void CreatePool()
    {
        GameObject bulletPools = new GameObject("BulletPools");
        // new GameObject("ObjectPools") 해당이름으로 게임오브젝트 생성됨 // 생성자 = 인수를 달리해 여러개를 오버로드 할 수 있다.
        for (int i = 0; i < maxBulletPool; i++)
        {
            var obj = Instantiate<GameObject>(bulletPrefab, bulletPools.transform);
            obj.name = "Bullet_" + i.ToString("00");
            obj.SetActive(false);
            // 해당 오브젝트를 비활성화
            bulletPool.Add(obj);
            // 비활성화 총알 10개를 리스트로 넣음
        }

    }

    public void OnPauseCLick()
    {
        isPause = !isPause; //토글버튼 만들 시 사용법
        Time.timeScale = (isPause) ? 0.0f : 1.0f; //삼항연산자 ? : Time.timeScale = 0일때 정지 ,1일때 시간속도 100%
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        var scripts = playerObj.GetComponents<MonoBehaviour>(); //다형성 = 상위클래스를 만들면 상위클래스의 인스턴스값을 넣을수 있다.
        foreach (var item in scripts)
        {
            item.enabled = !isPause;
        }

        var canvasGroup = GameObject.Find("Panel-Weapon").GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = !isPause;
    }

    public void AddKillCount()
    {
        //player prefs
        //++killCount;
        //killCountTxt.text = "KILL " + killCount.ToString("0000");
        //PlayerPrefs.SetInt("KILL_COUNT", killCount);

        //bf
        ++_gameData.killCount;
        killCountTxt.text = "KILL " + _gameData.killCount.ToString("0000");
    }

    private void OnApplicationQuit() //게임이 종료될 때 호출이됨
    {
        SaveGameData();
    }
}