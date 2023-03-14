using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Runtime;
using DG.Tweening;
using Google.Protobuf;
using Google.Protobuf.GameProtocol;
using RealTime;
using RealTime.Common;
using UnityEngine;
using UnityEngine.UI;

public class SupplyBox : MonoBehaviour
{
    [SerializeField] GameObject boxObject;
    [SerializeField] GameObject shadowObject;
    [SerializeField] GameObject effect;
    [SerializeField] SpriteRenderer ping;
    [SerializeField] Text infoText;

    Coroutine curCoroutine;
    WaitForSeconds waitForSeconds = new WaitForSeconds(5f);
    RealTimeEventManager realTimeEventManager;

    void Awake()
    {
        realTimeEventManager = RealTimeEventManager.Instance;
        realTimeEventManager.OnNewJoinPlayerEvent += NewJoinPlayerEvent;
        realTimeEventManager.OnGCSupplyDestory += GCSupplyDestoryEvent;
    }

    void OnDestroy()
    {
        realTimeEventManager.OnNewJoinPlayerEvent -= NewJoinPlayerEvent;
        realTimeEventManager.OnGCSupplyDestory -= GCSupplyDestoryEvent;
    }

    private void NewJoinPlayerEvent(CoreDefine.RT_G_C_New_Join_Player _packet)
    {
        if (gameObject.activeSelf && RealTimeNetwork.IsMasterClient && _packet.IsReJoin)
        {
            C_G_Supply_Rejoin packet = new C_G_Supply_Rejoin();
            packet.Position = new Google.Protobuf.ProtoObject.Position();
            packet.Position.X = transform.position.x;
            packet.Position.Y = transform.position.y;
            packet.Position.Z = transform.position.z;

            packet.SessionID = _packet.NewPlayer.SessionId;
            RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGSupplyRejoin, packet);
        }
    }

    private void GCSupplyDestoryEvent(IMessage _packet)
    {
        G_C_Supply_Destroy packet = (G_C_Supply_Destroy)_packet;
        Vector3 pos = new Vector3(packet.Position.X, packet.Position.Y, packet.Position.Z);
        if (transform.position == pos)
        {
            ObjectPoolManager.Instance.Destroy(this.gameObject);
        }
    }

    private void SetVisible(bool _value)
    {
        boxObject.SetActive(_value);
        shadowObject.SetActive(_value);
        effect.SetActive(!_value);
    }

    public void StartMove(float _delay = 0f)
    {
        ping.color = Color.black;
        SetVisible(true);
        infoText.gameObject.SetActive(false);
        boxObject.transform.position = new Vector3(transform.position.x, 25f, transform.position.z);
        shadowObject.transform.position = new Vector3(transform.position.x, -25f, transform.position.z);
        curCoroutine = StartCoroutine(Move(_delay));
    }

    IEnumerator Move(float _delay)
    {
        float moveTime = 5f - _delay;
        float destroyTime = GlobalSettings.Instance.SupplyBoxAliveTime - _delay;
        ping.DOColor(new Color(1f, 1f, 1f, 1), moveTime);
        boxObject.transform.DOMoveY(0f, moveTime).SetEase(Ease.Linear);
        shadowObject.transform.DOMoveY(0f, moveTime).SetEase(Ease.Linear);
        yield return new WaitForSeconds(moveTime + destroyTime);
        if (RealTimeNetwork.IsMasterClient)
        {
            C_G_Supply_Destroy packet = new C_G_Supply_Destroy();
            packet.Position = new Google.Protobuf.ProtoObject.Position();
            packet.Position.X = transform.position.x;
            packet.Position.Y = transform.position.y;
            packet.Position.Z = transform.position.z;
            RealTimeNetwork.SendMsg((UInt16)GamePacketId.CGSupplyDestroy, packet);
        }
    }

    public void ActiveEffect(string _text)
    {
        StartCoroutine(CoActiveEffect(_text));
    }

    IEnumerator CoActiveEffect(string _text)
    {
        if (curCoroutine != null)
        {
            StopCoroutine(curCoroutine);
        }
        SetVisible(false);
        if (_text != null)
        {
            infoText.gameObject.SetActive(true);
            infoText.text = _text + "+";
            yield return infoText.rectTransform.DOAnchorPosY(2f, 1f);
        }
        yield return new WaitForSeconds(1f);
        ObjectPoolManager.Instance.Destroy(this.gameObject);
    }

    public void BoxRejoin()
    {
        SetVisible(true);
        infoText.gameObject.SetActive(false);
    }
}
