using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using DG.Tweening;
using Google.Protobuf.GameProtocol;
using RealTime;
using UnityEngine;
using static RealTime.Common.CoreDefine;

public class DokdoCannon : RealTimeMonoBehaviour
{
    [SerializeField] SphereCollider col;
    [SerializeField] GameObject muzzle;
    [SerializeField] float power;
    [SerializeField] float maxCoolTime;
    [SerializeField] int index;
    float coolTime;
    bool isAttacking = false;
    WaitForSeconds waitForSeconds = new WaitForSeconds(5f);

    private float MaxCoolTime
    {
        get
        {
            float cool = maxCoolTime;
            cool = Mathf.Clamp(cool, 0.5f, 5f);
            return cool;
        }
    }

    void Awake()
    {
        coolTime = MaxCoolTime;
        col.radius = 2f;
    }

    void Update()
    {
        if (coolTime < MaxCoolTime)
        {
            coolTime += Time.deltaTime;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 10f);
    }

    void OnTriggerStay(Collider other)
    {
        if (!isAttacking && RealTimeNetwork.IsMasterClient && transform.parent.tag == "DokdoNeutrality")
        {
            if (!other.CompareTag("Untagged") && !other.CompareTag("CannonBall"))
                if (this.transform.parent.CompareTag("DokdoNeutrality"))
                {
                    if (other.CompareTag("Red") || other.CompareTag("Blue"))
                    {
                        isAttacking = true;
                        StartCoroutine(AttackFlow(other.gameObject));
                    }
                }
                else if (this.transform.parent.CompareTag("DokdoFriendly") || this.transform.parent.CompareTag("DokdoMine"))
                {
                    if (!other.CompareTag(ShipSpawner.i.GetTeam(RealTimeNetwork.SessionId)))
                    {
                        isAttacking = true;
                        StartCoroutine(AttackFlow(other.gameObject));
                    }
                }
                else if (this.transform.parent.CompareTag("DokdoEnemy"))
                {
                    if (other.CompareTag(ShipSpawner.i.GetTeam(RealTimeNetwork.SessionId)))
                    {
                        isAttacking = true;
                        StartCoroutine(AttackFlow(other.gameObject));
                    }
                }
        }
    }

    private IEnumerator AttackFlow(GameObject gameObject)
    {
        float lookAngle = transform.rotation.eulerAngles.y;
        Vector3 cannonDirVec = new Vector3(Mathf.Sin(lookAngle * Mathf.Deg2Rad), 0f, Mathf.Cos(lookAngle * Mathf.Deg2Rad)).normalized;
        cannonDirVec *= -1;

        Vector3 objDir = (gameObject.transform.position - this.transform.position);
        Vector3 objDirFlat = new Vector3(objDir.x, 0f, objDir.z).normalized;
        var deg = Mathf.Atan2(objDir.x, objDir.z) * Mathf.Rad2Deg;
        C_G_Island_Attack packet = new C_G_Island_Attack();
        packet.IsDokdo = true;
        packet.Degree = deg;
        packet.Index = index;
        packet.Position = new Google.Protobuf.ProtoObject.Position();
        packet.Position.X = gameObject.transform.position.x;
        packet.Position.Y = gameObject.transform.position.y;
        packet.Position.Z = gameObject.transform.position.z;
        RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGIslandAttack, packet);

        yield return waitForSeconds;
        isAttacking = false;
    }

    public void Shot(float _degree, Vector3 _pos)
    {
        transform.DORotate(new Vector3(0f, _degree + 180f, 0f), 0.5f).SetEase(Ease.Linear).OnComplete(() => Fire(_pos));
    }

    void Fire(Vector3 _target)
    {
        var go = ObjectPoolManager.Instance.Instantiate(ResourceDataManager.islandCannonBall, muzzle.transform.position, Quaternion.identity);
        go.gameObject.transform.DOLocalJump(_target, 2, 1, 1f).SetEase(Ease.Linear).OnComplete(() => { go.GetComponent<IslandCannonBall>().GetMeshRenderer().enabled = false; });
    }
}