using System.Collections;
using RealTime;
using UnityEngine;
using UnityEngine.UI;

public abstract class SkillBase : RealTimeMonoBehaviour
{
    [SerializeField] protected GameObject ps;
    [SerializeField] protected float skillDuration;

    public virtual void OnSkillBtnClicked(Button _button)
    {
        DisableBtn(_button);
        StartCoroutine(SkillFlow());
    }

    abstract public void DisableBtn(Button _button);

    abstract public IEnumerator SkillFlow();
}
