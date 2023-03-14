using System.Collections;
using System.Collections.Generic;
using RealTime;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCounter : MonoBehaviour
{
    [SerializeField] Text countText;

    void Update()
    {
        countText.text = $"{PlayerListManager.Instance.GetPlayerCount()} / {RealTimeConfig.MAX_PLAYER_PER_ROOM}";
    }
}
