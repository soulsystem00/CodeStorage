using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Google.Protobuf;
using Google.Protobuf.GameProtocol;
using RealTime;
using RealTime.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static RealTime.Common.CoreDefine;

public enum GameState
{
    Busy,
    FreeRome,
    Minimap,
    Spectator,
}

public class PlaySceneManager : MonoBehaviour
{
    [SerializeField] ShipControlUI shipControlUI;
    [SerializeField] Button startBtn;
    [SerializeField] Button ExitBtn;
    [SerializeField] Button miniMap;
    [SerializeField] Button miniMapExit;
    [SerializeField] GameObject inGameminiMapUI;
    [SerializeField] GameObject miniMapUI;
    [SerializeField] Button testBtn;
    [SerializeField] Timer timer;

    [SerializeField] DisconnectPanel disconnectPanel;
    [SerializeField] ResultPanel resultPanel;
    [SerializeField] PlayerCounter playerCounter;

    public CameraController mainCamera;
    public CameraController inGameMiniMapCamera;
    public MiniMapCamController miniMapCam;
    public BuffManager buffManager;
    public SpectatorManager spectatorManager;

    WaitForSeconds waitForSeconds = new WaitForSeconds(0.2f);
    Coroutine CoCheckMaster = null;
    RealTimeEventManager realTimeEventManager;

    public bool IsGameStarted = false;
    public Action ShipHandleUpdate = null;

    GameState state = GameState.Busy;
    GameState prevState = GameState.Busy;

    void Awake()
    {
        realTimeEventManager = RealTimeEventManager.Instance;

        CoCheckMaster = StartCoroutine(CheckMaster());
        startBtn?.onClick.AddListener(GameStart);
        ExitBtn?.onClick.AddListener(RoomExit);
        miniMap.onClick.AddListener(OnMinimapClicked);
        miniMapExit.onClick.AddListener(OnMiniMapExitClicked);
        testBtn.onClick.AddListener(() => { RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGBattleStart, new C_G_Battle_Start()); });

