using System.Collections;
using System.Collections.Generic;
using RealTime;
using UnityEngine;
using static RealTime.Common.CoreDefine;

public class SkillBoosterRange : RealTimeMonoBehaviour
{
    public void BoosterRange()
    {
        StartCoroutine(SkillFlow());
    }

    IEnumerator SkillFlow()
    {
        realtimeView.RPC("RPCBoosterRange", RpcTarget.All, true);
        yield return new WaitForSeconds(GlobalSettings.Instance.BoosterRangeDuration);
        realtimeView.RPC("RPCBoosterRange", RpcTarget.All, false);
    }
}
