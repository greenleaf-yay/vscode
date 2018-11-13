using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFire : MonoBehaviour
{
    private AudioSource _audio;
    private Animator _animator;
    private Transform playerTr;
    private Transform enemyTr;
    private float nextFire = 0.0f;
    private readonly float fireRate = 0.1f;
    private readonly float damping = 10.0f;
    private readonly float reloadTime = 2.0f;
    private readonly int maxBullet = 10;
    private int currBullet = 10;
    private bool isReload = false;
    private WaitForSeconds wsReload;

    private readonly int hashFire = Animator.
        StringToHash("Fire");
    private readonly int hashReload = Animator.
        StringToHash("Reload");

    public bool isFire = false;
    public AudioClip fireSfx;
    public AudioClip reloadSfx;
    public GameObject bullet;
    public Transform firePos;
    public MeshRenderer muzzleFlash;
    public LineRenderer line;

    void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<Transform>();
        enemyTr = GetComponent<Transform>();
        _animator = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();
        wsReload = new WaitForSeconds(reloadTime);
        muzzleFlash.enabled = false;

        line.enabled = false;
        line.startWidth = 0.1f;
        line.endWidth = 0.05f;
    }

    void Update()
    {
        if (!isReload && isFire)
        {
            if (Time.time >= nextFire)
            {
                Fire();
                nextFire = Time.time + fireRate
                    + Random.Range(0.0f, 0.3f);
            }
            Quaternion rot = Quaternion.LookRotation
                (playerTr.position - enemyTr.position);
            enemyTr.rotation = Quaternion.Slerp
                (enemyTr.rotation, rot, Time.deltaTime * damping);
        }
    }

    void Fire()
    {
        _animator.SetTrigger(hashFire);
        _audio.PlayOneShot(fireSfx, 1.0f);
        StartCoroutine(ShowMuzzleFlash());

        //GameObject _bullet = Instantiate(bullet,
        //        firePos.position, firePos.rotation);
        //    Destroy(_bullet, 3.0f);

        line.SetPosition(0, firePos.position);
        Ray ray = new Ray(firePos.position, firePos.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            object[] _params = new object[4];
            _params[0] = hit.point;
            _params[1] = hit.normal;
            _params[2] = GameManager.instance
                ._gameData.damage;
            _params[3] = firePos.position;

            //if (hit.collider.CompareTag("Player"))
            //{
            //    hit.collider.gameObject.SendMessage("OnDamage"
            //        , SendMessageOptions.DontRequireReceiver);
            //}

            hit.collider.gameObject.SendMessage("OnDamage", _params
                    , SendMessageOptions.DontRequireReceiver);
            line.SetPosition(1, hit.point);
        }
        else line.SetPosition(1, ray.GetPoint(100.0f));
        StartCoroutine(this.ShowLaserBeam());

        isReload = (--currBullet % maxBullet == 0);
        if (isReload)
        {
            StartCoroutine(Reloading());
        }
    }

    IEnumerator ShowLaserBeam()
    {
        line.enabled = true;
        yield return new WaitForSeconds(Random.Range(0.01f, 0.2f));
        line.enabled = false;
    }

    IEnumerator ShowMuzzleFlash()
    {
        muzzleFlash.enabled = true;
        Quaternion rot = Quaternion.Euler(Vector3.forward
            * Random.Range(0, 360));
        muzzleFlash.transform.localRotation = rot;
        muzzleFlash.transform.localScale = Vector3.one
            * Random.Range(1.0f, 2.0f);
        Vector2 offset = new Vector2(Random.Range(0, 2),
            Random.Range(0, 2)) * 0.5f;
        muzzleFlash.material.SetTextureOffset("_MainTex", offset);
        yield return new WaitForSeconds
            (Random.Range(0.01f, 0.05f));
        muzzleFlash.enabled = false;
    }

    IEnumerator Reloading()
    {
        muzzleFlash.enabled = false;
        _animator.SetTrigger(hashReload);
        _audio.PlayOneShot(reloadSfx, 1.0f);
        yield return wsReload;
        currBullet = maxBullet;
        isReload = false;
    }
}
