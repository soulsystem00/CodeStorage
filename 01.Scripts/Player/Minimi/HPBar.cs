using System;
using System.Collections;
using DG.Tweening;
using RealTime;
using UnityEngine;

public class HPBar : RealTimeMonoBehaviour
{
    [SerializeField] Transform hpBar;
    float curHp;
    void OnEnable()
    {
        curHp = 1f;
        StartCoroutine(MakeObjLookCam());
    }

    IEnumerator MakeObjLookCam()
    {
        while (true)
        {
            var qua = Quaternion.LookRotation(Camera.main.transform.position - this.transform.position);
            transform.rotation = Quaternion.Euler(-qua.eulerAngles.x, 0f, 0f);
            yield return new WaitForEndOfFrame();
        }
    }

    public float GetHP()
    {
        return curHp;
    }

    public void SetHPbar(float _target)
    {
        if (_target < hpBar.localScale.x)
            PlayerInfo.Instance.IncAttackCnt();
        _target = (float)Math.Round(Mathf.Clamp01(_target), 2);
        curHp = _target;
        hpBar.DOScaleX(_target, 1f).SetEase(Ease.Linear);
        if (_target <= 0f)
        {
            var dieParticle = Resources.Load("PlayScene/Die Particle") as GameObject;
            if (dieParticle != null)
            {
                Instantiate(dieParticle, transform.parent.position + new Vector3(0, 0.3f, 0f), Quaternion.identity);
            }
        }
        if (_target <= 0f && realtimeView.IsMine)
        {
            PlayerInfo.Instance.DestroyPlayer(this.gameObject);
        }
    }

    IEnumerator SetHPbarSmooth(float _target)
    {
        var scale = hpBar.localScale;
        var diff = scale.x - _target;
        var amount = diff * Time.deltaTime;

        while (true)
        {
            if (hpBar.localScale.x <= _target)
            {
                hpBar.localScale = new Vector3(_target, scale.y, scale.z);
                yield break;
            }

            hpBar.localScale = new Vector3(hpBar.localScale.x - amount, hpBar.localScale.y, hpBar.localScale.z);


            yield return null;
        }

    }
}
