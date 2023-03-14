using System.Collections;
using RealTime;
using UnityEngine;
using UnityEngine.UI;
using static RealTime.Common.CoreDefine;

public class SkillFlash : SkillBase
{
    public override void DisableBtn(Button _button)
    {
        _button.gameObject.SetActive(false);
    }

    public override IEnumerator SkillFlow()
    {
        SetParticle(true);
        var deg = transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
        Vector3 dirVec = new Vector3(Mathf.Sin(deg), 0f, Mathf.Cos(deg));
        transform.position += dirVec * 3.5f;
        yield return new WaitForSeconds(skillDuration);
        SetParticle(false);
    }

    public void SetParticle(bool _value)
    {
        realtimeView.RPC("RPCSetParticle2", RpcTarget.All, _value);
    }

    [RealTimeRPC]
    public void RPCSetParticle2(bool _value)
    {
        if (ps != null)
            ps.SetActive(_value);
    }
}