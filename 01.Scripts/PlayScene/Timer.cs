using System;
using System.Collections;
using RealTime;
using UnityEngine;
using UnityEngine.UI;
using static RealTime.Common.CoreDefine;

public class Timer : RealTimeGameSingleton<Timer>
{
    [SerializeField] Text timeText;
    public Action OnTimerStart;
    public Action OnTimerEnd;
    public bool isGameStarted = false;
    private float maxTime;
    private float time;

    void Start()
    {
        maxTime = GlobalSettings.i.time;
        time = maxTime;
        isGameStarted = true;
        StartCoroutine(TicTok());
    }

    IEnumerator TicTok()
    {
        time = maxTime;
        while (time >= 1)
        {
            if (isGameStarted && RealTimeNetwork.IsMasterClient)
            {
                time -= 0.2f;
                timeText.text = ((int)time).ToString();
                realtimeView.RPC("RPCSyncTime", RpcTarget.Others, time);
            }
            yield return new WaitForSeconds(0.2f);
        }

        if (RealTimeNetwork.IsMasterClient)
        {
            Debug.Log("end Player");
            realtimeView.RPC("RPCSyncTime", RpcTarget.Others, time);
            foreach (var player in PlayerInfo.Instance.playerGameobjectList)
            {
                if (player.Value != null)
                {
                    var p = player.Value.GetComponent<Player>();
                    if (p != null)
                        p.sendScore();
                }
            }
            PlayerInfo.Instance.SendAttackerScore();
        }
    }

    public int GetTime()
    {
        return Convert.ToInt32(timeText.text);
    }

    public int GetAliveTime()
    {
        return (int)(maxTime - time);
    }

    [RealTimeRPC]
    public void RPCSyncTime(float _time)
    {
        this.timeText.text = ((int)_time).ToString();
        time = _time;
    }
}
