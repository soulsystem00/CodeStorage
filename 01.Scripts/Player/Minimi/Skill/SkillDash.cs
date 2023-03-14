using System.Collections;
using RealTime;
using UnityEngine;
using UnityEngine.UI;
using static RealTime.Common.CoreDefine;

public class SkillDash : SkillBase
{
    public override void OnSkillBtnClicked(Button _button)
    {
        DisableBtn(_button);
        StartCoroutine(SkillFlow());
    }

    public override void DisableBtn(Button _button)
    {
        _button.gameObject.SetActive(false);
    }

    public override IEnumerator SkillFlow()
    {
        var playerMove = GetComponent<PlayerMove>();
        playerMove.isDash = true;
        SetParticle(true);
        yield return new WaitForSeconds(skillDuration);
        playerMove.isDash = false;
        SetParticle(false);
    }

    public void SetParticle(bool _value)
    {
        realtimeView.RPC("RPCSetParticle1", RpcTarget.All, _value);
    }

    [RealTimeRPC]
    public void RPCSetParticle1(bool _value)
    {
        if (ps != null)
            ps.SetActive(_value);
    }
}
