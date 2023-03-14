using System.Collections;
using System.Collections.Generic;
using RealTime;
using UnityEngine;
using static RealTime.Common.CoreDefine;

public class ShipHealthController : RealTimeMonoBehaviour
{
    public void ApplyDamage(float _damage)
    {
        realtimeView.RPC("RPCApplyDamage", RpcTarget.All, _damage);
    }

    public void ApplyCC(int _CCType)
    {
        realtimeView.RPC("RPCApplyCC", RpcTarget.All, _CCType);
    }

    public void SetMaxHealth(float _maxHealth)
    {
        realtimeView.RPC("RPCSetMaxHealth", RpcTarget.All, _maxHealth);
    }

    public void RejoinHealth(float _maxHealth, float _curHp)
    {
        realtimeView.RPC("RPCRejoinHealth", RpcTarget.All, _maxHealth, _curHp);
    }

    public void SetOrderNum(int _index)
    {
        realtimeView.RPC("RPCSetOrderNum", RpcTarget.All, _index);
    }
}