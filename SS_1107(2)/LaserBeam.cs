using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    private Transform tr;
    private LineRenderer line;
    private RaycastHit hit;
    private float fireRate;
    private float nextFire = 0.0f;

    private void Start()
    {
        tr = GetComponent<Transform>();
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.enabled = false; //처음엔 안보이게 비활성화
        line.startWidth = 0.1f; //레이저빔 두께 시작폭 10cm
        line.endWidth = 0.01f; //레이저빔 두께 끝 폭 1cm
        fireRate = GetComponentInParent<FireCtrl>().fireRate; //GetComponentInParent 상위로 올라가면서 <FireCtrl>를 찾음
    }

    private void Update()
    {
        Ray ray = new Ray(tr.position, tr.forward); //ray는 앞쪽방향 위치
        if (Input.GetMouseButton(0))
        {
            if (Time.time >= nextFire)
            {
                line.SetPosition(0, tr.InverseTransformPoint(ray.origin));
                if (Physics.Raycast(ray, out hit, 100.0f))
                {
                    line.SetPosition(1, tr.InverseTransformPoint(hit.point));
                }
                else line.SetPosition(1, tr.InverseTransformPoint(ray.GetPoint(100.0f))); //ray = 무한히 뻗는 ray에 GetPoint 값을가져온다 100m까지
                StartCoroutine(this.ShowLaserBeam()); //코르틴함수 적용
                nextFire = Time.time + fireRate;
            }
        }
    }
    IEnumerator ShowLaserBeam()
    {
        line.enabled = true;
        yield return new WaitForSeconds(Random.Range(0.01f, 0.2f));
        line.enabled = false;
    }
}