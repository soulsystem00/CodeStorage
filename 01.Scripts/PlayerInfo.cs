using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.GameProtocol;
using RealTime;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public static PlayerInfo inst;
    public static PlayerInfo Instance
    {
        get
        {
            if (inst == null)
            {
                inst = GameObject.FindObjectOfType<PlayerInfo>();
            }

            if (inst == null)
            {
                GameObject unityObject = new GameObject();
                var newSingleton = unityObject.AddComponent<PlayerInfo>();
                inst = newSingleton;

                unityObject.name = inst.GetType().Name;

                DontDestroyOnLoad(inst.gameObject);
            }

            return inst;
        }
    }

    public bool isAttacker = false;
    public int minimiIndex = 0;
    public int attackerIndex = 0;
    public Vector3[] startPosition;
    public Dictionary<ulong, string> playerNicknameList;
    public Dictionary<ulong, int> playerScoreList;
    public Dictionary<ulong, GameObject> playerGameobjectList;
    public ulong attackerID;
    public int attackerSuccess = 0;
    public Action OnAttackCntInc = null;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        playerNicknameList = new Dictionary<ulong, string>();
        playerScoreList = new Dictionary<ulong, int>();
        playerGameobjectList = new Dictionary<ulong, GameObject>();
    }

    public void AddPlayerNickname(ulong _sessionId, string _nickName)
    {
        if (!playerNicknameList.ContainsKey(_sessionId))
            playerNicknameList.Add(_sessionId, _nickName);
        if (!playerScoreList.ContainsKey(_sessionId))
            playerScoreList.Add(_sessionId, 0);
    }

    public int GetAttackerScore()
    {
        return (3000 * (GetPlayerCount() - GetAlivePlayerCount())) + (500 * attackerSuccess);
    }

    public void RemovePlayerNickname(ulong _sessionId)
    {
        if (playerNicknameList.ContainsKey(_sessionId))
            playerNicknameList.Remove(_sessionId);
        if (playerScoreList.ContainsKey(_sessionId))
            playerScoreList.Remove(_sessionId);
    }

    public void AddPlayerGameObject(ulong _sessionId, GameObject _obj)
    {
        if (!playerGameobjectList.ContainsKey(_sessionId))
            playerGameobjectList.Add(_sessionId, _obj);
    }

    public void RemovePlayerGameObject(ulong _sessionId)
    {
        if (playerGameobjectList.ContainsKey(_sessionId))
            playerGameobjectList.Remove(_sessionId);
    }

    public int GetIndex(ulong _sessionId)
    {
        return Array.IndexOf(playerNicknameList.Keys.ToArray(), _sessionId);
    }

    public int GetPlayerCount()
    {
        return playerNicknameList.Count();
    }

    public int GetAlivePlayerCount()
    {
        return playerGameobjectList.Where(x => x.Value != null).Count();
    }

    public int GetScore()
    {
        return 2000 * (GetPlayerCount() - GetAlivePlayerCount());
    }

    public void IncAttackCnt()
    {
        attackerSuccess++;
        OnAttackCntInc?.Invoke();
    }

    public void SendAttackerScore()
    {
        C_G_Score attackerScore = new C_G_Score();
        attackerScore.SessionID = attackerID;
        attackerScore.Score = PlayerInfo.Instance.GetAttackerScore();
        RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGScore, attackerScore);
    }

    public void DestroyPlayer(GameObject _go)
    {
        Debug.Log("Destroy Player");
        var player = _go.GetComponentInParent<Player>();
        if (player != null)
        {
            player.sendScore();
            /*var dieParticle = Resources.Load("PlayScene/Die Particle") as GameObject;
            if (dieParticle != null)
            {
                Instantiate(dieParticle, player.transform.position + new Vector3(0, 0.3f, 0f), Quaternion.identity);
            }*/
            RealTimeNetwork.Destroy(player.gameObject);
        }

        /*playerGameobjectList[RealTimeNetwork.SessionId] = null;
        var cnt = playerGameobjectList.Where(x => x.Value != null).Count();
        if (cnt == 0)
        {
            SendAttackerScore();
        }*/
    }

    public void DestroyPlayer2(GameObject _go, bool _isRetire = false)
    {
        var player = _go.GetComponent<Player>();
        if (player != null)
        {
            player.sendScore(_isRetire);
            RealTimeNetwork.Destroy(player.gameObject);
        }

        /*playerGameobjectList[RealTimeNetwork.SessionId] = null;
        var cnt = playerGameobjectList.Where(x => x.Value != null).Count();
        if (cnt == 0)
        {
            SendAttackerScore();
        }*/
    }

    [ContextMenu("player list")]
    public void DebugPlayerList()
    {
        foreach (var player in playerNicknameList)
        {
            Debug.Log($"{player.Key} : {player.Value}");
        }
        foreach (var player in playerGameobjectList)
        {
            Debug.Log($"{player.Key} : {player.Value}");
        }
        foreach (var player in playerScoreList)
        {
            Debug.Log($"{player.Key} : {player.Value}");
        }
    }

    [ContextMenu("attacker ID")]
    public void DebugAttackerID()
    {
        Debug.Log(attackerID);
    }
}