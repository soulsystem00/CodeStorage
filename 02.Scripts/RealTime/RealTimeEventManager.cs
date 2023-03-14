using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using Google.Protobuf;
using Google.Protobuf.GameProtocol;
using Google.Protobuf.SystemProtocol;
using RealTime;
using RealTime.Client;
using RealTime.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using static RealTime.Common.CoreDefine;

public class RealTimeEventManager : RealTimeCallback
{
    public static RealTimeEventManager inst;
    public static RealTimeEventManager Instance
    {
        get
        {
            if (inst == null)
            {
                inst = GameObject.FindObjectOfType<RealTimeEventManager>();
            }

            if (inst == null)
            {
                GameObject unityObject = new GameObject();
                var newSingleton = unityObject.AddComponent<RealTimeEventManager>();
                inst = newSingleton;

                unityObject.name = inst.GetType().Name;

                DontDestroyOnLoad(inst.gameObject);
            }

            return inst;
        }
    }

    public Action<RT_G_C_Create_Room> OnCreateRoomEvent = null;
    public Action<FailReasonType> OnCreateRoomFailedEvent = null;
    public Action<RT_G_C_Join_Room> OnJoinedRoomEvent = null;
    public Action<FailReasonType> OnJoinRoomFailedEvent = null;
    public Action<RT_G_C_New_Join_Player> OnNewJoinPlayerEvent = null;
    public Action<RT_G_C_Out_Player> OnOutPlayerEvent = null;
    public Action<DisconnectType> OnDisconnectRoomEvent = null;
    public Action OnEndRoomEvent = null;
    public Action<GameObject> OnInstantiateEvent = null;
    public Action<ulong, ulong> OnChangeMasterEvent = null;
    public Action<bool, RT_RoomInfo, List<RT_RoomPropertyInfo>> OnCanReJoinRoomEvent = null;
    public Action<List<GameObject>, bool> OnOtherGameObjectEvent = null;
    public Action OnReJoinRoomFailedEvent = null;
    public Action<ulong> OnExpiredRejoinEvent = null;

    public Action<IMessage> OnGCStartEvent = null;
    public Action<IMessage> OnGCEndEvent = null;
    public Action<IMessage> OnGCBattleStart = null;
    public Action<IMessage> OnGCBattleEnd = null;

    public Action<IMessage> OnGCShipSpawnEvent = null;
    public Action<IMessage> OnGCStatSyncEvent = null;
    public Action<IMessage> OnGCPlayerDie = null;

    public Action<IMessage> OnGCIslandSpawnEvent = null;
    public Action<IMessage> OnGCIslandHealthChangedEvent = null;
    public Action<IMessage> OnGCIslandAttack = null;
    public Action<IMessage> OnGCDokDoStat = null;

    public Action<IMessage> OnGCRejoinEvent = null;
    public Action<IMessage> OnGCRejoinObjEvent = null;

    public Action<IMessage> OnGCTimeSync = null;
    public Action<IMessage> OnGCRedZoneSync = null;

    public Action<IMessage> OnGCSupplySpawn = null;
    public Action<IMessage> OnGCSupplyRejoin = null;
    public Action<IMessage> OnGCSupplyDestory = null;

    void Awake()
    {
        Time.timeScale = 1f;

        Screen.SetResolution(720, 1280, true);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        DontDestroyOnLoad(this);
    }

