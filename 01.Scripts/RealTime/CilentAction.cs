using RealTime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static RealTime.Common.CoreDefine;

public class CilentAction : MonoBehaviour
{
    [SerializeField] InputField nickNameInput;

    void Awake()
    {
        AddEvent();
    }

    private void EventLoadingScene(RT_G_C_Create_Room _packet)
    {
        ClearPlayerInfo();

        PlayerInfo.Instance.AddPlayerNickname(_packet.Me.SessionId, _packet.Me.NickName);

        SceneManager.LoadScene(3);

        RemoveEvent();
    }

    private void EventLoadingScene(RT_G_C_Join_Room _packet)
    {
        ClearPlayerInfo();

        foreach (var player in _packet.OtherPlayer)
        {
            PlayerInfo.Instance.AddPlayerNickname(player.SessionId, player.NickName);
        }
        PlayerInfo.Instance.AddPlayerNickname(_packet.Me.SessionId, _packet.Me.NickName);

        SceneManager.LoadScene(3);

        RemoveEvent();
    }

    void ClearPlayerInfo()
    {
        PlayerInfo.Instance.playerNicknameList.Clear();
        PlayerInfo.Instance.playerScoreList.Clear();
        PlayerInfo.Instance.playerGameobjectList.Clear();
    }

    void AddEvent()
    {
        RealTimeEventManager.Instance.OnCreateRoomEvent += EventLoadingScene;
        RealTimeEventManager.Instance.OnJoinedRoomEvent += EventLoadingScene;
    }

    void RemoveEvent()
    {
        RealTimeEventManager.Instance.OnCreateRoomEvent -= EventLoadingScene;
        RealTimeEventManager.Instance.OnJoinedRoomEvent -= EventLoadingScene;
    }

    public void OnButtonClick()
    {
        RT_MatchingInfo matchingInfo = new RT_MatchingInfo();
        try
        {
            if (!string.IsNullOrEmpty(nickNameInput.text))
            {
                matchingInfo.NickName = nickNameInput.text;
                matchingInfo.UserId = SystemInfo.deviceUniqueIdentifier;
            }
            else
            {
                matchingInfo.NickName = "Default";
                matchingInfo.UserId = SystemInfo.deviceUniqueIdentifier;
            }
        }
        catch (System.Exception)
        {
            matchingInfo.NickName = "Default";
            matchingInfo.UserId = SystemInfo.deviceUniqueIdentifier;
        }
        matchingInfo.StringFormat = "Json Or Xml Etc..";

        RT_CreateRoomOption createRoomOption = new RT_CreateRoomOption();
        createRoomOption.RoomOption = new RT_RoomOption();
        createRoomOption.RoomOption.RoomType = RoomType.Pve;
        createRoomOption.RoomOption.MaxPlayerCount = 5;
        createRoomOption.RoomOption.Title = "Come here";
        createRoomOption.RoomOption.CreateState = CreateStateType.Open;
        createRoomOption.RoomOption.IsCreateCode = false;

        RealTimeNetwork.JoinRandomOrCreateRoom(matchingInfo, createRoomOption);
    }
}