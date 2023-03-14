using System.Collections;
using DG.Tweening;
using RealTime;
using UnityEngine;
using UnityEngine.UI;
using static RealTime.Common.CoreDefine;

public class SkillStealth : SkillBase
{
    [SerializeField] SkinnedMeshRenderer[] playerMesh;
    [SerializeField] MeshRenderer[] playerMesh2;
    [SerializeField] GameObject hpBar;

    public override void DisableBtn(Button _button)
    {
        _button.gameObject.SetActive(false);
    }

    public override IEnumerator SkillFlow()
    {
        realtimeView.RPC("RPCSetStealth", RpcTarget.All, true);
        realtimeView.RPC("RPCStealth", RpcTarget.All, true);
        yield return new WaitForSeconds(skillDuration);
        realtimeView.RPC("RPCStealth", RpcTarget.All, false);
        yield return new WaitForSeconds(0.5f);
        realtimeView.RPC("RPCSetStealth", RpcTarget.All, false);
    }

    [RealTimeRPC]
    public void RPCSetStealth(bool _value)
    {
        foreach (var m in playerMesh)
        {
            StandardShaderUtils.ChangeRenderMode(m.material, _value ? StandardShaderUtils.BlendMode.Transparent : StandardShaderUtils.BlendMode.Opaque);
        }

        foreach (var m in playerMesh2)
        {
            StandardShaderUtils.ChangeRenderMode(m.material, _value ? StandardShaderUtils.BlendMode.Transparent : StandardShaderUtils.BlendMode.Opaque);
        }

        if (!realtimeView.IsMine)
            hpBar.SetActive(!_value);
    }

    [RealTimeRPC]
    public void RPCStealth(bool _value)
    {
        foreach (var m in playerMesh)
        {
            if (_value)
            {
                m.material.DOFade(0f, 0.5f).OnComplete(() => { if (!realtimeView.IsMine) m.enabled = false; });
            }
            else
            {
                m.enabled = true;
                m.material.DOFade(1f, 0.5f);
            }
        }

        foreach (var m in playerMesh2)
        {
            if (_value)
            {
                m.material.DOFade(0f, 0.5f).OnComplete(() => { if (!realtimeView.IsMine) m.enabled = false; });
            }
            else
            {
                m.enabled = true;
                m.material.DOFade(1f, 0.5f);
            }
        }
    }
}

public static class StandardShaderUtils
{
    public enum BlendMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent
    }

    public static void ChangeRenderMode(Material standardShaderMaterial, BlendMode blendMode)
    {
        switch (blendMode)
        {
            case BlendMode.Opaque:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                standardShaderMaterial.SetInt("_ZWrite", 1);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = -1;
                break;
            case BlendMode.Cutout:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                standardShaderMaterial.SetInt("_ZWrite", 1);
                standardShaderMaterial.EnableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = 2450;
                break;
            case BlendMode.Fade:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                standardShaderMaterial.SetInt("_ZWrite", 0);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.EnableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = 3000;
                break;
            case BlendMode.Transparent:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                standardShaderMaterial.SetInt("_ZWrite", 0);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = 3000;
                break;
        }

    }
}