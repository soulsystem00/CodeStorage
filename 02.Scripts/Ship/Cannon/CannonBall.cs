using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Security.Permissions;
using Google.Protobuf.GameProtocol;
using RealTime;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    [SerializeField] MeshRenderer mr;
    [SerializeField] Rigidbody rb;
    [SerializeField] SphereCollider col;
    [SerializeField] ParticleSystem ps;
    [SerializeField] GameObject explosion;
    [SerializeField] BallEffect ballEffect;

    WaitForSeconds waitForSeconds = new WaitForSeconds(3f);
    Vector3 dirVec;
    Coroutine CoDestory;
    private ulong sessionId;
    private float damage;
    public int[] cannon = new int[7];

    void OnEnable()
    {
        CoDestory = StartCoroutine(DestroyAfterSec());
    }

    void OnTriggerEnter(Collider other)
    {
        if (sessionId == RealTimeNetwork.SessionId && (other.CompareTag("Enemy") || other.CompareTag("Neutrality")))
        {
            AttackIsland(other);
        }
        else if ((sessionId == RealTimeNetwork.SessionId) && (other.CompareTag("DokdoNeutrality")))
        {
            AttackDokdo(other);
        }
        else if (other.CompareTag("Enemy") || other.CompareTag("Neutrality") || other.CompareTag("Mine") || other.CompareTag("Friendly") || other.CompareTag("Water"))
        {
            SetInvisible();
        }
        else if ((this.CompareTag("RedCannon") && other.CompareTag("BlueBattle")) || (this.CompareTag("BlueCannon") && other.CompareTag("RedBattle")))
        {
            SetInvisible();
        }
    }

    public void Init(ulong _session_id, float _damage, int[] cannon, string _tag)
    {
        this.sessionId = _session_id;
        this.damage = _damage;
        this.cannon = cannon;
        if (_tag == "RedBattle")
        {
            this.tag = "RedCannon";
            explosion.tag = "RedCannon";
        }
        else if (_tag == "BlueBattle")
        {
            this.tag = "BlueCannon";
            explosion.tag = "BlueCannon";
        }

        ballEffect.SetEffect(cannon);
    }

    private void AttackIsland(Collider other)
    {
        var island = other.GetComponent<Island>();
        if (island != null)
        {
            ps.gameObject.SetActive(true);
            mr.enabled = false;
            rb.velocity = Vector3.zero;
            IslandOwner owner = island.GetTeam();
            if (owner == IslandOwner.Enemy || owner == IslandOwner.Neutrality)
            {
                C_G_Island_Hp_Changed packet = new C_G_Island_Hp_Changed();
                packet.SessionID = sessionId;
                packet.Damage = damage;
                packet.Index = other.GetComponent<Island>().key;
                packet.IsDokdo = false;

                RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGIslandHpChanged, packet);
            }
        }

        if (cannon[2] == 1)
        {
            explosion.SetActive(true);
        }
    }

    private void AttackDokdo(Collider other)
    {
        var dokdo = other.GetComponent<Dokdo>();
        if (dokdo != null)
        {
            ps.gameObject.SetActive(true);
            mr.enabled = false;
            col.enabled = false;
            rb.velocity = Vector3.zero;
            IslandOwner owner = dokdo.GetTeam();
            if (owner == IslandOwner.Enemy || owner == IslandOwner.Neutrality)
            {
                C_G_Island_Hp_Changed packet = new C_G_Island_Hp_Changed();
                packet.SessionID = sessionId;
                packet.Damage = damage;
                packet.Index = 0;
                packet.IsDokdo = true;

                RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGIslandHpChanged, packet);
            }
        }

        if (cannon[2] == 1)
        {
            explosion.SetActive(true);
        }
    }

    private void SetInvisible()
    {
        mr.enabled = false;
        col.enabled = false;
        ps.gameObject.SetActive(true);

        if (cannon[2] == 1)
        {
            explosion.SetActive(true);
        }
    }

    IEnumerator DestroyAfterSec()
    {
        rb.velocity = Vector3.zero;
        mr.enabled = true;
        col.enabled = true;
        explosion.SetActive(false);
        ps.gameObject.SetActive(false);

        yield return waitForSeconds;
        rb.velocity = Vector3.zero;
        mr.enabled = true;
        col.enabled = true;
        explosion.SetActive(false);
        ps.gameObject.SetActive(false);
        ballEffect.DisableAll();
        ObjectPoolManager.Instance.Destroy(this.gameObject);
    }

    public MeshRenderer GetMeshRenderer()
    {
        return mr;
    }
}
