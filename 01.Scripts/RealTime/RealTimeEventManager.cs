using System;
using Google.Protobuf;
using Google.Protobuf.GameProtocol;
using RealTime;
using RealTime.Client;
using UnityEngine;
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
    public Action<IMessage> OnGCStartEvent = null;
    public Action<IMessage> OnGCEndEvent = null;
    public Action<IMessage> OnGCScoreEvent = null;
    public Action<ulong, ulong> OnChangeMasterEvent = null;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        ClientEventHandler handler = new ClientEventHandler();
        handler.Register<G_C_Start>((UInt16)GamePacketId.GCStart, GCStart);
        handler.Register<G_C_End>((UInt16)GamePacketId.GCEnd, GCEnd);
        handler.Register<G_C_Score>((UInt16)GamePacketId.GCScore, GCScore);
        base.Init(handler);
    }

    static void GCScore(IMessage _packet)
    {
        Debug.Log("Get Player Score from Server");
        RealTimeEventManager.Instance.OnGCScoreEvent?.Invoke(_packet);
    }

    static void GCStart(IMessage _packet)
    {
        Debug.Log("Get Start Message from server");
        RealTimeEventManager.Instance.OnGCStartEvent?.Invoke(_packet);
    }

    static void GCEnd(IMessage _packet)
    {
        Debug.Log("Get End Message from server");
        RealTimeEventManager.Instance.OnGCEndEvent?.Invoke(_packet);
    }

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
        Debug.Log("On Instantiate " + _go.GetComponent<RealTimeView>().IsMine);
        var rot = _go.transform.rotation;
        _go.transform.rotation = Quaternion.Euler(rot.eulerAngles.x, rot.eulerAngles.y, 0);
        OnInstantiateEvent?.Invoke(_go);
    }

    protected override void OnChangeMaster(ulong _before_master_session_id, ulong _after_master_session_id)
    {
        Debug.Log("On Master Changed");
        OnChangeMasterEvent?.Invoke(_before_master_session_id, _after_master_session_id);
    }
}
