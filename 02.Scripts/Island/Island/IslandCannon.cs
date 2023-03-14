using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DG.Tweening;
using Google.Protobuf.GameProtocol;
using RealTime;
using UnityEngine;
using static RealTime.Common.CoreDefine;

public class IslandCannon : RealTimeMonoBehaviour
{
    [SerializeField] SphereCollider col;
    [SerializeField] GameObject muzzle;
    [SerializeField] float power;
    [SerializeField] float maxCoolTime;

    public int key;
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
        Gizmos.DrawWireSphere(transform.position, 5.5f);
    }

    void OnTriggerStay(Collider other)
    {
        if (!isAttacking && RealTimeNetwork.IsMasterClient && transform.parent.CompareTag("Neutrality"))
        {
            if (!other.CompareTag("Untagged") && !other.CompareTag("CannonBall") && !other.CompareTag("RedCannon") && !other.CompareTag("BlueCannon"))
                if (this.transform.parent.tag == "Neutrality")
                {
                    if (other.CompareTag("Red") || other.CompareTag("Blue"))
                    {
                        isAttacking = true;
                        StartCoroutine(AttackFlow(other));
                    }
                }
                else if (this.transform.parent.CompareTag("Friendly") || this.transform.parent.CompareTag("Mine"))
                {
                    if (!other.CompareTag(ShipSpawner.i.GetTeam(RealTimeNetwork.SessionId)))
                    {
                        isAttacking = true;
                        StartCoroutine(AttackFlow(other));
                    }
                }
                else if (this.transform.parent.CompareTag("Enemy"))
                {
                    if (other.CompareTag(ShipSpawner.i.GetTeam(RealTimeNetwork.SessionId)))
                    {
                        isAttacking = true;
                        StartCoroutine(AttackFlow(other));
                    }
                }
        }
    }

    private IEnumerator AttackFlow(Collider collider)
    {
        float lookAngle = transform.rotation.eulerAngles.y;
        Vector3 cannonDirVec = new Vector3(Mathf.Sin(lookAngle * Mathf.Deg2Rad), 0f, Mathf.Cos(lookAngle * Mathf.Deg2Rad)).normalized;
        cannonDirVec *= -1;

        Vector3 objDir = (collider.transform.position - this.transform.position);
        Vector3 objDirFlat = new Vector3(objDir.x, 0f, objDir.z).normalized;
        var deg = Mathf.Atan2(objDir.x, objDir.z) * Mathf.Rad2Deg;

        int randomValue = UnityEngine.Random.Range(1, 101);
        Vector3 targetPos = collider.ClosestPoint(transform.position);
        if (randomValue < 30)
        {
            targetPos += UnityEngine.Random.insideUnitSphere * 3f;
        }

        C_G_Island_Attack packet = new C_G_Island_Attack();
        packet.IsDokdo = false;
        packet.Degree = deg;
        packet.Index = key;
        packet.Position = new Google.Protobuf.ProtoObject.Position();
        packet.Position.X = targetPos.x;
        packet.Position.Y = targetPos.y;
        packet.Position.Z = targetPos.z;
        RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGIslandAttack, packet);

        yield return waitForSeconds;
        isAttacking = false;
    }

    public void Shot(float _deg, Vector3 _pos)
    {
        transform.DORotate(new Vector3(0f, _deg + 180f, 0f), 0.5f).SetEase(Ease.Linear).OnComplete(() => Fire(_pos));
    }

    void Fire(Vector3 _target)
    {
        var go = ObjectPoolManager.Instance.Instantiate(ResourceDataManager.islandCannonBall, muzzle.transform.position, Quaternion.identity);
        go.gameObject.transform.DOLocalJump(_target, 2, 1, 1f).SetEase(Ease.Linear).OnComplete(() => { go.GetComponent<IslandCannonBall>().GetMeshRenderer().enabled = false; });
    }
}