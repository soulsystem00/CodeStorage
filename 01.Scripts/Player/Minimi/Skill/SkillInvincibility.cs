using System.Collections;
using RealTime;
using UnityEngine;
using UnityEngine.UI;
using static RealTime.Common.CoreDefine;

public class SkillInvincibility : SkillBase
{
    public override void DisableBtn(Button _button)
    {
        _button.gameObject.SetActive(false);
    }

    public override IEnumerator SkillFlow()
    {
        var player = GetComponent<Player>();
        if (player != null)
        {
            player.IsInvincibility = true;
            SetParticle(true);
            yield return new WaitForSeconds(skillDuration);
            player.IsInvincibility = false;
            SetParticle(false);
        }
    }

    public void SetParticle(bool _value)
    {
        realtimeView.RPC("RPCSetParticle4", RpcTarget.All, _value);
    }

    [RealTimeRPC]
    public void RPCSetParticle4(bool _value)
    {
        if (ps != null)
            ps.SetActive(_value);
    }
}