using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using Google.Protobuf.GameProtocol;
using RealTime;
using UnityEngine;

public class IslandCannonBall : MonoBehaviour
{
    [SerializeField] MeshRenderer mr;
    [SerializeField] Rigidbody rb;
    [SerializeField] SphereCollider col;
    [SerializeField] ParticleSystem ps;
    WaitForSeconds waitForSeconds = new WaitForSeconds(3f);
    Vector3 dirVec;
    Coroutine CoDestory;
    public float damage;

    void OnEnable()
    {
        CoDestory = StartCoroutine(DestroyAfterSec());
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Red") || other.CompareTag("Blue"))
        {
            mr.enabled = false;
            rb.velocity = Vector3.zero;
            ps.gameObject.SetActive(true);
        }
        else if (other.CompareTag("Water"))
        {
            mr.enabled = false;
            rb.velocity = Vector3.zero;
            col.enabled = false;
        }
    }

    IEnumerator DestroyAfterSec()
    {
        rb.velocity = Vector3.zero;
        mr.enabled = true;
        col.enabled = true;
        ps.gameObject.SetActive(false);
        yield return waitForSeconds;
        rb.velocity = Vector3.zero;
        mr.enabled = true;
        col.enabled = true;
        ps.gameObject.SetActive(false);
        ObjectPoolManager.Instance.Destroy(this.gameObject);
    }

    public MeshRenderer GetMeshRenderer()
    {
        return mr;
    }
}
