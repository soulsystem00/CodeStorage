using System.Collections;
using System.Collections.Generic;
using RealTime;
using UnityEngine;
using static RealTime.Common.CoreDefine;

public class SkillHealRange : RealTimeMonoBehaviour
{
    public void HealRange()
    {
        StartCoroutine(SkillFlow());
    }

    IEnumerator SkillFlow()
    {
        realtimeView.RPC("RPCHealRange", RpcTarget.All, true);
        yield return new WaitForSeconds(GlobalSettings.Instance.HealRangeDuration);
        realtimeView.RPC("RPCHealRange", RpcTarget.All, false);
    }
}
