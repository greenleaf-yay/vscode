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

    public Sprite[] weaponIcons;
    public Image weaponImage;

    void Start()
    {
        muzzleFlash = firePos.
            GetComponentInChildren<ParticleSystem>();
        _audio = GetComponent<AudioSource>();
        _shake = GameObject.Find("CameraRig")
            .GetComponent<Shake>();
    }

    void Update()
    {

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        //if (Input.GetMouseButton(0))
        //{
        //    if (Time.time >= nextFire)
        //    {
        //        Fire();
        //        nextFire = Time.time + fireRate;
        //    }
        //}
        if (!isReloading && Input.GetMouseButtonDown(0))
        {
            --remainingBullet;
            Fire();
            if (remainingBullet == 0)
            {
                StartCoroutine(Reloading());
            }
        }
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
