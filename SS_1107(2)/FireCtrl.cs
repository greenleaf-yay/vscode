using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public struct PlayerSfx
{
    public AudioClip[] fire;
    public AudioClip[] reload;
}

public class FireCtrl : MonoBehaviour
{
    public enum WeaponType
    {
        RIFLE = 0, SHOTGUN, FLAMETHROWER, ROCKET
    }

    public WeaponType currWeapon = WeaponType.RIFLE;
    public GameObject bullet;
    public Transform firePos;
    public float fireRate = 0.1f;
    public ParticleSystem cartridge;
    public PlayerSfx playerSfx;
    public Image magazineImg;
    public Text magazineText;
    public int maxBullet = 10;
    public int remainingBullet = 10;
    //public float reloadTime = 2.0f;

    private ParticleSystem muzzleFlash;
    private float nextFire = 0.0f;
    private AudioSource _audio;
    private Shake _shake;
    private bool isReloading = false;
    private bool isFire = false;
    private int enemyLayer;
    private int obstacleLayer;
    private int layerMask;

    public Sprite[] weaponIcons;
    public Image weaponImage;

    void Start()
    {
        muzzleFlash = firePos.
            GetComponentInChildren<ParticleSystem>();
        _audio = GetComponent<AudioSource>();
        _shake = GameObject.Find("CameraRig")
            .GetComponent<Shake>();
        enemyLayer = LayerMask.NameToLayer("ENEMY");
        obstacleLayer = LayerMask.NameToLayer("OBSTACLE");
        layerMask = 1 << obstacleLayer | 1 << enemyLayer;
    }

    void Update()
    {
        Debug.DrawRay(firePos.position, firePos.forward * 20.0f //Ratcast 녹색광선 20m
            , Color.green);

        if (EventSystem.current.IsPointerOverGameObject()) return;

        #region 1초 10발 처리
        //RaycastHit hit;

        //if (Physics.Raycast(firePos.position, firePos.forward
        //    , out hit, 20.0f, layerMask))
        //{
        //    isFire = (hit.collider.CompareTag("ENEMY"));
        //}
        //else isFire = false;

        //if (!isReloading && isFire)
        //{
        //    if (Time.time >= nextFire)
        //    {
        //        --remainingBullet;
        //        Fire();
        //        if (remainingBullet == 0)
        //        {
        //            StartCoroutine(Reloading());
        //        }
        //        nextFire = Time.time + fireRate; 
        //    }
        //}
        #endregion

        if (Input.GetMouseButton(0))
        {
            if (Time.time >= nextFire)
            {
                FireRay();
                RaycastHit hit;
                if (Physics.Raycast(firePos.position, firePos.forward, out hit, 100.0f))
                {
                    if (hit.collider.CompareTag("ENEMY") || hit.collider.CompareTag("BARREL"))

                    {
                        object[] _params = new object[4];
                        _params[0] = hit.point; //hit.point = Ray에 hit된 위치값
                        _params[1] = hit.normal;
                        _params[2] = GameManager.instance._gameData.damage;
                        _params[3] = firePos.position;
                        hit.collider.gameObject.SendMessage("OnDamage", _params, SendMessageOptions.DontRequireReceiver);
                    }
                }
                nextFire = Time.time + fireRate;
            }
        }
        if (!isReloading && Input.GetMouseButtonDown(1))
        {
            --remainingBullet;
            Fire();
            if (remainingBullet == 0)
            {
                StartCoroutine(Reloading());
            }
        }
    }

    void FireRay()
    {
        StartCoroutine(_shake.ShakeCamera()); //카메라 흔들어주고
        muzzleFlash.Play();//총알 플래쉬
        cartridge.Play();//총알 탄피
        FireSfx();//총 소리
    }

    void Fire()
    {
        StartCoroutine(_shake.ShakeCamera());
        //GameObject _bullet = Instantiate(bullet,
        //    firePos.position, firePos.rotation);
        //Destroy(_bullet, 3.0f);
        var _bullet = GameManager.instance.GetBullet();
        if (_bullet != null)
        {
            _bullet.transform.position = firePos.position;
            _bullet.transform.rotation = firePos.rotation;
            _bullet.SetActive(true);
        }
        muzzleFlash.Play();
        cartridge.Play();
        FireSfx();
        magazineImg.fillAmount = (float)remainingBullet /
            (float)maxBullet;
        UpdateBulletText();
    }

    void FireSfx()
    {
        var _sfx = playerSfx.fire[(int)currWeapon];
        _audio.PlayOneShot(_sfx);
    }

    IEnumerator Reloading()
    {
        isReloading = true;
        _audio.PlayOneShot(playerSfx.reload[(int)currWeapon]
            , 1.0f);
        yield return new WaitForSeconds(playerSfx
            .reload[(int)currWeapon].length + 0.3f);
        isReloading = false;
        magazineImg.fillAmount = 1.0f;
        remainingBullet = maxBullet;
        UpdateBulletText();
    }

    void UpdateBulletText()
    {
        magazineText.text = string.Format("<color=#ff0000>" +
            "{0}</color>/{1}", remainingBullet, maxBullet);
    }

    public void OnChangeWeapon()
    {
        currWeapon = (WeaponType)((int)++currWeapon % 2);
        weaponImage.sprite = weaponIcons[(int)currWeapon];
    }
}