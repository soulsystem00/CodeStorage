using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Google.Protobuf.GameProtocol;
using RealTime;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] Text timeText;
    public float time;
    public float maxTime;

    WaitForSeconds waitForSeconds = new WaitForSeconds(0.5f);
    RealTimeEventManager realTimeEventManager;
    C_G_Time_Sync packet = new C_G_Time_Sync();

    void Awake()
    {
        time = maxTime;
        realTimeEventManager = RealTimeEventManager.Instance;
        realTimeEventManager.OnGCTimeSync += GCTimeSyncEvent;

        StartCoroutine(TimeSync());
    }

    void Update()
    {
        if (RealTimeNetwork.IsMasterClient && time >= 0f)
        {
            time -= Time.deltaTime;
            time = Mathf.Clamp(time, 0f, maxTime);
        }
        timeText.text = ((int)time).ToString();
    }

    void OnDestroy()
    {
        realTimeEventManager.OnGCTimeSync -= GCTimeSyncEvent;
    }

    private void GCTimeSyncEvent(IMessage _packet)
    {
        this.gameObject.SetActive(true);
        if (!RealTimeNetwork.IsMasterClient)
        {
            G_C_Time_Sync packet = (G_C_Time_Sync)_packet;
            time = packet.Time;
        }
    }

    IEnumerator TimeSync()
    {
        while (time > 0f)
        {
            if (RealTimeNetwork.IsMasterClient)
            {
                packet.Time = time;
                RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGTimeSync, packet);
            }
            yield return waitForSeconds;
        }

        if (RealTimeNetwork.IsMasterClient)
        {
            StartCoroutine(CheckBattleStartPacket());
            yield return null;
        }
    }

    IEnumerator CheckBattleStartPacket()
    {
        while (!PlayerListManager.Instance.GetBattleMessage)
        {
            RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGBattleStart, new C_G_Battle_Start());
            yield return new WaitForSeconds(1f);
        }
    }
}