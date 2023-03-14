using System;
using System.Collections;
using System.Collections.Generic;
using RealTime.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static RealTime.Common.CoreDefine;

public class ResultPanel : MonoBehaviour
{
    [SerializeField] Text resultText;
    [SerializeField] Button exitBtn;

    RealTimeEventManager realTimeEventManager;
    void Awake()
    {
        realTimeEventManager = RealTimeEventManager.Instance;
        realTimeEventManager.OnDisconnectRoomEvent += DisconnectRoomEvent;

        exitBtn.onClick.AddListener(OnExitBtnClicked);
    }

    void OnDestroy()
    {
        realTimeEventManager.OnDisconnectRoomEvent -= DisconnectRoomEvent;
    }

    private void DisconnectRoomEvent(DisconnectType _reson)
    {
        if (_reson == DisconnectType.OutRoom)
        {
            exitBtn.gameObject.SetActive(true);
        }
        else if (_reson == DisconnectType.EndRoom)
        {
            exitBtn.gameObject.SetActive(true);
        }
    }

    private void OnExitBtnClicked()
    {
        SceneManager.LoadScene(2);
    }

    public void SetResultText(bool _value)
    {
        if (_value)
        {
            resultText.text = "Win!!";
        }
        else
        {
            resultText.text = "Lose!!";
        }
    }
}
