using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime;
using DG.Tweening;
using Google.Protobuf.GameProtocol;
using RealTime;
using RealTime.Common;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static RealTime.Common.CoreDefine;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] TMP_InputField nickName;
    [SerializeField] Button joinBtn;
    [SerializeField] Image fadePanel;

    RealTimeEventManager realTimeEventManager;
    RT_MatchingInfo matchingInfo;
    AsyncOperation scene;

    void Awake()
    {
        realTimeEventManager = RealTimeEventManager.Instance;
        AddEvent();
        fadePanel.color = new Color(0f, 0f, 0f, 1f);
        fadePanel.DOFade(0f, 1.5f).OnComplete(() => Destroy(fadePanel));
    }

    void OnDestroy()
    {
        RemoveEvent();
    }

    private void AddEvent()
    {
        joinBtn?.onClick.AddListener(OnJoinBtnClicked);

        realTimeEventManager.OnCanReJoinRoomEvent += CanReJoinRoomEvent;
        realTimeEventManager.OnJoinRoomFailedEvent += JoinroomFailedEvent;
        realTimeEventManager.OnCreateRoomFailedEvent += CreateRoomFailedEvent;
    }

    private void RemoveEvent()
    {
        realTimeEventManager.OnCanReJoinRoomEvent -= CanReJoinRoomEvent;
        realTimeEventManager.OnJoinRoomFailedEvent -= JoinroomFailedEvent;
        realTimeEventManager.OnCreateRoomFailedEvent -= CreateRoomFailedEvent;
    }

    private void CanReJoinRoomEvent(bool _result, RT_RoomInfo _room_info, List<RT_RoomPropertyInfo> _list_room_peroperty)
    {
        if (_result)
        {
            // try rejoin
            if (matchingInfo == null)
            {
                matchingInfo = InitMatcingInfo();
            }
            RealTimeNetwork.ReJoinRoom(matchingInfo);
        }
        else
        {
            // joinorcreate room
            RT_CreateRoomOption createRoomOption = MakeRooomOption();
            RealTimeNetwork.JoinRandomOrCreateRoom(matchingInfo, createRoomOption);
        }
    }


    private RT_MatchingInfo InitMatcingInfo()
    {
        RT_MatchingInfo matchingInfo = new RT_MatchingInfo
        (
            SystemInfo.deviceUniqueIdentifier,
            string.IsNullOrEmpty(nickName.text) ? "Default" : nickName.text,
            "Json Or Xml Etc.."
        );

        return matchingInfo;
    }

    private void CreateRoomFailedEvent(FailReasonType obj)
    {
        UnityEngine.Debug.Log(obj);
        joinBtn.interactable = true;
    }

    private void JoinroomFailedEvent(FailReasonType obj)
    {
        UnityEngine.Debug.Log(obj);
        joinBtn.interactable = true;
    }

    private static RT_CreateRoomOption MakeRooomOption()
    {
        RT_CreateRoomOption createRoomOption = new RT_CreateRoomOption();
        createRoomOption.RoomOption = new RT_RoomOption();
        createRoomOption.RoomOption.RoomType = RoomType.Pve;
        createRoomOption.RoomOption.MaxPlayerCount = 6;
        createRoomOption.RoomOption.Title = "Come here";
        createRoomOption.RoomOption.CreateState = CreateStateType.Open;
        createRoomOption.RoomOption.IsCreateCode = false;

        createRoomOption.RejoinOption = new RT_ReJoinOption();
        createRoomOption.RejoinOption.Start = false;
        createRoomOption.RejoinOption.OutObjectType = OutObjectType.Keep_Alive;
        return createRoomOption;
    }

    public void OnJoinBtnClicked()
    {
        if (matchingInfo == null)
        {
            matchingInfo = InitMatcingInfo();
        }
        joinBtn.interactable = false;
        RealTimeNetwork.CanReJoinRoom(matchingInfo);
    }
}