    void Start()
    {
        ClientEventHandler handler = new ClientEventHandler();
        handler.Register<G_C_Start>((UInt16)GamePacketId.GCStart, GCStart);
        handler.Register<G_C_End>((UInt16)GamePacketId.GCEnd, GCEnd);
        handler.Register<G_C_Battle_Start>((UInt16)GamePacketId.GCBattleStart, GCBattleStart);
        handler.Register<G_C_Battle_End>((UInt16)GamePacketId.GCBattleEnd, GCBattleEnd);

        handler.Register<G_C_Ship_Spawn>((UInt16)GamePacketId.GCShipSpawn, GCShipSpawn);
        handler.Register<G_C_Stat_Sync>((UInt16)GamePacketId.GCStatSync, GCStatSync);
        handler.Register<G_C_Player_Die>((UInt16)GamePacketId.GCPlayerDie, GCPlayerDie);

        handler.Register<G_C_Island_Spawn>((UInt16)GamePacketId.GCIslandSpawn, GCIslandSpawn);
        handler.Register<G_C_Island_Hp_Changed>((UInt16)GamePacketId.GCIslandHpChanged, GCIslandHPChanged);
        handler.Register<G_C_Island_Attack>((UInt16)GamePacketId.GCIslandAttack, GCIslandAttack);
        handler.Register<G_C_Dokdo_Stat>((UInt16)GamePacketId.GCDokdoStat, GCDokDoStat);

        handler.Register<G_C_Rejoin>((UInt16)GamePacketId.GCRejoin, GCRejoin);
        handler.Register<G_C_Rejoin_Obj>((UInt16)GamePacketId.GCRejoinObj, GCRejoinObj);

        handler.Register<G_C_Time_Sync>((UInt16)GamePacketId.GCTimeSync, GCTimeSyne);
        handler.Register<G_C_Redzone_Sync>((UInt16)GamePacketId.GCRedzoneSync, GCRedZoneSync);

        handler.Register<G_C_Supply_Spawn>((UInt16)GamePacketId.GCSupplySpawn, GCSupplySpawn);
        handler.Register<G_C_Supply_Rejoin>((UInt16)GamePacketId.GCSupplyRejoin, GCSupplyRejoin);
        handler.Register<G_C_Supply_Destroy>((UInt16)GamePacketId.GCSupplyDestroy, GCSupplyDestory);

        base.Init(handler);
    }

    // Override
    protected override void OnCreatedRoom(RT_G_C_Create_Room _packet)
    {
        Debug.Log("Create room");
        OnCreateRoomEvent?.Invoke(_packet);
    }

    protected override void OnCreateRoomFailed(FailReasonType _reson)
    {
        Debug.Log("Create room failed " + _reson);
        OnCreateRoomFailedEvent?.Invoke(_reson);
    }

    protected override void OnJoinedRoom(RT_G_C_Join_Room _packet)
    {
        Debug.Log("Join room");
        OnJoinedRoomEvent?.Invoke(_packet);
    }

    protected override void OnJoinRoomFailed(FailReasonType _reson)
    {
        Debug.Log("Join room failed");
        OnJoinRoomFailedEvent?.Invoke(_reson);
    }

    protected override void OnNewJoinPlayer(RT_G_C_New_Join_Player _packet)
    {
        Debug.Log("New Joined " + _packet.NewPlayer.NickName);
        OnNewJoinPlayerEvent?.Invoke(_packet);
    }

    protected override void OnOutPlayer(RT_G_C_Out_Player _packet)
    {
        Debug.Log("Out Player " + _packet.SessionId);
        OnOutPlayerEvent?.Invoke(_packet);
    }

    protected override void OnDisconnectRoom(DisconnectType _reson)
    {
        Debug.Log("Disconnected " + _reson);
        OnDisconnectRoomEvent?.Invoke(_reson);
    }

    protected override void OnEndRoom()
    {
        Debug.Log("End Room");
        OnEndRoomEvent?.Invoke();
    }
    protected override void OnInstantiate(GameObject _go)
    {
        var rot = _go.transform.rotation;
        _go.transform.rotation = Quaternion.Euler(rot.eulerAngles.x, rot.eulerAngles.y, 0);
        OnInstantiateEvent?.Invoke(_go);
    }

    protected override void OnChangeMaster(ulong _before_master_session_id, ulong _after_master_session_id)
    {
        Debug.Log("On Master Changed");
        OnChangeMasterEvent?.Invoke(_before_master_session_id, _after_master_session_id);
    }

