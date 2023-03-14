using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RealTime;
using UnityEngine;
using UnityEngine.UI;
using static RealTime.Common.CoreDefine;

public class SkillManager : RealTimeMonoBehaviour
{
    [SerializeField] Button skillBtn;
    [SerializeField] Image btnImage;
    [SerializeField] Text btnText;
    [SerializeField] SkillHeal skillHeal;
    [SerializeField] SkillBooster skillBooster;
    [SerializeField] SkillBoosterRange skillBoosterRange;
    [SerializeField] SkillHealRange skillHealRange;

    Action Skill = null;

    void Awake()
    {
        skillBtn.onClick.AddListener(() => Skill?.Invoke());
    }

    public void Init(SkillType _skill)
    {
        if (_skill != SkillType.None)
        {
            this.gameObject.SetActive(true);
            btnText.text = StringDB.SkillName[_skill];
            SetSkill(_skill);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    private void SetSkill(SkillType _skill)
    {
        if (_skill == SkillType.BoosterRange)
        {
            Skill = () =>
            {
                skillBoosterRange.BoosterRange(); Debug.Log("BoosterRange");
                realtimeView.RPC("RPCEnableEffect", RpcTarget.All, (int)_skill);
            };
        }
        else if (_skill == SkillType.Flash)
        {
            Skill = () =>
            {
                skillBooster.Booster(); Debug.Log("Booster");
                realtimeView.RPC("RPCEnableEffect", RpcTarget.All, (int)_skill);
            };
        }
        else if (_skill == SkillType.Heal)
        {
            Skill = () =>
            {
                skillHeal.Heal(); Debug.Log("Heal");
                realtimeView.RPC("RPCEnableEffect", RpcTarget.All, (int)_skill);
            };
        }
        else if (_skill == SkillType.HealRange)
        {
            Skill = () =>
            {
                skillHealRange.HealRange(); Debug.Log("HealRange");
                realtimeView.RPC("RPCEnableEffect", RpcTarget.All, (int)_skill);
            };
        }
        Skill += () => StartCoroutine(DisableButton());
    }

    IEnumerator DisableButton()
    {
        skillBtn.interactable = false;
        btnImage.fillAmount = 0f;
        yield return btnImage.DOFillAmount(1f, GlobalSettings.Instance.SkillCoolTime).SetEase(Ease.Linear).OnComplete(() => skillBtn.interactable = true);
    }
}
