using DG.Tweening;
using UnityEngine;
public class ExpBar : MonoBehaviour
{
    public RectTransform expBar;
    public float curExp = 0f;

    public float GetExp()
    {
        return curExp;
    }

    public void SetExpbar(float _target)
    {
        _target = Mathf.Clamp01(_target);
        if (curExp < _target)
        {
            curExp = _target;
            expBar.DOScaleX(_target, 0.1f);
        }
    }
}
