using System.Collections;
using RealTime;
using UnityEngine;
using UnityEngine.UI;
using static RealTime.Common.CoreDefine;

public class SkillHealing : SkillBase
{
    public override void DisableBtn(Button _button)
    {
        _button.gameObject.SetActive(false);
    }

    public override IEnumerator SkillFlow()
    {
        var player = GetComponent<Player>();
        if (player != null)
            player.SetPlayerHp(-0.2f);
        SetParticle(true);
        yield return new WaitForSeconds(skillDuration);
        SetParticle(false);
    }

    public void SetParticle(bool _value)
    {
        realtimeView.RPC("RPCSetParticle3", RpcTarget.All, _value);
    }

    [RealTimeRPC]
    public void RPCSetParticle3(bool _value)
    {
        if (ps != null)
            ps.SetActive(_value);
    }
}