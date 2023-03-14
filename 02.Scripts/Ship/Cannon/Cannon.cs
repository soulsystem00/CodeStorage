using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using RealTime;
using UnityEngine;
using static RealTime.Common.CoreDefine;

public class Cannon : RealTimeMonoBehaviour
{
    [SerializeField] ShipStatManager stat;
    [SerializeField] SphereCollider col;
    [SerializeField] GameObject muzzle;
    [SerializeField] float power;
    [SerializeField] float maxCoolTime;
    float coolTime;

    int specialCannon = 0;

    private float MaxCoolTime
    {
        get
        {
            float cool = maxCoolTime - stat.AttackSpeed;
            cool = Mathf.Clamp(cool, 0.5f, 5f);
            return cool;
        }
    }

    void Awake()
    {
        coolTime = MaxCoolTime;
        stat.OnRangeChanged += RangeChangedEvent;
    }

    void OnEnable()
    {
        col.radius = stat.AttackRange;
    }

    void Update()
    {
        if (coolTime < MaxCoolTime)
        {
            coolTime += Time.deltaTime;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (realtimeView.IsMine)
        {
            if (!other.CompareTag("Untagged") && !other.CompareTag("CannonBall"))


                if (/*other.tag == "Enemy" || */other.CompareTag("Neutrality") || other.CompareTag("DokdoNeutrality"))
                {
                    DetectObj(other);
                }

                else if (stat.CompareTag("RedBattle") && other.CompareTag("BlueBattle"))
                {
                    if (other.isTrigger)
                        DetectObj(other);
                }
                else if (stat.CompareTag("BlueBattle") && other.CompareTag("RedBattle"))
                {
                    if (other.isTrigger)
                        DetectObj(other);
                }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 5f);
    }

    private void RangeChangedEvent(float _stat)
    {
        col.radius = _stat;
    }

    private void DetectObj(Collider other)
    {
        float lookAngle = transform.rotation.eulerAngles.y;
        Vector3 cannonDirVec = new Vector3(Mathf.Sin(lookAngle * Mathf.Deg2Rad), 0f, Mathf.Cos(lookAngle * Mathf.Deg2Rad)).normalized;
        cannonDirVec *= -1;

        Vector3 objDir = (other.transform.position - this.transform.position);
        Vector3 objDirFlat = new Vector3(objDir.x, 0f, objDir.z).normalized;

        float angle = Vector3.Angle(cannonDirVec, objDirFlat);

        if (angle <= 45f)
        {
            if (coolTime >= MaxCoolTime)
            {
                Fire(other.ClosestPoint(transform.position), objDir.magnitude);
                coolTime = 0f;
            }
        }

        Debug.DrawRay(transform.position, cannonDirVec * 10f, Color.red);
        Debug.DrawRay(transform.position, objDir * 10f, Color.red);
    }

    void Fire(Vector3 _target, float _power)
    {
        int randomValue = UnityEngine.Random.Range(1, 101);
        Vector3 targetPos = _target;
        if (randomValue < 10)
        {
            targetPos += UnityEngine.Random.insideUnitSphere * 2f;
        }

        float damage = stat.Attack;
        if (UnityEngine.Random.Range(1, 101) < stat.Critical)
        {
            damage *= 2;
        }

        if (specialCannon == 2)
        {
            realtimeView.RPC("RPCFire", RpcTarget.All,
                    muzzle.transform.position.x, muzzle.transform.position.y, muzzle.transform.position.z,
                    targetPos.x, targetPos.y, targetPos.z,
                    _power, RealTimeNetwork.SessionId, damage, stat.cannon);
            specialCannon = 0;
        }
        else
        {
            realtimeView.RPC("RPCFire", RpcTarget.All,
                    muzzle.transform.position.x, muzzle.transform.position.y, muzzle.transform.position.z,
                    _target.x, _target.y, _target.z,
                    _power, RealTimeNetwork.SessionId, damage, new int[7]);
            specialCannon++;
        }
    }
}