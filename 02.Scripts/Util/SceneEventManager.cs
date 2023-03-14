using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.GameProtocol;
using RealTime;
using RealTime.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneEventManager : GameSingleton<SceneEventManager>
{
    RealTimeEventManager realTimeEventManager;

    void Awake()
    {
        realTimeEventManager = RealTimeEventManager.Instance;
        realTimeEventManager.OnCreateRoomEvent += CreateRoomEvent;
        realTimeEventManager.OnJoinedRoomEvent += JoinedRoomEvent;
        realTimeEventManager.OnEndRoomEvent += EndRoomEvent;
    }

    private void EndRoomEvent()
    {
        SceneManager.LoadSceneAsync(2);
    }

    private void JoinedRoomEvent(CoreDefine.RT_G_C_Join_Room _packet)
    {
        SceneManager.LoadScene(3);
    }

    private void CreateRoomEvent(CoreDefine.RT_G_C_Create_Room _packet)
    {
        SceneManager.LoadScene(3);
    }

    private void LoadLobbyScene()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(2);
    }
}
