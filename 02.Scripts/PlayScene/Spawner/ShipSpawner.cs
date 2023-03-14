using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.GameProtocol;
using RealTime;
using UnityEngine;
using UnityEngine.UI;
using static RealTime.Common.CoreDefine;

public class ShipSpawner : MonoBehaviour
{
    [SerializeField] Transform[] shipSpawnPoint;
    [SerializeField] Transform[] battleSpawnPoint;
    RealTimeEventManager realTimeEventManager;
    Dictionary<ulong, string> teamList = new Dictionary<ulong, string>();
    public Dictionary<ulong, int> indexDic = new Dictionary<ulong, int>();

    public static ShipSpawner i;
    public int myIndex;

    bool isSpawning = false;

    void Awake()
    {
        i = this;
        realTimeEventManager = RealTimeEventManager.Instance;
        AddEvent();
    }

    void OnDestroy()
    {
        RemoveEvent();
    }

    private void AddEvent()
    {
        realTimeEventManager.OnGCShipSpawnEvent += GCShipSpawnEvent;
        realTimeEventManager.OnInstantiateEvent += InstantiateEvent;
        realTimeEventManager.OnGCStatSyncEvent += GCStatSyncEvent;
    }

    private void RemoveEvent()
    {
        realTimeEventManager.OnGCShipSpawnEvent -= GCShipSpawnEvent;
        realTimeEventManager.OnInstantiateEvent -= InstantiateEvent;
        realTimeEventManager.OnGCStatSyncEvent -= GCStatSyncEvent;
    }

    private void GCStatSyncEvent(IMessage _packet)
    {
        G_C_Stat_Sync packet = (G_C_Stat_Sync)_packet;

        if (!teamList.ContainsKey(packet.SessionID))
        {
            teamList.Add(packet.SessionID, packet.ShipStat.Team);
        }
        if (!indexDic.ContainsKey(packet.SessionID))
        {
            indexDic.Add(packet.SessionID, packet.ShipStat.Index);
        }
    }

    private void InstantiateEvent(GameObject obj)
    {
        ShipStatManager go = obj.GetComponent<ShipStatManager>();
        if (go != null)
        {
            obj.tag = teamList[go.realtimeView.OwnerId];
            go.team = teamList[go.realtimeView.OwnerId];
            go.index = indexDic[go.realtimeView.OwnerId];
            if (go.realtimeView.IsMine)
            {
                go.index = myIndex;
                go.StatSyncEvent();
            }
        }
    }

    private void GCShipSpawnEvent(IMessage _packet)
    {
        StartCoroutine(SpawnShip(_packet));
    }

    IEnumerator SpawnShip(IMessage _packet)
    {
        // Debug.Log("Ship Spawn");
        if (!isSpawning)
        {
            isSpawning = true;
            G_C_Ship_Spawn packet = (G_C_Ship_Spawn)_packet;
            if (!teamList.ContainsKey(packet.SessionID))
                teamList.Add(packet.SessionID, packet.Team);
            if (!indexDic.ContainsKey(packet.SessionID))
                indexDic.Add(packet.SessionID, packet.Index);

            if (packet.SessionID == RealTimeNetwork.SessionId)
            {
                if (!PlayerListManager.Instance.CheckPlayerObj(RealTimeNetwork.SessionId))
                {
                    myIndex = packet.Index;
                    Quaternion spawnQua = SetSpawnQua();
                    RealTimeNetwork.Instantiate("Ship/Ship4", shipSpawnPoint[packet.Index].position + GlobalSettings.Instance.ShipSpawnOffset, spawnQua);
                }
            }
            isSpawning = false;
        }
        yield return null;
    }

    private Quaternion SetSpawnQua()
    {
        Quaternion spawnQua = Quaternion.identity;
        if (myIndex == 0)
        {
            spawnQua = Quaternion.Euler(0f, -90f, 0f);
        }
        else if (myIndex == 1)
        {
            spawnQua = Quaternion.Euler(0f, 90f, 0f);
        }
        else if (myIndex == 2)
        {
            spawnQua = Quaternion.Euler(0f, 30f, 0f);
        }
        else if (myIndex == 3)
        {
            spawnQua = Quaternion.Euler(0f, -150f, 0f);
        }
        else if (myIndex == 4)
        {
            spawnQua = Quaternion.Euler(0f, 150f, 0f);
        }
        else if (myIndex == 5)
        {
            spawnQua = Quaternion.Euler(0f, -30f, 0f);
        }

        return spawnQua;
    }

    public string GetTeam(ulong _session_id)
    {
        return teamList[_session_id];
    }

    public Vector3 GetSpawnPoint(int _index, bool _isBattle)
    {
        if (_isBattle)
            return battleSpawnPoint[_index].position;
        else
            return shipSpawnPoint[_index].position;
    }

    public void StartBattle(GameObject _go)
    {
        _go.transform.position = battleSpawnPoint[myIndex].position;
        if (GetTeam(RealTimeNetwork.SessionId) == "Red")
        {
            _go.transform.rotation = Quaternion.Euler(0f, 45f, 0f);
            _go.tag = "RedBattle";
            ShipStatManager stat = _go.GetComponent<ShipStatManager>();
            stat.realtimeView.RPC("RPCApplyDamage", RpcTarget.All, -1000f);
            stat.StatSyncEvent();

            stat.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        else
        {
            _go.transform.rotation = Quaternion.Euler(0f, 180f + 45f, 0f);
            _go.tag = "BlueBattle";
            ShipStatManager stat = _go.GetComponent<ShipStatManager>();
            stat.realtimeView.RPC("RPCApplyDamage", RpcTarget.All, -1000f);
            stat.StatSyncEvent();

            stat.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    public void StartBattleOutOwner(ulong _session_id, GameObject _go)
    {
        _go.transform.position = battleSpawnPoint[indexDic[_session_id]].position;
        if (GetTeam(_session_id) == "Red")
        {
            _go.transform.rotation = Quaternion.Euler(0f, 45f, 0f);
            _go.tag = "RedBattle";
            ShipStatManager stat = _go.GetComponent<ShipStatManager>();
            stat.realtimeView.RPC("RPCApplyDamage", RpcTarget.All, -10000f);
            stat.StatSyncEvent();

            stat.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        else
        {
            _go.transform.rotation = Quaternion.Euler(0f, 180f + 45f, 0f);
            _go.tag = "BlueBattle";
            ShipStatManager stat = _go.GetComponent<ShipStatManager>();
            stat.realtimeView.RPC("RPCApplyDamage", RpcTarget.All, -10000f);
            stat.StatSyncEvent();

            stat.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}