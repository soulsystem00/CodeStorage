using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using Google.Protobuf;
using Google.Protobuf.GameProtocol;
using Google.Protobuf.ProtoObject;
using RealTime;
using UnityEngine;

public class SupplySystem : MonoBehaviour
{
    RealTimeEventManager realTimeEventManager;
    Coroutine curCoroutine;

    void Awake()
    {
        realTimeEventManager = RealTimeEventManager.Instance;
        AddEvent();
    }


    void OnDestroy()
    {
        RemoveEvent();
    }

    private void AddEvent()
    {
        realTimeEventManager.OnGCBattleStart += GCBattleStartEvent;
        realTimeEventManager.OnGCSupplySpawn += GCSupplySpawnEvent;
        realTimeEventManager.OnGCSupplyRejoin += GCSupplyRejoinEvent;
        realTimeEventManager.OnGCRejoinObjEvent += GCRejoinObjEvent;
    }

    private void RemoveEvent()
    {
        realTimeEventManager.OnGCBattleStart -= GCBattleStartEvent;
        realTimeEventManager.OnGCSupplySpawn -= GCSupplySpawnEvent;
        realTimeEventManager.OnGCSupplyRejoin -= GCSupplyRejoinEvent;
        realTimeEventManager.OnGCRejoinObjEvent -= GCRejoinObjEvent;
    }

    private void GCBattleStartEvent(IMessage _packet)
    {
        StartSupplyCoroutine();
    }

    private void GCSupplySpawnEvent(IMessage _packet)
    {
        G_C_Supply_Spawn packet = (G_C_Supply_Spawn)_packet;
        Vector3 spawnPos = new Vector3(packet.Position.X, packet.Position.Y, packet.Position.Z);
        SupplyBox box = ObjectPoolManager.Instance.Instantiate(ResourceDataManager.supplyBox, spawnPos, Quaternion.identity).GetComponent<SupplyBox>();
        box.StartMove();
    }

    private void GCRejoinObjEvent(IMessage obj)
    {
        G_C_Rejoin_Obj packet = (G_C_Rejoin_Obj)obj;
        if (packet.Value[0] > 500)
        {
            StartSupplyCoroutine();
        }
    }

    private void GCSupplyRejoinEvent(IMessage _packet)
    {
        G_C_Supply_Rejoin packet = (G_C_Supply_Rejoin)_packet;
        Vector3 spawnPos = new Vector3(packet.Position.X, packet.Position.Y, packet.Position.Z);
        SupplyBox box = ObjectPoolManager.Instance.Instantiate(ResourceDataManager.supplyBox, spawnPos, Quaternion.identity).GetComponent<SupplyBox>();
        box.BoxRejoin();
    }

    IEnumerator StartSupply()
    {
        while (true)
        {
            if (RealTimeNetwork.IsMasterClient)
            {
                C_G_Supply_Spawn packet = new C_G_Supply_Spawn();
                packet.Position = new Google.Protobuf.ProtoObject.Position();
                packet.Position = GenerateRandomPos();
                packet.Time = DateTime.Now.Ticks;
                RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGSupplySpawn, packet);
            }
            yield return new WaitForSeconds(GlobalSettings.Instance.SupplyBoxSpawnDuration);
        }
    }

    private void StartSupplyCoroutine()
    {
        if (curCoroutine == null)
        {
            curCoroutine = StartCoroutine(StartSupply());
        }
    }

    private Position GenerateRandomPos()
    {
        Position randomPos = new Position();
        randomPos.X = UnityEngine.Random.Range(950f, 1050f);
        randomPos.Y = 0f;
        randomPos.Z = UnityEngine.Random.Range(-50f, 50f);

        return randomPos;
    }
}
