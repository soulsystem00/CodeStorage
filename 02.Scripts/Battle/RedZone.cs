using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using System.Runtime.CompilerServices;
using DG.Tweening;
using Google.Protobuf;
using Google.Protobuf.GameProtocol;
using RealTime;
using RealTime.Common;
using UnityEngine;

public class RedZone : MonoBehaviour
{
    [SerializeField] Transform redZone;
    [SerializeField] float size;
    RealTimeEventManager realTimeEventManager;
    C_G_Redzone_Sync packet;
    WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);
    public static RedZone i;

    void Awake()
    {
        i = this;
        realTimeEventManager = RealTimeEventManager.Instance;
        packet = new C_G_Redzone_Sync();
        AddEvent();
    }
    void OnDestroy()
    {
        RemoveEvent();
    }

    private void AddEvent()
    {
        realTimeEventManager.OnGCBattleStart += GCBattleStartEvent;
        realTimeEventManager.OnGCRedZoneSync += GCRedZoneSyncEvent;
        realTimeEventManager.OnGCRejoinObjEvent += GCRejoinObjEvent;
    }

    private void RemoveEvent()
    {
        realTimeEventManager.OnGCBattleStart -= GCBattleStartEvent;
        realTimeEventManager.OnGCRedZoneSync -= GCRedZoneSyncEvent;
        realTimeEventManager.OnGCRejoinObjEvent -= GCRejoinObjEvent;
    }

    private void GCBattleStartEvent(IMessage obj)
    {
        StartCoroutine(BattleStart());
    }

    private void GCRedZoneSyncEvent(IMessage obj)
    {
        G_C_Redzone_Sync packet = (G_C_Redzone_Sync)obj;
        if (packet.Size <= size)
        {
            size = packet.Size;
            redZone.transform.DOScale(Vector3.one * size, 0.1f).SetEase(Ease.Linear);
        }
    }

    private void GCRejoinObjEvent(IMessage obj)
    {
        G_C_Rejoin_Obj packet = (G_C_Rejoin_Obj)obj;
        if (packet.Value[0] > 500f)
        {
            StartCoroutine(RejoinRedZone());
        }
    }

    IEnumerator BattleStart()
    {
        while (size > 30f)
        {
            ReduceRedZone(30f);
            yield return waitForSeconds;
        }

        yield return new WaitForSeconds(10f);

        while (size > 0f)
        {
            ReduceRedZone(0f);
            yield return waitForSeconds;
        }
    }

    IEnumerator RejoinRedZone()
    {
        while (size > 30f)
        {
            ReduceRedZone(30f);
            yield return waitForSeconds;
        }

        yield return new WaitForSeconds(10f);

        while (size > 0f)
        {
            ReduceRedZone(0f);
            yield return waitForSeconds;
        }
    }

    private void ReduceRedZone(float _minSize)
    {
        if (RealTimeNetwork.IsMasterClient)
        {
            float delta = (size - 3f * 0.1f);
            delta = Mathf.Clamp(delta, _minSize, 200f);
            if (RealTimeNetwork.IsMasterClient)
            {
                packet.Size = delta;
                RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGRedzoneSync, packet);
            }
        }
    }

    public float GetSize()
    {
        return redZone.transform.localScale.x / 2;
    }
}
