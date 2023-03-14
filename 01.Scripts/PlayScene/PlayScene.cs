using System;
using System.Collections;
using System.Linq;
using Google.Protobuf;
using Google.Protobuf.GameProtocol;
using RealTime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static RealTime.Common.CoreDefine;

public class PlayScene : RealTimeMonoBehaviour
{
    [SerializeField] Button startBtn;
    [SerializeField] Button exitBtn;
    [SerializeField] ScoreBoard scoreBoard;
    [SerializeField] Toggle scoreBoardToggle;
    [SerializeField] GameObject disconnectPanel;
    [SerializeField] GameObject notEnoughMsg;

    private RealTimeEventManager realTimeEventManager;
    private PlayerInfo playerInfo;

    Coroutine runningCo;
    Coroutine CoSpawnBooster;
    public bool isGameStarted = false;

    void AddEvent()
    {
        realTimeEventManager.OnDisconnectRoomEvent += EventDisConnected;
        realTimeEventManager.OnEndRoomEvent += EventEndRoom;
        realTimeEventManager.OnNewJoinPlayerEvent += EventNewJoinPlayer;
        realTimeEventManager.OnOutPlayerEvent += EventOutPlayer;

        realTimeEventManager.OnGCStartEvent += EventGCStart;
        realTimeEventManager.OnGCEndEvent += EventGCEnd;

        realTimeEventManager.OnGCScoreEvent += EventGCScore;

    }

    // 스코어보드 뜨고 exit 버튼 눌렀을 때 호출시키기
    void RemoveEvent()
    {
        realTimeEventManager.OnDisconnectRoomEvent -= EventDisConnected;
        realTimeEventManager.OnEndRoomEvent -= EventEndRoom;
        realTimeEventManager.OnNewJoinPlayerEvent -= EventNewJoinPlayer;
        realTimeEventManager.OnOutPlayerEvent -= EventOutPlayer;

        realTimeEventManager.OnGCStartEvent -= EventGCStart;
        realTimeEventManager.OnGCEndEvent -= EventGCEnd;

        realTimeEventManager.OnGCScoreEvent -= EventGCScore;
    }

    void Awake()
    {
        realTimeEventManager = RealTimeEventManager.Instance;
        playerInfo = PlayerInfo.Instance;
        AddEvent();

        runningCo = StartCoroutine(EnableButton());
    }

    public void OnExitButtonClicked()
    {
        if (!isGameStarted)
        {
            // 게임 시작 전 - 그냥 나감
            if (RealTimeNetwork.IsMasterClient && PlayerInfo.Instance.GetPlayerCount() <= 1)
            {
                RealTimeNetwork.CloseJoin_Master();
                RealTimeNetwork.EndRoom_Master();
            }
            else
            {
                RealTimeNetwork.OutRoom();
            }
        }
        else
        {
            // 게임 시작 후 - 스코어 보드 활성화
            if (RealTimeNetwork.SessionId == playerInfo.attackerID)
            {
                foreach (var player in PlayerInfo.Instance.playerGameobjectList)
                {
                    if (player.Value != null)
                    {
                        var p = player.Value.GetComponent<Player>();
                        p.sendScore();
                    }
                }
                playerInfo.SendAttackerScore();
            }
            else
            {
                var player = playerInfo.playerGameobjectList[RealTimeNetwork.SessionId];
                if (player != null) // 살아있는 상태에서 exit 버튼 누르면
                {
                    playerInfo.DestroyPlayer2(player, true);
                }
                else // 죽은 상태에서 exit 버튼 누르면
                {
                    scoreBoard.gameObject.SetActive(true);
                }
            }
        }
    }

    public void OnExitButtonClicked2()
    {
        if (RealTimeNetwork.IsMasterClient && PlayerInfo.Instance.GetPlayerCount() <= 1)
        {
            RealTimeNetwork.CloseJoin_Master();
            RealTimeNetwork.EndRoom_Master();
        }
        else
        {
            if (RealTimeNetwork.IsInRoom)
                RealTimeNetwork.OutRoom();
        }

        RemoveEvent();
        SceneManager.LoadScene(2);
    }

