using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Google.Protobuf.GameProtocol;
using RealTime;
using UnityEngine;

public class Turtle : RealTimeMonoBehaviour
{
    [SerializeField] BoxCollider col;
    [SerializeField] ParticleSystem ps;

    float damage = 30f;

    void OnEnable()
    {
        StartCoroutine(AttackFlow());
    }

    void OnTriggerEnter(Collider other)
    {
        if (realtimeView.IsMine && (/*other.tag == "Enemy" || */other.CompareTag("Neutrality")))
        {
            AttackIsland(other);
        }
        else if (realtimeView.IsMine && (other.CompareTag("DokdoNeutrality")))
        {
            AttackDokdo(other);
        }
    }

    IEnumerator AttackFlow()
    {
        while (true)
        {
            SetActiveObj(true);
            yield return new WaitForSeconds(3f);
            SetActiveObj(false);
            yield return new WaitForSeconds(3f);
        }
    }

    void SetActiveObj(bool _value)
    {
        col.enabled = _value;
        ps.gameObject.SetActive(_value);
    }

    private void AttackIsland(Collider other)
    {
        var island = other.GetComponent<Island>();
        if (island != null)
        {
            IslandOwner owner = island.GetTeam();
            if (owner == IslandOwner.Enemy || owner == IslandOwner.Neutrality)
            {
                C_G_Island_Hp_Changed packet = new C_G_Island_Hp_Changed();
                packet.SessionID = realtimeView.OwnerId;
                packet.Damage = damage;
                packet.Index = other.GetComponent<Island>().key;
                packet.IsDokdo = false;

                RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGIslandHpChanged, packet);
            }
        }
    }

    private void AttackDokdo(Collider other)
    {
        var dokdo = other.GetComponent<Dokdo>();
        if (dokdo != null)
        {
            ps.gameObject.SetActive(true);
            IslandOwner owner = dokdo.GetTeam();
            if (owner == IslandOwner.Enemy || owner == IslandOwner.Neutrality)
            {
                C_G_Island_Hp_Changed packet = new C_G_Island_Hp_Changed();
                packet.SessionID = realtimeView.OwnerId; ;
                packet.Damage = damage;
                packet.Index = 0;
                packet.IsDokdo = true;

                RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGIslandHpChanged, packet);
            }
        }
    }
}
