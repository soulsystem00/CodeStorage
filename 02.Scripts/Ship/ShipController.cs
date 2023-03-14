using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RealTime;
using UnityEngine;
using static RealTime.Common.CoreDefine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(RealTimeView))]
public class ShipController : RealTimeMonoBehaviour
{
    [SerializeField] BoxCollider col;
    [SerializeField] ShipStatManager stat;
    [SerializeField] Rigidbody rb;
    [SerializeField] ParticleSystem ps;
    [SerializeField] SpriteRenderer sr;

    ParticleSystem.EmissionModule em;
    ParticleSystem.MainModule mm;

    static float syncTime = 0.2f;
    Vector3 dirVec;
    Vector3 startPos;
    Vector3 endPos;
    Vector3 prevPos;

    WaitForSeconds waitForSeconds = new WaitForSeconds(syncTime);
    PlaySceneManager playSceneManager;

    void Start()
    {
        em = ps.emission;
        mm = ps.main;
        if (realtimeView.IsMine)
        {
            Init();
            sr.color = Color.green;
        }
        else
        {
            if (CompareTag("Red") || CompareTag("RedBattle"))
                sr.color = Color.red;
            else if (CompareTag("Blue") || CompareTag("BlueBattle"))
                sr.color = Color.blue;
        }
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (RealTimeNetwork.IsMasterClient)
        {
            DetectCrash(collisionInfo);
        }
    }

    void HandleUpdate()
    {
        if (realtimeView.IsMine)
        {
            em.rateOverTime = rb.velocity.magnitude * 3f;
            if (Input.GetMouseButtonDown(0))
            {
                startPos = Input.mousePosition;
            }
            if (Input.GetMouseButton(0))
            {
                endPos = Input.mousePosition;
                var prevdir = dirVec;
                dirVec = (endPos - startPos).normalized;
                dirVec.z = dirVec.y;
                dirVec.y = 0;
                var tmp = Mathf.Atan2(dirVec.x, dirVec.z) * Mathf.Rad2Deg;
                var endAngle = new Vector3(0f, tmp, 0f);
                var endqua = Quaternion.Euler(0f, tmp, 0f);
                rb.AddForce(transform.forward * stat.MoveSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, endqua, Time.deltaTime * 0.2f);
            }
        }
    }

    private void Init()
    {
        playSceneManager = FindObjectOfType<PlaySceneManager>();
        playSceneManager.mainCamera.playerObj = this.gameObject;
        playSceneManager.inGameMiniMapCamera.playerObj = this.gameObject;
        playSceneManager.miniMapCam.playerObj = this.gameObject;
        playSceneManager.ShipHandleUpdate = HandleUpdate;
        playSceneManager.buffManager.Init(this.gameObject);

        if (transform.position.x > 500)
        {
            playSceneManager.miniMapCam.moveOffSet = 1000f;
        }
        else
        {
            playSceneManager.miniMapCam.moveOffSet = 0f;
        }
        StartCoroutine(SyncTransform());
    }

    private IEnumerator SyncTransform()
    {
        float[] array = new float[7];
        while (true)
        {
            if (RealTimeNetwork.IsInRoom)
            {
                array[0] = transform.position.x;
                array[1] = transform.position.y;
                array[2] = transform.position.z;
                array[3] = transform.rotation.x;
                array[4] = transform.rotation.y;
                array[5] = transform.rotation.z;
                array[6] = transform.rotation.w;
                realtimeView.RPC("RPCSyncTransform", RpcTarget.Others, array);
            }


            yield return waitForSeconds;
        }
    }

    private void DetectCrash(Collision collisionInfo)
    {
        if (collisionInfo.collider.CompareTag("BoarderLine") || collisionInfo.collider.CompareTag("Neutrality") || collisionInfo.collider.CompareTag("Enemy") || collisionInfo.collider.CompareTag("Mine") || collisionInfo.collider.CompareTag("Friendly") || collisionInfo.collider.CompareTag("DokdoNeutrality") || collisionInfo.collider.CompareTag("DokdoEnemy") || collisionInfo.collider.CompareTag("DokdoMine") || collisionInfo.collider.CompareTag("DokdoFriendly"))
        {
            Crash(collisionInfo.contacts[0].point, collisionInfo.contacts[0].normal);
        }
        else if (collisionInfo.collider.CompareTag("Red") || collisionInfo.collider.CompareTag("Blue"))
        {
            if (!collisionInfo.collider.isTrigger)
            {
                Crash(collisionInfo.contacts[0].point, collisionInfo.contacts[0].normal);
            }
        }
        else if (collisionInfo.collider.CompareTag("RedBattle") || collisionInfo.collider.CompareTag("BlueBattle"))
        {
            if (!collisionInfo.collider.isTrigger)
            {
                Crash(collisionInfo.contacts[0].point, collisionInfo.contacts[0].normal);
            }
        }
    }

    private void Crash(Vector3 _collisionInfo, Vector3 _normal)
    {
        Vector3 dirVec = (_collisionInfo - transform.position).normalized;
        Vector3 reflectVec = Vector3.Reflect(dirVec, _normal);
        realtimeView.RPC("RPCCrash", RpcTarget.All, reflectVec.x, reflectVec.y, reflectVec.z);
    }

    [RealTimeRPC]
    public void RPCSyncTransform(float[] _transform)
    {
        Vector3 position = new Vector3(_transform[0], _transform[1], _transform[2]);
        Quaternion rotation = new Quaternion(_transform[3], _transform[4], _transform[5], _transform[6]);
        transform.DOMove(position, syncTime).SetEase(Ease.Linear);
        transform.DORotate(rotation.eulerAngles, syncTime).SetEase(Ease.Linear);
    }
}