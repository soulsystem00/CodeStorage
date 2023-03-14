using System.Collections.Generic;
using RealTime;
using UnityEngine;
using static RealTime.Common.CoreDefine;

public class Exit : MonoBehaviour
{
    public void OnExit()
    {
        if (RealTimeNetwork.IsMasterClient)
        {
            List<RT_SavePlayerData> list_result = new List<RT_SavePlayerData>();
            RT_SavePlayerData asdf = new RT_SavePlayerData();
            RealTimeNetwork.EndRoom_Master();
        }
        else
        {
            RealTimeNetwork.OutRoom();
        }
    }
}
