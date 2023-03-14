using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DG.Tweening;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] Transform hpBar;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] SpriteRenderer indicator;
    [SerializeField] SpriteMask spriteMask;
    public float maxHealth;
    float curHp = 1.0f;

    void OnEnable()
    {
        curHp = 1f;
        StartCoroutine(MakeObjLookCam());
    }

    IEnumerator MakeObjLookCam()
    {
        while (true)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                var qua = Quaternion.LookRotation(cam.transform.position - this.transform.position);
                transform.rotation = Quaternion.Euler(-qua.eulerAngles.x, 0f, 0f);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public float GetHP()
    {
        return curHp;
    }

    public float ApplyDamage(float _damage, float _maxHealth)
    {
        float target = _damage / _maxHealth;
        curHp -= target;
        curHp = Mathf.Clamp01(curHp);
        hpBar.DOScaleX(curHp, 1f).SetEase(Ease.Linear);

        return curHp;
    }

    public float SetHpBar(float _maxHealth)
    {
        if (_maxHealth > this.maxHealth)
        {
            var diff = _maxHealth - this.maxHealth;
            var curhealth = this.maxHealth * this.curHp;
            var updateHealth = curhealth + diff;
            curHp = updateHealth / _maxHealth;
            curHp = Mathf.Clamp01(curHp);
            hpBar.DOScaleX(curHp, 1f).SetEase(Ease.Linear);
            indicator.gameObject.transform.localScale = new Vector3(100f / _maxHealth, 2.1f, 1f);
        }
        else
        {
            var curhealth = this.maxHealth * this.curHp;
            curHp = curhealth / _maxHealth;
            curHp = Mathf.Clamp01(curHp);
            hpBar.DOScaleX(curHp, 1f).SetEase(Ease.Linear);
            indicator.gameObject.transform.localScale = new Vector3(100f / _maxHealth, 2.1f, 1f);
        }
        maxHealth = _maxHealth;

        return curHp;
    }

    public void RejoinHealth(float _maxHealth, float _curHp)
    {
        this.maxHealth = _maxHealth;
        this.curHp = _curHp;
        indicator.gameObject.transform.localScale = new Vector3(100f / _maxHealth, 2.1f, 1f);
        hpBar.DOScaleX(curHp, 1f).SetEase(Ease.Linear);
    }

    public void SetHpbarColor(IslandOwner _owner)
    {
        if (_owner == IslandOwner.Enemy)
        {
            sr.color = Color.red;
        }
        else if (_owner == IslandOwner.Mine)
        {
            sr.color = Color.green;
        }
        else if (_owner == IslandOwner.Neutrality)
        {
            sr.color = Color.magenta;
        }
        else if (_owner == IslandOwner.Friendly)
        {
            sr.color = Color.blue;
        }
    }

    public void SetOrderNum(int _index)
    {
        int orderNum = _index * 2;
        indicator.sortingOrder = orderNum;
        spriteMask.frontSortingOrder = orderNum + 1;
        spriteMask.backSortingOrder = orderNum - 1;
    }
}