        AddEvent();
    }

    void Start()
    {
        RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGReady, new C_G_Ready());
    }

    void Update()
    {
        if (state == GameState.FreeRome)
        {
            shipControlUI.HandleUpdate();
            mainCamera.ZoomInOut();
            mainCamera.HandleUpdate();
            inGameMiniMapCamera.HandleUpdate();
            ShipHandleUpdate?.Invoke();
        }
        else if (state == GameState.Minimap)
        {
            miniMapCam.HandleUpdate();
        }
        else if (state == GameState.Spectator)
        {
            mainCamera.ZoomInOut();
            mainCamera.HandleUpdate();
            inGameMiniMapCamera.HandleUpdate();
        }

        if (Time.frameCount % 30 == 0)
        {
            System.GC.Collect();
        }
    }

    void OnDestroy()
    {
        RemoveEvent();
    }

    private void AddEvent()
    {
        realTimeEventManager.OnGCStartEvent += GCStartEvent;
        realTimeEventManager.OnNewJoinPlayerEvent += NewJoinPlayerEvent;
        realTimeEventManager.OnGCRejoinEvent += GCRejoinEvent;
        realTimeEventManager.OnGCBattleStart += GCBattleStartEvent;
        realTimeEventManager.OnGCPlayerDie += GCPlayerDieEvent;
        realTimeEventManager.OnDisconnectRoomEvent += DisConnectRoomEvent;
        realTimeEventManager.OnGCBattleEnd += GCBattleEnd;
        realTimeEventManager.OnGCTimeSync += GCTimeSyncEvent;
    }

    private void RemoveEvent()
    {
        realTimeEventManager.OnGCStartEvent -= GCStartEvent;
        realTimeEventManager.OnNewJoinPlayerEvent -= NewJoinPlayerEvent;
        realTimeEventManager.OnGCRejoinEvent -= GCRejoinEvent;
        realTimeEventManager.OnGCBattleStart -= GCBattleStartEvent;
        realTimeEventManager.OnGCPlayerDie -= GCPlayerDieEvent;
        realTimeEventManager.OnDisconnectRoomEvent -= DisConnectRoomEvent;
        realTimeEventManager.OnGCBattleEnd -= GCBattleEnd;
        realTimeEventManager.OnGCTimeSync -= GCTimeSyncEvent;
    }

    private void GCStartEvent(IMessage obj)
    {
        if (CoCheckMaster != null)
        {
            StopCoroutine(CoCheckMaster);
            startBtn.gameObject.SetActive(false);
        }
        // IsGameStarted = true;
        state = GameState.FreeRome;
        timer.gameObject.SetActive(true);
        playerCounter.gameObject.SetActive(false);

        if (RealTimeNetwork.IsMasterClient)
        {
            RealTimeNetwork.CloseJoin_Master();
            RealTimeNetwork.StartReJoin_Master();
        }
    }

    private void NewJoinPlayerEvent(CoreDefine.RT_G_C_New_Join_Player _packet)
    {
        if (_packet.IsReJoin)
        {
            if (RealTimeNetwork.IsMasterClient)
            {
                C_G_Rejoin response = new C_G_Rejoin();
                response.SessionID = _packet.NewPlayer.SessionId;
                RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGRejoin, response);
            }
        }
    }

    private void GCRejoinEvent(IMessage obj)
    {
        if (CoCheckMaster != null)
        {
            StopCoroutine(CoCheckMaster);
            startBtn.gameObject.SetActive(false);
        }
        state = GameState.FreeRome;
        playerCounter.gameObject.SetActive(false);
    }

    private void GCBattleStartEvent(IMessage _packet)
    {
        miniMapCam.moveOffSet = 1000f;
        timer.gameObject.SetActive(false);
    }

    private void GCPlayerDieEvent(IMessage obj)
    {
        G_C_Player_Die packet = (G_C_Player_Die)obj;
        if (packet.SessionID == RealTimeNetwork.SessionId)
        {
            ShipHandleUpdate = null;
            spectatorManager.gameObject.SetActive(true);

            if (state == GameState.Minimap)
                prevState = GameState.Spectator;
            else
                state = GameState.Spectator;
        }
    }

    private void DisConnectRoomEvent(DisconnectType _reson)
    {
        if (_reson == DisconnectType.UnstableConnection)
        {
            disconnectPanel.gameObject.SetActive(true);
        }
    }

    private void GCBattleEnd(IMessage _packet)
    {
        G_C_Battle_End packet = (G_C_Battle_End)_packet;
        resultPanel.gameObject.SetActive(true);
        resultPanel.SetResultText(ShipSpawner.i.GetTeam(RealTimeNetwork.SessionId) == packet.WinningTeam);

        RealTimeNetwork.OutRoom();
    }

    private void GCTimeSyncEvent(IMessage obj)
    {
        timer.gameObject.SetActive(true);
        realTimeEventManager.OnGCTimeSync -= GCTimeSyncEvent;
    }

    private void GameStart()
    {
        if (RealTimeNetwork.IsMasterClient)
        {
            RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGStart, new C_G_Start());
            StartCoroutine(StartBtnInActive());
        }
    }

    private void RoomExit()
    {
        var go = FindObjectsOfType<ShipController>().Where(x => x.realtimeView.IsMine).FirstOrDefault();
        if (go != null)
        {
            C_G_Player_Die packet = new C_G_Player_Die();
            packet.SessionID = RealTimeNetwork.SessionId;
            RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGPlayerDie, packet);
        }
        RealTimeNetwork.OutRoom();
        SceneManager.LoadScene(2);
    }

    IEnumerator CheckMaster()
    {
        startBtn.gameObject.SetActive(false);
        while (true)
        {
            if (RealTimeNetwork.IsMasterClient)
            {
                startBtn.gameObject.SetActive(true);
            }
            yield return waitForSeconds;
        }
    }

    IEnumerator StartBtnInActive()
    {
        startBtn.interactable = false;
        yield return new WaitForSeconds(0.5f);
        startBtn.interactable = true;
    }

    public void GameEnd()
    {
        RealTimeNetwork.EndRoom_Master();
    }

    public void OnMinimapClicked()
    {
        prevState = state;
        state = GameState.Minimap;
        SetDisplay(false);

        if (prevState == GameState.Spectator)
            spectatorManager.gameObject.SetActive(false);
    }

    public void OnMiniMapExitClicked()
    {
        state = prevState;
        SetDisplay(true);

        if (state == GameState.Spectator)
            spectatorManager.gameObject.SetActive(true);
    }

    private void SetDisplay(bool _value)
    {
        mainCamera.gameObject.SetActive(_value);
        inGameminiMapUI.SetActive(_value);
        miniMapCam.gameObject.SetActive(!_value);
        miniMapUI.gameObject.SetActive(!_value);
    }
}