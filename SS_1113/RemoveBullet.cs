using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    public GameObject sparkEffect;

    void OnDamage(object[] _params)
    {
        ShowEffect(_params);
    }

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.tag == "BULLET")
        {
            object[] _params = new object[2];
            _params[0] = coll.contacts[0].point;
            _params[1] = coll.contacts[0].normal;
            ShowEffect(_params);

            //Destroy(coll.gameObject);
            coll.gameObject.SetActive(false);
        }
    }

    //void ShowEffect(Collision coll)
    //{
    //    ContactPoint contact = coll.contacts[0];
    //    Quaternion rot = Quaternion.
    //        FromToRotation(-Vector3.forward, contact.normal);
    //    GameObject spark = Instantiate(sparkEffect,
    //        contact.point, rot);
    //    spark.transform.SetParent(this.transform);
    //}

    void ShowEffect(object[] _params)
    {
        Vector3 pos = (Vector3)_params[0];
        Vector3 _normal = (Vector3)_params[1];
        Quaternion rot = Quaternion.
            FromToRotation(Vector3.forward, _normal);
        GameObject spark = Instantiate(sparkEffect, pos, rot);
        spark.transform.SetParent(this.transform);
    }
}
