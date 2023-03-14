using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.AccessControl;
using Google.Protobuf;
using Google.Protobuf.GameProtocol;
using RealTime;
using RealTime.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using static RealTime.Common.CoreDefine;

public class PlayerListManager : GameSingleton<PlayerListManager>
{
    Dictionary<ulong, string> playerNickNameList = new Dictionary<ulong, string>();
    Dictionary<ulong, GameObject> playerGameObject = new Dictionary<ulong, GameObject>();
    Dictionary<ulong, int> playerIndex = new Dictionary<ulong, int>();
    bool IsGameStarted = false;
    public bool GetBattleMessage = false;
    WaitForSeconds waitForSeconds = new WaitForSeconds(1f);

    Coroutine co;

    public Dictionary<ulong, GameObject> PlayerGameObject => playerGameObject;
    void Awake()
    {
        AddEvent();
    }

    private void AddEvent()
    {
        RealTimeEventManager.Instance.OnCreateRoomEvent += CreateRoomEvent;
        RealTimeEventManager.Instance.OnJoinedRoomEvent += JoinedRoomEvent;
        RealTimeEventManager.Instance.OnNewJoinPlayerEvent += NewJoinPlayerEvent;
        RealTimeEventManager.Instance.OnOutPlayerEvent += OutPlayerEvent;
        RealTimeEventManager.Instance.OnDisconnectRoomEvent += DisconnectRoomEvent;
        RealTimeEventManager.Instance.OnGCStartEvent += StartEvent;
        RealTimeEventManager.Instance.OnInstantiateEvent += InstantiateEvent;
        RealTimeEventManager.Instance.OnOtherGameObjectEvent += OtherGameObjectEvent;
        RealTimeEventManager.Instance.OnGCPlayerDie += GCPlayerDie;
        RealTimeEventManager.Instance.OnGCBattleStart += (_) =>
        {
            GetBattleMessage = true;
            if (playerGameObject[RealTimeNetwork.SessionId].transform.position.x < 500)
                ShipSpawner.i.StartBattle(playerGameObject[RealTimeNetwork.SessionId]);
            if (RealTimeNetwork.IsMasterClient)
            {
                if (co == null)
                    co = StartCoroutine(CheckBattleStart());
            }
        };
    }

    private void GCPlayerDie(IMessage obj)
    {
        //Debug.Log("Player Die or Exit");
        //Debug.Log(playerGameObject.Count);
        G_C_Player_Die packet = (G_C_Player_Die)obj;
        if (playerGameObject[packet.SessionID] != null)
        {
            playerGameObject[packet.SessionID].SetActive(false);
        }
        playerGameObject[packet.SessionID] = null;

        string team = ShipSpawner.i.GetTeam(packet.SessionID);

        StartCoroutine(CheckTeam(team));
    }

