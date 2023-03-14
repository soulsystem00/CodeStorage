using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Google.Protobuf;
using Google.Protobuf.GameProtocol;
using RealTime;
using RealTime.Common;
using UnityEngine;

public class IslandManager : MonoBehaviour
{
    [SerializeField] Dokdo dokdo;
    [SerializeField] BuffManager buffManager;
    [SerializeField] IslandSpawnPoint[] spawnPoints;
    [SerializeField] IslandBase[] islandBases;
    [SerializeField] Island[] islands;

    List<G_C_Island_Spawn> spawnPackets = new List<G_C_Island_Spawn>();
    Dictionary<int, Island> islandDic = new Dictionary<int, Island>();
    RealTimeEventManager realTimeEventManager;

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
        realTimeEventManager.OnGCStartEvent += GCStartEvent;
        realTimeEventManager.OnGCIslandSpawnEvent += GCIslandSpawnEvent;
        realTimeEventManager.OnNewJoinPlayerEvent += NewJoinedRoomEvent;
        realTimeEventManager.OnGCIslandHealthChangedEvent += GCIslandHpChangedEvent;
        realTimeEventManager.OnGCIslandAttack += GCIslandAttackEvent;
        dokdo.OnChangeOwner = buffManager.Apply;
    }

    private void RemoveEvent()
    {
        realTimeEventManager.OnGCStartEvent -= GCStartEvent;
        realTimeEventManager.OnGCIslandSpawnEvent -= GCIslandSpawnEvent;
        realTimeEventManager.OnNewJoinPlayerEvent -= NewJoinedRoomEvent;
        realTimeEventManager.OnGCIslandHealthChangedEvent -= GCIslandHpChangedEvent;
        realTimeEventManager.OnGCIslandAttack -= GCIslandAttackEvent;
    }

    private void GCStartEvent(IMessage obj)
    {
        if (RealTimeNetwork.IsMasterClient)
        {
            System.Random random = new System.Random();
            spawnPoints = spawnPoints.OrderBy(x => random.Next()).ToArray();
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                int islandBaseID = UnityEngine.Random.Range(0, islands.Length);
                int islandKey = i;

                C_G_Island_Spawn response = new C_G_Island_Spawn();
                Vector3 point = spawnPoints[i].GetRandomPoint();
                response.IsRejoin = false;
                response.IslandBaseID = islandBaseID;
                response.IslandKey = islandKey;
                response.Hp = 100f;

                response.Position = new Google.Protobuf.ProtoObject.Position();
                response.Position.X = point.x;
                response.Position.Y = point.y;
                response.Position.Z = point.z;

                response.SessionID = 0;

                response.OwnerID = null;

                RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGIslandSpawn, response);
            }
        }
    }

    private void GCIslandSpawnEvent(IMessage _packet)
    {
        StartCoroutine(CoSpawnShip(_packet));
    }

    private void NewJoinedRoomEvent(CoreDefine.RT_G_C_New_Join_Player _packet)
    {
        if (RealTimeNetwork.IsMasterClient && _packet.IsReJoin)
        {
            foreach (var packet in spawnPackets)
            {
                C_G_Island_Spawn response = MakeIslandPacket(_packet, packet);
                RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGIslandSpawn, response);
            }

            C_G_Dokdo_Stat res = MakeDokdoPacket(_packet);
            RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGDokdoStat, res);
        }
    }

    private void GCIslandHpChangedEvent(IMessage _packet)
    {
        G_C_Island_Hp_Changed packet = (G_C_Island_Hp_Changed)_packet;
        if (packet.IsDokdo)
            dokdo.ApplyDamage(packet.Damage, packet.SessionID);
        else
            islandDic[packet.Index].ApplyDamage(packet.Damage, packet.SessionID);
    }

    private void GCIslandAttackEvent(IMessage _packet)
    {
        G_C_Island_Attack packet = (G_C_Island_Attack)_packet;
        Vector3 pos = new Vector3(packet.Position.X, packet.Position.Y, packet.Position.Z);
        if (packet.IsDokdo)
            dokdo.Shot(packet.Degree, pos, packet.Index);
        else
            islandDic[packet.Index].Shot(packet.Degree, pos);
    }

    IEnumerator CoSpawnShip(IMessage _packet)
    {
        G_C_Island_Spawn packet = (G_C_Island_Spawn)_packet;
        Vector3 pos = new Vector3(packet.Position.X, packet.Position.Y, packet.Position.Z);

        Island island;
        GameObject spawnIsland;
        if (packet.IslandBaseID == 0)
        {
            spawnIsland = ResourceDataManager.islandStat;
        }
        else if (packet.IslandBaseID == 1)
        {
            spawnIsland = ResourceDataManager.islandSkill;
        }
        else
        {
            spawnIsland = ResourceDataManager.islandBullet;
        }

        island = ObjectPoolManager.Instance.Instantiate(spawnIsland, pos, Quaternion.identity).GetComponent<Island>();
        island.Init(packet.IslandKey, packet.Hp, packet.OwnerID);

        spawnPackets.Add(packet);
        islandDic.Add(packet.IslandKey, island);
        island.OnChangeOwner = buffManager.Apply;

        yield return null;
    }

    private C_G_Dokdo_Stat MakeDokdoPacket(CoreDefine.RT_G_C_New_Join_Player _packet)
    {
        C_G_Dokdo_Stat res = new C_G_Dokdo_Stat();
        res.Hp = dokdo.Health;
        res.OwnerID = dokdo.OwnerID;
        res.SessionID = _packet.NewPlayer.SessionId;
        return res;
    }

    private C_G_Island_Spawn MakeIslandPacket(CoreDefine.RT_G_C_New_Join_Player _packet, G_C_Island_Spawn packet)
    {
        C_G_Island_Spawn response = new C_G_Island_Spawn();
        response.IsRejoin = true;
        response.IslandBaseID = packet.IslandBaseID;
        response.IslandKey = packet.IslandKey;
        response.Hp = islandDic[packet.IslandKey].Health;
        response.Position = new Google.Protobuf.ProtoObject.Position();
        response.Position = packet.Position;
        response.SessionID = _packet.NewPlayer.SessionId;
        response.OwnerID = islandDic[packet.IslandKey].OwnerID;
        return response;
    }
}
