using System.Collections;
using System.Collections.Generic;
using RealTime;
using UnityEngine;
using static RealTime.Common.CoreDefine;

public class ShipBoxCollider : RealTimeMonoBehaviour
{
    [SerializeField] ShipStatManager shipStatManager;
    [SerializeField] ShipHealthController shipHealthController;
    [SerializeField] Rigidbody rb;

    List<Collider> boosterCol = new List<Collider>();
    List<Collider> healcol = new List<Collider>();

    void Update()
    {
        this.tag = transform.parent.tag;
        CheckBoosterCollider();
        CheckHealCollider();
    }

    void OnTriggerEnter(Collider other)
    {
        if (realtimeView.IsMine)
        {
            TakeDamage(other);
            DetectRangeSkill(other);
            DetectCrash(other);
        }
        else if (realtimeView.IsOutOwner && RealTimeNetwork.IsMasterClient)
        {
            if (RealTimeNetwork.IsMasterClient)
            {
                TakeDamage(other);
                DetectRangeSkill(other);
                DetectCrash(other);
            }
        }

        DetectSupply(other);
    }

    private void CheckBoosterCollider()
    {
        if (boosterCol.Count > 0)
        {
            shipStatManager.boost = true;
            foreach (var col in boosterCol)
            {
                if (!col.enabled)
                {
                    boosterCol.Remove(col);
                    break;
                }
            }
        }
        else if (boosterCol.Count == 0)
        {
            shipStatManager.boost = false;
        }
    }

    private void CheckHealCollider()
    {
        if (healcol.Count > 0)
        {
            foreach (var col in healcol)
            {
                if (!col.enabled)
                {
                    healcol.Remove(col);
                    break;
                }
            }
        }
    }

    private void TakeDamage(Collider other)
    {
        if (other.CompareTag("CannonBall"))
        {
            shipHealthController.ApplyDamage(10f);
        }

        if (other.CompareTag("Atom"))
        {
            shipHealthController.ApplyDamage(5f);
        }

        if ((other.CompareTag("RedCannon") && this.transform.parent.CompareTag("BlueBattle")) || (other.CompareTag("BlueCannon") && this.transform.CompareTag("RedBattle")))
        {
            shipHealthController.ApplyDamage(10f);
            var cannonBallType = other.GetComponent<CannonBall>().cannon;
            if (cannonBallType[3] == 1)
            {
                realtimeView.RPC("RPCApplyCC", RpcTarget.All, 0);
            }
            if (cannonBallType[4] == 1)
            {
                realtimeView.RPC("RPCApplyCC", RpcTarget.All, 1);
            }
            if (cannonBallType[5] == 1)
            {
                realtimeView.RPC("RPCApplyCC", RpcTarget.All, 2);
            }
            if (cannonBallType[6] == 1)
            {
                realtimeView.RPC("RPCApplyCC", RpcTarget.All, 3);
            }
        }
    }

    private void DetectRangeSkill(Collider other)
    {
        if (other.CompareTag("Booster") && (other.transform.parent.CompareTag(this.transform.parent.tag)))
        {
            if (!boosterCol.Contains(other))
            {
                boosterCol.Add(other);
            }
        }

        if (other.CompareTag("Heal") && (other.transform.parent.CompareTag(this.transform.parent.tag)))
        {
            if (!healcol.Contains(other))
            {
                healcol.Add(other);
                shipHealthController.ApplyDamage(-GlobalSettings.Instance.RangeHealAmount);
            }
        }
    }

    private void DetectCrash(Collider other)
    {
        if (other.CompareTag("RedBattle") || other.CompareTag("BlueBattle"))
        {
            if (other.isTrigger)
            {
                if (string.Compare(this.tag, other.tag) != 0)
                {
                    shipHealthController.ApplyDamage(GlobalSettings.Instance.CrashDamage);
                }
            }
        }
    }

    private void DetectSupply(Collider other)
    {
        if (other.CompareTag("SupplyBox"))
        {
            if (realtimeView.IsMine)
            {
                int buff = Random.Range(0, 7);
                string buffInfo = null;
                if (buff == 0)
                {
                    shipStatManager.ApplyBoost(IslandType.StatUpgrade, 2, Stat.Attack);
                    buffInfo = "Attack";
                }
                else if (buff == 1)
                {
                    shipStatManager.ApplyBoost(IslandType.StatUpgrade, 2, Stat.Speed);
                    buffInfo = "Speed";
                }
                else if (buff == 2)
                {
                    shipStatManager.ApplyBoost(IslandType.StatUpgrade, 2, Stat.Health);
                    buffInfo = "Health";
                }
                else if (buff == 3)
                {
                    shipStatManager.ApplyBoost(IslandType.StatUpgrade, 2, Stat.Critical);
                    buffInfo = "Critical";
                }
                else if (buff == 4)
                {
                    shipStatManager.ApplyBoost(IslandType.StatUpgrade, 2, Stat.AttackSpeed);
                    buffInfo = "AttackSpeed";
                }
                else if (buff == 5)
                {
                    shipStatManager.ApplyBoost(IslandType.StatUpgrade, 2, Stat.CannonCount);
                    buffInfo = "CannonCount";
                }
                else if (buff == 6)
                {
                    shipStatManager.ApplyBoost(IslandType.StatUpgrade, 2, Stat.Range);
                    buffInfo = "Range";
                }
                other.GetComponentInParent<SupplyBox>().ActiveEffect(buffInfo);
            }
            else
            {
                other.GetComponentInParent<SupplyBox>().ActiveEffect(null);
            }
        }
    }
}