    public void OnStartBtnClicked()
    {
        if (playerInfo.playerNicknameList.Count >= GlobalSettings.i.minimumPlayer) // 2명으로 수정하기
        {
            Debug.Log("Send Start Packet");
            C_G_Start response = new C_G_Start();
            RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGStart, response);
        }
        else
        {
            Debug.Log("Need More Player!");
            Instantiate(notEnoughMsg, Vector3.zero, Quaternion.identity);

        }
    }

    public void OnEndBtnClicked()
    {
        Debug.Log("Send End Packet");
        C_G_End response = new C_G_End();
        RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGEnd, response);
    }

    public void OnFilpToggleClicked()
    {
        bool objectState = scoreBoard.gameObject.activeSelf;
        scoreBoard.gameObject.SetActive(!objectState);
    }

    private void EventGCStart(IMessage obj)
    {
        // 호스트가 시작 메세지를 보내면 서버에서 시작 메세지를 뿌려서 여기서 반응함
        Debug.Log("Game Start!!");
        isGameStarted = true;
        // 각자 클라이언트에서 실행해야 하는 부분
        if (runningCo != null)
            StopCoroutine(runningCo);
        startBtn.gameObject.SetActive(false);
        // exitBtn.gameObject.SetActive(false); // comment out
        CoSpawnBooster = StartCoroutine(SpawnBooster());

        // 방장(방에 있는 사람 중 한 사람)만 실행 시키는 코드
        if (RealTimeNetwork.IsMasterClient)
        {
            RealTimeNetwork.CloseJoin_Master();
            System.Random rand = new System.Random();
            var rndObj = playerInfo.playerGameobjectList.Keys.ToList()[rand.Next(playerInfo.playerGameobjectList.Count)];

            if (RealTimeNetwork.SessionId != rndObj)
                RealTimeNetwork.ChangeMaster_Master(rndObj);

            playerInfo.playerGameobjectList[rndObj].GetComponent<Player>().SetAttacker(rndObj);

            runningCo = StartCoroutine(SpawnItemBox());
            realtimeView.RPC("RPCSendMasterID", RpcTarget.All, rndObj);
        }
    }

    private void EventNewJoinPlayer(RT_G_C_New_Join_Player obj)
    {
        if (!playerInfo.playerNicknameList.ContainsKey(obj.NewPlayer.SessionId))
        {
            playerInfo.AddPlayerNickname(obj.NewPlayer.SessionId, obj.NewPlayer.NickName);
        }
    }

    private void EventOutPlayer(RT_G_C_Out_Player obj)
    {
        if (playerInfo.playerNicknameList.ContainsKey(obj.SessionId) && !isGameStarted)
        {
            playerInfo.RemovePlayerNickname(obj.SessionId);
            playerInfo.RemovePlayerGameObject(obj.SessionId);
        }
        if (isGameStarted)
        {
            if (obj.SessionId == playerInfo.attackerID)
            {
                scoreBoard.gameObject.SetActive(true);
            }
        }
    }

    private void EventGCScore(IMessage _packet)
    {
        Debug.Log("Insert Player Score");

        G_C_Score player = (G_C_Score)_packet;
        playerInfo.playerScoreList[player.SessionID] = player.Score;
        playerInfo.playerGameobjectList[player.SessionID] = null;

        if (player.SessionID == RealTimeNetwork.SessionId)
        {
            exitBtn.gameObject.SetActive(false);
            scoreBoardToggle.gameObject.SetActive(true);
            scoreBoard.gameObject.SetActive(true);
            if (CoSpawnBooster != null)
                StopCoroutine(CoSpawnBooster);
        }

        if (RealTimeNetwork.SessionId == playerInfo.attackerID)
        {
            if (player.SessionID == RealTimeNetwork.SessionId)
            {
                RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGEnd, new C_G_End());
            }
            else if (playerInfo.playerGameobjectList.Where(x => x.Value != null).Count() == 0)
            {
                playerInfo.SendAttackerScore();
            }
        }
    }

    private void EventGCEnd(IMessage obj)
    {
        // 호스트가 종료 메세지를 보내면 서버에서 뿌려서 여기서 반응함;
        Debug.Log("Game End!!");
        if (RealTimeNetwork.IsMasterClient)
        {
            if (runningCo != null)
                StopCoroutine(runningCo);
        }
        RealTimeNetwork.EndRoom_Master();
    }

    private void EventEndRoom()
    {
        if (isGameStarted)
        {
            scoreBoard.gameObject.SetActive(false);
            scoreBoard.gameObject.SetActive(true);
        }
        else
        {
            RemoveEvent();
            SceneManager.LoadScene(2);
        }
    }

    private void EventDisConnected(DisconnectType _reson)
    {
        if (_reson == DisconnectType.UnstableConnection)
        {
            disconnectPanel.SetActive(true);
        }
        else if (_reson == DisconnectType.Ban)
        {

        }
        else if (_reson == DisconnectType.OutRoom)
        {
            if (!isGameStarted)
            {
                RemoveEvent();
                SceneManager.LoadScene(2);
            }
            else
            {
                scoreBoard.gameObject.SetActive(true);
            }
        }

        playerInfo.playerGameobjectList.Clear();
        playerInfo.playerNicknameList.Clear();
        playerInfo.playerScoreList.Clear();
    }

    Vector3 GetRandomPos()
    {
        Vector3 pos;
        while (true)
        {
            float x = UnityEngine.Random.Range(-6.5f, 6.5f);
            float z = UnityEngine.Random.Range(-6.5f, 6.5f);
            pos = new Vector3(x, 0.5f, z);
            if (pos.magnitude <= 6.5f)
            {
                break;
            }
        }
        return pos;
    }

    IEnumerator EnableButton()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            startBtn.gameObject.SetActive(RealTimeNetwork.IsMasterClient);
            exitBtn.gameObject.SetActive(true);
        }
    }

    IEnumerator SpawnItemBox()
    {
        while (isGameStarted && RealTimeNetwork.IsInRoom)
        {
            var pos = GetRandomPos();
            pos.y = 0.5f;
            realtimeView.RPC("RPCSpawnItemBox", RpcTarget.All, pos.x, pos.y, pos.z);
            yield return new WaitForSeconds(8f);
        }
    }

    IEnumerator SpawnBooster()
    {
        while (isGameStarted)
        {
            if (RealTimeNetwork.SessionId != playerInfo.attackerID)
            {
                var pos = GetRandomPos();
                pos.y = 0.5f;
                ObjectPoolManager.Instance.Instantiate(ResourceDataManager.booster, pos, Quaternion.identity);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    [RealTimeRPC]
    public void RPCSpawnItemBox(float _posX, float _posY, float _posZ)
    {
        var pos = new Vector3(_posX, 1f, _posZ);
        ObjectPoolManager.Instance.Instantiate(ResourceDataManager.itemBox, pos, Quaternion.identity);
    }

    [RealTimeRPC]
    public void RPCSendMasterID(ulong _sessionID)
    {
        playerInfo.attackerID = _sessionID;

        if (RealTimeNetwork.SessionId == _sessionID)
        {
            RealTimeNetwork.Instantiate("UI/Timer", Vector3.zero, Quaternion.identity);
        }
    }
}
