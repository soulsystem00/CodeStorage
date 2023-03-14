using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using System.Threading.Tasks;
using DG.Tweening;
using Google.Protobuf.GameProtocol;
using RealTime;
using UnityEngine;

public class ShipRPC : RealTimeMonoBehaviour
{
    [SerializeField] ShipStatManager shipStatManager;
    [SerializeField] HPBar hPBar;
    [SerializeField] SphereCollider col;
    [SerializeField] ShipEffectManager shipEffectManager;
    [SerializeField] Rigidbody rb;


    [RealTimeRPC]
    public void RPCBoosterRange(bool _value)
    {
        col.enabled = _value;
        col.tag = "Booster";
    }

    [RealTimeRPC]
    public void RPCHealRange(bool _value)
    {
        col.enabled = _value;
        col.tag = "Heal";
    }

    [RealTimeRPC]
    public void RPCApplyDamage(float _damage)
    {
        shipStatManager.curHp = hPBar.ApplyDamage(_damage, shipStatManager.Health);
        if (shipStatManager.curHp <= 0f)
        {
            if (transform.position.x < 500)
            {
                Respawn();
            }
            else
            {
                if (realtimeView.IsMine || (realtimeView.IsOutOwner && RealTimeNetwork.IsMasterClient))
                {
                    PlayerDie();
                }
            }
        }
    }


    [RealTimeRPC]
    public void RPCSetMaxHealth(float _maxHealth)
    {
        shipStatManager.curHp = hPBar.SetHpBar(_maxHealth);
    }

    [RealTimeRPC]
    public void RPCRejoinHealth(float _maxHealth, float _curHp)
    {
        hPBar.RejoinHealth(_maxHealth, _curHp);
    }

    [RealTimeRPC]
    public void RPCSetOrderNum(int _index)
    {
        hPBar.SetOrderNum(_index);
    }

    [RealTimeRPC]
    public void RPCFire(float _muzzleX, float _muzzleY, float _muzzleZ,
        float _targetX, float _targetY, float _targetZ,
        float _power, ulong _session_id, float _damage, int[] cannon)
    {
        Vector3 spawnPoint = new Vector3(_muzzleX, _muzzleY, _muzzleZ);
        Vector3 target = new Vector3(_targetX, _targetY, _targetZ);
        var go = ObjectPoolManager.Instance.Instantiate(
            ResourceDataManager.cannonBall, spawnPoint, Quaternion.identity).GetComponent<Rigidbody>();
        CannonBall cannonBall = go.GetComponent<CannonBall>();
        cannonBall.Init(_session_id, _damage, cannon, tag);
        if (go != null)
        {
            cannonBall.gameObject.transform.DOLocalJump(
                target, 1, 1, 1f).SetEase(Ease.Linear)
                .OnComplete(() => { cannonBall.GetMeshRenderer().enabled = false; }
                );
        }
    }

    [RealTimeRPC]
    public void RPCApplyCC(int _CCType)
    {
        if (_CCType == 0)
        {
            shipStatManager.ApplyIgnite();
        }
        else if (_CCType == 1)
        {
            shipStatManager.ApplySlow();
        }
        else if (_CCType == 2)
        {
            shipStatManager.ApplyFaint();
        }
        else if (_CCType == 3)
        {
            shipStatManager.ApplySilence();
        }
    }

    [RealTimeRPC]
    public void RPCEnableEffect(int _skillNum)
    {
        if ((SkillType)_skillNum == SkillType.BoosterRange)
        {
            shipEffectManager.ActiveBoosterRangeEffect();
        }
        else if ((SkillType)_skillNum == SkillType.Flash)
        {
            shipEffectManager.ActiveBoosterEffect();
        }
        else if ((SkillType)_skillNum == SkillType.Heal)
        {
            shipEffectManager.ActiveHealEffect();
        }
        else if ((SkillType)_skillNum == SkillType.HealRange)
        {
            shipEffectManager.ActiveHealRangeEffect();
        }
    }

    [RealTimeRPC]
    public void RPCCrash(float _x, float _y, float _z)
    {
        if (realtimeView.IsMine)
        {
            Vector3 reflectVec = new Vector3(_x, _y, _z);
            rb.AddForce(reflectVec * 2f, ForceMode.VelocityChange);
        }
    }

    private Quaternion SetRotate(Quaternion spawnQua)
    {
        if (shipStatManager.index == 0)
        {
            spawnQua = Quaternion.Euler(0f, -90f, 0f);
        }
        else if (shipStatManager.index == 1)
        {
            spawnQua = Quaternion.Euler(0f, 90f, 0f);
        }
        else if (shipStatManager.index == 2)
        {
            spawnQua = Quaternion.Euler(0f, 30f, 0f);
        }
        else if (shipStatManager.index == 3)
        {
            spawnQua = Quaternion.Euler(0f, -150f, 0f);
        }
        else if (shipStatManager.index == 4)
        {
            spawnQua = Quaternion.Euler(0f, 150f, 0f);
        }
        else if (shipStatManager.index == 5)
        {
            spawnQua = Quaternion.Euler(0f, -30f, 0f);
        }

        return spawnQua;
    }

    private void Respawn()
    {
        transform.position = ShipSpawner.i.GetSpawnPoint(shipStatManager.index, transform.position.x > 500f);

        Quaternion spawnQua = Quaternion.identity;
        spawnQua = SetRotate(spawnQua);
        transform.rotation = spawnQua;
        shipStatManager.curHp = hPBar.ApplyDamage(-shipStatManager.Health, shipStatManager.Health);
    }

    private void PlayerDie()
    {
        RealTimeNetwork.Destroy(this.gameObject);
        C_G_Player_Die packet = new C_G_Player_Die();
        packet.SessionID = realtimeView.OwnerId;
        RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGPlayerDie, packet);
    }
}