    private IEnumerator CheckTeam(string team)
    {
        yield return new WaitForSeconds(1f);
        if (RealTimeNetwork.IsMasterClient)
        {
            var playerList = playerGameObject.Where(x => x.Value != null).ToList();
            //Debug.Log(playerList);
            if (playerList.Count == 0)
            {
                C_G_Battle_End response = new C_G_Battle_End();
                if (team == "Red")
                {
                    response.WinningTeam = "Blue";
                }
                else
                {
                    response.WinningTeam = "Red";
                }

                RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGBattleEnd, response);
            }
            else
            {
                int teamCnt = playerList.Where(x => x.Value.GetComponent<ShipStatManager>().team == team).Count();
                if (teamCnt == 0)
                {
                    C_G_Battle_End response = new C_G_Battle_End();
                    if (team == "Red")
                    {
                        response.WinningTeam = "Blue";
                    }
                    else
                    {
                        response.WinningTeam = "Red";
                    }

                    RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGBattleEnd, response);
                }
            }
        }
    }

    private void OtherGameObjectEvent(List<GameObject> _list_go, bool _is_rejoin)
    {
        foreach (var go in _list_go)
        {
            IsGameStarted = true;
            AddObject(go.GetComponent<RealTimeView>().OwnerId, go);
            Debug.Log($"{go.transform.position} {go.GetComponent<RealTimeView>().IsMine} {go.GetComponent<RealTimeView>().OwnerId}");
        }

        if (IsGameStarted)
        {
            GameObject go = playerGameObject.Where(x => x.Value.GetComponent<RealTimeView>().IsMine).FirstOrDefault().Value;

            if (go == null)
            {
                RealTimeNetwork.OutRoom();
                SceneManager.LoadScene(2);
            }
        }
    }

    IEnumerator CheckBattleStart()
    {
        while (true)
        {
            int i = 0;
            foreach (var go in playerGameObject)
            {
                var rv = go.Value.GetComponent<RealTimeView>();
                if (rv.IsOutOwner)
                {
                    ShipSpawner.i.StartBattleOutOwner(rv.OwnerId, go.Value);
                }
                else if (go.Value.transform.position.x < 500)
                {
                    RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGBattleStart, new C_G_Battle_Start());
                    break;
                }
                i++;
            }

            if (i == playerGameObject.Count)
            {
                yield break;
            }
            else
            {
                yield return waitForSeconds;
            }
        }
    }

    private void InstantiateEvent(GameObject _packet)
    {
        ShipStatManager ship = _packet.GetComponent<ShipStatManager>();
        if (ship != null)
        {
            ulong sessionID = ship.realtimeView.OwnerId;
            AddObject(ship.realtimeView.OwnerId, _packet);
            ship.SetNickName(playerNickNameList[sessionID], ShipSpawner.i.GetTeam(sessionID), ShipSpawner.i.indexDic[sessionID]);
        }

    }

    private void StartEvent(IMessage _packet)
    {
        IsGameStarted = true;

        if (RealTimeNetwork.IsMasterClient)
        {
            StartCoroutine(SpawnFunction());
        }
    }

    private IEnumerator SpawnFunction()
    {
        while (playerGameObject.Count != playerNickNameList.Count)
        {
            // Debug.Log("Ship Spawn send");
            int i = 0;
            foreach (var player in playerNickNameList)
            {
                if (!playerGameObject.ContainsKey(player.Key))
                {
                    C_G_Ship_Spawn packet = new C_G_Ship_Spawn();
                    packet.SessionID = player.Key;
                    packet.Index = i;
                    if (i % 2 == 0)
                    {
                        packet.Team = "Red";
                    }
                    else
                    {
                        packet.Team = "Blue";
                    }
                    RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGShipSpawn, packet);
                }
                i++;
            }
            yield return new WaitForSeconds(2f);
        }
    }

    private void DisconnectRoomEvent(DisconnectType _packet)
    {
        if (_packet == DisconnectType.OutRoom)
        {
            ClearPlayerList();
            playerGameObject.Clear();
        }
        else if (_packet == DisconnectType.EndRoom)
        {
            ClearPlayerList();
            playerGameObject.Clear();
        }
        else if (_packet == DisconnectType.UnstableConnection)
        {
            ClearPlayerList();
            playerGameObject.Clear();
        }
        IsGameStarted = false;
        GetBattleMessage = false;
    }

    private void OutPlayerEvent(RT_G_C_Out_Player _packet)
    {
        if (!IsGameStarted)
        {
            RemovePlayer(_packet.SessionId);
        }
        else
        {
            ShipController go = FindObjectsOfType<ShipController>().Where(x => x.realtimeView.OwnerId == _packet.SessionId).FirstOrDefault();
            if (go != null)
            {
                playerGameObject[_packet.SessionId].transform.position = go.transform.position;
                playerGameObject[_packet.SessionId].transform.rotation = go.transform.rotation;
            }
        }
    }

    private void NewJoinPlayerEvent(RT_G_C_New_Join_Player _packet)
    {
        if (!IsGameStarted)
        {
            AddPlayer(_packet.NewPlayer.SessionId, _packet.NewPlayer.NickName);
        }
        else
        {
            if (RealTimeNetwork.IsMasterClient)
            {
                C_G_Rejoin_Obj response = new C_G_Rejoin_Obj();
                response.SessionID = _packet.NewPlayer.SessionId;
                var go = playerGameObject[_packet.NewPlayer.SessionId];
                response.Value.Add(go.transform.position.x);
                response.Value.Add(go.transform.position.y);
                response.Value.Add(go.transform.position.z);
                response.Value.Add(go.transform.rotation.x);
                response.Value.Add(go.transform.rotation.y);
                response.Value.Add(go.transform.rotation.z);
                response.Value.Add(go.transform.rotation.w);

                response.ShipStat = go.GetComponent<ShipStatManager>().GetStat();

                RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGRejoinObj, response);
            }
        }
    }

    private void JoinedRoomEvent(RT_G_C_Join_Room _packet)
    {
        ClearPlayerList();
        // StartCoroutine(PlayerAdd(_packet));
        foreach (var player in _packet.OtherPlayer)
        {
            AddPlayer(player.SessionId, player.NickName);
        }
        AddPlayer(_packet.Me.SessionId, _packet.Me.NickName);
    }

    private IEnumerator PlayerAdd(RT_G_C_Join_Room _packet)
    {
        foreach (var player in _packet.OtherPlayer)
        {
            AddPlayer(player.SessionId, player.NickName);
            yield return null;
        }
        AddPlayer(_packet.Me.SessionId, _packet.Me.NickName);
        // Debug.Log($"playerGameobject Count : {playerGameObject.Count}");
    }

    private void CreateRoomEvent(RT_G_C_Create_Room _packet)
    {
        ClearPlayerList();
        AddPlayer(_packet.Me.SessionId, _packet.Me.NickName);

    }

    private bool ClearPlayerList()
    {
        playerNickNameList.Clear();

        return playerNickNameList.Count == 0;
    }

    private bool AddPlayer(ulong _session_id, string _nickName)
    {
        string tmp;
        if (!playerNickNameList.TryGetValue(_session_id, out tmp))
        {
            playerNickNameList.Add(_session_id, _nickName);
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool RemovePlayer(ulong _session_id)
    {
        if (playerNickNameList.ContainsKey(_session_id))
        {
            playerNickNameList.Remove(_session_id);
            return true;
        }
        else
        {
            RealTimeNetwork.OutRoom();
            SceneManager.LoadScene(2);
            return false;
        }
    }

    private bool AddObject(ulong _session_id, GameObject _go)
    {
        if (!playerGameObject.ContainsKey(_session_id))
        {
            playerGameObject.Add(_session_id, _go);
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool RemoveObject(ulong _session_id)
    {
        if (playerGameObject.ContainsKey(_session_id))
        {
            playerGameObject.Remove(_session_id);
            return true;
        }
        else
        {

            return false;
        }
    }

    public int GetPlayerCount()
    {
        return playerNickNameList.Count;
    }

    public GameObject GetRandomGameObject()
    {
        System.Random rand = new System.Random();
        var tmpList = playerGameObject.Where(x => x.Value != null).ToList();
        if (tmpList.Count == 0)
            return null;
        else
            return tmpList.ElementAt(rand.Next(0, tmpList.Count)).Value;
    }

    public bool CheckPlayerObj(ulong _session_id)
    {
        return playerGameObject.ContainsKey(_session_id);
    }

    [ContextMenu("Print Player List")]
    public void printlist()
    {
        foreach (var player in playerNickNameList)
        {
            Debug.Log($"{player.Key} {player.Value}");
        }
    }
}