    protected override void OnCanReJoinRoom(bool _result, RT_RoomInfo _room_info, List<RT_RoomPropertyInfo> _list_room_peroperty)
    {
        OnCanReJoinRoomEvent?.Invoke(_result, _room_info, _list_room_peroperty);
    }

    protected override void OnOtherGameObject(List<GameObject> _list_go, bool _is_rejoin)
    {
        OnOtherGameObjectEvent?.Invoke(_list_go, _is_rejoin);
    }

    protected override void OnReJoinRoomFailed()
    {
        Debug.Log("Rejoin failed");
        OnReJoinRoomFailedEvent?.Invoke();
    }

    protected override void OnExpiredReJoin(ulong _session_id)
    {
        Debug.Log("expire rejoin");
        OnExpiredRejoinEvent?.Invoke(_session_id);
    }

    // Client Packet Function
    private void GCStart(IMessage _packet)
    {
        Debug.Log("Start");
        OnGCStartEvent?.Invoke(_packet);
    }

    private void GCEnd(IMessage obj)
    {
        OnGCEndEvent?.Invoke(obj);
    }

    private void GCBattleStart(IMessage _packet)
    {
        OnGCBattleStart?.Invoke(_packet);
    }

    private void GCBattleEnd(IMessage _packet)
    {
        OnGCBattleEnd?.Invoke(_packet);
    }

    private void GCShipSpawn(IMessage _packet)
    {
        OnGCShipSpawnEvent?.Invoke(_packet);
    }

    private void GCStatSync(IMessage _packet)
    {
        OnGCStatSyncEvent?.Invoke(_packet);
    }

    private void GCPlayerDie(IMessage _packet)
    {
        OnGCPlayerDie?.Invoke(_packet);
    }

    private void GCIslandSpawn(IMessage _packet)
    {
        // Debug.Log("Receive spawn message");
        OnGCIslandSpawnEvent?.Invoke(_packet);
    }

    private void GCIslandHPChanged(IMessage _packet)
    {
        // Debug.Log("Receive HP Message");
        OnGCIslandHealthChangedEvent?.Invoke(_packet);
    }

    private void GCIslandAttack(IMessage _packet)
    {
        OnGCIslandAttack?.Invoke(_packet);
    }

    private void GCDokDoStat(IMessage _packet)
    {
        OnGCDokDoStat?.Invoke(_packet);
    }

    private void GCRejoin(IMessage _packet)
    {
        Debug.Log("Rejoin");
        OnGCRejoinEvent?.Invoke(_packet);
    }

    private void GCRejoinObj(IMessage _packet)
    {
        G_C_Rejoin_Obj response = (G_C_Rejoin_Obj)_packet;

        var go = FindObjectsOfType<ShipStatManager>().Where(x => x.GetComponent<RealTimeView>().IsMine).FirstOrDefault();
        Debug.Log("GCRejoinObj" + go);
        if (go != null)
        {
            go.transform.position = new Vector3(response.Value[0], response.Value[1], response.Value[2]);
            go.transform.rotation = new Quaternion(response.Value[3], response.Value[4], response.Value[5], response.Value[6]);
            go.InitStat(response.ShipStat);
        }
        OnGCRejoinObjEvent?.Invoke(_packet);
    }

    private void GCTimeSyne(IMessage _packet)
    {
        OnGCTimeSync?.Invoke(_packet);
    }

    private void GCRedZoneSync(IMessage _packet)
    {
        OnGCRedZoneSync?.Invoke(_packet);
    }

    private void GCSupplySpawn(IMessage _packet)
    {
        OnGCSupplySpawn?.Invoke(_packet);
    }

    private void GCSupplyRejoin(IMessage _packet)
    {
        OnGCSupplyRejoin?.Invoke(_packet);
    }

    private void GCSupplyDestory(IMessage _packet)
    {
        OnGCSupplyDestory?.Invoke(_packet);
    }
}
