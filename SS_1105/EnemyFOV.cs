using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour {

    public float viewRange = 15.0f;
    [Range(0, 360)]
    public float viewAngle = 120.0f;

    private Transform enemyTr;
    private Transform playerTr;
    private int playerLayer;
    private int obstacleLayer;
    private int layerMask;

	// Use this for initialization
	void Start ()
    {
        enemyTr = GetComponent<Transform>();
        playerTr = GameObject.FindGameObjectWithTag("Player")
            .transform;
        playerLayer = LayerMask.NameToLayer("PLAYER");
        obstacleLayer = LayerMask.NameToLayer("OBSTACLE");
        layerMask = 1 << playerLayer | 1 << obstacleLayer;
	}
	
	public Vector3 CirclePoint(float angle)
    {
        angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad)
            , 0.0f, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    public bool IsTracePlayer()
    {
        bool isTrace = false;
        Collider[] colls = Physics.OverlapSphere(enemyTr.position
            , viewRange, 1 << layerMask);
        if (colls.Length == 1)
        {
            Vector3 dir = (playerTr.position - enemyTr.position)
                .normalized;
            if (Vector3.Angle(enemyTr.forward, dir) < viewAngle * 0.5f)
            {
                isTrace = true;
            }
        }
        return isTrace;
    }

    public bool IsViewPlayer()
    {
        bool isView = false;
        RaycastHit hit;
        Vector3 dir = (playerTr.position - enemyTr.position)
            .normalized;
        if (Physics.Raycast(enemyTr.position, dir, out hit
            , viewRange, layerMask))
        {
            isView = (hit.collider.CompareTag("Player"));
        }
        return isView;
    }
}
