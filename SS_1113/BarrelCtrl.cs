using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    public GameObject expEffect;
    public GameObject fireEffect;
    public GameObject smokeEffect;
    public Mesh[] meshes;
    public Texture[] textures;
    public float expRadius = 10.0f;
    public AudioClip expSfx;

    private int hitCount = 0;
    private Rigidbody rb;
    private MeshFilter _meshFilter;
    private MeshRenderer _renderer;
    private bool isFire = false;
    private AudioSource _audio;
    private Shake _shake;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _meshFilter = GetComponent<MeshFilter>();
        _renderer = GetComponent<MeshRenderer>();
        _renderer.material.mainTexture
            = textures[Random.Range(0, textures.Length - 1)];
        fireEffect.SetActive(false);
        smokeEffect.SetActive(false);
        _audio = GetComponent<AudioSource>();
        //_shake = GameObject.Find("CameraRig").GetComponent<Shake>();
        StartCoroutine(GetShake());
    }

    IEnumerator GetShake()
    {
        while (!UnityEngine.SceneManagement.SceneManager.GetSceneByName("scPlay").isLoaded)
        {
            yield return null;
        }
        _shake = GameObject.Find("CameraRig").GetComponent<Shake>();
    }

    void OnDamage(object[] _params)
    {
        Vector3 hitPos = (Vector3)_params[0];
        Vector3 firePos = (Vector3)_params[3];
        Vector3 incomeVector = (hitPos - firePos).normalized;
        rb.AddForceAtPosition(incomeVector * 1000.0f, hitPos);
        if (++hitCount == 3)
        {
            ExpBarrel();
        }
    }

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("BULLET"))
        {
            if (++hitCount == 3)
            {
                ExpBarrel();
            }
        }
    }

    void ExpBarrel()
    {
        _audio.PlayOneShot(expSfx);
        GameObject effect = Instantiate(expEffect,
            transform.position, Quaternion.identity);
        _renderer.material.mainTexture
            = textures[textures.Length - 1];
        OnFire();
        IndirectDamage(transform.position);
        Destroy(effect, 3.8f);
        int idx = Random.Range(0, meshes.Length);
        _meshFilter.sharedMesh = meshes[idx];
        //Destroy(this.gameObject, 10.0f);
        StartCoroutine(_shake.ShakeCamera(0.1f, 0.2f, 0.5f));
    }

    void IndirectDamage(Vector3 pos)
    {
        Collider[] colls = Physics.OverlapSphere(pos, expRadius,
            3 << 8);
        foreach (var item in colls)
        {
            if (item.gameObject.tag != "Player")
            {
                var _rb = item.GetComponent<Rigidbody>();
                if (item.GetComponent<BarrelCtrl>().isFire == false)
                {
                    item.GetComponent<BarrelCtrl>().isFire = true;
                    StartCoroutine(item.GetComponent<BarrelCtrl>()
                    .DelayExp());
                }
                _rb.mass = 1.0f;
                _rb.AddExplosionForce(1200.0f, pos, expRadius
                    , 1000.0f);
            }
        }
    }

    IEnumerator DelayExp()
    {
        this.gameObject.GetComponent<BarrelCtrl>().OnFire();
        yield return new WaitForSeconds(Random.Range(5.0f, 10.0f));
        this.ExpBarrel();
    }

    void OnFire()
    {
        fireEffect.SetActive(true);
        smokeEffect.SetActive(true);
    }
}
