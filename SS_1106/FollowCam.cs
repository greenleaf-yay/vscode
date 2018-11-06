using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform target;
    public float moveDamping = 15.0f;
    public float rotateDamping = 10.0f;
    public float distance = 4.0f;
    public float height = 3.0f;
    public float targetOffset = 2.0f;

    [Header("Wall Object Setting")]
    public float heightAboveWall = 10.0f;
    public float colliderRadius = 1.8f;
    public float overDamping = 5.0f;

    private float originHeight;

    [Header("Etc Obstacle Setting")]
    public float heightAboveObstacle = 12.0f;
    public float castOffset = 1.0f;

    private Transform tr;

    void Start()
    {
        tr = GetComponent<Transform>();
        originHeight = height;
    }

    private void Update()
    {
        if (Physics.CheckSphere(tr.position, colliderRadius))
        {
            height = Mathf.Lerp(height, heightAboveWall
                , Time.deltaTime * overDamping);
        }
        else
        {
            height = Mathf.Lerp(height, originHeight
                , Time.deltaTime * overDamping);
        }

        Vector3 castTarget = target.position + (target.up
            * castOffset);
        Vector3 castDir = (castTarget - tr.position).normalized;
        RaycastHit hit;
        if (Physics.Raycast(tr.position, castDir, out hit
            , Mathf.Infinity))
        {
            if (!hit.collider.CompareTag("Player"))
            {
                height = Mathf.Lerp(height, heightAboveObstacle
                , Time.deltaTime * overDamping);
            }
            else
            {
                height = Mathf.Lerp(height, originHeight
                    , Time.deltaTime * overDamping);
            }
        }
    }

    void LateUpdate()
    {
        var camPos = target.position -
            (target.forward * distance) + (target.up * height);
        tr.position = Vector3.Slerp(tr.position, camPos
            , Time.deltaTime * moveDamping);
        tr.rotation = Quaternion.Slerp(tr.rotation
            , target.rotation, Time.deltaTime * rotateDamping);
        tr.LookAt(target.position + (target.up * targetOffset));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(target.position +
            (target.up * targetOffset), 0.1f);
        Gizmos.DrawLine(target.position +
            (target.up * targetOffset), transform.position);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, colliderRadius);
    }
}