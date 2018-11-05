using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    public GameObject sparkEffect;

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.tag == "BULLET")
        {
            ShowEffect(coll);

            //Destroy(coll.gameObject);
            coll.gameObject.SetActive(false);
        }
    }

    void ShowEffect(Collision coll)
    {
        ContactPoint contact = coll.contacts[0];
        Quaternion rot = Quaternion.
            FromToRotation(-Vector3.forward, contact.normal);
        GameObject spark = Instantiate(sparkEffect,
            contact.point, rot);
        spark.transform.SetParent(this.transform);
    }
}
