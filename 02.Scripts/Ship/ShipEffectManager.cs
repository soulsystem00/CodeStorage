using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipEffectManager : MonoBehaviour
{
    [SerializeField] ShipStatManager shipStatManager;
    [SerializeField] GameObject igniteEffect;
    [SerializeField] GameObject faintEffect;
    [SerializeField] GameObject slowEffect;
    [SerializeField] GameObject silence;
    [SerializeField] GameObject boosterRangeEffect;
    [SerializeField] GameObject healRangeEffect;
    [SerializeField] GameObject boosterEffect;
    [SerializeField] GameObject healEffect;

    void Update()
    {
        igniteEffect.SetActive(shipStatManager.IgniteTime > 0f);
        faintEffect.SetActive(shipStatManager.FaintTime > 0f);
        slowEffect.SetActive(shipStatManager.SlowTime > 0f);
        silence.SetActive(shipStatManager.SilenceTime > 0f);
    }

    public void ActiveBoosterRangeEffect()
    {
        StartCoroutine(CoBoosterRangeEffect());
    }

    public void ActiveHealRangeEffect()
    {
        StartCoroutine(CoHealRangeEffect());
    }

    public void ActiveHealEffect()
    {
        StartCoroutine(CoHealEffect());
    }

    public void ActiveBoosterEffect()
    {
        StartCoroutine(CoBoosterEffect());
    }

    IEnumerator CoBoosterRangeEffect()
    {
        boosterRangeEffect.SetActive(true);
        yield return new WaitForSeconds(GlobalSettings.Instance.BoosterRangeDuration);
        boosterRangeEffect.SetActive(false);
    }

    IEnumerator CoHealRangeEffect()
    {
        healRangeEffect.SetActive(true);
        yield return new WaitForSeconds(GlobalSettings.Instance.HealRangeDuration);
        healRangeEffect.SetActive(false);
    }

    IEnumerator CoHealEffect()
    {
        healEffect.SetActive(true);
        yield return new WaitForSeconds(1f);
        healEffect.SetActive(false);
    }

    IEnumerator CoBoosterEffect()
    {
        boosterEffect.SetActive(true);
        yield return new WaitForSeconds(1f);
        boosterEffect.SetActive(false);
    }
}
